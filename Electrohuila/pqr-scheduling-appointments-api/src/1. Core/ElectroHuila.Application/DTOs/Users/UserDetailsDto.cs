namespace ElectroHuila.Application.DTOs.Users;

/// <summary>
/// Data transfer object representing detailed user information.
/// Used to return complete user details including roles and permissions in API responses.
/// </summary>
public class UserDetailsDto
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

    /// <summary>
    /// The collection of roles assigned to this user.
    /// </summary>
    public IEnumerable<UserRoleDto> Roles { get; set; } = new List<UserRoleDto>();
}

/// <summary>
/// Data transfer object representing a role assigned to a user.
/// Used to show role information within user details.
/// </summary>
public class UserRoleDto
{
    /// <summary>
    /// The unique identifier of the role.
    /// </summary>
    public int RoleId { get; set; }

    /// <summary>
    /// The display name of the role.
    /// </summary>
    public string RoleName { get; set; } = string.Empty;

    /// <summary>
    /// The unique code identifier for the role.
    /// </summary>
    public string RoleCode { get; set; } = string.Empty;
}