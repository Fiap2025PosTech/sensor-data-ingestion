using SensorDataIngestion.Application.DTOs;

namespace SensorDataIngestion.Tests.Application.DTOs;

public class TelemetryRequestTests
{
    [Fact]
    public void CreateRequest_ShouldInitializePropertiesCorrectly()
    {
        // Arrange & Act
        var request = new TelemetryRequest
        {
            SensorId = "SENSOR-001",
            Temperature = 25.5m,
            Humidity = 60.0m,
            ReadingTimestamp = DateTime.UtcNow
        };

        // Assert
        request.SensorId.Should().Be("SENSOR-001");
        request.Temperature.Should().Be(25.5m);
        request.Humidity.Should().Be(60.0m);
        request.ReadingTimestamp.Should().NotBeNull();
    }

    [Fact]
    public void CreateRequest_WithoutReadingTimestamp_ShouldBeNull()
    {
        // Arrange & Act
        var request = new TelemetryRequest
        {
            SensorId = "SENSOR-001",
            Temperature = 25.5m,
            Humidity = 60.0m
        };

        // Assert
        request.ReadingTimestamp.Should().BeNull();
    }
}

public class TelemetryResponseTests
{
    [Fact]
    public void CreateResponse_ShouldInitializePropertiesCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var receivedAt = DateTime.UtcNow;

        // Act
        var response = new TelemetryResponse
        {
            Id = id,
            SensorId = "SENSOR-001",
            Message = "Success",
            ReceivedAt = receivedAt,
            Success = true
        };

        // Assert
        response.Id.Should().Be(id);
        response.SensorId.Should().Be("SENSOR-001");
        response.Message.Should().Be("Success");
        response.ReceivedAt.Should().Be(receivedAt);
        response.Success.Should().BeTrue();
    }

    [Fact]
    public void CreateFailureResponse_ShouldIndicateFailure()
    {
        // Arrange & Act
        var response = new TelemetryResponse
        {
            Id = Guid.Empty,
            SensorId = "SENSOR-001",
            Message = "Validation error",
            ReceivedAt = DateTime.UtcNow,
            Success = false
        };

        // Assert
        response.Success.Should().BeFalse();
        response.Id.Should().BeEmpty();
    }
}
