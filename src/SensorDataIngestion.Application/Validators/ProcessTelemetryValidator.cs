using FluentValidation;
using SensorDataIngestion.Application.Commands;

namespace SensorDataIngestion.Application.Validators;

/// <summary>
/// Validator for the telemetry command
/// </summary>
public class ProcessTelemetryValidator : AbstractValidator<ProcessTelemetryCommand>
{
    public ProcessTelemetryValidator()
    {
        RuleFor(x => x.SensorId)
            .NotEmpty().WithMessage("SensorId is required")
            .MaximumLength(100).WithMessage("SensorId must be at most 100 characters");

        RuleFor(x => x.Temperature)
            .InclusiveBetween(-50, 100).WithMessage("Temperature must be between -50°C and 100°C");

        RuleFor(x => x.Humidity)
            .InclusiveBetween(0, 100).WithMessage("Humidity must be between 0% and 100%");

        RuleFor(x => x.ApiKey)
            .NotEmpty().WithMessage("ApiKey is required");
    }
}
