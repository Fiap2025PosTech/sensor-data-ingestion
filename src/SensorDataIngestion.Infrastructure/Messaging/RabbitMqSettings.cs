namespace SensorDataIngestion.Infrastructure.Messaging;

/// <summary>
/// RabbitMQ configuration settings
/// </summary>
public class RabbitMqSettings
{
    public const string SectionName = "RabbitMq";
    
    public string Host { get; set; } = "localhost";
    public string VirtualHost { get; set; } = "/";
    public string Username { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public int Port { get; set; } = 5672;
    public string QueueName { get; set; } = "telemetria-queue";
    public string ExchangeName { get; set; } = "telemetria-exchange";
}
