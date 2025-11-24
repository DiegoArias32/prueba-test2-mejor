namespace ElectroHuila.Application.DTOs.Auth;

/// <summary>
/// Data transfer object representing a user's complete permission structure.
/// Used to retrieve and display all permissions granted to a user through their roles.
/// </summary>
public class UserPermissionsDto
{
    /// <summary>
    /// The unique identifier of the user.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// The username of the user.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// The collection of roles and their associated permissions for this user.
    /// </summary>
    public IEnumerable<RolePermissionDto> Roles { get; set; } = new List<RolePermissionDto>();

    /// <summary>
    /// A flattened list of all permissions available to the user across all roles.
    /// </summary>
    public IEnumerable<string> AllPermissions { get; set; } = new List<string>();

    /// <summary>
    /// Comma-separated list of tab identifiers the user is allowed to access.
    /// </summary>
    public string? AllowedTabs { get; set; }
}

/// <summary>
/// Data transfer object representing a role and its associated form permissions.
/// Used to show permissions grouped by role.
/// </summary>
public class RolePermissionDto
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

    /// <summary>
    /// The collection of form-level permissions associated with this role.
    /// </summary>
    public IEnumerable<FormPermissionDto> FormPermissions { get; set; } = new List<FormPermissionDto>();
}

/// <summary>
/// Data transfer object representing permissions for a specific form or module.
/// Defines what CRUD operations a user can perform on a particular form.
/// </summary>
public class FormPermissionDto
{
    /// <summary>
    /// The unique identifier of the form or module.
    /// </summary>
    public int FormId { get; set; }

    /// <summary>
    /// The display name of the form or module.
    /// </summary>
    public string FormName { get; set; } = string.Empty;

    /// <summary>
    /// The unique code identifier for the form or module.
    /// </summary>
    public string FormCode { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether the user can read/view data in this form.
    /// </summary>
    public bool CanRead { get; set; }

    /// <summary>
    /// Indicates whether the user can create new records in this form.
    /// </summary>
    public bool CanCreate { get; set; }

    /// <summary>
    /// Indicates whether the user can update existing records in this form.
    /// </summary>
    public bool CanUpdate { get; set; }

    /// <summary>
    /// Indicates whether the user can delete records in this form.
    /// </summary>
    public bool CanDelete { get; set; }
}