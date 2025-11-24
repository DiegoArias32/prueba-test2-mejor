namespace ElectroHuila.Domain.Exceptions.Appointments;

/// <summary>
/// Excepción que se lanza cuando se intenta cancelar una cita que ya está cancelada
/// </summary>
public sealed class AppointmentAlreadyCancelledException : DomainException
{
    /// <summary>
    /// Constructor para excepción de cita ya cancelada por ID
    /// </summary>
    /// <param name="appointmentId">ID de la cita</param>
    /// <param name="cancelledDate">Fecha en que fue cancelada</param>
    public AppointmentAlreadyCancelledException(int appointmentId, DateTime cancelledDate)
        : base("APPOINTMENT_ALREADY_CANCELLED",
               $"Appointment with ID {appointmentId} was already cancelled on {cancelledDate:yyyy-MM-dd HH:mm}.",
               new { AppointmentId = appointmentId, CancelledDate = cancelledDate })
    {
    }

    /// <summary>
    /// Constructor para excepción de cita ya cancelada por número
    /// </summary>
    /// <param name="appointmentNumber">Número de la cita</param>
    /// <param name="cancelledDate">Fecha en que fue cancelada</param>
    public AppointmentAlreadyCancelledException(string appointmentNumber, DateTime cancelledDate)
        : base("APPOINTMENT_ALREADY_CANCELLED",
               $"Appointment {appointmentNumber} was already cancelled on {cancelledDate:yyyy-MM-dd HH:mm}.",
               new { AppointmentNumber = appointmentNumber, CancelledDate = cancelledDate })
    {
    }
}