namespace SensorDataIngestion.Domain.Entities;

/// <summary>
/// Entity representing a registered sensor
/// </summary>
public class Sensor
{
    public Guid Id { get; private set; }
    public string SensorId { get; private set; } = string.Empty;
    public string ApiKey { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Location { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastReadingAt { get; private set; }

    private Sensor() { }

    public Sensor(string sensorId, string apiKey, string name, string location)
    {
        Id = Guid.NewGuid();
        SensorId = sensorId ?? throw new ArgumentNullException(nameof(sensorId));
        ApiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Location = location ?? string.Empty;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public bool ValidateApiKey(string apiKey)
    {
        return IsActive && ApiKey == apiKey;
    }

    public void UpdateLastReading()
    {
        LastReadingAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }
}
