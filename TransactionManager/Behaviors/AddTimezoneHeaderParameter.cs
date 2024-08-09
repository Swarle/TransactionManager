using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using TransactionManager.StaticConstants;

namespace TransactionManager.Behaviors;

/// <summary>
/// This class implements the <see cref="IOperationFilter"/> interface to add a custom header parameter 
/// to Swagger-generated API documentation. The header parameter added represents the user's timezone 
/// in IANA format, which can be used in API requests.
/// </summary>
public class AddTimezoneHeaderParameter : IOperationFilter
{
    /// <summary>
    /// Applies the custom header parameter to the API operation.
    /// If the operation's parameter list is null, it initializes a new list. 
    /// Then, it adds a parameter to the operation's parameters, representing 
    /// the <see cref="SD.UserTimezoneHeaderKey"/> header, which contains the user's timezone in IANA format.
    /// </summary>
    /// <param name="operation">
    /// The operation to which the header parameter will be added. 
    /// This represents an API endpoint in the Swagger-generated documentation.
    /// </param>
    /// <param name="context">
    /// The filter context which provides access to various aspects of the API operation, 
    /// such as its metadata, route information, and more.
    /// </param>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Parameters == null)
            operation.Parameters = new List<OpenApiParameter>();
        
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = SD.UserTimezoneHeaderKey,
            In = ParameterLocation.Header,
            Description = "Timezone in IANA format",
            Required = false,
            Schema = new OpenApiSchema
            {
                Type = "string"
            }
        });
    }
}