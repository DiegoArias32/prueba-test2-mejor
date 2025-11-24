namespace ElectroHuila.Application.Features.Appointments.Commands.ScheduleSimpleAppointment;

/// <summary>
/// Respuesta del comando de agendamiento simple de cita.
/// Contiene la información de confirmación de la cita creada.
/// </summary>
public record ScheduleSimpleAppointmentResponse
{
    /// <summary>
    /// Número único del cliente generado por el sistema
    /// </summary>
    public string ClientNumber { get; init; } = string.Empty;

    /// <summary>
    /// Número único de la cita generado por el sistema
    /// </summary>
    public string AppointmentNumber { get; init; } = string.Empty;

    /// <summary>
    /// Mensaje de confirmación de la operación
    /// </summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// Fecha de la cita agendada
    /// </summary>
    public DateTime AppointmentDate { get; init; }

    /// <summary>
    /// Hora de la cita agendada en formato HH:mm
    /// </summary>
    public string AppointmentTime { get; init; } = string.Empty;

    /// <summary>
    /// Nombre de la sucursal donde se realizará la cita
    /// </summary>
    public string BranchName { get; init; } = string.Empty;
}
