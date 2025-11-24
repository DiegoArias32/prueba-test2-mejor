namespace ElectroHuila.Application.DTOs.Notifications;

/// <summary>
/// Data transfer object for creating a new notification.
/// Contains the required data to send a notification to a user.
/// </summary>
public class CreateNotificationDto
{
    /// <summary>
    /// Gets or sets the identifier of the user who will receive the notification.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the associated appointment (optional).
    /// </summary>
    public int? AppointmentId { get; set; }

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
    /// Gets or sets additional metadata in JSON format (optional).
    /// Can contain reference IDs, personalization parameters, etc.
    /// </summary>
    public string? Metadata { get; set; }
}
