using CC.Application.Services;
using CC.Domain.Interfaces.Repositories;
using CC.Domain.Interfaces.Services;
using CC.Infrastructure.Configurations;
using CC.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Serilog;
using Serilog.Core;
using ILogger = Serilog.ILogger;
using Microsoft.Extensions.Configuration;

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
    }
}