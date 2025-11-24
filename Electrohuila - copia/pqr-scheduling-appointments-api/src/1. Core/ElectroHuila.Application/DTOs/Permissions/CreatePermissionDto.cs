namespace ElectroHuila.Application.DTOs.Permissions;

/// <summary>
/// Data transfer object for creating a new permission.
/// Used when defining new permissions with specific CRUD capabilities.
/// </summary>
public class CreatePermissionDto
{
    /// <summary>
    /// Indicates whether the permission grants read/view access.
    /// </summary>
    public bool CanRead { get; set; }

    /// <summary>
    /// Indicates whether the permission grants create access.
    /// </summary>
    public bool CanCreate { get; set; }

    /// <summary>
    /// Indicates whether the permission grants update access.
    /// </summary>
    public bool CanUpdate { get; set; }

    /// <summary>
    /// Indicates whether the permission grants delete access.
    /// </summary>
    public bool CanDelete { get; set; }
}
