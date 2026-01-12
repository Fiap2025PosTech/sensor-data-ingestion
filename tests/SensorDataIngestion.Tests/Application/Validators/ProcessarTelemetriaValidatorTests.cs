using FluentValidation.TestHelper;
using SensorDataIngestion.Application.Commands;
using SensorDataIngestion.Application.Validators;

namespace SensorDataIngestion.Tests.Application.Validators;

public class ProcessTelemetryValidatorTests
{
    private readonly ProcessTelemetryValidator _validator;

    public ProcessTelemetryValidatorTests()
    {
        _validator = new ProcessTelemetryValidator();
    }

    [Fact]
    public void Validate_WithValidData_ShouldNotReturnErrors()
    {
        // Arrange
        var command = new ProcessTelemetryCommand
        {
            SensorId = "SENSOR-001",
            Temperature = 25.5m,
            Humidity = 60.0m,
            ApiKey = "api-key-sensor-001"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithoutSensorId_ShouldReturnError()
    {
        // Arrange
        var command = new ProcessTelemetryCommand
        {
            SensorId = "",
            Temperature = 25.5m,
            Humidity = 60.0m,
            ApiKey = "api-key-sensor-001"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SensorId)
            .WithErrorMessage("SensorId is required");
    }

    [Fact]
    public void Validate_WithSensorIdTooLong_ShouldReturnError()
    {
        // Arrange
        var command = new ProcessTelemetryCommand
        {
            SensorId = new string('A', 101),
            Temperature = 25.5m,
            Humidity = 60.0m,
            ApiKey = "api-key-sensor-001"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SensorId)
            .WithErrorMessage("SensorId must be at most 100 characters");
    }

    [Theory]
    [InlineData(-51)]
    [InlineData(101)]
    public void Validate_WithTemperatureOutOfRange_ShouldReturnError(decimal temperature)
    {
        // Arrange
        var command = new ProcessTelemetryCommand
        {
            SensorId = "SENSOR-001",
            Temperature = temperature,
            Humidity = 60.0m,
            ApiKey = "api-key-sensor-001"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Temperature)
            .WithErrorMessage("Temperature must be between -50°C and 100°C");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void Validate_WithHumidityOutOfRange_ShouldReturnError(decimal humidity)
    {
        // Arrange
        var command = new ProcessTelemetryCommand
        {
            SensorId = "SENSOR-001",
            Temperature = 25.5m,
            Humidity = humidity,
            ApiKey = "api-key-sensor-001"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Humidity)
            .WithErrorMessage("Humidity must be between 0% and 100%");
    }

    [Fact]
    public void Validate_WithoutApiKey_ShouldReturnError()
    {
        // Arrange
        var command = new ProcessTelemetryCommand
        {
            SensorId = "SENSOR-001",
            Temperature = 25.5m,
            Humidity = 60.0m,
            ApiKey = ""
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ApiKey)
            .WithErrorMessage("ApiKey is required");
    }

    [Theory]
    [InlineData(-50, 0)]
    [InlineData(100, 100)]
    [InlineData(0, 50)]
    [InlineData(25.5, 60.0)]
    public void Validate_WithValuesAtBoundaries_ShouldNotReturnErrors(decimal temperature, decimal humidity)
    {
        // Arrange
        var command = new ProcessTelemetryCommand
        {
            SensorId = "SENSOR-001",
            Temperature = temperature,
            Humidity = humidity,
            ApiKey = "api-key-sensor-001"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Temperature);
        result.ShouldNotHaveValidationErrorFor(x => x.Humidity);
    }
}
