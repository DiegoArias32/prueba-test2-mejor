namespace ElectroHuila.Application.DTOs.Users;

/// <summary>
/// Data transfer object representing basic user information.
/// Used to return user data in list views and summary responses.
/// </summary>
public class UserDto
{
    /// <summary>
    /// The unique identifier of the user.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The username of the user.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// The email address of the user.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Comma-separated list of tab identifiers the user is allowed to access in the UI.
    /// </summary>
    public string? AllowedTabs { get; set; }

    /// <summary>
    /// The date and time when this user account was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// The date and time when this user account was last updated, if applicable.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Indicates whether this user account is currently active.
    /// </summary>
    public bool IsActive { get; set; }
}