using System.Diagnostics;

namespace ElectroHuila.WebApi.Middleware;

/// <summary>
/// Middleware para monitoreo de rendimiento de solicitudes HTTP.
/// Detecta solicitudes lentas (>1000ms) y genera advertencias en los logs.
/// </summary>
public class PerformanceMonitoringMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PerformanceMonitoringMiddleware> _logger;

    /// <summary>
    /// Umbral en milisegundos para considerar una solicitud como lenta.
    /// Solicitudes que excedan este tiempo generarán un log de advertencia.
    /// </summary>
    private const int SlowRequestThresholdMs = 1000;

    /// <summary>
    /// Constructor del middleware de monitoreo de rendimiento.
    /// </summary>
    /// <param name="next">Siguiente middleware en el pipeline.</param>
    /// <param name="logger">Logger para registrar métricas de rendimiento.</param>
    public PerformanceMonitoringMiddleware(RequestDelegate next, ILogger<PerformanceMonitoringMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Invoca el middleware y mide el tiempo de ejecución de la solicitud.
    /// </summary>
    /// <param name="context">Contexto HTTP de la solicitud actual.</param>
    /// <remarks>
    /// Si el tiempo de ejecución supera los 1000ms, se registra como advertencia.
    /// Todas las solicitudes se registran con su tiempo de ejecución y código de estado.
    /// </remarks>
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestPath = context.Request.Path;
        var requestMethod = context.Request.Method;

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

            if (elapsedMilliseconds > SlowRequestThresholdMs)
            {
                _logger.LogWarning(
                    "Slow request detected: {Method} {Path} took {ElapsedMs}ms (Status: {StatusCode})",
                    requestMethod,
                    requestPath,
                    elapsedMilliseconds,
                    context.Response.StatusCode);
            }
            else
            {
                _logger.LogInformation(
                    "{Method} {Path} completed in {ElapsedMs}ms (Status: {StatusCode})",
                    requestMethod,
                    requestPath,
                    elapsedMilliseconds,
                    context.Response.StatusCode);
            }
        }
    }
}