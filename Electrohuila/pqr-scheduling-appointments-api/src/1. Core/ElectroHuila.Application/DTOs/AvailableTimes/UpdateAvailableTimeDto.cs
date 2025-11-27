namespace ElectroHuila.Application.DTOs.AvailableTimes;

/// <summary>
/// Data transfer object for updating an existing available time slot.
/// Used when modifying time slot details such as time value, branch, or appointment type.
/// </summary>
public class UpdateAvailableTimeDto
{
    /// <summary>
    /// The updated time value for the slot (e.g., "09:00", "14:30").
    /// </summary>
    public string Time { get; set; } = string.Empty;

    /// <summary>
    /// The updated branch ID where this time slot will be available.
    /// </summary>
    public int BranchId { get; set; }

    /// <summary>
    /// The optional updated appointment type ID this time slot is designated for.
    /// </summary>
    public int? AppointmentTypeId { get; set; }

    /// <summary>
    /// Indicates whether the available time is active.
    /// If null, the current value will be preserved.
    /// </summary>
    public bool? IsActive { get; set; }
}
