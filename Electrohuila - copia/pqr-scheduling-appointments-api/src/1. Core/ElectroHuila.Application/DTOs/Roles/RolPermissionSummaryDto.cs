using ElectroHuila.Application.DTOs.Auth;

namespace ElectroHuila.Application.DTOs.Roles;

/// <summary>
/// Data transfer object representing a summary of a role's permissions.
/// Used to display role permissions with statistics in summary views and reports.
/// </summary>
public class RolPermissionSummaryDto
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

    /// <summary>
    /// The total number of permissions configured for this role.
    /// </summary>
    public int TotalPermissions { get; set; }

    /// <summary>
    /// The number of currently active permissions for this role.
    /// </summary>
    public int ActivePermissions { get; set; }
}