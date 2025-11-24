namespace ElectroHuila.Application.DTOs.Notifications;

/// <summary>
/// Simplified data transfer object for listing notifications.
/// Used for efficient notification list displays with essential information only.
/// </summary>
public class NotificationListDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the notification.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the type of notification: EMAIL, SMS, WHATSAPP, IN_APP.
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the title or subject of the notification.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a brief preview of the message content.
    /// Typically the first 100 characters of the message.
    /// </summary>
    public string MessagePreview { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current status: PENDING, SENT, FAILED.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the notification has been read.
    /// Only applicable for IN_APP notifications.
    /// </summary>
    public bool IsRead { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the notification was sent.
    /// Null if not yet sent.
    /// </summary>
    public DateTime? SentAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the notification was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the appointment number (if associated with an appointment).
    /// </summary>
    public string? AppointmentNumber { get; set; }

    /// <summary>
    /// Gets or sets the client name (if the notification is for a client).
    /// Useful for admins to see which client received the notification.
    /// </summary>
    public string? ClientName { get; set; }

    /// <summary>
    /// Gets or sets the user name (if the notification is for a user/admin).
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// Gets or sets the client ID (if the notification is for a client).
    /// </summary>
    public int? ClientId { get; set; }

    /// <summary>
    /// Gets or sets the user ID (if the notification is for a user/admin).
    /// </summary>
    public int? UserId { get; set; }
}
