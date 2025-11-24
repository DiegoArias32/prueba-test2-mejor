using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ElectroHuila.Application.Common.Behaviors;

/// <summary>
/// Comportamiento de pipeline de MediatR que monitorea el rendimiento de las solicitudes
/// </summary>
/// <typeparam name="TRequest">Tipo de solicitud que implementa IRequest</typeparam>
/// <typeparam name="TResponse">Tipo de respuesta de la solicitud</typeparam>
public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// Constructor que inyecta el logger para registrar problemas de rendimiento
    /// </summary>
    /// <param name="logger">Instancia del logger</param>
    public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Mide el tiempo de ejecución de la solicitud y registra advertencias si supera el umbral
    /// </summary>
    /// <param name="request">Solicitud a procesar</param>
    /// <param name="next">Delegado para continuar con el siguiente comportamiento en el pipeline</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Respuesta del handler</returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Iniciar cronómetro para medir el tiempo de ejecución
        var stopwatch = Stopwatch.StartNew();
        var response = await next();
        stopwatch.Stop();

        var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

        // Registrar advertencia si la solicitud tarda más de 500ms
        if (elapsedMilliseconds > 500)
        {
            var requestName = typeof(TRequest).Name;
            _logger.LogWarning("Long Running Request: {RequestName} ({ElapsedMilliseconds} milliseconds)", requestName, elapsedMilliseconds);
        }

        return response;
    }
}