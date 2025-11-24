namespace ElectroHuila.Infrastructure.DTOs.ExternalApis;

/// <summary>
/// DTO que contiene los datos necesarios para enviar una confirmación de cita.
/// Se utiliza para integración con APIs externas de WhatsApp y Gmail.
/// </summary>
public record AppointmentConfirmationData
{
    /// <summary>
    /// Número único de la cita
    /// </summary>
    public required string NumeroCita { get; init; }

    /// <summary>
    /// Nombre completo del cliente
    /// </summary>
    public required string NombreCliente { get; init; }

    /// <summary>
    /// Fecha de la cita en formato yyyy-MM-dd
    /// </summary>
    public required string Fecha { get; init; }

    /// <summary>
    /// Hora de la cita en formato HH:mm
    /// </summary>
    public required string Hora { get; init; }

    /// <summary>
    /// Nombre del profesional asignado a la cita
    /// </summary>
    public required string Profesional { get; init; }

    /// <summary>
    /// Ubicación donde se realizará la cita (nombre de la sucursal)
    /// </summary>
    public required string Ubicacion { get; init; }

    /// <summary>
    /// Dirección completa de la sucursal (opcional)
    /// </summary>
    public string? Direccion { get; init; }

    /// <summary>
    /// Tipo de cita (opcional)
    /// </summary>
    public string? TipoCita { get; init; }

    // ============================================================================
    // CAMPOS ADICIONALES PARA WHATSAPP MEJORADO
    // ============================================================================

    /// <summary>
    /// ID del cliente (número de cuenta) (opcional)
    /// </summary>
    public string? ClienteId { get; init; }

    /// <summary>
    /// Teléfono del cliente (opcional)
    /// </summary>
    public string? Telefono { get; init; }

    /// <summary>
    /// Dirección del domicilio del cliente (opcional)
    /// </summary>
    public string? DireccionCliente { get; init; }

    /// <summary>
    /// Observaciones o notas importantes sobre la cita (opcional)
    /// </summary>
    public string? Observaciones { get; init; }

    /// <summary>
    /// URL del código QR para verificación de la cita (opcional)
    /// </summary>
    public string? QrUrl { get; init; }
}
