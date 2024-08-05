using Microsoft.EntityFrameworkCore;
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
        
        return services;
    }
}