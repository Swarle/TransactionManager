using System.Reflection;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TransactionManager.Behaviors;
using TransactionManager.DataAccess;
using TransactionManager.DataAccess.Interfaces;
using TransactionManager.Persistence;
using TransactionManager.Services;
using TransactionManager.Services.Interfaces;
using TransactionManager.StaticConstants;

namespace TransactionManager.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(opt =>
            opt.UseNpgsql(configuration.GetConnectionString(SD.DefaultConnectionStringName)));

        services.AddSingleton(_ =>
        {
            var connectionString = configuration.GetConnectionString(SD.DefaultConnectionStringName) ??
                        throw new NullReferenceException("The connection period is not specified in the settings");;
            
            return new SqlConnectionFactory(connectionString);
        });
        
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<ITransactionDataAccess, TransactionDataAccess>();

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        
        return services;
    }

    public static IServiceCollection AddSwaggerGenConfigured(this IServiceCollection services)
    {
        services.AddSwaggerGen(opt =>
        {
            opt.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "TransactionManager",
                Version = "v1"
            });
            
            opt.OperationFilter<AddTimezoneHeaderParameter>();
        });
        
        return services;
    }
}