namespace ElectroHuila.Application.DTOs.Roles;

/// <summary>
/// Data transfer object for creating a new role.
/// Used when defining new user roles in the system.
/// </summary>
public class CreateRolDto
{
    /// <summary>
    /// The display name for the new role.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The unique code identifier for the new role.
    /// </summary>
    public string Code { get; set; } = string.Empty;
}