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

/// <summary>
/// Provides extension methods to <see cref="IServiceCollection"/>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Extension method for IServiceCollection that adds application services
    /// </summary>
    /// <param name="services">An <see cref="IServiceCollection"/> object.</param>
    /// <param name="configuration">An <see cref="IConfiguration"/> object.</param>
    /// <returns>An <see cref="IServiceCollection"/> object.</returns>
    /// <exception cref="NullReferenceException">
    /// Throws an exception if no connection string was defined
    /// </exception>
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
    
    /// <summary>
    /// An extension method that adds a configurable SwaggerGen
    /// </summary>
    /// <param name="services">An <see cref="IServiceCollection"/> object.</param>
    /// <returns>An <see cref="IServiceCollection"/> object.</returns>
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
            
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            opt.IncludeXmlComments(xmlPath);
        });
        
        return services;
    }
}