using ElectroHuila.Domain.Entities.Security;

namespace ElectroHuila.Application.Contracts.Repositories;

/// <summary>
/// Repository interface for managing roles in the security system.
/// Provides methods for CRUD operations and queries on role entities used for user authorization.
/// </summary>
public interface IRolRepository
{
    /// <summary>
    /// Retrieves a role by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the role.</param>
    /// <returns>The role if found; otherwise, null.</returns>
    Task<Rol?> GetByIdAsync(int id);

    /// <summary>
    /// Retrieves a role by its unique code.
    /// </summary>
    /// <param name="code">The unique code of the role.</param>
    /// <returns>The role if found; otherwise, null.</returns>
    Task<Rol?> GetByCodeAsync(string code);

    /// <summary>
    /// Retrieves a role by its name.
    /// </summary>
    /// <param name="name">The name of the role.</param>
    /// <returns>The role if found; otherwise, null.</returns>
    Task<Rol?> GetByNameAsync(string name);

    /// <summary>
    /// Retrieves all roles from the system.
    /// </summary>
    /// <returns>A collection of all roles.</returns>
    Task<IEnumerable<Rol>> GetAllAsync();

    /// <summary>
    /// Retrieves all roles including inactive ones from the system.
    /// </summary>
    /// <returns>A collection of all roles (active and inactive).</returns>
    Task<IEnumerable<Rol>> GetAllIncludingInactiveAsync();

    /// <summary>
    /// Retrieves all active roles from the system.
    /// </summary>
    /// <returns>A collection of active roles.</returns>
    Task<IEnumerable<Rol>> GetActiveAsync();

    /// <summary>
    /// Retrieves all roles assigned to a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>A collection of roles assigned to the specified user.</returns>
    Task<IEnumerable<Rol>> GetByUserIdAsync(int userId);

    /// <summary>
    /// Adds a new role to the system.
    /// </summary>
    /// <param name="role">The role to add.</param>
    /// <returns>The added role with generated values.</returns>
    Task<Rol> AddAsync(Rol role);

    /// <summary>
    /// Updates an existing role in the system.
    /// </summary>
    /// <param name="role">The role with updated values.</param>
    Task UpdateAsync(Rol role);

    /// <summary>
    /// Deletes a role by its unique identifier (hard delete).
    /// </summary>
    /// <param name="id">The unique identifier of the role to delete.</param>
    Task DeleteAsync(int id);

    /// <summary>
    /// Checks if a role exists by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier to check.</param>
    /// <returns>True if the role exists; otherwise, false.</returns>
    Task<bool> ExistsAsync(int id);

    /// <summary>
    /// Checks if a role exists by its unique code.
    /// </summary>
    /// <param name="code">The code to check.</param>
    /// <returns>True if a role with the given code exists; otherwise, false.</returns>
    Task<bool> ExistsByCodeAsync(string code);

    /// <summary>
    /// Checks if a role exists by its name.
    /// </summary>
    /// <param name="name">The name to check.</param>
    /// <returns>True if a role with the given name exists; otherwise, false.</returns>
    Task<bool> ExistsByNameAsync(string name);

    /// <summary>
    /// Performs a logical delete of a role by its unique identifier.
    /// The role is marked as deleted but not physically removed from the database.
    /// </summary>
    /// <param name="id">The unique identifier of the role to logically delete.</param>
    Task DeleteLogicalAsync(int id);
}