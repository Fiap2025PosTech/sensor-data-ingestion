using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using SensorDataIngestion.API.Filters;

namespace SensorDataIngestion.Tests.API.Filters;

public class ApiKeyAuthAttributeTests
{
    private readonly ApiKeyAuthAttribute _attribute;

    public ApiKeyAuthAttributeTests()
    {
        _attribute = new ApiKeyAuthAttribute();
    }

    [Fact]
    public async Task OnActionExecutionAsync_WithValidApiKey_ShouldExecuteNextFilter()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["X-Api-Key"] = "valid-api-key";

        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            new ActionDescriptor());

        var context = new ActionExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new Dictionary<string, object?>(),
            new object());

        var nextCalled = false;
        var next = new ActionExecutionDelegate(() =>
        {
            nextCalled = true;
            return Task.FromResult(new ActionExecutedContext(actionContext, new List<IFilterMetadata>(), new object()));
        });

        // Act
        await _attribute.OnActionExecutionAsync(context, next);

        // Assert
        nextCalled.Should().BeTrue();
        httpContext.Items["ApiKey"].Should().Be("valid-api-key");
        context.Result.Should().BeNull();
    }

    [Fact]
    public async Task OnActionExecutionAsync_WithoutApiKey_ShouldReturnUnauthorized()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        // Do not add X-Api-Key header

        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            new ActionDescriptor());

        var context = new ActionExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new Dictionary<string, object?>(),
            new object());

        var nextCalled = false;
        var next = new ActionExecutionDelegate(() =>
        {
            nextCalled = true;
            return Task.FromResult(new ActionExecutedContext(actionContext, new List<IFilterMetadata>(), new object()));
        });

        // Act
        await _attribute.OnActionExecutionAsync(context, next);

        // Assert
        nextCalled.Should().BeFalse();
        context.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task OnActionExecutionAsync_WithEmptyApiKey_ShouldReturnUnauthorized()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["X-Api-Key"] = "";

        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            new ActionDescriptor());

        var context = new ActionExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new Dictionary<string, object?>(),
            new object());

        var nextCalled = false;
        var next = new ActionExecutionDelegate(() =>
        {
            nextCalled = true;
            return Task.FromResult(new ActionExecutedContext(actionContext, new List<IFilterMetadata>(), new object()));
        });

        // Act
        await _attribute.OnActionExecutionAsync(context, next);

        // Assert
        nextCalled.Should().BeFalse();
        context.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }
}
