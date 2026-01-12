namespace SensorDataIngestion.API.Models;

/// <summary>
/// Request for token generation
/// </summary>
public record TokenRequest
{
    public string Subject { get; init; } = "test-user";
    public string? Name { get; init; }
    public Dictionary<string, string>? Claims { get; init; }
}
