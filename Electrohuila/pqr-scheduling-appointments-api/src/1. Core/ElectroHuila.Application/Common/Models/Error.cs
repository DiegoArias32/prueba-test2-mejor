namespace ElectroHuila.Application.Common.Models;

/// <summary>
/// Representa un error en la aplicación con código, mensaje y detalles adicionales
/// </summary>
public record Error
{
    /// <summary>
    /// Código único del error
    /// </summary>
    public string Code { get; init; } = string.Empty;

    /// <summary>
    /// Mensaje descriptivo del error
    /// </summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// Detalles adicionales del error (opcional)
    /// </summary>
    public string? Details { get; init; }

    /// <summary>
    /// Metadatos adicionales del error (opcional)
    /// </summary>
    public Dictionary<string, object>? Metadata { get; init; }

    /// <summary>
    /// Error vacío que representa ausencia de error
    /// </summary>
    public static Error None => new();

    /// <summary>
    /// Crea un error de validación
    /// </summary>
    /// <param name="message">Mensaje del error</param>
    /// <param name="details">Detalles adicionales (opcional)</param>
    /// <returns>Error de validación</returns>
    public static Error Validation(string message, string? details = null) =>
        new() { Code = "VALIDATION_ERROR", Message = message, Details = details };

    /// <summary>
    /// Crea un error de recurso no encontrado
    /// </summary>
    /// <param name="message">Mensaje del error</param>
    /// <param name="details">Detalles adicionales (opcional)</param>
    /// <returns>Error de recurso no encontrado</returns>
    public static Error NotFound(string message, string? details = null) =>
        new() { Code = "NOT_FOUND", Message = message, Details = details };

    /// <summary>
    /// Crea un error de acceso no autorizado
    /// </summary>
    /// <param name="message">Mensaje del error (por defecto "Unauthorized access")</param>
    /// <returns>Error de acceso no autorizado</returns>
    public static Error Unauthorized(string message = "Unauthorized access") =>
        new() { Code = "UNAUTHORIZED", Message = message };

    /// <summary>
    /// Crea un error de acceso prohibido
    /// </summary>
    /// <param name="message">Mensaje del error (por defecto "Forbidden access")</param>
    /// <returns>Error de acceso prohibido</returns>
    public static Error Forbidden(string message = "Forbidden access") =>
        new() { Code = "FORBIDDEN", Message = message };

    /// <summary>
    /// Crea un error de conflicto
    /// </summary>
    /// <param name="message">Mensaje del error</param>
    /// <param name="details">Detalles adicionales (opcional)</param>
    /// <returns>Error de conflicto</returns>
    public static Error Conflict(string message, string? details = null) =>
        new() { Code = "CONFLICT", Message = message, Details = details };

    /// <summary>
    /// Crea un error interno del servidor
    /// </summary>
    /// <param name="message">Mensaje del error (por defecto "Internal server error")</param>
    /// <returns>Error interno del servidor</returns>
    public static Error Internal(string message = "Internal server error") =>
        new() { Code = "INTERNAL_ERROR", Message = message };

    /// <summary>
    /// Crea un error personalizado con código, mensaje y metadatos específicos
    /// </summary>
    /// <param name="code">Código del error</param>
    /// <param name="message">Mensaje del error</param>
    /// <param name="details">Detalles adicionales (opcional)</param>
    /// <param name="metadata">Metadatos adicionales (opcional)</param>
    /// <returns>Error personalizado</returns>
    public static Error Custom(string code, string message, string? details = null, Dictionary<string, object>? metadata = null) =>
        new() { Code = code, Message = message, Details = details, Metadata = metadata };
}