namespace ElectroHuila.Application.DTOs.AvailableTimes;

/// <summary>
/// Data transfer object for creating multiple available time slots at once.
/// Used when configuring multiple time slots simultaneously for a branch and appointment type.
/// </summary>
public class BulkCreateAvailableTimesDto
{
    /// <summary>
    /// The ID of the branch where these time slots will be available.
    /// </summary>
    public int BranchId { get; set; }

    /// <summary>
    /// The optional ID of the appointment type these time slots are designated for.
    /// </summary>
    public int? AppointmentTypeId { get; set; }

    /// <summary>
    /// The list of time slots to create.
    /// </summary>
    public List<TimeSlotDto> TimeSlots { get; set; } = new();
}
