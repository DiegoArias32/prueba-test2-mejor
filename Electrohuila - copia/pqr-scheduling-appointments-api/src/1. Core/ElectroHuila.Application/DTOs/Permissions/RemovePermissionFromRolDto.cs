namespace ElectroHuila.Application.DTOs.Permissions;

/// <summary>
/// Data transfer object for removing permissions from a role for a specific form.
/// Used when revoking role access to a form or module.
/// </summary>
public class RemovePermissionFromRolDto
{
    /// <summary>
    /// The ID of the role to remove permissions from.
    /// </summary>
    public int RolId { get; set; }

    /// <summary>
    /// The ID of the form or module to revoke permissions for.
    /// </summary>
    public int FormId { get; set; }
}
