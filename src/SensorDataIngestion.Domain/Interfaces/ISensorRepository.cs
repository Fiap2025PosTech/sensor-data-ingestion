using SensorDataIngestion.Domain.Entities;

namespace SensorDataIngestion.Domain.Interfaces;

/// <summary>
/// Sensor repository interface
/// </summary>
public interface ISensorRepository
{
    Task<Sensor?> GetBySensorIdAsync(string sensorId, CancellationToken cancellationToken = default);
    Task<Sensor?> GetByApiKeyAsync(string apiKey, CancellationToken cancellationToken = default);
    Task<bool> ValidateApiKeyAsync(string sensorId, string apiKey, CancellationToken cancellationToken = default);
    Task AddAsync(Sensor sensor, CancellationToken cancellationToken = default);
    Task UpdateAsync(Sensor sensor, CancellationToken cancellationToken = default);
}
