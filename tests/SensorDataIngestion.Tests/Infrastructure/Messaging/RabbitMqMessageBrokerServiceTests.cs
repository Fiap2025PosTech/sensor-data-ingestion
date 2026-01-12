using MassTransit;
using Microsoft.Extensions.Logging;
using SensorDataIngestion.Domain.Entities;
using SensorDataIngestion.Domain.Events;
using SensorDataIngestion.Infrastructure.Messaging;

namespace SensorDataIngestion.Tests.Infrastructure.Messaging;

public class RabbitMqMessageBrokerServiceTests
{
    private readonly Mock<IPublishEndpoint> _publishEndpointMock;
    private readonly Mock<ILogger<RabbitMqMessageBrokerService>> _loggerMock;
    private readonly RabbitMqMessageBrokerService _service;

    public RabbitMqMessageBrokerServiceTests()
    {
        _publishEndpointMock = new Mock<IPublishEndpoint>();
        _loggerMock = new Mock<ILogger<RabbitMqMessageBrokerService>>();
        
        _service = new RabbitMqMessageBrokerService(
            _publishEndpointMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task PublishTelemetryAsync_WithValidTelemetry_ShouldPublishEvent()
    {
        // Arrange
        var telemetry = new SensorTelemetry("SENSOR-001", 25.5m, 60.0m);

        _publishEndpointMock
            .Setup(x => x.Publish(It.IsAny<TelemetryReceivedEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.PublishTelemetryAsync(telemetry);

        // Assert
        _publishEndpointMock.Verify(
            x => x.Publish(It.IsAny<TelemetryReceivedEvent>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task PublishTelemetryAsync_ShouldConvertTelemetryToEvent()
    {
        // Arrange
        var telemetry = new SensorTelemetry("SENSOR-001", 25.5m, 60.0m);
        TelemetryReceivedEvent? capturedEvent = null;

        _publishEndpointMock
            .Setup(x => x.Publish(It.IsAny<TelemetryReceivedEvent>(), It.IsAny<CancellationToken>()))
            .Callback<TelemetryReceivedEvent, CancellationToken>((e, _) => capturedEvent = e)
            .Returns(Task.CompletedTask);

        // Act
        await _service.PublishTelemetryAsync(telemetry);

        // Assert
        capturedEvent.Should().NotBeNull();
        capturedEvent!.SensorId.Should().Be("SENSOR-001");
        capturedEvent.Temperature.Should().Be(25.5m);
        capturedEvent.Humidity.Should().Be(60.0m);
        capturedEvent.Id.Should().Be(telemetry.Id);
        capturedEvent.ReadingTimestamp.Should().Be(telemetry.ReadingTimestamp);
        capturedEvent.ReceivedAt.Should().Be(telemetry.ReceivedAt);
        capturedEvent.PublishedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task PublishTelemetryAsync_WithCancellationToken_ShouldPassToken()
    {
        // Arrange
        var telemetry = new SensorTelemetry("SENSOR-001", 25.5m, 60.0m);
        var cancellationToken = new CancellationTokenSource().Token;

        _publishEndpointMock
            .Setup(x => x.Publish(It.IsAny<TelemetryReceivedEvent>(), cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        await _service.PublishTelemetryAsync(telemetry, cancellationToken);

        // Assert
        _publishEndpointMock.Verify(
            x => x.Publish(It.IsAny<TelemetryReceivedEvent>(), cancellationToken),
            Times.Once);
    }
}
