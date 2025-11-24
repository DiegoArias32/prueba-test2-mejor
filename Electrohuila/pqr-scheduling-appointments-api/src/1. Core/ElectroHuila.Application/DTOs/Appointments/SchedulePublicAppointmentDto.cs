namespace ElectroHuila.Application.DTOs.Appointments;

/// <summary>
/// Data transfer object for scheduling a public appointment for an existing client.
/// Used when clients schedule appointments through public-facing interfaces using their client number.
/// </summary>
public class SchedulePublicAppointmentDto
{
    /// <summary>
    /// The unique client number identifying the customer making the appointment.
    /// </summary>
    public string ClientNumber { get; set; } = string.Empty;

    /// <summary>
    /// The ID of the branch where the appointment will take place.
    /// </summary>
    public int BranchId { get; set; }

    /// <summary>
    /// The ID of the appointment type being scheduled.
    /// </summary>
    public int AppointmentTypeId { get; set; }

    /// <summary>
    /// The date when the appointment is scheduled.
    /// </summary>
    public DateTime AppointmentDate { get; set; }

    /// <summary>
    /// The specific time slot for the appointment.
    /// </summary>
    public TimeSpan AppointmentTime { get; set; }

    /// <summary>
    /// Optional observations or special requests for the appointment.
    /// </summary>
    public string? Observations { get; set; }
}
