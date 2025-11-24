namespace ElectroHuila.Domain.Exceptions;

/// <summary>
/// Clase base abstracta para todas las excepciones del dominio
/// </summary>
public abstract class DomainException : Exception
{
    /// <summary>
    /// Código único que identifica el tipo de excepción
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Detalles adicionales sobre la excepción
    /// </summary>
    public object? Details { get; }

    /// <summary>
    /// Constructor protegido para excepciones del dominio
    /// </summary>
    /// <param name="code">Código de la excepción</param>
    /// <param name="message">Mensaje descriptivo de la excepción</param>
    /// <param name="details">Detalles adicionales opcionales</param>
    protected DomainException(string code, string message, object? details = null)
        : base(message)
    {
        Code = code;
        Details = details;
    }

    /// <summary>
    /// Constructor protegido para excepciones del dominio con excepción interna
    /// </summary>
    /// <param name="code">Código de la excepción</param>
    /// <param name="message">Mensaje descriptivo de la excepción</param>
    /// <param name="innerException">Excepción interna que causó esta excepción</param>
    /// <param name="details">Detalles adicionales opcionales</param>
    protected DomainException(string code, string message, Exception innerException, object? details = null)
        : base(message, innerException)
    {
        Code = code;
        Details = details;
    }
}