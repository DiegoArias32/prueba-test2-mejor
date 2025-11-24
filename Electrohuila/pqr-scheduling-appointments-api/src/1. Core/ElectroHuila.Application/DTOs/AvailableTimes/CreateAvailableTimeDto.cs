namespace ElectroHuila.Application.DTOs.AvailableTimes;

/// <summary>
/// Data transfer object for creating a new available time slot.
/// Used when adding individual time slots to the system for appointment scheduling.
/// </summary>
public class CreateAvailableTimeDto
{
    /// <summary>
    /// The time value for the new slot (e.g., "09:00", "14:30").
    /// </summary>
    public string Time { get; set; } = string.Empty;

    /// <summary>
    /// The ID of the branch where this time slot will be available.
    /// </summary>
    public int BranchId { get; set; }

    /// <summary>
    /// The optional ID of the appointment type this time slot is designated for.
    /// </summary>
    public int? AppointmentTypeId { get; set; }
}
