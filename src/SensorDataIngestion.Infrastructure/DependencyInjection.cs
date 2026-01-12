using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SensorDataIngestion.Domain.Events;
using SensorDataIngestion.Domain.Interfaces;
using SensorDataIngestion.Infrastructure.Messaging;
using SensorDataIngestion.Infrastructure.Repositories;

namespace SensorDataIngestion.Infrastructure;

/// <summary>
/// Extension for Infrastructure layer DI configuration
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure RabbitMQ Settings
        var rabbitMqSettings = new RabbitMqSettings();
        configuration.GetSection(RabbitMqSettings.SectionName).Bind(rabbitMqSettings);
        services.Configure<RabbitMqSettings>(configuration.GetSection(RabbitMqSettings.SectionName));

        // Configure MassTransit with RabbitMQ
        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();

            busConfigurator.UsingRabbitMq((context, configurator) =>
            {
                var hostAddress = new Uri($"rabbitmq://{rabbitMqSettings.Host}:{rabbitMqSettings.Port}{rabbitMqSettings.VirtualHost}");
                
                configurator.Host(hostAddress, h =>
                {
                    h.Username(rabbitMqSettings.Username);
                    h.Password(rabbitMqSettings.Password);
                });

                // Configure exchange and queue for telemetry
                configurator.Message<TelemetryReceivedEvent>(x =>
                {
                    x.SetEntityName(rabbitMqSettings.ExchangeName);
                });

                configurator.Publish<TelemetryReceivedEvent>(x =>
                {
                    x.ExchangeType = "fanout";
                });

                configurator.ConfigureEndpoints(context);
            });
        });

        // Register repositories
        services.AddSingleton<ISensorRepository, InMemorySensorRepository>();

        // Register messaging services
        services.AddScoped<IMessageBrokerService, RabbitMqMessageBrokerService>();

        return services;
    }
}
