namespace ElectroHuila.Infrastructure.DTOs.ExternalApis;

/// <summary>
/// DTO que contiene los datos necesarios para enviar un correo de bienvenida.
/// Se utiliza para integración con la API externa de Gmail.
/// </summary>
public record WelcomeData
{
    /// <summary>
    /// Nombre completo del nuevo usuario
    /// </summary>
    public required string NombreUsuario { get; init; }

    /// <summary>
    /// Email del usuario
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    /// URL de la aplicación o portal
    /// </summary>
    public required string AppUrl { get; init; }

    /// <summary>
    /// Tipo de cuenta o rol del usuario (opcional)
    /// </summary>
    public string? TipoCuenta { get; init; }

    /// <summary>
    /// Fecha de registro en formato yyyy-MM-dd (opcional)
    /// </summary>
    public string? FechaRegistro { get; init; }
}
