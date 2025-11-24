using ElectroHuila.Domain.Entities.Security;

namespace ElectroHuila.Application.Contracts.Repositories;

/// <summary>
/// Repository interface for managing permissions in the security system.
/// Provides methods for CRUD operations and queries on permission entities that control access to system functionality.
/// </summary>
public interface IPermissionRepository
{
    /// <summary>
    /// Retrieves a permission by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the permission.</param>
    /// <returns>The permission if found; otherwise, null.</returns>
    Task<Permission?> GetByIdAsync(int id);

    /// <summary>
    /// Retrieves all permissions from the system.
    /// </summary>
    /// <returns>A collection of all permissions.</returns>
    Task<IEnumerable<Permission>> GetAllAsync();

    /// <summary>
    /// Retrieves all permissions assigned to a specific role.
    /// </summary>
    /// <param name="rolId">The unique identifier of the role.</param>
    /// <returns>A collection of permissions for the specified role.</returns>
    Task<IEnumerable<Permission>> GetByRolIdAsync(int rolId);

    /// <summary>
    /// Retrieves all permissions for a specific form.
    /// </summary>
    /// <param name="formId">The unique identifier of the form.</param>
    /// <returns>A collection of permissions for the specified form.</returns>
    Task<IEnumerable<Permission>> GetByFormIdAsync(int formId);

    /// <summary>
    /// Adds a new permission to the system.
    /// </summary>
    /// <param name="permission">The permission to add.</param>
    /// <returns>The added permission with generated values.</returns>
    Task<Permission> AddAsync(Permission permission);

    /// <summary>
    /// Updates an existing permission in the system.
    /// </summary>
    /// <param name="permission">The permission with updated values.</param>
    Task UpdateAsync(Permission permission);

    /// <summary>
    /// Deletes a permission by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the permission to delete.</param>
    Task DeleteAsync(int id);

    /// <summary>
    /// Checks if a permission exists by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier to check.</param>
    /// <returns>True if the permission exists; otherwise, false.</returns>
    Task<bool> ExistsAsync(int id);
}