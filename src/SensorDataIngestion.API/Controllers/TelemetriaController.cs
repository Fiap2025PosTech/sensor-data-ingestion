using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SensorDataIngestion.API.Filters;
using SensorDataIngestion.Application.Commands;
using SensorDataIngestion.Application.DTOs;
using System.ComponentModel.DataAnnotations;

namespace SensorDataIngestion.API.Controllers;

/// <summary>
/// Controller responsible for receiving telemetry data from sensors
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[ApiKeyAuth]
public class TelemetryController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TelemetryController> _logger;

    public TelemetryController(IMediator mediator, ILogger<TelemetryController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Receives telemetry data from sensor and queues it for processing
    /// </summary>
    /// <param name="request">Sensor telemetry data</param>
    /// <returns>Receipt confirmation</returns>
    /// <response code="202">Telemetry received and queued successfully</response>
    /// <response code="400">Invalid data</response>
    /// <response code="401">Unauthorized - Invalid JWT or API Key</response>
    [HttpPost]
    [ProducesResponseType(typeof(TelemetryResponse), StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Post([FromBody][Required] TelemetryRequest request)
    {
        _logger.LogInformation("Receiving telemetry from sensor {SensorId}", request.SensorId);

        // Retrieve API Key from context (inserted by filter)
        var apiKey = HttpContext.Items["ApiKey"]?.ToString() ?? string.Empty;

        var command = new ProcessTelemetryCommand
        {
            SensorId = request.SensorId,
            Temperature = request.Temperature,
            Humidity = request.Humidity,
            ReadingTimestamp = request.ReadingTimestamp,
            ApiKey = apiKey
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            _logger.LogWarning("Failed to process telemetry from sensor {SensorId}: {Message}", 
                request.SensorId, result.Message);
            return BadRequest(result);
        }

        _logger.LogInformation("Telemetry from sensor {SensorId} accepted with ID {Id}", 
            request.SensorId, result.Id);

        return Accepted(result);
    }

    /// <summary>
    /// Health check for the telemetry endpoint
    /// </summary>
    [HttpGet("health")]
    [AllowAnonymous]
    public IActionResult Health()
    {
        return Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow });
    }
}
