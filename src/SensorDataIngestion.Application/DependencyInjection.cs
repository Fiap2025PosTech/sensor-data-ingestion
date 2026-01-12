using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SensorDataIngestion.Application.Behaviors;
using SensorDataIngestion.Application.Handlers;
using SensorDataIngestion.Application.Validators;

namespace SensorDataIngestion.Application;

/// <summary>
/// Extension for Application layer DI configuration
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        
        // Register Validators
        services.AddValidatorsFromAssemblyContaining<ProcessTelemetryValidator>();
        
        // Register Behaviors (Pipeline)
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        
        return services;
    }
}
