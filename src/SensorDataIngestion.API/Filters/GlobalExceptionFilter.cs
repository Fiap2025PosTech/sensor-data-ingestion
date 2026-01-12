using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace SensorDataIngestion.API.Filters;

/// <summary>
/// Global exception handling filter
/// </summary>
public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;
        
        _logger.LogError(exception, "Unhandled error: {Message}", exception.Message);

        object response;
        HttpStatusCode statusCode;

        if (exception is ValidationException validationException)
        {
            response = new
            {
                Code = "VALIDATION_ERROR",
                Message = "Validation error",
                Errors = validationException.Errors.Select(e => new { Field = e.PropertyName, Error = e.ErrorMessage })
            };
            statusCode = HttpStatusCode.BadRequest;
        }
        else if (exception is UnauthorizedAccessException)
        {
            response = new
            {
                Code = "UNAUTHORIZED",
                Message = "Unauthorized access"
            };
            statusCode = HttpStatusCode.Unauthorized;
        }
        else
        {
            response = new
            {
                Code = "INTERNAL_ERROR",
                Message = "Internal server error"
            };
            statusCode = HttpStatusCode.InternalServerError;
        }

        context.Result = new ObjectResult(response)
        {
            StatusCode = (int)statusCode
        };

        context.ExceptionHandled = true;
    }
}
