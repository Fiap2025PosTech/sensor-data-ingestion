using SensorDataIngestion.Infrastructure.Messaging;

namespace SensorDataIngestion.Tests.Infrastructure.Messaging;

public class RabbitMqSettingsTests
{
    [Fact]
    public void RabbitMqSettings_ShouldContainDefaultValues()
    {
        // Arrange & Act
        var settings = new RabbitMqSettings();

        // Assert
        settings.Host.Should().Be("localhost");
        settings.VirtualHost.Should().Be("/");
        settings.Username.Should().Be("guest");
        settings.Password.Should().Be("guest");
        settings.Port.Should().Be(5672);
        settings.QueueName.Should().Be("telemetria-queue");
        settings.ExchangeName.Should().Be("telemetria-exchange");
    }

    [Fact]
    public void RabbitMqSettings_SectionName_ShouldBeRabbitMq()
    {
        // Assert
        RabbitMqSettings.SectionName.Should().Be("RabbitMq");
    }

    [Fact]
    public void RabbitMqSettings_ShouldAllowChangingValues()
    {
        // Arrange
        var settings = new RabbitMqSettings
        {
            Host = "rabbitmq.example.com",
            Port = 5673,
            VirtualHost = "/vhost",
            Username = "admin",
            Password = "password123",
            QueueName = "my-queue",
            ExchangeName = "my-exchange"
        };

        // Assert
        settings.Host.Should().Be("rabbitmq.example.com");
        settings.Port.Should().Be(5673);
        settings.VirtualHost.Should().Be("/vhost");
        settings.Username.Should().Be("admin");
        settings.Password.Should().Be("password123");
        settings.QueueName.Should().Be("my-queue");
        settings.ExchangeName.Should().Be("my-exchange");
    }
}
