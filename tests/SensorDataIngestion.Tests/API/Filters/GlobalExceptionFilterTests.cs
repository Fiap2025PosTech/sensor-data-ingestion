using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using SensorDataIngestion.API.Filters;

namespace SensorDataIngestion.Tests.API.Filters;

public class GlobalExceptionFilterTests
{
    private readonly Mock<ILogger<GlobalExceptionFilter>> _loggerMock;
    private readonly GlobalExceptionFilter _filter;

    public GlobalExceptionFilterTests()
    {
        _loggerMock = new Mock<ILogger<GlobalExceptionFilter>>();
        _filter = new GlobalExceptionFilter(_loggerMock.Object);
    }

    [Fact]
    public void OnException_WithValidationException_ShouldReturnBadRequest()
    {
        // Arrange
        var failures = new List<ValidationFailure>
        {
            new("SensorId", "SensorId is required"),
            new("Temperature", "Invalid temperature")
        };
        var exception = new ValidationException(failures);

        var context = CreateExceptionContext(exception);

        // Act
        _filter.OnException(context);

        // Assert
        context.ExceptionHandled.Should().BeTrue();
        var result = context.Result.Should().BeOfType<ObjectResult>().Subject;
        result.StatusCode.Should().Be(400);
    }

    [Fact]
    public void OnException_WithUnauthorizedAccessException_ShouldReturnUnauthorized()
    {
        // Arrange
        var exception = new UnauthorizedAccessException("Access denied");
        var context = CreateExceptionContext(exception);

        // Act
        _filter.OnException(context);

        // Assert
        context.ExceptionHandled.Should().BeTrue();
        var result = context.Result.Should().BeOfType<ObjectResult>().Subject;
        result.StatusCode.Should().Be(401);
    }

    [Fact]
    public void OnException_WithGenericException_ShouldReturnInternalServerError()
    {
        // Arrange
        var exception = new Exception("Internal error");
        var context = CreateExceptionContext(exception);

        // Act
        _filter.OnException(context);

        // Assert
        context.ExceptionHandled.Should().BeTrue();
        var result = context.Result.Should().BeOfType<ObjectResult>().Subject;
        result.StatusCode.Should().Be(500);
    }

    [Fact]
    public void OnException_ShouldMarkExceptionAsHandled()
    {
        // Arrange
        var exception = new Exception("Any error");
        var context = CreateExceptionContext(exception);

        // Act
        _filter.OnException(context);

        // Assert
        context.ExceptionHandled.Should().BeTrue();
    }

    private static ExceptionContext CreateExceptionContext(Exception exception)
    {
        var httpContext = new DefaultHttpContext();
        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            new ActionDescriptor());

        return new ExceptionContext(actionContext, new List<IFilterMetadata>())
        {
            Exception = exception
        };
    }
}
