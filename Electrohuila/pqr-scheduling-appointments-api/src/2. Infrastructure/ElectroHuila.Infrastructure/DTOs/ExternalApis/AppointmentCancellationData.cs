namespace ElectroHuila.Infrastructure.DTOs.ExternalApis;

/// <summary>
/// DTO que contiene los datos necesarios para enviar una notificación de cancelación de cita.
/// Se utiliza para integración con APIs externas de WhatsApp y Gmail.
/// </summary>
public record AppointmentCancellationData
{
    /// <summary>
    /// Nombre completo del cliente
    /// </summary>
    public required string NombreCliente { get; init; }

    /// <summary>
    /// Fecha de la cita cancelada en formato yyyy-MM-dd
    /// </summary>
    public required string Fecha { get; init; }

    /// <summary>
    /// Hora de la cita cancelada en formato HH:mm
    /// </summary>
    public required string Hora { get; init; }

    /// <summary>
    /// Motivo de la cancelación
    /// </summary>
    public required string Motivo { get; init; }

    /// <summary>
    /// URL para reagendar la cita
    /// </summary>
    public required string UrlReagendar { get; init; }

    /// <summary>
    /// Ubicación de la cita cancelada (opcional)
    /// </summary>
    public string? Ubicacion { get; init; }

    /// <summary>
    /// Número único de la cita (opcional)
    /// </summary>
    public string? NumeroCita { get; init; }
}
