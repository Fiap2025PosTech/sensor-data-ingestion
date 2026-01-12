namespace SensorDataIngestion.API.Models;

/// <summary>
/// Response with generated token
/// </summary>
public record TokenResponse
{
    public string Token { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
    public string TokenType { get; init; } = "Bearer";
}
