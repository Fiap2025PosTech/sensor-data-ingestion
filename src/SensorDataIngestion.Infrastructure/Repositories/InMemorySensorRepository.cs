using SensorDataIngestion.Domain.Entities;
using SensorDataIngestion.Domain.Interfaces;
using System.Collections.Concurrent;

namespace SensorDataIngestion.Infrastructure.Repositories;

/// <summary>
/// In-memory implementation of the sensor repository (for development/testing)
/// In production, replace with a database implementation
/// </summary>
public class InMemorySensorRepository : ISensorRepository
{
    private readonly ConcurrentDictionary<string, Sensor> _sensors = new();

    public InMemorySensorRepository()
    {
        // Add sample sensors for development
        var sensor1 = new Sensor("SENSOR-001", "api-key-sensor-001", "Temperature Sensor Room 1", "Room 1");
        var sensor2 = new Sensor("SENSOR-002", "api-key-sensor-002", "Temperature Sensor Room 2", "Room 2");
        var sensor3 = new Sensor("SENSOR-003", "api-key-sensor-003", "Humidity Sensor Laboratory", "Laboratory");
        
        _sensors.TryAdd(sensor1.SensorId, sensor1);
        _sensors.TryAdd(sensor2.SensorId, sensor2);
        _sensors.TryAdd(sensor3.SensorId, sensor3);
    }

    public Task<Sensor?> GetBySensorIdAsync(string sensorId, CancellationToken cancellationToken = default)
    {
        _sensors.TryGetValue(sensorId, out var sensor);
        return Task.FromResult(sensor);
    }

    public Task<Sensor?> GetByApiKeyAsync(string apiKey, CancellationToken cancellationToken = default)
    {
        var sensor = _sensors.Values.FirstOrDefault(s => s.ApiKey == apiKey);
        return Task.FromResult(sensor);
    }

    public Task<bool> ValidateApiKeyAsync(string sensorId, string apiKey, CancellationToken cancellationToken = default)
    {
        if (_sensors.TryGetValue(sensorId, out var sensor))
        {
            return Task.FromResult(sensor.ValidateApiKey(apiKey));
        }
        return Task.FromResult(false);
    }

    public Task AddAsync(Sensor sensor, CancellationToken cancellationToken = default)
    {
        _sensors.TryAdd(sensor.SensorId, sensor);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Sensor sensor, CancellationToken cancellationToken = default)
    {
        _sensors.AddOrUpdate(sensor.SensorId, sensor, (_, _) => sensor);
        return Task.CompletedTask;
    }
}
