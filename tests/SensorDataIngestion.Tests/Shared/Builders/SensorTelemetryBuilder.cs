using SensorDataIngestion.Domain.Entities;

namespace SensorDataIngestion.Tests.Shared.Builders;

/// <summary>
/// Builder for creating SensorTelemetry objects for tests
/// </summary>
public class SensorTelemetryBuilder
{
    private string _sensorId = "SENSOR-TEST";
    private decimal _temperature = 25.0m;
    private decimal _humidity = 50.0m;
    private DateTime? _readingTimestamp = null;

    public SensorTelemetryBuilder WithSensorId(string sensorId)
    {
        _sensorId = sensorId;
        return this;
    }

    public SensorTelemetryBuilder WithTemperature(decimal temperature)
    {
        _temperature = temperature;
        return this;
    }

    public SensorTelemetryBuilder WithHumidity(decimal humidity)
    {
        _humidity = humidity;
        return this;
    }

    public SensorTelemetryBuilder WithReadingTimestamp(DateTime readingTimestamp)
    {
        _readingTimestamp = readingTimestamp;
        return this;
    }

    public SensorTelemetryBuilder WithValidData()
    {
        _sensorId = "SENSOR-001";
        _temperature = 25.5m;
        _humidity = 60.0m;
        return this;
    }

    public SensorTelemetryBuilder WithInvalidTemperature()
    {
        _temperature = 150.0m;
        return this;
    }

    public SensorTelemetryBuilder WithInvalidHumidity()
    {
        _humidity = 150.0m;
        return this;
    }

    public SensorTelemetry Build()
    {
        return new SensorTelemetry(_sensorId, _temperature, _humidity, _readingTimestamp);
    }

    public static SensorTelemetryBuilder New() => new();
}
