namespace ElectroHuila.Infrastructure.DTOs.ExternalApis;

/// <summary>
/// DTO que contiene los datos necesarios para enviar un correo de restablecimiento de contraseña.
/// Se utiliza para integración con la API externa de Gmail.
/// </summary>
public record PasswordResetData
{
    /// <summary>
    /// Nombre completo del usuario
    /// </summary>
    public required string NombreUsuario { get; init; }

    /// <summary>
    /// Token de restablecimiento de contraseña
    /// </summary>
    public required string ResetToken { get; init; }

    /// <summary>
    /// URL completa para restablecer la contraseña
    /// </summary>
    public required string ResetUrl { get; init; }

    /// <summary>
    /// Tiempo de expiración del token en horas
    /// </summary>
    public int? ExpirationHours { get; init; }

    /// <summary>
    /// Email del usuario (para referencia)
    /// </summary>
    public string? Email { get; init; }
}
