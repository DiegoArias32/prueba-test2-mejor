namespace ElectroHuila.Domain.Exceptions.Security;

/// <summary>
/// Excepción que se lanza cuando las credenciales de inicio de sesión son inválidas
/// </summary>
public sealed class InvalidCredentialsException : DomainException
{
    /// <summary>
    /// Constructor para excepción de credenciales inválidas sin detalles
    /// </summary>
    public InvalidCredentialsException()
        : base("INVALID_CREDENTIALS", "Invalid username or password.")
    {
    }

    /// <summary>
    /// Constructor para excepción de credenciales inválidas con nombre de usuario
    /// </summary>
    /// <param name="username">Nombre de usuario que intentó iniciar sesión</param>
    public InvalidCredentialsException(string username)
        : base("INVALID_CREDENTIALS",
               "Invalid username or password.",
               new { Username = username })
    {
    }

    /// <summary>
    /// Constructor para excepción de credenciales inválidas con nombre de usuario y motivo
    /// </summary>
    /// <param name="username">Nombre de usuario que intentó iniciar sesión</param>
    /// <param name="reason">Motivo específico del fallo de autenticación</param>
    public InvalidCredentialsException(string username, string reason)
        : base("INVALID_CREDENTIALS",
               $"Invalid username or password. {reason}",
               new { Username = username, Reason = reason })
    {
    }
}