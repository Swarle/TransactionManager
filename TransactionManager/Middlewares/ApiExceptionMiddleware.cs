using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TransactionManager.Exceptions;
using TransactionManager.StaticConstants;

namespace TransactionManager.Middlewares;

/// <summary>
/// Middleware that handles exceptions globally in an ASP.NET Core application. 
/// It logs exceptions, manages custom HTTP exceptions, and formats error responses in JSON.
/// </summary>
public class ApiExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;
    private readonly JsonSerializerSettings _serializerSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiExceptionMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the request pipeline.</param>
    /// <param name="logger">The logger to log exceptions.</param>
    /// <param name="env">The hosting environment to determine the mode (Development or Production).</param>
    public ApiExceptionMiddleware(RequestDelegate next,
        ILogger<ApiExceptionMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;

        // JSON serializer settings for consistent response formatting.
        _serializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            },
            Formatting = Formatting.Indented,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        };
    }

    /// <summary>
    /// Invokes the middleware with the current HTTP context, handling any exceptions thrown.
    /// </summary>
    /// <param name="httpContext">The current HTTP context.</param>
    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (HttpException ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
        catch (Exception ex)
        {
            await HandleInternalServerError(httpContext, ex);
        }
    }

    /// <summary>
    /// Handles HTTP-specific exceptions, setting the appropriate status code and JSON response.
    /// </summary>
    /// <param name="context">The HTTP context for the current request.</param>
    /// <param name="ex">The HTTP-specific exception.</param>
    private async Task HandleExceptionAsync(HttpContext context, HttpException ex)
    {
        if (ex.StatusCode == HttpStatusCode.InternalServerError)
        {
            await HandleInternalServerError(context, ex);
        }
        else
        {
            _logger.LogError(ex, ex.ErrorMessages.ToString());
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int) ex.StatusCode;

            var jsonResponse = JsonConvert.SerializeObject(ex.ErrorMessages);

            await context.Response.WriteAsync(jsonResponse);
        }
    }

    /// <summary>
    /// Handles generic server errors (500 Internal Server Error), providing detailed information 
    /// in development mode and logging the error.
    /// </summary>
    /// <param name="context">The HTTP context for the current request.</param>
    /// <param name="ex">The unhandled exception.</param>
    private async Task HandleInternalServerError(HttpContext context, Exception ex)
    {
        _logger.LogError(ex, ex.Message);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;

        var response = new Dictionary<string, string>
        {
            { SD.DefaultErrorKey, ex.Message }
        };

        if (_env.IsDevelopment() && ex.StackTrace is not null)
        {
            response.Add(SD.StackTraceErrorKey, ex.StackTrace);
        }

        var json = JsonConvert.SerializeObject(response, _serializerSettings);

        await context.Response.WriteAsync(json);
    }
}

/// <summary>
/// Provides extension methods to <see cref="IApplicationBuilder"/>
/// </summary>
public static class ApiExceptionMiddlewareExtensions
{
    /// <summary>
    /// Extension method to add the <see cref="ApiExceptionMiddleware"/> to the ASP.NET Core request pipeline.
    /// </summary>
    /// <param name="builder">The application builder.</param>
    /// <returns>The application builder with the exception middleware configured.</returns>
    public static IApplicationBuilder UseApiExceptionMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ApiExceptionMiddleware>();
    }
}