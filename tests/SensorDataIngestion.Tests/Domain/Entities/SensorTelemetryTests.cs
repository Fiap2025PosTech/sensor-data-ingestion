using SensorDataIngestion.Domain.Entities;

namespace SensorDataIngestion.Tests.Domain.Entities;

public class SensorTelemetryTests
{
    [Fact]
    public void Constructor_ShouldInitializePropertiesCorrectly()
    {
        // Arrange
        var sensorId = "SENSOR-001";
        var temperature = 25.5m;
        var humidity = 60.0m;

        // Act
        var telemetry = new SensorTelemetry(sensorId, temperature, humidity);

        // Assert
        telemetry.SensorId.Should().Be(sensorId);
        telemetry.Temperature.Should().Be(temperature);
        telemetry.Humidity.Should().Be(humidity);
        telemetry.Id.Should().NotBeEmpty();
        telemetry.ReadingTimestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        telemetry.ReceivedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Constructor_WithSpecificReadingTimestamp_ShouldUseProvidedDate()
    {
        // Arrange
        var sensorId = "SENSOR-001";
        var temperature = 25.5m;
        var humidity = 60.0m;
        var readingTimestamp = new DateTime(2025, 1, 1, 10, 0, 0, DateTimeKind.Utc);

        // Act
        var telemetry = new SensorTelemetry(sensorId, temperature, humidity, readingTimestamp);

        // Assert
        telemetry.ReadingTimestamp.Should().Be(readingTimestamp);
    }

    [Fact]
    public void ValidateData_WithValidData_ShouldReturnTrue()
    {
        // Arrange
        var telemetry = new SensorTelemetry("SENSOR-001", 25.5m, 60.0m);

        // Act
        var result = telemetry.ValidateData();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ValidateData_WithEmptySensorId_ShouldReturnFalse()
    {
        // Arrange
        var telemetry = new SensorTelemetry("", 25.5m, 60.0m);

        // Act
        var result = telemetry.ValidateData();

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(-51)]
    [InlineData(101)]
    public void ValidateData_WithTemperatureOutOfRange_ShouldReturnFalse(decimal temperature)
    {
        // Arrange
        var telemetry = new SensorTelemetry("SENSOR-001", temperature, 60.0m);

        // Act
        var result = telemetry.ValidateData();

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void ValidateData_WithHumidityOutOfRange_ShouldReturnFalse(decimal humidity)
    {
        // Arrange
        var telemetry = new SensorTelemetry("SENSOR-001", 25.5m, humidity);

        // Act
        var result = telemetry.ValidateData();

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(-50, 0)]
    [InlineData(100, 100)]
    [InlineData(0, 50)]
    public void ValidateData_WithValuesAtBoundaries_ShouldReturnTrue(decimal temperature, decimal humidity)
    {
        // Arrange
        var telemetry = new SensorTelemetry("SENSOR-001", temperature, humidity);

        // Act
        var result = telemetry.ValidateData();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Constructor_WithNullSensorId_ShouldThrowArgumentNullException()
    {
        // Arrange & Act
        var action = () => new SensorTelemetry(null!, 25.5m, 60.0m);

        // Assert
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("sensorId");
    }
}
