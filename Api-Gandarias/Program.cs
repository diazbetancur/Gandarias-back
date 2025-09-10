using CC.Domain.Entities;
using CC.Infrastructure.Configurations;
using Gandarias.Handlers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Environment & Config
Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Services
builder.Services.AddHealthChecks();
builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Swagger
SwaggerHandler.SwaggerConfig(builder.Services);

// Dependency Injection (pass configuration & environment)
DependencyInyectionHandler.DepencyInyectionConfig(builder.Services, builder.Configuration, builder.Environment.EnvironmentName);

// Identity
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

// JWT - read from env first, then config
var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
            ?? builder.Configuration["jwtKey"]
            ?? string.Empty;

if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.Contains("${"))
{
    throw new InvalidOperationException("JWT secret key not configured properly. Set JWT_SECRET_KEY env var with at least 32 characters.");
}
if (jwtKey.Length < 32)
{
    throw new InvalidOperationException($"JWT secret key too short: {jwtKey.Length} chars. Minimum 32 required.");
}
Console.WriteLine($"JWT key length OK: {jwtKey.Length} chars");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false; // App Runner terminates TLS
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.FromMinutes(5)
        };
    });

var app = builder.Build();

// Health endpoint
app.MapHealthChecks("/health");

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
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

    // Security headers
    app.Use(async (context, next) =>
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
    });
}

// CORS (wide open - restrict as needed)
app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true)
    .AllowCredentials());

// HTTPS redirection: disable in App Runner (PORT set); allow in local dev
var disableHttps = Environment.GetEnvironmentVariable("DISABLE_HTTPS_REDIRECT") == "true";
var portEnv = Environment.GetEnvironmentVariable("PORT");
if (!disableHttps && string.IsNullOrEmpty(portEnv) && app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseMiddleware(typeof(ErrorHandlingMiddleware));
app.UseAuthentication();
app.UseMiddleware<ActivityLoggingMiddleware>();
app.UseAuthorization();
app.MapControllers();

// Bind to PORT (App Runner) or default 8080
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://0.0.0.0:{port}");
Console.WriteLine($"Listening on http://0.0.0.0:{port}");

app.Run();