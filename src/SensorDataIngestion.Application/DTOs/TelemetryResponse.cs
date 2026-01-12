namespace SensorDataIngestion.Application.DTOs;

/// <summary>
/// DTO for telemetry response
/// </summary>
public record TelemetryResponse
{
    public Guid Id { get; init; }
    public string SensorId { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public DateTime ReceivedAt { get; init; }
    public bool Success { get; init; }
}
