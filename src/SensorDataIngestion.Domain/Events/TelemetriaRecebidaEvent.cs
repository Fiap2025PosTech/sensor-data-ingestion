namespace SensorDataIngestion.Domain.Events;

/// <summary>
/// Telemetry received event for publishing to the message broker
/// </summary>
public record TelemetryReceivedEvent
{
    public Guid Id { get; init; }
    public string SensorId { get; init; } = string.Empty;
    public decimal Temperature { get; init; }
    public decimal Humidity { get; init; }
    public DateTime ReadingTimestamp { get; init; }
    public DateTime ReceivedAt { get; init; }
    public DateTime PublishedAt { get; init; }
}
