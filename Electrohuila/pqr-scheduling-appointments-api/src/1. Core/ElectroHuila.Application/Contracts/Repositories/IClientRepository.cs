using ElectroHuila.Domain.Entities.Clients;

namespace ElectroHuila.Application.Contracts.Repositories;

/// <summary>
/// Repository interface for managing client information in the system.
/// Provides methods for CRUD operations and queries on client entities.
/// </summary>
public interface IClientRepository
{
    /// <summary>
    /// Retrieves a client by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the client.</param>
    /// <returns>The client if found; otherwise, null.</returns>
    Task<Client?> GetByIdAsync(int id);

    /// <summary>
    /// Retrieves a client by their unique client number.
    /// </summary>
    /// <param name="clientNumber">The unique client number.</param>
    /// <returns>The client if found; otherwise, null.</returns>
    Task<Client?> GetByClientNumberAsync(string clientNumber);

    /// <summary>
    /// Retrieves a client by their document number.
    /// </summary>
    /// <param name="documentNumber">The document number of the client.</param>
    /// <returns>The client if found; otherwise, null.</returns>
    Task<Client?> GetByDocumentNumberAsync(string documentNumber);

    /// <summary>
    /// Retrieves a client by their document number (alternative method).
    /// </summary>
    /// <param name="documentNumber">The document number of the client.</param>
    /// <returns>The client if found; otherwise, null.</returns>
    Task<Client?> GetByDocumentAsync(string documentNumber);

    /// <summary>
    /// Retrieves a client by their email address.
    /// </summary>
    /// <param name="email">The email address of the client.</param>
    /// <returns>The client if found; otherwise, null.</returns>
    Task<Client?> GetByEmailAsync(string email);

    /// <summary>
    /// Retrieves all clients from the system.
    /// </summary>
    /// <returns>A collection of all clients.</returns>
    Task<IEnumerable<Client>> GetAllAsync();

    /// <summary>
    /// Retrieves all clients including inactive ones from the system.
    /// </summary>
    /// <returns>A collection of all clients (active and inactive).</returns>
    Task<IEnumerable<Client>> GetAllIncludingInactiveAsync();

    /// <summary>
    /// Adds a new client to the system.
    /// </summary>
    /// <param name="client">The client to add.</param>
    /// <returns>The added client with generated values.</returns>
    Task<Client> AddAsync(Client client);

    /// <summary>
    /// Updates an existing client in the system.
    /// </summary>
    /// <param name="client">The client with updated values.</param>
    Task UpdateAsync(Client client);

    /// <summary>
    /// Deletes a client by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the client to delete.</param>
    Task DeleteAsync(int id);

    /// <summary>
    /// Checks if a client exists by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier to check.</param>
    /// <returns>True if the client exists; otherwise, false.</returns>
    Task<bool> ExistsAsync(int id);

    /// <summary>
    /// Checks if a client exists by their document number.
    /// </summary>
    /// <param name="documentNumber">The document number to check.</param>
    /// <returns>True if a client with the given document number exists; otherwise, false.</returns>
    Task<bool> ExistsByDocumentNumberAsync(string documentNumber);

    /// <summary>
    /// Checks if a client exists by their email address.
    /// </summary>
    /// <param name="email">The email address to check.</param>
    /// <returns>True if a client with the given email exists; otherwise, false.</returns>
    Task<bool> ExistsByEmailAsync(string email);

    /// <summary>
    /// Gets the client ID by their client number without loading related entities.
    /// </summary>
    /// <param name="clientNumber">The unique client number.</param>
    /// <returns>The client ID if found; otherwise, null.</returns>
    Task<int?> GetClientIdByNumberAsync(string clientNumber);
}