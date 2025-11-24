namespace ElectroHuila.Application.DTOs.AppointmentTypes;

/// <summary>
/// Data transfer object representing an appointment type in the system.
/// Used to return appointment type information in API responses and queries.
/// </summary>
public class AppointmentTypeDto
{
    /// <summary>
    /// The unique identifier for the appointment type.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The name of the appointment type.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// A detailed description of what this appointment type is for.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The icon identifier used to visually represent this appointment type.
    /// </summary>
    public string Icon { get; set; } = string.Empty;

    /// <summary>
    /// The estimated duration in minutes for appointments of this type.
    /// </summary>
    public int EstimatedTimeMinutes { get; set; }

    /// <summary>
    /// Indicates whether appointments of this type require supporting documentation.
    /// </summary>
    public bool RequiresDocumentation { get; set; }

    /// <summary>
    /// The date and time when this appointment type was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// The date and time when this appointment type was last updated, if applicable.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Indicates whether this appointment type is currently active and available for use.
    /// </summary>
    public bool IsActive { get; set; }
}
