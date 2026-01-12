using SensorDataIngestion.Domain.Entities;

namespace SensorDataIngestion.Tests.Shared.Builders;

/// <summary>
/// Builder for creating Sensor objects for tests
/// </summary>
public class SensorBuilder
{
    private string _sensorId = "SENSOR-TEST";
    private string _apiKey = "api-key-test";
    private string _name = "Test Sensor";
    private string _location = "Test Location";
    private bool _isActive = true;

    public SensorBuilder WithSensorId(string sensorId)
    {
        _sensorId = sensorId;
        return this;
    }

    public SensorBuilder WithApiKey(string apiKey)
    {
        _apiKey = apiKey;
        return this;
    }

    public SensorBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public SensorBuilder WithLocation(string location)
    {
        _location = location;
        return this;
    }

    public SensorBuilder Active()
    {
        _isActive = true;
        return this;
    }

    public SensorBuilder Inactive()
    {
        _isActive = false;
        return this;
    }

    public Sensor Build()
    {
        var sensor = new Sensor(_sensorId, _apiKey, _name, _location);
        
        if (!_isActive)
        {
            sensor.Deactivate();
        }
        
        return sensor;
    }

    public static SensorBuilder New() => new();
}
