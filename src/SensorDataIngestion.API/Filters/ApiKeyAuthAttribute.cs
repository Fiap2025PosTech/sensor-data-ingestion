using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SensorDataIngestion.API.Filters;

/// <summary>
/// Filter for sensor API Key validation
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ApiKeyAuthAttribute : Attribute, IAsyncActionFilter
{
    private const string ApiKeyHeaderName = "X-Api-Key";

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
        {
            context.Result = new UnauthorizedObjectResult(new 
            { 
                Message = "API Key not provided",
                Code = "API_KEY_MISSING"
            });
            return;
        }

        var apiKey = extractedApiKey.ToString();
        
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            context.Result = new UnauthorizedObjectResult(new 
            { 
                Message = "Invalid API Key",
                Code = "API_KEY_INVALID"
            });
            return;
        }

        // Store the API Key in HttpContext for later use
        context.HttpContext.Items["ApiKey"] = apiKey;

        await next();
    }
}
