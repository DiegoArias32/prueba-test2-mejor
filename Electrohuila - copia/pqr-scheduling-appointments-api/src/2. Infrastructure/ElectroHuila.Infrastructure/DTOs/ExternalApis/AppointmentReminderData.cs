namespace ElectroHuila.Infrastructure.DTOs.ExternalApis;

/// <summary>
/// DTO que contiene los datos necesarios para enviar un recordatorio de cita.
/// Se utiliza para integración con APIs externas de WhatsApp y Gmail.
/// </summary>
public record AppointmentReminderData
{
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
    /// Ubicación donde se realizará la cita (nombre de la sucursal)
    /// </summary>
    public required string Ubicacion { get; init; }

    /// <summary>
    /// Dirección completa de la sucursal (opcional)
    /// </summary>
    public string? Direccion { get; init; }

    /// <summary>
    /// Número único de la cita (opcional)
    /// </summary>
    public string? NumeroCita { get; init; }

    /// <summary>
    /// Tiempo antes de la cita para el recordatorio (en horas)
    /// </summary>
    public int? HorasAntes { get; init; }
}
