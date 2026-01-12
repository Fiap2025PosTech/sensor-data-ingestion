using MediatR;
using SensorDataIngestion.Application.DTOs;

namespace SensorDataIngestion.Application.Commands;

/// <summary>
/// Command to process received telemetry
/// </summary>
public record ProcessTelemetryCommand : IRequest<TelemetryResponse>
{
    public string SensorId { get; init; } = string.Empty;
    public decimal Temperature { get; init; }
    public decimal Humidity { get; init; }
    public DateTime? ReadingTimestamp { get; init; }
    public string ApiKey { get; init; } = string.Empty;
}
