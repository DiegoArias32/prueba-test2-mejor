namespace ElectroHuila.Application.DTOs.Appointments;

/// <summary>
/// Data transfer object for completing an appointment.
/// Used when marking an appointment as completed after the service has been provided.
/// </summary>
public class CompleteAppointmentDto
{
    /// <summary>
    /// Optional notes or comments about the completed appointment.
    /// </summary>
    public string? Notes { get; set; }
}
