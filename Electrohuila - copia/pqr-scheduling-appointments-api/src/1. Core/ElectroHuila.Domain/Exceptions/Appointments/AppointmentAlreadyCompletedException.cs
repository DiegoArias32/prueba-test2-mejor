namespace ElectroHuila.Domain.Exceptions.Appointments;

/// <summary>
/// Excepción que se lanza cuando se intenta modificar una cita que ya está completada
/// </summary>
public sealed class AppointmentAlreadyCompletedException : DomainException
{
    /// <summary>
    /// Constructor para excepción de cita ya completada por ID
    /// </summary>
    /// <param name="appointmentId">ID de la cita</param>
    /// <param name="completedDate">Fecha en que fue completada</param>
    public AppointmentAlreadyCompletedException(int appointmentId, DateTime completedDate)
        : base("APPOINTMENT_ALREADY_COMPLETED",
               $"Appointment with ID {appointmentId} was already completed on {completedDate:yyyy-MM-dd HH:mm}.",
               new { AppointmentId = appointmentId, CompletedDate = completedDate })
    {
    }

    /// <summary>
    /// Constructor para excepción de cita ya completada por número
    /// </summary>
    /// <param name="appointmentNumber">Número de la cita</param>
    /// <param name="completedDate">Fecha en que fue completada</param>
    public AppointmentAlreadyCompletedException(string appointmentNumber, DateTime completedDate)
        : base("APPOINTMENT_ALREADY_COMPLETED",
               $"Appointment {appointmentNumber} was already completed on {completedDate:yyyy-MM-dd HH:mm}.",
               new { AppointmentNumber = appointmentNumber, CompletedDate = completedDate })
    {
    }
}