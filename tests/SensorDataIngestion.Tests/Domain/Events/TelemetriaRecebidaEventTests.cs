using SensorDataIngestion.Domain.Events;

namespace SensorDataIngestion.Tests.Domain.Events;

public class TelemetryReceivedEventTests
{
    [Fact]
    public void CreateEvent_ShouldInitializePropertiesCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var sensorId = "SENSOR-001";
        var temperature = 25.5m;
        var humidity = 60.0m;
        var readingTimestamp = DateTime.UtcNow;
        var receivedAt = DateTime.UtcNow;
        var publishedAt = DateTime.UtcNow;

        // Act
        var eventObj = new TelemetryReceivedEvent
        {
            Id = id,
            SensorId = sensorId,
            Temperature = temperature,
            Humidity = humidity,
            ReadingTimestamp = readingTimestamp,
            ReceivedAt = receivedAt,
            PublishedAt = publishedAt
        };

        // Assert
        eventObj.Id.Should().Be(id);
        eventObj.SensorId.Should().Be(sensorId);
        eventObj.Temperature.Should().Be(temperature);
        eventObj.Humidity.Should().Be(humidity);
        eventObj.ReadingTimestamp.Should().Be(readingTimestamp);
        eventObj.ReceivedAt.Should().Be(receivedAt);
        eventObj.PublishedAt.Should().Be(publishedAt);
    }

    [Fact]
    public void CreateEvent_ShouldBeImmutable()
    {
        // Arrange & Act
        var event1 = new TelemetryReceivedEvent
        {
            Id = Guid.NewGuid(),
            SensorId = "SENSOR-001",
            Temperature = 25.5m,
            Humidity = 60.0m
        };

        var event2 = event1 with { SensorId = "SENSOR-002" };

        // Assert
        event1.SensorId.Should().Be("SENSOR-001");
        event2.SensorId.Should().Be("SENSOR-002");
        event1.Should().NotBeSameAs(event2);
    }
}
