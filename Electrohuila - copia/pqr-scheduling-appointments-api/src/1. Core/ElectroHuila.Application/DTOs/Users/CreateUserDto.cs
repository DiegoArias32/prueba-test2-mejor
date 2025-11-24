namespace ElectroHuila.Application.DTOs.Users;

/// <summary>
/// Data transfer object for creating a new user.
/// Used when registering a new user account in the system.
/// </summary>
public class CreateUserDto
{
    /// <summary>
    /// The username for the new user account.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// The email address for the new user account.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The password for the new user account.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Comma-separated list of tab identifiers the user is allowed to access in the UI.
    /// </summary>
    public string? AllowedTabs { get; set; }
}