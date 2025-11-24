namespace ElectroHuila.Application.DTOs.Appointments;

/// <summary>
/// Data transfer object for creating a new appointment in the system.
/// Contains the required information to schedule an appointment for a client.
/// </summary>
public class CreateAppointmentDto
{
    /// <summary>
    /// Gets or sets the date when the appointment should be scheduled.
    /// </summary>
    public DateTime AppointmentDate { get; set; }

    /// <summary>
    /// Gets or sets the time slot for the appointment.
    /// Should correspond to an available time slot for the selected branch and appointment type.
    /// </summary>
    public string? AppointmentTime { get; set; }

    /// <summary>
    /// Gets or sets optional notes or special requirements for the appointment.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the client who is booking the appointment.
    /// </summary>
    public int ClientId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the branch where the appointment will take place.
    /// </summary>
    public int BranchId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the type of service or appointment being scheduled.
    /// </summary>
    public int AppointmentTypeId { get; set; }
}