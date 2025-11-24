namespace ElectroHuila.Domain.Exceptions.Appointments;

/// <summary>
/// Excepción que se lanza cuando un cliente intenta agendar una nueva cita
/// pero tiene citas pendientes o no asistidas
/// </summary>
public sealed class ClientHasPendingAppointmentsException : DomainException
{
    /// <summary>
    /// Crea una nueva excepción indicando que el cliente tiene citas pendientes o no asistidas
    /// </summary>
    /// <param name="documentNumber">Número de documento del cliente</param>
    /// <param name="pendingAppointmentCount">Cantidad de citas pendientes o no asistidas</param>
    public ClientHasPendingAppointmentsException(string documentNumber, int pendingAppointmentCount)
        : base("CLIENT_HAS_PENDING_APPOINTMENTS",
               $"Client with document number '{documentNumber}' has {pendingAppointmentCount} pending or unattended appointment(s). Please complete or attend existing appointments before scheduling a new one.",
               new { DocumentNumber = documentNumber, PendingAppointmentCount = pendingAppointmentCount })
    {
    }

    /// <summary>
    /// Crea una nueva excepción indicando que el cliente tiene citas pendientes o no asistidas con detalles
    /// </summary>
    /// <param name="documentNumber">Número de documento del cliente</param>
    /// <param name="appointmentNumbers">Números de las citas pendientes</param>
    public ClientHasPendingAppointmentsException(string documentNumber, IEnumerable<string> appointmentNumbers)
        : base("CLIENT_HAS_PENDING_APPOINTMENTS",
               $"Client with document number '{documentNumber}' has pending or unattended appointments: {string.Join(", ", appointmentNumbers)}. Please complete or attend existing appointments before scheduling a new one.",
               new { DocumentNumber = documentNumber, AppointmentNumbers = appointmentNumbers })
    {
    }
}
