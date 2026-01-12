using SensorDataIngestion.Domain.Entities;
using SensorDataIngestion.Infrastructure.Repositories;

namespace SensorDataIngestion.Tests.Infrastructure.Repositories;

public class InMemorySensorRepositoryTests
{
    private readonly InMemorySensorRepository _repository;

    public InMemorySensorRepositoryTests()
    {
        _repository = new InMemorySensorRepository();
    }

    [Fact]
    public async Task GetBySensorIdAsync_WithExistingSensor_ShouldReturnSensor()
    {
        // Arrange
        var sensorId = "SENSOR-001";

        // Act
        var result = await _repository.GetBySensorIdAsync(sensorId);

        // Assert
        result.Should().NotBeNull();
        result!.SensorId.Should().Be(sensorId);
    }

    [Fact]
    public async Task GetBySensorIdAsync_WithNonExistentSensor_ShouldReturnNull()
    {
        // Arrange
        var sensorId = "SENSOR-NONEXISTENT";

        // Act
        var result = await _repository.GetBySensorIdAsync(sensorId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByApiKeyAsync_WithExistingApiKey_ShouldReturnSensor()
    {
        // Arrange
        var apiKey = "api-key-sensor-001";

        // Act
        var result = await _repository.GetByApiKeyAsync(apiKey);

        // Assert
        result.Should().NotBeNull();
        result!.ApiKey.Should().Be(apiKey);
    }

    [Fact]
    public async Task GetByApiKeyAsync_WithNonExistentApiKey_ShouldReturnNull()
    {
        // Arrange
        var apiKey = "api-key-nonexistent";

        // Act
        var result = await _repository.GetByApiKeyAsync(apiKey);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task ValidateApiKeyAsync_WithValidSensorAndApiKey_ShouldReturnTrue()
    {
        // Arrange
        var sensorId = "SENSOR-001";
        var apiKey = "api-key-sensor-001";

        // Act
        var result = await _repository.ValidateApiKeyAsync(sensorId, apiKey);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateApiKeyAsync_WithNonExistentSensor_ShouldReturnFalse()
    {
        // Arrange
        var sensorId = "SENSOR-NONEXISTENT";
        var apiKey = "any-api-key";

        // Act
        var result = await _repository.ValidateApiKeyAsync(sensorId, apiKey);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ValidateApiKeyAsync_WithInvalidApiKey_ShouldReturnFalse()
    {
        // Arrange
        var sensorId = "SENSOR-001";
        var apiKey = "wrong-api-key";

        // Act
        var result = await _repository.ValidateApiKeyAsync(sensorId, apiKey);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task AddAsync_ShouldAddSensorToRepository()
    {
        // Arrange
        var newSensor = new Sensor("SENSOR-NEW", "api-key-new", "New Sensor", "New Location");

        // Act
        await _repository.AddAsync(newSensor);
        var result = await _repository.GetBySensorIdAsync("SENSOR-NEW");

        // Assert
        result.Should().NotBeNull();
        result!.SensorId.Should().Be("SENSOR-NEW");
        result.ApiKey.Should().Be("api-key-new");
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateExistingSensor()
    {
        // Arrange
        var sensor = await _repository.GetBySensorIdAsync("SENSOR-001");
        sensor.Should().NotBeNull();
        sensor!.Deactivate();

        // Act
        await _repository.UpdateAsync(sensor);
        var result = await _repository.GetBySensorIdAsync("SENSOR-001");

        // Assert
        result.Should().NotBeNull();
        result!.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task RepositoryInitialized_ShouldContainTestSensors()
    {
        // Act
        var sensor1 = await _repository.GetBySensorIdAsync("SENSOR-001");
        var sensor2 = await _repository.GetBySensorIdAsync("SENSOR-002");
        var sensor3 = await _repository.GetBySensorIdAsync("SENSOR-003");

        // Assert
        sensor1.Should().NotBeNull();
        sensor2.Should().NotBeNull();
        sensor3.Should().NotBeNull();
    }
}
