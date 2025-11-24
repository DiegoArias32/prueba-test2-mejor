namespace ElectroHuila.Application.DTOs.Permissions;

/// <summary>
/// Data transfer object for updating the allowed tabs for a user.
/// Used when modifying which UI tabs a user can access in the application.
/// </summary>
public class UpdateUserTabsDto
{
    /// <summary>
    /// The ID of the user whose allowed tabs are being updated.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Comma-separated list of tab identifiers the user is allowed to access.
    /// </summary>
    public string AllowedTabs { get; set; } = string.Empty;
}
