namespace SensorDataIngestion.Domain.Entities;

/// <summary>
/// Entity representing sensor telemetry data
/// </summary>
public class SensorTelemetry
{
    public Guid Id { get; private set; }
    public string SensorId { get; private set; } = string.Empty;
    public decimal Temperature { get; private set; }
    public decimal Humidity { get; private set; }
    public DateTime ReadingTimestamp { get; private set; }
    public DateTime ReceivedAt { get; private set; }

    private SensorTelemetry() { }

    public SensorTelemetry(string sensorId, decimal temperature, decimal humidity, DateTime? readingTimestamp = null)
    {
        Id = Guid.NewGuid();
        SensorId = sensorId ?? throw new ArgumentNullException(nameof(sensorId));
        Temperature = temperature;
        Humidity = humidity;
        ReadingTimestamp = readingTimestamp ?? DateTime.UtcNow;
        ReceivedAt = DateTime.UtcNow;
    }

    public bool ValidateData()
    {
        return !string.IsNullOrWhiteSpace(SensorId) &&
               Humidity >= 0 && Humidity <= 100 &&
               Temperature >= -50 && Temperature <= 100;
    }
}
