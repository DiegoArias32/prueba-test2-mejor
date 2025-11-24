namespace ElectroHuila.Domain.Exceptions.Security;

/// <summary>
/// Excepción que se lanza cuando un usuario no está autorizado
/// </summary>
public sealed class UnauthorizedException : DomainException
{
    /// <summary>
    /// Constructor para excepción de acceso no autorizado genérica
    /// </summary>
    public UnauthorizedException()
        : base("UNAUTHORIZED", "Access denied. Authentication required.")
    {
    }

    /// <summary>
    /// Constructor para excepción de acceso no autorizado con mensaje personalizado
    /// </summary>
    /// <param name="message">Mensaje personalizado de la excepción</param>
    public UnauthorizedException(string message)
        : base("UNAUTHORIZED", message)
    {
    }

    /// <summary>
    /// Constructor para excepción de acceso no autorizado a un recurso específico
    /// </summary>
    /// <param name="resource">Recurso al que se intentó acceder</param>
    /// <param name="action">Acción que se intentó realizar</param>
    public UnauthorizedException(string resource, string action)
        : base("UNAUTHORIZED",
               $"Access denied. User is not authorized to {action} {resource}.",
               new { Resource = resource, Action = action })
    {
    }
}