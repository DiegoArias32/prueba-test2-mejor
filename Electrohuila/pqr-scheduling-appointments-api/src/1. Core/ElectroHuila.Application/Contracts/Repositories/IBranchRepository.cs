using ElectroHuila.Domain.Entities.Locations;

namespace ElectroHuila.Application.Contracts.Repositories;

/// <summary>
/// Repository interface for managing branch locations in the system.
/// Provides methods for CRUD operations and queries on branch entities.
/// </summary>
public interface IBranchRepository
{
    /// <summary>
    /// Retrieves a branch by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the branch.</param>
    /// <returns>The branch if found; otherwise, null.</returns>
    Task<Branch?> GetByIdAsync(int id);

    /// <summary>
    /// Retrieves a branch by its unique code.
    /// </summary>
    /// <param name="code">The unique code of the branch.</param>
    /// <returns>The branch if found; otherwise, null.</returns>
    Task<Branch?> GetByCodeAsync(string code);

    /// <summary>
    /// Retrieves the main branch of the organization.
    /// </summary>
    /// <returns>The main branch if configured; otherwise, null.</returns>
    Task<Branch?> GetMainBranchAsync();

    /// <summary>
    /// Retrieves all branches from the system.
    /// </summary>
    /// <returns>A collection of all branches.</returns>
    Task<IEnumerable<Branch>> GetAllAsync();

    /// <summary>
    /// Retrieves all branches including inactive ones from the system.
    /// </summary>
    /// <returns>A collection of all branches (active and inactive).</returns>
    Task<IEnumerable<Branch>> GetAllIncludingInactiveAsync();

    /// <summary>
    /// Retrieves all active branches from the system.
    /// </summary>
    /// <returns>A collection of active branches.</returns>
    Task<IEnumerable<Branch>> GetActiveAsync();

    /// <summary>
    /// Adds a new branch to the system.
    /// </summary>
    /// <param name="branch">The branch to add.</param>
    /// <returns>The added branch with generated values.</returns>
    Task<Branch> AddAsync(Branch branch);

    /// <summary>
    /// Updates an existing branch in the system.
    /// </summary>
    /// <param name="branch">The branch with updated values.</param>
    Task UpdateAsync(Branch branch);

    /// <summary>
    /// Deletes a branch by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the branch to delete.</param>
    Task DeleteAsync(int id);

    /// <summary>
    /// Checks if a branch exists by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier to check.</param>
    /// <returns>True if the branch exists; otherwise, false.</returns>
    Task<bool> ExistsAsync(int id);

    /// <summary>
    /// Checks if a branch exists by its unique code.
    /// </summary>
    /// <param name="code">The code to check.</param>
    /// <returns>True if a branch with the given code exists; otherwise, false.</returns>
    Task<bool> ExistsByCodeAsync(string code);
}