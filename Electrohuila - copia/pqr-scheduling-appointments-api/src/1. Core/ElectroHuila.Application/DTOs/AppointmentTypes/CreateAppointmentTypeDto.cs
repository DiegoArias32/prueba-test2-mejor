namespace ElectroHuila.Application.DTOs.AppointmentTypes;

/// <summary>
/// Data transfer object for creating a new appointment type.
/// Used when defining new types of appointments that can be scheduled in the system.
/// </summary>
public class CreateAppointmentTypeDto
{
    /// <summary>
    /// The name of the new appointment type.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// A description explaining the purpose of this appointment type.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The icon identifier to represent this appointment type visually.
    /// </summary>
    public string Icon { get; set; } = string.Empty;

    /// <summary>
    /// The estimated duration in minutes for appointments of this type. Defaults to 120 minutes.
    /// </summary>
    public int EstimatedTimeMinutes { get; set; } = 120;

    /// <summary>
    /// Indicates whether appointments of this type require supporting documentation. Defaults to true.
    /// </summary>
    public bool RequiresDocumentation { get; set; } = true;
}
