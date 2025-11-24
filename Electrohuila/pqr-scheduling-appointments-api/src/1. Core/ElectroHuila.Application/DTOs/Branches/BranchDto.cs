namespace ElectroHuila.Application.DTOs.Branches;

/// <summary>
/// Data transfer object representing complete branch information.
/// Used to return full details of a branch in API responses and detail views.
/// </summary>
public class BranchDto
{
    /// <summary>
    /// The unique identifier of the branch.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The name of the branch.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The unique code identifier for the branch.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// The physical address of the branch.
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// The contact phone number for the branch.
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// The city where the branch is located.
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// The state or department where the branch is located.
    /// </summary>
    public string State { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether this is the main or headquarters branch.
    /// </summary>
    public bool IsMain { get; set; }

    /// <summary>
    /// The date and time when this branch was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// The date and time when this branch was last updated, if applicable.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Indicates whether this branch is currently active and operational.
    /// </summary>
    public bool IsActive { get; set; }
}