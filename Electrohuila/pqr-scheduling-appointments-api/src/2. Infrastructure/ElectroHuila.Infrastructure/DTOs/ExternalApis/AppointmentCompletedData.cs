namespace ElectroHuila.Infrastructure.DTOs.ExternalApis;

/// <summary>
/// DTO que contiene los datos necesarios para enviar una notificación de cita completada.
/// Se utiliza para integración con APIs externas de WhatsApp y Gmail.
/// </summary>
public record AppointmentCompletedData
{
    /// <summary>
    /// Nombre completo del cliente
    /// </summary>
    public required string NombreCliente { get; init; }

    /// <summary>
    /// Fecha de la cita completada en formato yyyy-MM-dd
    /// </summary>
    public required string Fecha { get; init; }

    /// <summary>
    /// Hora de la cita completada en formato HH:mm
    /// </summary>
    public required string Hora { get; init; }

    /// <summary>
    /// Ubicación donde se realizó la cita
    /// </summary>
    public required string Ubicacion { get; init; }

    /// <summary>
    /// Número único de la cita
    /// </summary>
    public string? NumeroCita { get; init; }

    /// <summary>
    /// Tipo de cita o servicio realizado (opcional)
    /// </summary>
    public string? TipoCita { get; init; }

    /// <summary>
    /// Observaciones sobre el servicio completado (opcional)
    /// </summary>
    public string? Observaciones { get; init; }
}
