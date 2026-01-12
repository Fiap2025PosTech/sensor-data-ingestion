using SensorDataIngestion.Application.Commands;

namespace SensorDataIngestion.Tests.Shared.Builders;

/// <summary>
/// Builder for creating ProcessTelemetryCommand for tests
/// </summary>
public class ProcessTelemetryCommandBuilder
{
    private string _sensorId = "SENSOR-TEST";
    private decimal _temperature = 25.0m;
    private decimal _humidity = 50.0m;
    private DateTime? _readingTimestamp = null;
    private string _apiKey = "api-key-test";

    public ProcessTelemetryCommandBuilder WithSensorId(string sensorId)
    {
        _sensorId = sensorId;
        return this;
    }

    public ProcessTelemetryCommandBuilder WithTemperature(decimal temperature)
    {
        _temperature = temperature;
        return this;
    }

    public ProcessTelemetryCommandBuilder WithHumidity(decimal humidity)
    {
        _humidity = humidity;
        return this;
    }

    public ProcessTelemetryCommandBuilder WithReadingTimestamp(DateTime readingTimestamp)
    {
        _readingTimestamp = readingTimestamp;
        return this;
    }

    public ProcessTelemetryCommandBuilder WithApiKey(string apiKey)
    {
        _apiKey = apiKey;
        return this;
    }

    public ProcessTelemetryCommandBuilder WithValidData()
    {
        _sensorId = "SENSOR-001";
        _temperature = 25.5m;
        _humidity = 60.0m;
        _apiKey = "api-key-sensor-001";
        return this;
    }

    public ProcessTelemetryCommand Build()
    {
        return new ProcessTelemetryCommand
        {
            SensorId = _sensorId,
            Temperature = _temperature,
            Humidity = _humidity,
            ReadingTimestamp = _readingTimestamp,
            ApiKey = _apiKey
        };
    }

    public static ProcessTelemetryCommandBuilder New() => new();
}
