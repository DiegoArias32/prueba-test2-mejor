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
    /// Full name of the user.
    /// </summary>
    public string? FullName { get; set; }

    /// <summary>
    /// Identification type (e.g., "CC", "TI", "CE", "PP", "NIT").
    /// </summary>
    public string? IdentificationType { get; set; }

    /// <summary>
    /// Identification number.
    /// </summary>
    public string? IdentificationNumber { get; set; }

    /// <summary>
    /// Contact phone number.
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Physical address.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Comma-separated list of tab identifiers the user is allowed to access in the UI.
    /// </summary>
    public string? AllowedTabs { get; set; }

    /// <summary>
    /// List of role IDs to assign to the user.
    /// </summary>
    public List<int>? RoleIds { get; set; }
}