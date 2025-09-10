using CC.Application.Services;
using CC.Domain.Helpers;
using CC.Domain.Interfaces.Repositories;
using CC.Domain.Interfaces.Services;
using CC.Infrastructure.Configurations;
using CC.Infrastructure.EmailServices;
using CC.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using System.Reflection;
using ILogger = Serilog.ILogger;

namespace Gandarias.Handlers;

public class DependencyInyectionHandler
{
    public static void DepencyInyectionConfig(IServiceCollection services)
    {
        try
        {
            // FIXED: Properly detect environment and load correct configuration files
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
            
            Console.WriteLine($"🌍 Detected Environment: {environment}");
            
            // Build configuration from multiple sources with proper environment detection
            IConfigurationBuilder configBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            
            // FIXED: Load environment-specific file based on actual environment
            if (environment == "Development")
            {
                configBuilder.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
                Console.WriteLine("📋 Loading appsettings.Development.json");
            }
            else if (environment == "Production")
            {
                configBuilder.AddJsonFile("appsettings.Production.json", optional: true, reloadOnChange: true);
                Console.WriteLine("📋 Loading appsettings.Production.json");
            }
            else
            {
                // For other environments (Staging, Testing, etc.)
                configBuilder.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);
                Console.WriteLine($"📋 Loading appsettings.{environment}.json");
            }
            
            configBuilder.AddEnvironmentVariables();

            IConfiguration configuration = configBuilder.Build();
            services.AddSingleton(configuration);

            #region Database Configuration

            // Get connection string from environment variables first, then fallback to config
            string connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");
            
            if (string.IsNullOrEmpty(connectionString) || connectionString.Contains("${"))
            {
                connectionString = configuration.GetConnectionString("PgSQL");
                Console.WriteLine("🔗 Using connection string from configuration file");
            }
            else
            {
                Console.WriteLine("🔗 Using connection string from environment variable");
            }
            
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string not found");
            }

