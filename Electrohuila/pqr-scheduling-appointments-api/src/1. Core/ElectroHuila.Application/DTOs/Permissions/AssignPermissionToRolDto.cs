namespace ElectroHuila.Application.DTOs.Permissions;

/// <summary>
/// Data transfer object for assigning permissions to a role for a specific form.
/// Used when configuring CRUD permissions for a role on a particular form or module.
/// </summary>
public class AssignPermissionToRolDto
{
    /// <summary>
    /// The ID of the role to assign permissions to.
    /// </summary>
    public int RolId { get; set; }

    /// <summary>
    /// The ID of the form or module to grant permissions for.
    /// </summary>
    public int FormId { get; set; }

    /// <summary>
    /// Indicates whether the role can read/view data in this form.
    /// </summary>
    public bool CanRead { get; set; }

    /// <summary>
    /// Indicates whether the role can create new records in this form.
    /// </summary>
    public bool CanCreate { get; set; }

    /// <summary>
    /// Indicates whether the role can update existing records in this form.
    /// </summary>
    public bool CanUpdate { get; set; }

    /// <summary>
    /// Indicates whether the role can delete records in this form.
    /// </summary>
    public bool CanDelete { get; set; }
}
