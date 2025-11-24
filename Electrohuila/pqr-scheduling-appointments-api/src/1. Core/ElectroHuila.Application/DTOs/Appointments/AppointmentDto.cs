namespace ElectroHuila.Application.DTOs.Appointments;

/// <summary>
/// Data transfer object representing an appointment in the system.
/// Used for transferring complete appointment information between layers.
/// </summary>
public class AppointmentDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the appointment.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the unique appointment number used for tracking and reference.
    /// </summary>
    public string AppointmentNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date when the appointment is scheduled.
    /// </summary>
    public DateTime AppointmentDate { get; set; }

    /// <summary>
    /// Gets or sets the time slot for the appointment.
    /// </summary>
    public string? AppointmentTime { get; set; }

    /// <summary>
    /// Gets or sets the current status of the appointment (e.g., Pending, Completed, Cancelled).
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets additional notes or comments about the appointment.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the appointment was completed.
    /// Null if the appointment has not been completed yet.
    /// </summary>
    public DateTime? CompletedDate { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the client associated with this appointment.
    /// </summary>
    public int ClientId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the branch where the appointment is scheduled.
    /// </summary>
    public int BranchId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the type of appointment.
    /// </summary>
    public int AppointmentTypeId { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the appointment was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time of the last update to the appointment.
    /// Null if the appointment has never been updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the appointment is active.
    /// </summary>
    public bool IsActive { get; set; }
}