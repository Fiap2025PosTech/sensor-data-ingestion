using SensorDataIngestion.Domain.Entities;

namespace SensorDataIngestion.Tests.Domain.Entities;

public class SensorTests
{
    [Fact]
    public void Constructor_ShouldInitializePropertiesCorrectly()
    {
        // Arrange
        var sensorId = "SENSOR-001";
        var apiKey = "api-key-001";
        var name = "Test Sensor";
        var location = "Room 1";

        // Act
        var sensor = new Sensor(sensorId, apiKey, name, location);

        // Assert
        sensor.SensorId.Should().Be(sensorId);
        sensor.ApiKey.Should().Be(apiKey);
        sensor.Name.Should().Be(name);
        sensor.Location.Should().Be(location);
        sensor.IsActive.Should().BeTrue();
        sensor.Id.Should().NotBeEmpty();
        sensor.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void ValidateApiKey_WithCorrectApiKey_ShouldReturnTrue()
    {
        // Arrange
        var apiKey = "api-key-001";
        var sensor = new Sensor("SENSOR-001", apiKey, "Test Sensor", "Room 1");

        // Act
        var result = sensor.ValidateApiKey(apiKey);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ValidateApiKey_WithIncorrectApiKey_ShouldReturnFalse()
    {
        // Arrange
        var sensor = new Sensor("SENSOR-001", "api-key-001", "Test Sensor", "Room 1");

        // Act
        var result = sensor.ValidateApiKey("wrong-api-key");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ValidateApiKey_WithDeactivatedSensor_ShouldReturnFalse()
    {
        // Arrange
        var apiKey = "api-key-001";
        var sensor = new Sensor("SENSOR-001", apiKey, "Test Sensor", "Room 1");
        sensor.Deactivate();

        // Act
        var result = sensor.ValidateApiKey(apiKey);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Deactivate_ShouldMarkSensorAsInactive()
    {
        // Arrange
        var sensor = new Sensor("SENSOR-001", "api-key-001", "Test Sensor", "Room 1");

        // Act
        sensor.Deactivate();

        // Assert
        sensor.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Activate_ShouldMarkSensorAsActive()
    {
        // Arrange
        var sensor = new Sensor("SENSOR-001", "api-key-001", "Test Sensor", "Room 1");
        sensor.Deactivate();

        // Act
        sensor.Activate();

        // Assert
        sensor.IsActive.Should().BeTrue();
    }

    [Fact]
    public void UpdateLastReading_ShouldUpdateLastReadingDate()
    {
        // Arrange
        var sensor = new Sensor("SENSOR-001", "api-key-001", "Test Sensor", "Room 1");

        // Act
        sensor.UpdateLastReading();

        // Assert
        sensor.LastReadingAt.Should().NotBeNull();
        sensor.LastReadingAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Constructor_WithNullSensorId_ShouldThrowArgumentNullException()
    {
        // Arrange & Act
        var action = () => new Sensor(null!, "api-key", "name", "location");

        // Assert
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("sensorId");
    }

    [Fact]
    public void Constructor_WithNullApiKey_ShouldThrowArgumentNullException()
    {
        // Arrange & Act
        var action = () => new Sensor("sensor-id", null!, "name", "location");

        // Assert
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("apiKey");
    }

    [Fact]
    public void Constructor_WithNullName_ShouldThrowArgumentNullException()
    {
        // Arrange & Act
        var action = () => new Sensor("sensor-id", "api-key", null!, "location");

        // Assert
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("name");
    }
}
