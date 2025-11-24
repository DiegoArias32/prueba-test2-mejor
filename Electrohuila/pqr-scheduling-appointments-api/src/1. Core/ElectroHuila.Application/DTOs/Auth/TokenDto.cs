namespace ElectroHuila.Application.DTOs.Auth;

/// <summary>
/// Data transfer object representing JWT authentication tokens.
/// Used when issuing or refreshing authentication tokens.
/// </summary>
public class TokenDto
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
    /// The type of token, typically "Bearer" for JWT tokens.
    /// </summary>
    public string TokenType { get; set; } = "Bearer";
}