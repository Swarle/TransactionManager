using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Results;

namespace TransactionManager.Behaviors;

/// <summary>
/// A custom implementation of the <see cref="IFluentValidationAutoValidationResultFactory"/> interface 
/// that creates a specific result format for validation errors in ASP.NET Core applications.
/// This factory is used to generate a consistent structure for validation error responses.
/// </summary>
public class CustomResultFactory : IFluentValidationAutoValidationResultFactory
{
    /// <summary>
    /// Creates an <see cref="IActionResult"/> containing validation errors that occurred during 
    /// model binding in an ASP.NET Core controller action. The error keys are transformed 
    /// to have a lowercase initial character for consistency.
    /// </summary>
    /// <param name="context">
    /// The context for the action that is being executed. It provides access to the action, 
    /// the controller, and other related information.
    /// </param>
    /// <param name="validationProblemDetails">
    /// An object containing detailed information about the validation errors. This may be null 
    /// if no errors occurred.
    /// </param>
    /// <returns>
    /// A <see cref="BadRequestObjectResult"/> containing a custom error response with a title 
    /// and a dictionary of validation errors where each error key starts with a lowercase letter.
    /// </returns>
    public IActionResult CreateActionResult(ActionExecutingContext context,
        ValidationProblemDetails? validationProblemDetails)
    {
        var transformedErrors = validationProblemDetails?.Errors
            .ToDictionary(
                kvp => 
                {
                    if (kvp.Key.Length > 0)
                        return char.ToLowerInvariant(kvp.Key[0]) + kvp.Key.Substring(1);
                    else
                        return kvp.Key;
                },
                kvp => kvp.Value
            );
        
        return new BadRequestObjectResult(new 
        { 
            Title = "Validation errors",
            ValidationErrors = transformedErrors 
        });
    }
}