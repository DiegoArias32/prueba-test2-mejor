namespace ElectroHuila.Application.DTOs.Appointments;

/// <summary>
/// Data transfer object for canceling an appointment.
/// Used when a client or administrator needs to cancel a scheduled appointment.
/// </summary>
public class CancelAppointmentDto
{
    /// <summary>
    /// The optional reason for canceling the appointment.
    /// </summary>
    public string? Reason { get; set; }
}
