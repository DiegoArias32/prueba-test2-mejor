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
    /// The updated full name of the user.
    /// </summary>
    public string? FullName { get; set; }

    /// <summary>
    /// The updated identification type.
    /// </summary>
    public string? IdentificationType { get; set; }

    /// <summary>
    /// The updated identification number.
    /// </summary>
    public string? IdentificationNumber { get; set; }

    /// <summary>
    /// The updated contact phone number.
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// The updated physical address.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// The updated comma-separated list of tab identifiers the user is allowed to access.
    /// </summary>
    public string? AllowedTabs { get; set; }

    /// <summary>
    /// List of role IDs to assign to the user.
    /// </summary>
    public List<int>? RoleIds { get; set; }

    /// <summary>
    /// Indicates whether the user is active.
    /// If null, the current value will be preserved.
    /// </summary>
    public bool? IsActive { get; set; }
}