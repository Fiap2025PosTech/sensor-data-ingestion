namespace SensorDataIngestion.Application.DTOs;

/// <summary>
/// DTO for receiving telemetry data
/// </summary>
public record TelemetryRequest
{
    public string SensorId { get; init; } = string.Empty;
    public decimal Temperature { get; init; }
    public decimal Humidity { get; init; }
    public DateTime? ReadingTimestamp { get; init; }
}
