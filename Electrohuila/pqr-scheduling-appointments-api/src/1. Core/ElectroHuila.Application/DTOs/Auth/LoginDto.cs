namespace ElectroHuila.Application.DTOs.Auth;

/// <summary>
/// Data transfer object for user authentication/login.
/// Contains credentials and options for the login process.
/// </summary>
public class LoginDto
{
    /// <summary>
    /// Gets or sets the username for authentication.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password for authentication.
    /// This should be handled securely and never logged or exposed.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the user session should be persisted.
    /// When true, the authentication token will have a longer expiration time.
    /// </summary>
    public bool RememberMe { get; set; } = false;
}