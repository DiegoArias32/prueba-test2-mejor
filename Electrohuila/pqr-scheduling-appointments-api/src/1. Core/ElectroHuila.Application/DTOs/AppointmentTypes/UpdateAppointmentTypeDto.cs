namespace ElectroHuila.Application.DTOs.AppointmentTypes;

/// <summary>
/// Data transfer object for updating an existing appointment type.
/// Used in update operations to modify appointment type information.
/// </summary>
public class UpdateAppointmentTypeDto
{
    /// <summary>
    /// The updated name for the appointment type.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The updated description for the appointment type.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The updated icon identifier for the appointment type.
    /// </summary>
    public string Icon { get; set; } = string.Empty;

    /// <summary>
    /// The updated estimated duration in minutes for this appointment type.
    /// </summary>
    public int EstimatedTimeMinutes { get; set; }

    /// <summary>
    /// Indicates whether appointments of this type should require documentation.
    /// </summary>
    public bool RequiresDocumentation { get; set; }
}
