namespace ElectroHuila.Application.DTOs.Users;

/// <summary>
/// Data transfer object for updating an existing user.
/// Used when modifying user account information.
/// </summary>
public class UpdateUserDto
{
    /// <summary>
    /// The updated username for the user account.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// The updated email address for the user account.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The updated comma-separated list of tab identifiers the user is allowed to access.
    /// </summary>
    public string? AllowedTabs { get; set; }
}