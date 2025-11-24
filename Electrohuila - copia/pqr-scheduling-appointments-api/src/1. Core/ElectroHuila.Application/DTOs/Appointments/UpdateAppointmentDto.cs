namespace ElectroHuila.Application.DTOs.Appointments;

/// <summary>
/// Data transfer object for updating an existing appointment.
/// Used when modifying appointment details such as date, time, status, or location.
/// </summary>
public class UpdateAppointmentDto
{
    /// <summary>
    /// The updated date for the appointment.
    /// </summary>
    public DateTime AppointmentDate { get; set; }

    /// <summary>
    /// The updated time slot for the appointment.
    /// </summary>
    public string? AppointmentTime { get; set; }

    /// <summary>
    /// The updated status ID of the appointment (references AppointmentStatuses catalog).
    /// </summary>
    public int StatusId { get; set; }

    /// <summary>
    /// Optional notes or comments about the appointment.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// The updated branch ID where the appointment will take place.
    /// </summary>
    public int BranchId { get; set; }

    /// <summary>
    /// The updated appointment type ID.
    /// </summary>
    public int AppointmentTypeId { get; set; }
}