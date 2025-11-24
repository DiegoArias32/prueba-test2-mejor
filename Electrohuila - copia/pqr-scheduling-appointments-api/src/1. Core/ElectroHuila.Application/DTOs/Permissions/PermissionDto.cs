namespace ElectroHuila.Application.DTOs.Permissions;

/// <summary>
/// Data transfer object representing a permission with CRUD capabilities.
/// Used to return permission information including what operations are allowed.
/// </summary>
public class PermissionDto
{
    /// <summary>
    /// The unique identifier of the permission.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Indicates whether this permission grants read/view access.
    /// </summary>
    public bool CanRead { get; set; }

    /// <summary>
    /// Indicates whether this permission grants create access.
    /// </summary>
    public bool CanCreate { get; set; }

    /// <summary>
    /// Indicates whether this permission grants update access.
    /// </summary>
    public bool CanUpdate { get; set; }

    /// <summary>
    /// Indicates whether this permission grants delete access.
    /// </summary>
    public bool CanDelete { get; set; }

    /// <summary>
    /// The date and time when this permission was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// The date and time when this permission was last updated, if applicable.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Indicates whether this permission is currently active.
    /// </summary>
    public bool IsActive { get; set; }
}