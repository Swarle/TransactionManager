﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Results;

namespace TransactionManager.Behaviors;

public class CustomResultFactory : IFluentValidationAutoValidationResultFactory
{
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