namespace ElectroHuila.Application.DTOs.AvailableTimes;

/// <summary>
/// Data transfer object representing an available time slot for appointments.
/// Used to return information about time slots configured for specific branches and appointment types.
/// </summary>
public class AvailableTimeDto
{
    /// <summary>
    /// The unique identifier for this available time slot.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The time value for this slot (e.g., "09:00", "14:30").
    /// </summary>
    public string Time { get; set; } = string.Empty;

    /// <summary>
    /// The ID of the branch where this time slot is available.
    /// </summary>
    public int BranchId { get; set; }

    /// <summary>
    /// The name of the branch where this time slot is available.
    /// </summary>
    public string? BranchName { get; set; }

    /// <summary>
    /// The ID of the appointment type this time slot is designated for, if specific to a type.
    /// </summary>
    public int? AppointmentTypeId { get; set; }

    /// <summary>
    /// The name of the appointment type this time slot is designated for, if specific to a type.
    /// </summary>
    public string? AppointmentTypeName { get; set; }

    /// <summary>
    /// The date and time when this time slot was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// The date and time when this time slot was last updated, if applicable.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Indicates whether this time slot is currently active and available for booking.
    /// </summary>
    public bool IsActive { get; set; }
}
