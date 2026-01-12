using Microsoft.Extensions.Logging;
using SensorDataIngestion.Application.Commands;
using SensorDataIngestion.Application.Handlers;
using SensorDataIngestion.Domain.Entities;
using SensorDataIngestion.Domain.Interfaces;

namespace SensorDataIngestion.Tests.Application.Handlers;

public class ProcessTelemetryHandlerTests
{
    private readonly Mock<IMessageBrokerService> _messageBrokerServiceMock;
    private readonly Mock<ISensorRepository> _sensorRepositoryMock;
    private readonly Mock<ILogger<ProcessTelemetryHandler>> _loggerMock;
    private readonly ProcessTelemetryHandler _handler;

    public ProcessTelemetryHandlerTests()
    {
        _messageBrokerServiceMock = new Mock<IMessageBrokerService>();
        _sensorRepositoryMock = new Mock<ISensorRepository>();
        _loggerMock = new Mock<ILogger<ProcessTelemetryHandler>>();
        
        _handler = new ProcessTelemetryHandler(
            _messageBrokerServiceMock.Object,
            _sensorRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidData_ShouldPublishToQueueAndReturnSuccess()
    {
        // Arrange
        var command = new ProcessTelemetryCommand
        {
            SensorId = "SENSOR-001",
            Temperature = 25.5m,
            Humidity = 60.0m,
            ApiKey = "api-key-sensor-001"
        };

        _sensorRepositoryMock
            .Setup(x => x.ValidateApiKeyAsync(command.SensorId, command.ApiKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _messageBrokerServiceMock
            .Setup(x => x.PublishTelemetryAsync(It.IsAny<SensorTelemetry>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.SensorId.Should().Be(command.SensorId);
        result.Id.Should().NotBeEmpty();
        result.Message.Should().Contain("success");

        _messageBrokerServiceMock.Verify(
            x => x.PublishTelemetryAsync(It.IsAny<SensorTelemetry>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithInvalidApiKey_ShouldReturnFailure()
    {
        // Arrange
        var command = new ProcessTelemetryCommand
        {
            SensorId = "SENSOR-001",
            Temperature = 25.5m,
            Humidity = 60.0m,
            ApiKey = "invalid-api-key"
        };

        _sensorRepositoryMock
            .Setup(x => x.ValidateApiKeyAsync(command.SensorId, command.ApiKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Id.Should().BeEmpty();
        result.Message.Should().Contain("Unauthorized");

        _messageBrokerServiceMock.Verify(
            x => x.PublishTelemetryAsync(It.IsAny<SensorTelemetry>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Theory]
    [InlineData("", 25.5, 60.0)]
    [InlineData("SENSOR-001", -100, 60.0)]
    [InlineData("SENSOR-001", 25.5, 150)]
    public async Task Handle_WithInvalidData_ShouldReturnFailure(string sensorId, decimal temperature, decimal humidity)
    {
        // Arrange
        var command = new ProcessTelemetryCommand
        {
            SensorId = sensorId,
            Temperature = temperature,
            Humidity = humidity,
            ApiKey = "api-key-sensor-001"
        };

        _sensorRepositoryMock
            .Setup(x => x.ValidateApiKeyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Invalid");

        _messageBrokerServiceMock.Verify(
            x => x.PublishTelemetryAsync(It.IsAny<SensorTelemetry>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WithSpecificReadingTimestamp_ShouldUseProvidedDate()
    {
        // Arrange
        var readingTimestamp = new DateTime(2025, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var command = new ProcessTelemetryCommand
        {
            SensorId = "SENSOR-001",
            Temperature = 25.5m,
            Humidity = 60.0m,
            ApiKey = "api-key-sensor-001",
            ReadingTimestamp = readingTimestamp
        };

        _sensorRepositoryMock
            .Setup(x => x.ValidateApiKeyAsync(command.SensorId, command.ApiKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        SensorTelemetry? capturedTelemetry = null;
        _messageBrokerServiceMock
            .Setup(x => x.PublishTelemetryAsync(It.IsAny<SensorTelemetry>(), It.IsAny<CancellationToken>()))
            .Callback<SensorTelemetry, CancellationToken>((t, _) => capturedTelemetry = t)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        capturedTelemetry.Should().NotBeNull();
        capturedTelemetry!.ReadingTimestamp.Should().Be(readingTimestamp);
    }
}
