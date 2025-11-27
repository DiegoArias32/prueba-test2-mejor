namespace ElectroHuila.Application.DTOs.AppointmentTypes;

/// <summary>
/// Data transfer object for updating an existing appointment type.
/// Used in update operations to modify appointment type information.
/// All fields are optional - only provided fields will be updated.
/// </summary>
public class UpdateAppointmentTypeDto
{
    /// <summary>
    /// The updated name for the appointment type.
    /// If null, the current value will be preserved.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// The updated description for the appointment type.
    /// If null, the current value will be preserved.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The updated icon identifier for the appointment type.
    /// If null, the current value will be preserved.
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// The updated estimated duration in minutes for this appointment type.
    /// If null, the current value will be preserved.
    /// </summary>
    public int? EstimatedTimeMinutes { get; set; }

    /// <summary>
    /// Indicates whether appointments of this type should require documentation.
    /// If null, the current value will be preserved.
    /// </summary>
    public bool? RequiresDocumentation { get; set; }

    /// <summary>
    /// Indicates whether the appointment type is active.
    /// If null, the current value will be preserved.
    /// </summary>
    public bool? IsActive { get; set; }
}
