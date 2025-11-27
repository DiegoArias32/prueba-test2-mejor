namespace ElectroHuila.Application.DTOs.Branches;

/// <summary>
/// Data transfer object for updating an existing branch.
/// Used when modifying branch information such as contact details or location.
/// </summary>
public class UpdateBranchDto
{
    /// <summary>
    /// The updated name for the branch.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The updated code identifier for the branch.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// The updated physical address for the branch.
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// The updated contact phone number for the branch.
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// The updated city where the branch is located.
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// The updated state or department where the branch is located.
    /// </summary>
    public string State { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether this should be designated as the main or headquarters branch.
    /// </summary>
    public bool IsMain { get; set; }

    /// <summary>
    /// Indicates whether the branch is active.
    /// If null, the current value will be preserved.
    /// </summary>
    public bool? IsActive { get; set; }
}