using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SensorDataIngestion.API.Controllers;
using SensorDataIngestion.Application.Commands;
using SensorDataIngestion.Application.DTOs;

namespace SensorDataIngestion.Tests.API.Controllers;

public class TelemetryControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<TelemetryController>> _loggerMock;
    private readonly TelemetryController _controller;

    public TelemetryControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _loggerMock = new Mock<ILogger<TelemetryController>>();
        
        _controller = new TelemetryController(_mediatorMock.Object, _loggerMock.Object);
        
        // Configure HttpContext with API Key
        var httpContext = new DefaultHttpContext();
        httpContext.Items["ApiKey"] = "api-key-sensor-001";
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }

    [Fact]
    public async Task Post_WithValidData_ShouldReturnAccepted()
    {
        // Arrange
        var request = new TelemetryRequest
        {
            SensorId = "SENSOR-001",
            Temperature = 25.5m,
            Humidity = 60.0m
        };

        var expectedResponse = new TelemetryResponse
        {
            Id = Guid.NewGuid(),
            SensorId = "SENSOR-001",
            Message = "Telemetry received and queued successfully",
            ReceivedAt = DateTime.UtcNow,
            Success = true
        };

        _mediatorMock
            .Setup(x => x.Send(It.IsAny<ProcessTelemetryCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Post(request);

        // Assert
        var acceptedResult = result.Should().BeOfType<AcceptedResult>().Subject;
        var response = acceptedResult.Value.Should().BeOfType<TelemetryResponse>().Subject;
        response.Success.Should().BeTrue();
        response.SensorId.Should().Be("SENSOR-001");
    }

    [Fact]
    public async Task Post_WithProcessingFailure_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new TelemetryRequest
        {
            SensorId = "SENSOR-001",
            Temperature = 25.5m,
            Humidity = 60.0m
        };

        var expectedResponse = new TelemetryResponse
        {
            Id = Guid.Empty,
            SensorId = "SENSOR-001",
            Message = "Unauthorized sensor",
            ReceivedAt = DateTime.UtcNow,
            Success = false
        };

        _mediatorMock
            .Setup(x => x.Send(It.IsAny<ProcessTelemetryCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Post(request);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var response = badRequestResult.Value.Should().BeOfType<TelemetryResponse>().Subject;
        response.Success.Should().BeFalse();
    }

    [Fact]
    public async Task Post_ShouldSendApiKeyInCommand()
    {
        // Arrange
        var request = new TelemetryRequest
        {
            SensorId = "SENSOR-001",
            Temperature = 25.5m,
            Humidity = 60.0m
        };

        ProcessTelemetryCommand? capturedCommand = null;

        _mediatorMock
            .Setup(x => x.Send(It.IsAny<ProcessTelemetryCommand>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<TelemetryResponse>, CancellationToken>((c, _) => capturedCommand = c as ProcessTelemetryCommand)
            .ReturnsAsync(new TelemetryResponse { Success = true });

        // Act
        await _controller.Post(request);

        // Assert
        capturedCommand.Should().NotBeNull();
        capturedCommand!.ApiKey.Should().Be("api-key-sensor-001");
        capturedCommand.SensorId.Should().Be("SENSOR-001");
    }

    [Fact]
    public void Health_ShouldReturnOk()
    {
        // Act
        var result = _controller.Health();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().NotBeNull();
    }
}
