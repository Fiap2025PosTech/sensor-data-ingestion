using MediatR;
using SensorDataIngestion.Application.DTOs;
using SensorDataIngestion.Domain.Entities;
using SensorDataIngestion.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace SensorDataIngestion.Application.Handlers;

/// <summary>
/// Handler for telemetry command processing
/// </summary>
public class ProcessTelemetryHandler : IRequestHandler<Commands.ProcessTelemetryCommand, TelemetryResponse>
{
    private readonly IMessageBrokerService _messageBrokerService;
    private readonly ISensorRepository _sensorRepository;
    private readonly ILogger<ProcessTelemetryHandler> _logger;

    public ProcessTelemetryHandler(
        IMessageBrokerService messageBrokerService,
        ISensorRepository sensorRepository,
        ILogger<ProcessTelemetryHandler> logger)
    {
        _messageBrokerService = messageBrokerService;
        _sensorRepository = sensorRepository;
        _logger = logger;
    }

    public async Task<TelemetryResponse> Handle(Commands.ProcessTelemetryCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing telemetry from sensor {SensorId}", request.SensorId);

        // Validate sensor API Key
        var isValidSensor = await _sensorRepository.ValidateApiKeyAsync(request.SensorId, request.ApiKey, cancellationToken);
        
        if (!isValidSensor)
        {
            _logger.LogWarning("Invalid API Key for sensor {SensorId}", request.SensorId);
            return new TelemetryResponse
            {
                Id = Guid.Empty,
                SensorId = request.SensorId,
                Message = "Unauthorized sensor or invalid API Key",
                ReceivedAt = DateTime.UtcNow,
                Success = false
            };
        }

        // Create telemetry entity
        var telemetry = new SensorTelemetry(
            request.SensorId,
            request.Temperature,
            request.Humidity,
            request.ReadingTimestamp
        );

        // Validate telemetry data
        if (!telemetry.ValidateData())
        {
            _logger.LogWarning("Invalid telemetry data for sensor {SensorId}", request.SensorId);
            return new TelemetryResponse
            {
                Id = Guid.Empty,
                SensorId = request.SensorId,
                Message = "Invalid telemetry data",
                ReceivedAt = DateTime.UtcNow,
                Success = false
            };
        }

        // Publish to queue (enqueue only, no processing)
        await _messageBrokerService.PublishTelemetryAsync(telemetry, cancellationToken);

        _logger.LogInformation("Telemetry from sensor {SensorId} published to queue successfully", request.SensorId);

        return new TelemetryResponse
        {
            Id = telemetry.Id,
            SensorId = request.SensorId,
            Message = "Telemetry received and queued successfully",
            ReceivedAt = telemetry.ReceivedAt,
            Success = true
        };
    }
}
