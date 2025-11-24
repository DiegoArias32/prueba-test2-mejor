using ElectroHuila.Domain.Entities.Security;

namespace ElectroHuila.Application.Contracts.Repositories;

/// <summary>
/// Repository interface for managing application modules in the security system.
/// Provides methods for CRUD operations and queries on module entities used for organizing forms and permissions.
/// </summary>
public interface IModuleRepository
{
    /// <summary>
    /// Retrieves a module by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the module.</param>
    /// <returns>The module if found; otherwise, null.</returns>
    Task<Module?> GetByIdAsync(int id);

    /// <summary>
    /// Retrieves a module by its unique code.
    /// </summary>
    /// <param name="code">The unique code of the module.</param>
    /// <returns>The module if found; otherwise, null.</returns>
    Task<Module?> GetByCodeAsync(string code);

    /// <summary>
    /// Retrieves a module by its name.
    /// </summary>
    /// <param name="name">The name of the module.</param>
    /// <returns>The module if found; otherwise, null.</returns>
    Task<Module?> GetByNameAsync(string name);

    /// <summary>
    /// Retrieves all modules from the system.
    /// </summary>
    /// <returns>A collection of all modules.</returns>
    Task<IEnumerable<Module>> GetAllAsync();

    /// <summary>
    /// Retrieves all active modules from the system.
    /// </summary>
    /// <returns>A collection of active modules.</returns>
    Task<IEnumerable<Module>> GetActiveAsync();

    /// <summary>
    /// Adds a new module to the system.
    /// </summary>
    /// <param name="module">The module to add.</param>
    /// <returns>The added module with generated values.</returns>
    Task<Module> AddAsync(Module module);

    /// <summary>
    /// Updates an existing module in the system.
    /// </summary>
    /// <param name="module">The module with updated values.</param>
    Task UpdateAsync(Module module);

    /// <summary>
    /// Deletes a module by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the module to delete.</param>
    Task DeleteAsync(int id);

    /// <summary>
    /// Checks if a module exists by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier to check.</param>
    /// <returns>True if the module exists; otherwise, false.</returns>
    Task<bool> ExistsAsync(int id);

    /// <summary>
    /// Checks if a module exists by its unique code.
    /// </summary>
    /// <param name="code">The code to check.</param>
    /// <returns>True if a module with the given code exists; otherwise, false.</returns>
    Task<bool> ExistsByCodeAsync(string code);

    /// <summary>
    /// Checks if a module exists by its name.
    /// </summary>
    /// <param name="name">The name to check.</param>
    /// <returns>True if a module with the given name exists; otherwise, false.</returns>
    Task<bool> ExistsByNameAsync(string name);
}