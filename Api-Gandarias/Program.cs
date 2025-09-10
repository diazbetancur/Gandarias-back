using CC.Domain.Entities;
using CC.Infrastructure.Configurations;
using Gandarias.Handlers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;

try
{
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    // Load configurations
    builder.Configuration
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
        .AddEnvironmentVariables();

    // Health Check endpoint
    builder.Services.AddHealthChecks();

    // Add services to the container
    builder.Services.AddControllers();
    builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    #region Swagger

    try
    {
        SwaggerHandler.SwaggerConfig(builder.Services);
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"Swagger configuration failed: {ex.Message}");
    }

    #endregion Swagger

    #region Dependency Injection

    try
    {
        DependencyInyectionHandler.DepencyInyectionConfig(builder.Services);
    }
    catch (Exception ex)
    {
        throw new InvalidOperationException("Dependency injection configuration failed", ex);
    }

    #endregion Dependency Injection

    #region Identity

    try
    {
        builder.Services.AddIdentity<User, Role>(opt =>
        {
            opt.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
            opt.SignIn.RequireConfirmedEmail = false;
            opt.Password.RequiredLength = 8;
            opt.Password.RequireLowercase = true;
            opt.Password.RequireUppercase = true;
            opt.Password.RequireNonAlphanumeric = true;
            opt.Password.RequiredUniqueChars = 1;
            opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
        }).AddRoles<Role>().AddEntityFrameworkStores<DBContext>().AddDefaultTokenProviders();
    }
    catch (Exception ex)
    {
        throw new InvalidOperationException("Identity configuration failed", ex);
    }

    #endregion Identity

    #region JWT - FIXED for both Development and Production

    try
    {
        var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
                    ?? builder.Configuration["jwtKey"]
                    ?? "default-development-key-not-for-production-use-12345678901234567890";
        Console.WriteLine($"? JWT key length: {jwtKey.Length} characters ({jwtKey.Length * 8} bits)");
        Console.WriteLine($"? JWT key Environment {Environment.GetEnvironmentVariable("JWT_SECRET_KEY")}");
        Console.WriteLine($"? JWT key configuration {builder.Configuration["jwtKey"]}");
        Console.WriteLine($"? jwtKey {jwtKey}");

        // CRITICAL FIX: Ensure JWT key meets minimum length requirements for HS256
        if (jwtKey.Length < 32)
        {
            // If key is too short, pad it or use a secure fallback
            if (builder.Environment.IsDevelopment())
            {
                // Development: Use a properly sized development key
                jwtKey = "development-jwt-secret-key-that-meets-256-bit-minimum-requirement-for-HS256-algorithm";
                Console.WriteLine("?? Using development JWT key (original key too short)");
            }
            else
            {
                // Production: This should never happen, but provide a fallback
                Console.WriteLine($"? CRITICAL: JWT key too short ({jwtKey.Length * 8} bits), minimum 256 bits required");
                throw new InvalidOperationException($"JWT secret key must be at least 32 characters (256 bits) long. Current key is {jwtKey.Length} characters ({jwtKey.Length * 8} bits). Please configure a proper JWT_SECRET_KEY environment variable.");
            }
        }
        else
        {
            Console.WriteLine($"? JWT key validated: {jwtKey.Length} characters ({jwtKey.Length * 8} bits)");
        }

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false; // CRITICAL: Allow HTTP for development and App Runner
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtKey)),
                    ClockSkew = TimeSpan.FromMinutes(5), // Allow 5 minutes tolerance

                    // Enhanced lifetime validator for timezone issues
                    LifetimeValidator = (notBefore, expires, token, param) =>
                    {
                        var utcNow = DateTime.UtcNow;
                        var effectiveNotBefore = notBefore?.AddMinutes(-5) ?? DateTime.MinValue;
                        var effectiveExpires = expires?.AddMinutes(5) ?? DateTime.MaxValue;
                        return utcNow >= effectiveNotBefore && utcNow <= effectiveExpires;
                    }
                };

                // Enhanced events for debugging
                x.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (builder.Environment.IsDevelopment())
                        {
                            System.Diagnostics.Debug.WriteLine($"JWT Auth failed: {context.Exception.Message}");
                        }
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        if (builder.Environment.IsDevelopment())
                        {
                            System.Diagnostics.Debug.WriteLine("JWT Token validated successfully");
                        }
                        return Task.CompletedTask;
                    }
                };
            });
    }
    catch (Exception ex)
    {
        throw new InvalidOperationException("JWT configuration failed", ex);
    }

    #endregion JWT - FIXED for both Development and Production

    var app = builder.Build();

    // Health Check endpoint
    app.MapHealthChecks("/health");

    // Configure the HTTP request pipeline based on environment
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gandarias API V1");
            c.RoutePrefix = "swagger";
        });

        // Seed data in development if requested
        if (Environment.GetEnvironmentVariable("ENABLE_SEED_DATA") == "true")
        {
            try
            {
                SeedDataSafely(app);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Development seed data failed: {ex.Message}");
            }
        }
    }
    else
    {
        app.UseHsts();
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gandarias API V1");
            c.RoutePrefix = "swagger";
        });

        #region Security Headers

        app.Use(async (context, next) =>
        {
            try
            {
                context.Response.Headers.Clear();
                context.Response.Headers.Add("Content-Security-Policy",
                    "default-src 'self'; style-src 'self' 'unsafe-inline'; script-src 'self' 'unsafe-inline'; img-src 'self' data:");
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("X-Frame-Options", "DENY");
                context.Response.Headers.Add("X-Permitted-Cross-Domain-Policies", "master-only");
                context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
                context.Response.Headers.Add("Cache-Control", "no-cache,no-store,must-revalidate");
                context.Response.Headers.Add("Pragma", "no-cache");
                context.Response.Headers.Remove("X-Powered-By");
                context.Response.Headers.Remove("Server");
                context.Response.Headers.Add("Referrer-Policy", "no-referrer");
                context.Response.Headers.Add("Permissions-Policy", "fullscreen=(), geolocation=()");
                context.Request.Headers.Add("X-Content-Type-Options", "nosniff");
                await next();
            }
            catch (Exception)
            {
                await next();
            }
        });

        #endregion Security Headers
    }

    // Force seed data if environment variable is set
    if (Environment.GetEnvironmentVariable("FORCE_SEED_DATA") == "true")
    {
        try
        {
            SeedDataSafely(app);
        }
        catch (Exception)
        {
            // Continue with application startup even if seed fails
        }
    }

    // Configure middleware pipeline
    app.UseCors(x => x
        .AllowAnyMethod()
        .AllowAnyHeader()
        .SetIsOriginAllowed(origin => true)
        .AllowCredentials());

    // FIXED: Only use HTTPS redirection in production AND when not in App Runner
    if (!app.Environment.IsDevelopment() && string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PORT")))
    {
        try
        {
            app.UseHttpsRedirection();
        }
        catch (Exception)
        {
            // Continue without HTTPS redirection if it fails
        }
    }

    try
    {
        app.UseMiddleware(typeof(ErrorHandlingMiddleware));
    }
    catch (Exception)
    {
        // Continue without error handling middleware if it fails
    }

    app.UseAuthentication();

    try
    {
        app.UseMiddleware<ActivityLoggingMiddleware>();
    }
    catch (Exception)
    {
        // Continue without activity logging middleware if it fails
    }

    app.UseAuthorization();
    app.MapControllers();

    // FIXED: App Runner compatible port configuration
    var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";

    // Universal configuration that works in all environments:
    // - Development: 0.0.0.0:8080 (consistent with production)
    // - Docker: 0.0.0.0:8080 (matches EXPOSE directive)
    // - Production/App Runner: 0.0.0.0:8080 (required for load balancer)
    app.Urls.Add($"http://0.0.0.0:{port}");

    if (app.Environment.IsDevelopment())
    {
        Console.WriteLine($"?? Development mode on port {port}");
        Console.WriteLine($"?? Available at: http://localhost:{port}");
        Console.WriteLine($"?? Swagger at: http://localhost:{port}/swagger");
        Console.WriteLine($"??  Note: Using port {port} for consistency with production");
    }
    else
    {
        Console.WriteLine($"?? Production mode on port {port}");
        Console.WriteLine($"?? Listening on all interfaces (0.0.0.0:{port})");
    }

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"?? Fatal application error: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
    Environment.Exit(1);
}

void SeedDataSafely(WebApplication app)
{
    try
    {
        var scopedFactory = app.Services.GetService<IServiceScopeFactory>();
        if (scopedFactory == null)
        {
            Console.WriteLine("?? Service scope factory not available for seeding");
            return;
        }

        using var scope = scopedFactory.CreateScope();
        var service = scope.ServiceProvider.GetService<SeedDB>();

        if (service != null)
        {
            Console.WriteLine("?? Starting database seeding with extended timeout...");

            var seedTask = service.SeedAsync();

            // ENHANCED: Increased timeout for seeding operations (10 minutes)
            using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(10));

            try
            {
                seedTask.Wait(cts.Token);
                Console.WriteLine("? Database seeding completed successfully");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("? Database seeding timed out after 10 minutes");
                throw new TimeoutException("Seed data operation timed out after 10 minutes");
            }
        }
        else
        {
            Console.WriteLine("?? SeedDB service not available");
        }
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("? Database seeding was cancelled due to timeout");
        throw new TimeoutException("Seed data operation timed out");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"? Database seeding failed: {ex.Message}");
        if (ex.InnerException != null)
        {
            Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
        }
        throw new InvalidOperationException("Seed data operation failed", ex);
    }
}