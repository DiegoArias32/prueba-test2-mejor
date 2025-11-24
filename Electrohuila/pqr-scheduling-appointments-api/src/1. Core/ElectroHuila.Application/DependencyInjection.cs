using ElectroHuila.Application.Common.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ElectroHuila.Application;

/// <summary>
/// Clase estática que configura la inyección de dependencias para la capa de aplicación.
/// Registra todos los servicios, comportamientos y validadores necesarios para CQRS y MediatR.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Agrega los servicios de la capa de aplicación al contenedor de dependencias.
    /// </summary>
    /// <param name="services">Colección de servicios donde se registrarán las dependencias.</param>
    /// <returns>La colección de servicios modificada para permitir encadenamiento fluido.</returns>
    /// <remarks>
    /// Este método registra:
    /// - AutoMapper: Para mapeo automático entre DTOs y entidades de dominio
    /// - MediatR: Para implementar el patrón CQRS (Command Query Responsibility Segregation)
    /// - FluentValidation: Para validación de comandos y queries
    /// - Pipeline Behaviors: Para validación, logging y monitoreo de rendimiento
    ///   * ValidationBehavior: Valida automáticamente todos los comandos/queries antes de ejecutarlos
    ///   * LoggingBehavior: Registra información de todas las operaciones ejecutadas
    ///   * PerformanceBehavior: Monitorea el tiempo de ejecución de las operaciones
    /// </remarks>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Registrar AutoMapper con todos los perfiles de mapeo del ensamblado actual
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        // Registrar MediatR y escanear todos los handlers del ensamblado
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // Registrar todos los validadores de FluentValidation del ensamblado
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Registrar comportamientos del pipeline de MediatR (se ejecutan en orden)
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));

        return services;
    }
}