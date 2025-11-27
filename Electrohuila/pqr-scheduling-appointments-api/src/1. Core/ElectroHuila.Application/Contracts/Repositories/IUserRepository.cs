using ElectroHuila.Domain.Entities.Security;

namespace ElectroHuila.Application.Contracts.Repositories;

/// <summary>
/// Repository interface for managing users in the system.
/// Provides methods for CRUD operations, queries, and permission retrieval for user entities.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <returns>The user if found; otherwise, null.</returns>
    Task<User?> GetByIdAsync(int id);

    /// <summary>
    /// Retrieves a user by their username.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <returns>The user if found; otherwise, null.</returns>
    Task<User?> GetByUsernameAsync(string username);

    /// <summary>
    /// Retrieves a user by their email address.
    /// </summary>
    /// <param name="email">The email address of the user.</param>
    /// <returns>The user if found; otherwise, null.</returns>
    Task<User?> GetByEmailAsync(string email);

    /// <summary>
    /// Retrieves all users from the system.
    /// </summary>
    /// <returns>A collection of all users.</returns>
    Task<IEnumerable<User>> GetAllAsync();

    /// <summary>
    /// Retrieves all users including inactive ones from the system.
    /// </summary>
    /// <returns>A collection of all users (active and inactive).</returns>
    Task<IEnumerable<User>> GetAllIncludingInactiveAsync();

    /// <summary>
    /// Retrieves all active users from the system.
    /// </summary>
    /// <returns>A collection of active users.</returns>
    Task<IEnumerable<User>> GetActiveAsync();

    /// <summary>
    /// Retrieves all users assigned to a specific role.
    /// </summary>
    /// <param name="roleId">The unique identifier of the role.</param>
    /// <returns>A collection of users assigned to the specified role.</returns>
    Task<IEnumerable<User>> GetByRoleIdAsync(int roleId);

    /// <summary>
    /// Adds a new user to the system.
    /// </summary>
    /// <param name="user">The user to add.</param>
    /// <returns>The added user with generated values.</returns>
    Task<User> AddAsync(User user);

    /// <summary>
    /// Updates an existing user in the system.
    /// </summary>
    /// <param name="user">The user with updated values.</param>
    Task UpdateAsync(User user);

    /// <summary>
    /// Deletes a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user to delete.</param>
    Task DeleteAsync(int id);

    /// <summary>
    /// Checks if a user exists by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier to check.</param>
    /// <returns>True if the user exists; otherwise, false.</returns>
    Task<bool> ExistsAsync(int id);

    /// <summary>
    /// Checks if a user exists by their username.
    /// </summary>
    /// <param name="username">The username to check.</param>
    /// <returns>True if a user with the given username exists; otherwise, false.</returns>
    Task<bool> ExistsByUsernameAsync(string username);

    /// <summary>
    /// Checks if a user exists by their email address.
    /// </summary>
    /// <param name="email">The email address to check.</param>
    /// <returns>True if a user with the given email exists; otherwise, false.</returns>
    Task<bool> ExistsByEmailAsync(string email);

    /// <summary>
    /// Retrieves all permissions assigned to a specific user.
    /// This includes permissions from all roles assigned to the user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>An object containing the user's permissions data.</returns>
    Task<object> GetUserPermissionsAsync(int userId);
}