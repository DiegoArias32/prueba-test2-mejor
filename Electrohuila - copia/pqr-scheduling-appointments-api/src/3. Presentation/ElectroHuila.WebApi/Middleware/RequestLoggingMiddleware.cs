using System.Diagnostics;

namespace ElectroHuila.WebApi.Middleware;

/// <summary>
/// Middleware para logging de solicitudes HTTP.
/// Registra información de cada solicitud incluyendo método, ruta, IP origen, código de respuesta y tiempo de ejecución.
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    /// <summary>
    /// Constructor del middleware de logging de solicitudes.
    /// </summary>
    /// <param name="next">Siguiente middleware en el pipeline.</param>
    /// <param name="logger">Logger para registrar información de solicitudes.</param>
    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Invoca el middleware, registra el inicio y fin de la solicitud con métricas de tiempo.
    /// </summary>
    /// <param name="context">Contexto HTTP de la solicitud actual.</param>
    /// <remarks>
    /// Genera un RequestId único para trazabilidad y mide el tiempo de ejecución.
    /// Registra el método HTTP, ruta, IP origen, código de estado y tiempo transcurrido.
    /// </remarks>
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestId = Guid.NewGuid().ToString();

        _logger.LogInformation(
            "Request {RequestId} started: {Method} {Path} from {RemoteIpAddress}",
            requestId,
            context.Request.Method,
            context.Request.Path,
            context.Connection.RemoteIpAddress);

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            _logger.LogInformation(
                "Request {RequestId} completed: {Method} {Path} responded {StatusCode} in {ElapsedMilliseconds}ms",
                requestId,
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds);
        }
    }
}