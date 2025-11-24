namespace ElectroHuila.Domain.Exceptions.Appointments;

/// <summary>
/// Excepción que se lanza cuando no se encuentra una cita
/// </summary>
public sealed class AppointmentNotFoundException : DomainException
{
    /// <summary>
    /// Constructor para excepción de cita no encontrada por ID
    /// </summary>
    /// <param name="appointmentId">ID de la cita no encontrada</param>
    public AppointmentNotFoundException(int appointmentId)
        : base("APPOINTMENT_NOT_FOUND", $"Appointment with ID {appointmentId} was not found.", new { AppointmentId = appointmentId })
    {
    }

    /// <summary>
    /// Constructor para excepción de cita no encontrada por número
    /// </summary>
    /// <param name="appointmentNumber">Número de la cita no encontrada</param>
    public AppointmentNotFoundException(string appointmentNumber)
        : base("APPOINTMENT_NOT_FOUND", $"Appointment with number {appointmentNumber} was not found.", new { AppointmentNumber = appointmentNumber })
    {
    }
}