namespace ElectroHuila.Application.DTOs.AvailableTimes;

/// <summary>
/// Data transfer object representing a simple time slot.
/// Used in bulk operations to specify individual time values.
/// </summary>
public class TimeSlotDto
{
    /// <summary>
    /// The time value for this slot (e.g., "09:00", "14:30").
    /// </summary>
    public string Time { get; set; } = string.Empty;
}
