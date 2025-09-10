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
    public static void DepencyInyectionConfig(IServiceCollection services, IConfiguration configuration, string environment)
    {
        try
        {
            Console.WriteLine($"Environment detected in DI: {environment}");

            #region Database Configuration

            // Prefer environment variable in container
            string? connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                connectionString = configuration.GetConnectionString("PgSQL");
                Console.WriteLine("DB: Using configuration connection string (PgSQL)");
            }
            else
            {
                Console.WriteLine("DB: Using DATABASE_URL from environment");
            }

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Database connection string not found");

            services.AddDbContext<DBContext>(opt =>
            {
                opt.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorCodesToAdd: null);
                    npgsqlOptions.CommandTimeout(300);
                    npgsqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                });

                opt.EnableSensitiveDataLogging(environment == "Development");
                opt.EnableDetailedErrors(environment == "Development");
                opt.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
            });

            Console.WriteLine("✅ PostgreSQL DbContext configured");

            #endregion

            #region AutoMapper

            services.AddAutoMapper(Assembly.Load("CC.Domain"));

            #endregion

            #region Services and Repositories

            ServicesRegistration(services);
            RepositoryRegistration(services);

            #endregion

            #region EmailService

            try
            {
                services.Configure<EmailServiceOptions>(options =>
                {
                    options.SmtpServer = GetValidEnvironmentVariable("SMTP_SERVER")
                        ?? configuration["EmailService:smtpServer"]
                        ?? "localhost";

                    var smtpPortEnv = GetValidIntFromEnvironment("SMTP_PORT", 587);
                    var smtpPortConfig = configuration["EmailService:smtpPort"];
                    if (smtpPortEnv.HasValue) options.SmtpPort = smtpPortEnv.Value;
                    else if (!string.IsNullOrEmpty(smtpPortConfig) && !smtpPortConfig.Contains("${")) options.SmtpPort = int.Parse(smtpPortConfig);
                    else options.SmtpPort = 587;

                    options.SmtpUser = GetValidEnvironmentVariable("SMTP_USER")
                        ?? configuration["EmailService:smtpUser"]
                        ?? "test@localhost";

                    options.SmtpPassword = GetValidEnvironmentVariable("SMTP_PASSWORD")
                        ?? configuration["EmailService:smtpPassword"]
                        ?? "password";

                    var enableSslEnv = GetValidBoolFromEnvironment("SMTP_ENABLE_SSL", true);
                    var enableSslConfig = configuration["EmailService:EnableSsl"];
                    if (enableSslEnv.HasValue) options.EnableSsl = enableSslEnv.Value;
                    else if (!string.IsNullOrEmpty(enableSslConfig) && !enableSslConfig.Contains("${")) options.EnableSsl = bool.Parse(enableSslConfig);
                    else options.EnableSsl = true;
                });

                Console.WriteLine("✅ Email service configured");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email service configuration warning: {ex.Message}");
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

            logger.Information("DI configured for {Environment}", environment);
            services.AddSingleton<ILogger>(logger);

            #endregion

            services.AddTransient<SeedDB>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"DI failure: {ex.Message}");
            throw new InvalidOperationException("Dependency injection configuration failed", ex);
        }
    }

    private static string? GetValidEnvironmentVariable(string variableName)
    {
        var value = Environment.GetEnvironmentVariable(variableName);
        if (string.IsNullOrWhiteSpace(value) || value.Contains("${") || (value.Contains("$") && value.Contains("{") && value.Contains("}")))
            return null;
        return value;
    }

    private static int? GetValidIntFromEnvironment(string variableName, int defaultValue)
    {
        var value = GetValidEnvironmentVariable(variableName);
        if (value == null) return null;
        if (int.TryParse(value, out int result)) return result;
        Console.WriteLine($"Warning: env {variableName} invalid int: {value}");
        return null;
    }

    private static bool? GetValidBoolFromEnvironment(string variableName, bool defaultValue)
    {
        var value = GetValidEnvironmentVariable(variableName);
        if (value == null) return null;
        if (bool.TryParse(value, out bool result)) return result;
        Console.WriteLine($"Warning: env {variableName} invalid bool: {value}");
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