using MediatR;
using Microsoft.Extensions.Logging;

namespace ElectroHuila.Application.Common.Behaviors;

/// <summary>
/// Comportamiento de pipeline de MediatR que registra el inicio y fin del procesamiento de solicitudes
/// </summary>
/// <typeparam name="TRequest">Tipo de solicitud que implementa IRequest</typeparam>
/// <typeparam name="TResponse">Tipo de respuesta de la solicitud</typeparam>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// Constructor que inyecta el logger para registrar el procesamiento de solicitudes
    /// </summary>
    /// <param name="logger">Instancia del logger</param>
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Registra el inicio y fin del procesamiento de una solicitud
    /// </summary>
    /// <param name="request">Solicitud a procesar</param>
    /// <param name="next">Delegado para continuar con el siguiente comportamiento en el pipeline</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Respuesta del handler</returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        // Registrar el inicio del procesamiento
        _logger.LogInformation("Handling {RequestName}", requestName);

        // Procesar la solicitud
        var response = await next();

        // Registrar la finalización del procesamiento
        _logger.LogInformation("Handled {RequestName}", requestName);
        return response;
    }
}