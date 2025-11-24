namespace ElectroHuila.Application.DTOs.Roles;

/// <summary>
/// Data transfer object for updating an existing role.
/// Used when modifying role information such as name or code.
/// </summary>
public class UpdateRolDto
{
    /// <summary>
    /// The updated display name for the role.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The updated code identifier for the role.
    /// </summary>
    public string Code { get; set; } = string.Empty;
}
