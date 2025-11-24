namespace ElectroHuila.Application.DTOs.Permissions;

/// <summary>
/// Data transfer object for updating role permissions for a specific form.
/// Used when modifying the CRUD permissions a role has on a particular form or module.
/// </summary>
public class UpdateRolFormPermissionDto
{
    /// <summary>
    /// The ID of the role whose permissions are being updated.
    /// </summary>
    public int RolId { get; set; }

    /// <summary>
    /// The ID of the form or module to update permissions for.
    /// </summary>
    public int FormId { get; set; }

    /// <summary>
    /// Indicates whether the role can insert/create new records in this form.
    /// </summary>
    public bool CanInsert { get; set; }

    /// <summary>
    /// Indicates whether the role can update existing records in this form.
    /// </summary>
    public bool CanUpdate { get; set; }

    /// <summary>
    /// Indicates whether the role can delete records in this form.
    /// </summary>
    public bool CanDelete { get; set; }

    /// <summary>
    /// Indicates whether the role can view/read data in this form.
    /// </summary>
    public bool CanView { get; set; }
}
