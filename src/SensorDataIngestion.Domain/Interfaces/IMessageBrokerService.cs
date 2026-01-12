using SensorDataIngestion.Domain.Entities;

namespace SensorDataIngestion.Domain.Interfaces;

/// <summary>
/// Messaging service interface for publishing events
/// </summary>
public interface IMessageBrokerService
{
    Task PublishTelemetryAsync(SensorTelemetry telemetry, CancellationToken cancellationToken = default);
}
