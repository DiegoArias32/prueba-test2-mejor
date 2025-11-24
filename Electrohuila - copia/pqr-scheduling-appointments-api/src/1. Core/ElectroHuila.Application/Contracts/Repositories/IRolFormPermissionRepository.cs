namespace ElectroHuila.Application.Contracts.Repositories;

/// <summary>
/// Repository interface for managing role-form-permission assignments in the security system.
/// Provides methods for querying and updating the relationship between roles, forms, and their specific permissions.
/// </summary>
public interface IRolFormPermissionRepository
{
    /// <summary>
    /// Retrieves a summary of all role permissions in the system.
    /// </summary>
    /// <returns>An object containing summarized role permission data.</returns>
    Task<object> GetAllRolPermissionsSummaryAsync();

    /// <summary>
    /// Retrieves role-form permission assignments filtered by role and/or form.
    /// </summary>
    /// <param name="rolId">Optional role identifier to filter by.</param>
    /// <param name="formId">Optional form identifier to filter by.</param>
    /// <returns>An object containing the permission assignments matching the criteria.</returns>
    Task<object> GetRolFormPermissionsAssignmentsAsync(int? rolId, int? formId);

    /// <summary>
    /// Updates the permissions for a specific role-form combination.
    /// Allows granular control over insert, update, delete, and view permissions.
    /// </summary>
    /// <param name="rolId">The unique identifier of the role.</param>
    /// <param name="formId">The unique identifier of the form.</param>
    /// <param name="canInsert">Whether the role can insert records in this form.</param>
    /// <param name="canUpdate">Whether the role can update records in this form.</param>
    /// <param name="canDelete">Whether the role can delete records in this form.</param>
    /// <param name="canView">Whether the role can view records in this form.</param>
    Task UpdateRolFormPermissionAsync(int rolId, int formId, bool canInsert, bool canUpdate, bool canDelete, bool canView);
}
