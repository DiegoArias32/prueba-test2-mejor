namespace ElectroHuila.Application.DTOs.Notifications;

/// <summary>
/// Data transfer object representing a notification in the system.
/// Used for transferring complete notification information between layers.
/// </summary>
public class NotificationDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the notification.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user (admin) receiving the notification.
    /// Null if the notification is for a client.
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// Gets or sets the name of the user (admin) receiving the notification.
    /// Null if the notification is for a client.
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the client receiving the notification.
    /// Null if the notification is for a user (admin).
    /// </summary>
    public int? ClientId { get; set; }

    /// <summary>
    /// Gets or sets the name of the client receiving the notification.
    /// Null if the notification is for a user (admin).
    /// </summary>
    public string? ClientName { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the associated appointment (if any).
    /// </summary>
    public int? AppointmentId { get; set; }

    /// <summary>
    /// Gets or sets the appointment number (if associated with an appointment).
    /// </summary>
    public string? AppointmentNumber { get; set; }

    /// <summary>
    /// Gets or sets the type of notification: EMAIL, SMS, WHATSAPP, IN_APP.
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the title or subject of the notification.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the message content of the notification.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current status: PENDING, SENT, FAILED.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date and time when the notification was sent.
    /// Null if not yet sent.
    /// </summary>
    public DateTime? SentAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the notification was read by the user.
    /// Null if not yet read (only applicable for IN_APP notifications).
    /// </summary>
    public DateTime? ReadAt { get; set; }

    /// <summary>
    /// Gets or sets whether the notification has been read.
    /// Only applicable for IN_APP notifications.
    /// </summary>
    public bool IsRead { get; set; }

    /// <summary>
    /// Gets or sets the error message if the notification failed to send.
    /// Null if no errors occurred.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets additional metadata in JSON format.
    /// </summary>
    public string? Metadata { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the notification was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time of the last update to the notification.
    /// Null if the notification has never been updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets whether the notification is active.
    /// </summary>
    public bool IsActive { get; set; }
}
