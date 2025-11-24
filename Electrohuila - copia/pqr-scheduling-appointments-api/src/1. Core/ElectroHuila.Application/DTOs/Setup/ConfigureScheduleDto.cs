namespace ElectroHuila.Application.DTOs.Setup;

/// <summary>
/// Data transfer object for configuring available schedules for a branch and appointment type.
/// Used during system setup to define time slots for appointments.
/// </summary>
public class ConfigureScheduleDto
{
    /// <summary>
    /// The ID of the branch to configure schedules for.
    /// </summary>
    public int BranchId { get; set; }

    /// <summary>
    /// The ID of the appointment type to configure schedules for.
    /// </summary>
    public int AppointmentTypeId { get; set; }

    /// <summary>
    /// The list of time slots to make available (e.g., "09:00", "10:30", "14:00").
    /// </summary>
    public List<string> Times { get; set; } = new();
}
