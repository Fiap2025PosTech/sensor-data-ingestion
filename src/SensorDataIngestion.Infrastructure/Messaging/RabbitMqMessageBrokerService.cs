using MassTransit;
using Microsoft.Extensions.Logging;
using SensorDataIngestion.Domain.Entities;
using SensorDataIngestion.Domain.Events;
using SensorDataIngestion.Domain.Interfaces;

namespace SensorDataIngestion.Infrastructure.Messaging;

/// <summary>
/// Messaging service implementation using MassTransit/RabbitMQ
/// </summary>
public class RabbitMqMessageBrokerService : IMessageBrokerService
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<RabbitMqMessageBrokerService> _logger;

    public RabbitMqMessageBrokerService(
        IPublishEndpoint publishEndpoint,
        ILogger<RabbitMqMessageBrokerService> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task PublishTelemetryAsync(SensorTelemetry telemetry, CancellationToken cancellationToken = default)
    {
        var @event = new TelemetryReceivedEvent
        {
            Id = telemetry.Id,
            SensorId = telemetry.SensorId,
            Temperature = telemetry.Temperature,
            Humidity = telemetry.Humidity,
            ReadingTimestamp = telemetry.ReadingTimestamp,
            ReceivedAt = telemetry.ReceivedAt,
            PublishedAt = DateTime.UtcNow
        };

        _logger.LogInformation(
            "Publishing telemetry event for sensor {SensorId} - Temp: {Temperature}°C, Humidity: {Humidity}%",
            telemetry.SensorId,
            telemetry.Temperature,
            telemetry.Humidity);

        await _publishEndpoint.Publish(@event, cancellationToken);

        _logger.LogInformation("Telemetry event published successfully for sensor {SensorId}", telemetry.SensorId);
    }
}
