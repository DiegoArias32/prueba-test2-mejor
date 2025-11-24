namespace ElectroHuila.Application.DTOs.Auth;

/// <summary>
/// Data transfer object representing the response after a successful login.
/// Contains authentication tokens and user information.
/// </summary>
public class LoginResponseDto
{
    /// <summary>
    /// The JWT access token for authenticating API requests.
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// The refresh token used to obtain new access tokens when they expire.
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// The date and time when the access token expires.
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// The authenticated user's details.
    /// </summary>
    public UserDetailsDto User { get; set; } = new();

    /// <summary>
    /// The list of roles assigned to the authenticated user.
    /// </summary>
    public IEnumerable<string> Roles { get; set; } = new List<string>();

    /// <summary>
    /// The list of permissions granted to the authenticated user.
    /// </summary>
    public IEnumerable<string> Permissions { get; set; } = new List<string>();
}

/// <summary>
/// Data transfer object representing basic user details included in authentication responses.
/// Contains essential user information for client-side display.
/// </summary>
public class UserDetailsDto
{
    /// <summary>
    /// The unique identifier of the user.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The username of the authenticated user.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// The email address of the authenticated user.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Comma-separated list of tab identifiers the user is allowed to access in the UI.
    /// </summary>
    public string? AllowedTabs { get; set; }
}