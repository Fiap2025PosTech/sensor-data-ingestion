using Prometheus;

namespace SensorDataIngestion.API.Metrics;

/// <summary>
/// Custom metrics for sensor telemetry
/// </summary>
public static class TelemetryMetrics
{
    private static readonly Counter TelemetryReceivedTotal = Prometheus.Metrics.CreateCounter(
        "sensor_telemetry_received_total",
        "Total telemetry received",
        new CounterConfiguration
        {
            LabelNames = new[] { "sensor_id", "status" }
        });

    private static readonly Histogram TelemetryProcessingDuration = Prometheus.Metrics.CreateHistogram(
        "sensor_telemetry_processing_duration_seconds",
        "Telemetry processing duration in seconds",
        new HistogramConfiguration
        {
            LabelNames = new[] { "sensor_id" },
            Buckets = new[] { .001, .005, .01, .025, .05, .1, .25, .5, 1, 2.5, 5, 10 }
        });

    private static readonly Gauge CurrentTemperature = Prometheus.Metrics.CreateGauge(
        "sensor_temperature_celsius",
        "Current sensor temperature in Celsius",
        new GaugeConfiguration
        {
            LabelNames = new[] { "sensor_id" }
        });

    private static readonly Gauge CurrentHumidity = Prometheus.Metrics.CreateGauge(
        "sensor_humidity_percentage",
        "Current sensor humidity percentage",
        new GaugeConfiguration
        {
            LabelNames = new[] { "sensor_id" }
        });

    private static readonly Counter TelemetryErrorsTotal = Prometheus.Metrics.CreateCounter(
        "sensor_telemetry_errors_total",
        "Total telemetry processing errors",
        new CounterConfiguration
        {
            LabelNames = new[] { "sensor_id", "error_type" }
        });

    public static void IncrementTelemetryReceived(string sensorId, bool success)
    {
        TelemetryReceivedTotal.WithLabels(sensorId, success ? "success" : "failure").Inc();
    }

    public static IDisposable MeasureProcessingTime(string sensorId)
    {
        return TelemetryProcessingDuration.WithLabels(sensorId).NewTimer();
    }

    public static void UpdateTemperature(string sensorId, double temperature)
    {
        CurrentTemperature.WithLabels(sensorId).Set(temperature);
    }

    public static void UpdateHumidity(string sensorId, double humidity)
    {
        CurrentHumidity.WithLabels(sensorId).Set(humidity);
    }

    public static void IncrementError(string sensorId, string errorType)
    {
        TelemetryErrorsTotal.WithLabels(sensorId, errorType).Inc();
    }
}
