namespace ElectroHuila.Application.DTOs.Branches;

/// <summary>
/// Data transfer object for creating a new branch.
/// Used when registering a new branch location in the system.
/// </summary>
public class CreateBranchDto
{
    /// <summary>
    /// The name of the new branch.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The unique code identifier for the new branch.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// The physical address of the new branch.
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// The contact phone number for the new branch.
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// The city where the new branch will be located.
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// The state or department where the new branch will be located.
    /// </summary>
    public string State { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether this will be the main or headquarters branch.
    /// </summary>
    public bool IsMain { get; set; }
}