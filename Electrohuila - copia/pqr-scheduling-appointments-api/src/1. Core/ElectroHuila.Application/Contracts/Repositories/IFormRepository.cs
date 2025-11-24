using ElectroHuila.Domain.Entities.Security;

namespace ElectroHuila.Application.Contracts.Repositories;

/// <summary>
/// Repository interface for managing forms (UI screens/pages) in the security system.
/// Provides methods for CRUD operations and queries on form entities used for permission control.
/// </summary>
public interface IFormRepository
{
    /// <summary>
    /// Retrieves a form by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the form.</param>
    /// <returns>The form if found; otherwise, null.</returns>
    Task<Form?> GetByIdAsync(int id);

    /// <summary>
    /// Retrieves a form by its unique code.
    /// </summary>
    /// <param name="code">The unique code of the form.</param>
    /// <returns>The form if found; otherwise, null.</returns>
    Task<Form?> GetByCodeAsync(string code);

    /// <summary>
    /// Retrieves a form by its name.
    /// </summary>
    /// <param name="name">The name of the form.</param>
    /// <returns>The form if found; otherwise, null.</returns>
    Task<Form?> GetByNameAsync(string name);

    /// <summary>
    /// Retrieves all forms from the system.
    /// </summary>
    /// <returns>A collection of all forms.</returns>
    Task<IEnumerable<Form>> GetAllAsync();

    /// <summary>
    /// Retrieves all active forms from the system.
    /// </summary>
    /// <returns>A collection of active forms.</returns>
    Task<IEnumerable<Form>> GetActiveAsync();

    /// <summary>
    /// Retrieves all forms belonging to a specific module.
    /// </summary>
    /// <param name="moduleId">The unique identifier of the module.</param>
    /// <returns>A collection of forms in the specified module.</returns>
    Task<IEnumerable<Form>> GetByModuleIdAsync(int moduleId);

    /// <summary>
    /// Adds a new form to the system.
    /// </summary>
    /// <param name="form">The form to add.</param>
    /// <returns>The added form with generated values.</returns>
    Task<Form> AddAsync(Form form);

    /// <summary>
    /// Updates an existing form in the system.
    /// </summary>
    /// <param name="form">The form with updated values.</param>
    Task UpdateAsync(Form form);

    /// <summary>
    /// Deletes a form by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the form to delete.</param>
    Task DeleteAsync(int id);

    /// <summary>
    /// Checks if a form exists by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier to check.</param>
    /// <returns>True if the form exists; otherwise, false.</returns>
    Task<bool> ExistsAsync(int id);

    /// <summary>
    /// Checks if a form exists by its unique code.
    /// </summary>
    /// <param name="code">The code to check.</param>
    /// <returns>True if a form with the given code exists; otherwise, false.</returns>
    Task<bool> ExistsByCodeAsync(string code);

    /// <summary>
    /// Checks if a form exists by its name.
    /// </summary>
    /// <param name="name">The name to check.</param>
    /// <returns>True if a form with the given name exists; otherwise, false.</returns>
    Task<bool> ExistsByNameAsync(string name);
}