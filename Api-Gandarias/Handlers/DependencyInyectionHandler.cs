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
        IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");

        IConfiguration configuration = builder.Build();

        services.AddSingleton(configuration);

        #region PgSQL

        services.AddDbContext<DBContext>(opt => opt.UseNpgsql(configuration.GetConnectionString("PgSQL")));

        #endregion PgSQL

        #region Automapper

        services.AddAutoMapper(Assembly.Load("CC.Domain"));

        #endregion Automapper

        #region ServiceRegistrarion

        ServicesRegistration(services);

        #endregion ServiceRegistrarion

        #region RepositoriesRegistrarion

        RepositoryRegistration(services);

        #endregion RepositoriesRegistrarion

        #region EmailService

        services.Configure<EmailServiceOptions>(
            configuration.GetSection("EmailService")
        );

        #endregion EmailService

        services.AddSingleton<ExceptionControl>();

        #region Logs

        Logger logger = new LoggerConfiguration()
            .WriteTo
            .File("log.txt",
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        logger.Information("Done setting up serilog - Application starting up");

        services.AddSingleton<ILogger>(logger);

        #endregion Logs

        services.AddTransient<SeedDB>();
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
        //services.AddScoped<IAuditService, AuditService>();
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
        //services.AddScoped<IAuditRepository, AuditRepository>();
    }
}