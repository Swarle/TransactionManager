using TimeZoneConverter;
using TransactionManager.Extensions;
using TransactionManager.Middlewares;

namespace TransactionManager;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddApplicationServices(builder.Configuration);
        
        builder.Services.AddSwaggerGen();

        var app = builder.Build();
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseApiExceptionMiddleware();
        
        app.UseHttpsRedirection();

        app.MapControllers();
        
        app.Run();
    }
}