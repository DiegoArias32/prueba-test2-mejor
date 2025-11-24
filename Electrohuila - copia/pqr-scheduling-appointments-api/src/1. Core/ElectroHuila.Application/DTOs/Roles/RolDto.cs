namespace ElectroHuila.Application.DTOs.Roles;

/// <summary>
/// Data transfer object representing a role in the system.
/// Used to return role information including user count statistics.
/// </summary>
public class RolDto
{
    /// <summary>
    /// The unique identifier of the role.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The display name of the role.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The unique code identifier for the role.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// The date and time when this role was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// The date and time when this role was last updated, if applicable.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Indicates whether this role is currently active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// The number of users assigned to this role.
    /// </summary>
    public int UsersCount { get; set; }
}