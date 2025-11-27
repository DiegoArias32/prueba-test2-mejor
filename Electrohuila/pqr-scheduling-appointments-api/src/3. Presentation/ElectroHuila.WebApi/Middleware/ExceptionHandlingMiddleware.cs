using ElectroHuila.Domain.Exceptions;
using FluentValidation;
using System.Net;
using System.Text.Json;

namespace ElectroHuila.WebApi.Middleware;

/// <summary>
/// Middleware para manejo centralizado de excepciones en el pipeline HTTP.
/// Intercepta todas las excepciones no controladas y las convierte en respuestas HTTP apropiadas.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    /// <summary>
    /// Constructor del middleware de manejo de excepciones.
    /// </summary>
    /// <param name="next">Siguiente middleware en el pipeline.</param>
    /// <param name="logger">Logger para registrar excepciones.</param>
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Invoca el middleware, ejecuta el siguiente middleware y captura cualquier excepción.
    /// </summary>
    /// <param name="context">Contexto HTTP de la solicitud actual.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Maneja una excepción y genera una respuesta HTTP JSON apropiada.
    /// </summary>
    /// <param name="context">Contexto HTTP.</param>
    /// <param name="exception">Excepción capturada.</param>
    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        object response;

        // Handle FluentValidation ValidationException specially to return validation errors
        if (exception is ValidationException validationException)
        {
            var errors = validationException.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );

            response = new
            {
                error = "Validation failed",
                errors = errors
            };
        }
        else
        {
            response = new
            {
                error = GetErrorMessage(exception),
                details = exception.Message
            };
        }

        context.Response.StatusCode = GetStatusCode(exception);

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }

    /// <summary>
    /// Determina el código de estado HTTP basado en el tipo de excepción.
    /// </summary>
    /// <param name="exception">Excepción a analizar.</param>
    /// <returns>Código de estado HTTP apropiado.</returns>
    private static int GetStatusCode(Exception exception)
    {
        return exception switch
        {
            ValidationException => (int)HttpStatusCode.BadRequest,
            DomainException => (int)HttpStatusCode.BadRequest,
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
            ArgumentException => (int)HttpStatusCode.BadRequest,
            KeyNotFoundException => (int)HttpStatusCode.NotFound,
            _ => (int)HttpStatusCode.InternalServerError
        };
    }

    /// <summary>
    /// Obtiene un mensaje de error amigable basado en el tipo de excepción.
    /// </summary>
    /// <param name="exception">Excepción a analizar.</param>
    /// <returns>Mensaje de error descriptivo.</returns>
    private static string GetErrorMessage(Exception exception)
    {
        return exception switch
        {
            ValidationException => "Validation failed",
            DomainException => "A business rule violation occurred",
            UnauthorizedAccessException => "Access denied",
            ArgumentException => "Invalid input provided",
            KeyNotFoundException => "Resource not found",
            _ => "An internal server error occurred"
        };
    }
}

/// <summary>
/// Extension method to register the ExceptionHandlingMiddleware
/// </summary>
public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}