            services.AddDbContext<DBContext>(opt =>
            {
                opt.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    // ENHANCED: More robust retry policy for Azure PostgreSQL
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorCodesToAdd: null);
                    
                    // ENHANCED: Increased command timeout for complex operations
                    npgsqlOptions.CommandTimeout(300); // 5 minutes instead of 2 minutes
                    
                    // ENHANCED: Connection timeout for initial connection
                    npgsqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                });

                // ENHANCED: Additional EF Core optimizations
                opt.EnableServiceProviderCaching();
                opt.EnableSensitiveDataLogging(environment == "Development");
                opt.EnableDetailedErrors(environment == "Development");
                
                // ENHANCED: Query tracking behavior for better performance
                opt.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);

                if (environment == "Development")
                {
                    Console.WriteLine("🔍 Database debug logging enabled for Development");
                    Console.WriteLine("⏱️  Extended timeouts: CommandTimeout=300s, Retry=5x");
                }
                else
                {
                    Console.WriteLine("🚀 Production database configuration with extended timeouts");
                }
            });

            #endregion

            #region AutoMapper

            services.AddAutoMapper(Assembly.Load("CC.Domain"));

            #endregion

            #region Services and Repositories

            ServicesRegistration(services);
            RepositoryRegistration(services);

            #endregion

            #region EmailService - FIXED

            try
            {
                services.Configure<EmailServiceOptions>(options =>
                {
                    // SMTP Server
                    options.SmtpServer = GetValidEnvironmentVariable("SMTP_SERVER")
                        ?? configuration["EmailService:smtpServer"]
                        ?? "localhost";

                    // SMTP Port - FIXED with proper error handling
                    var smtpPortEnv = GetValidIntFromEnvironment("SMTP_PORT", 587);
                    var smtpPortConfig = configuration["EmailService:smtpPort"];

                    if (smtpPortEnv.HasValue)
                    {
                        options.SmtpPort = smtpPortEnv.Value;
                    }
                    else if (!string.IsNullOrEmpty(smtpPortConfig) && !smtpPortConfig.Contains("${"))
                    {
                        options.SmtpPort = int.Parse(smtpPortConfig);
                    }
                    else
                    {
                        options.SmtpPort = 587; // Default fallback
                    }

                    // SMTP User
                    options.SmtpUser = GetValidEnvironmentVariable("SMTP_USER")
                        ?? configuration["EmailService:smtpUser"]
                        ?? "test@localhost";

                    // SMTP Password
                    options.SmtpPassword = GetValidEnvironmentVariable("SMTP_PASSWORD")
                        ?? configuration["EmailService:smtpPassword"]
                        ?? "password";

                    // Enable SSL - FIXED with proper error handling
                    var enableSslEnv = GetValidBoolFromEnvironment("SMTP_ENABLE_SSL", true);
                    var enableSslConfig = configuration["EmailService:EnableSsl"];
                    
                    if (enableSslEnv.HasValue)
                    {
                        options.EnableSsl = enableSslEnv.Value;
                    }
                    else if (!string.IsNullOrEmpty(enableSslConfig) && !enableSslConfig.Contains("${"))
                    {
                        options.EnableSsl = bool.Parse(enableSslConfig);
                    }
                    else
                    {
                        options.EnableSsl = true; // Default fallback
                    }
                });
                
                Console.WriteLine("✅ Email service configured successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Email service configuration failed: {ex.Message}");
                // Don't throw here, email is not critical for startup
            }

            #endregion

            services.AddSingleton<ExceptionControl>();

            #region Logging

            Logger logger = new LoggerConfiguration()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File("logs/log-.txt",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                    retainedFileCountLimit: 7)
                .CreateLogger();

            logger.Information("Application dependency injection configured successfully for {Environment}", environment);
            services.AddSingleton<ILogger>(logger);

            #endregion

            services.AddTransient<SeedDB>();
            
            Console.WriteLine($"🎉 Dependency injection configuration completed for {environment} environment");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"💥 FATAL: Dependency injection configuration failed: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw new InvalidOperationException("Dependency injection configuration failed", ex);
        }
    }

    /// <summary>
    /// Gets a valid environment variable, returning null if it's empty, null, or contains template syntax
    /// </summary>
    private static string? GetValidEnvironmentVariable(string variableName)
    {
        var value = Environment.GetEnvironmentVariable(variableName);

        // Return null if:
        // - Value is null or empty
        // - Value contains template syntax like ${VARIABLE_NAME}
        // - Value is just whitespace
        if (string.IsNullOrWhiteSpace(value) ||
            value.Contains("${") ||
            value.Contains("$") && value.Contains("{") && value.Contains("}"))
        {
            return null;
        }

        return value;
    }

    /// <summary>
    /// Gets a valid integer from environment variable with fallback
    /// </summary>
    private static int? GetValidIntFromEnvironment(string variableName, int defaultValue)
    {
        var value = GetValidEnvironmentVariable(variableName);

        if (value == null)
        {
            return null;
        }

        if (int.TryParse(value, out int result))
        {
            return result;
        }

        // If parsing fails, log warning and return null to use config fallback
        Console.WriteLine($"⚠️ Environment variable {variableName} has invalid integer value: {value}");
        return null;
    }

    /// <summary>
    /// Gets a valid boolean from environment variable with fallback
    /// </summary>
    private static bool? GetValidBoolFromEnvironment(string variableName, bool defaultValue)
    {
        var value = GetValidEnvironmentVariable(variableName);

        if (value == null)
        {
            return null;
        }

        if (bool.TryParse(value, out bool result))
        {
            return result;
        }

        // If parsing fails, log warning and return null to use config fallback
        Console.WriteLine($"⚠️ Environment variable {variableName} has invalid boolean value: {value}");
        return null;
    }

    public static void ServicesRegistration(IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IWorkAreaService, WorkAreaService>();
        services.AddScoped<IWorkstationService, WorkstationService>();
        services.AddScoped<IRolePermissionService, RolePermissionService>();
        services.AddScoped<IHireTypeService, HireTypeService>();
        services.AddScoped<IShiftTypeService, ShiftTypeService>();
        services.AddScoped<ILicenseService, LicenseService>();
        services.AddScoped<IHybridWorkstationService, HybridWorkstationService>();
        services.AddScoped<IUserWorkstationService, UserWorkstationService>();
        services.AddScoped<IEmployeeScheduleRestrictionService, EmployeeScheduleRestrictionService>();
        services.AddScoped<IAbsenteeismTypeService, AbsenteeismTypeService>();
        services.AddScoped<IUserAbsenteeismService, UserAbsenteeismService>();
        services.AddScoped<IEmployeeScheduleExceptionService, EmployeeScheduleExceptionService>();
        services.AddScoped<ILawRestrictionService, LawRestrictionService>();
        services.AddScoped<IWorkstationDemandService, WorkstationDemandService>();
        services.AddScoped<IWorkstationDemandTemplateService, WorkstationDemandTemplateService>();
        services.AddScoped<IEmployeeShiftTypeRestrictionService, EmployeeShiftTypeRestrictionService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IUserShiftService, UserShiftService>();
        services.AddScoped<IScheduleService, ScheduleService>();
        services.AddTransient<IQrCodeService, QrCodeRepository>();
        services.AddTransient<IEncryptionService, AesEncryptionService>();
        services.AddScoped<ISigningService, SigningService>();

        services.AddHttpClient();
    }

    public static void RepositoryRegistration(IServiceCollection services)
    {
        services.AddScoped<IQueryableUnitOfWork, DBContext>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IWorkAreaRepository, WorkAreaRepository>();
        services.AddScoped<IWorkstationRepository, WorkstationRepository>();
        services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
        services.AddScoped<IHireTypeRepository, HireTypeRepository>();
        services.AddScoped<IShiftTypeRepository, ShiftTypeRepository>();
        services.AddScoped<ILicenseRepository, LicenseRepository>();
        services.AddScoped<IHybridWorkstationRepository, HybridWorkstationRepository>();
        services.AddScoped<IUserWorkstationRepository, UserWorkstationRepository>();
        services.AddScoped<IEmployeeScheduleRestrictionRepository, EmployeeScheduleRestrictionRepository>();
        services.AddScoped<IAbsenteeismTypeRepository, AbsenteeismTypeRepository>();
        services.AddScoped<IUserAbsenteeismRepository, UserAbsenteeismRepository>();
        services.AddScoped<IEmployeeScheduleExceptionRepository, EmployeeScheduleExceptionRepository>();
        services.AddScoped<ILawRestrictionRepository, LawRestrictionRepository>();
        services.AddScoped<IWorkstationDemandRepository, WorkstationDemandRepository>();
        services.AddScoped<IWorkstationDemandTemplateRepository, WorkstationDemandTemplateRepository>();
        services.AddScoped<IEmployeeShiftRestrictionRepository, EmployeeShiftRestrictionRepository>();
        services.AddScoped<IUserShiftRepository, UserShiftRepository>();
        services.AddScoped<IScheduleRepository, ScheduleRepository>();
        services.AddScoped<ISigningRepository, SigningRepository>();
    }
}