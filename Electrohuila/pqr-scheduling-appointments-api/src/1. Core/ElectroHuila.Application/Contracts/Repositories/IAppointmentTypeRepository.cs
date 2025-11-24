using ElectroHuila.Domain.Entities.Appointments;

namespace ElectroHuila.Application.Contracts.Repositories;

/// <summary>
/// Repository interface for managing appointment types in the system.
/// Provides methods for CRUD operations and queries on appointment type entities.
/// </summary>
public interface IAppointmentTypeRepository
{
    /// <summary>
    /// Retrieves an appointment type by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the appointment type.</param>
    /// <returns>The appointment type if found; otherwise, null.</returns>
    Task<AppointmentType?> GetByIdAsync(int id);

    /// <summary>
    /// Retrieves an appointment type by its name.
    /// </summary>
    /// <param name="name">The name of the appointment type.</param>
    /// <returns>The appointment type if found; otherwise, null.</returns>
    Task<AppointmentType?> GetByNameAsync(string name);

    /// <summary>
    /// Retrieves all appointment types from the system.
    /// </summary>
    /// <returns>A collection of all appointment types.</returns>
    Task<IEnumerable<AppointmentType>> GetAllAsync();

    /// <summary>
    /// Retrieves all appointment types including inactive ones from the system.
    /// </summary>
    /// <returns>A collection of all appointment types (active and inactive).</returns>
    Task<IEnumerable<AppointmentType>> GetAllIncludingInactiveAsync();

    /// <summary>
    /// Retrieves all active appointment types from the system.
    /// </summary>
    /// <returns>A collection of active appointment types.</returns>
    Task<IEnumerable<AppointmentType>> GetActiveAsync();

    /// <summary>
    /// Adds a new appointment type to the system.
    /// </summary>
    /// <param name="appointmentType">The appointment type to add.</param>
    /// <returns>The added appointment type with generated values.</returns>
    Task<AppointmentType> AddAsync(AppointmentType appointmentType);

    /// <summary>
    /// Updates an existing appointment type in the system.
    /// </summary>
    /// <param name="appointmentType">The appointment type with updated values.</param>
    Task UpdateAsync(AppointmentType appointmentType);

    /// <summary>
    /// Deletes an appointment type by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the appointment type to delete.</param>
    Task DeleteAsync(int id);

    /// <summary>
    /// Checks if an appointment type exists by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier to check.</param>
    /// <returns>True if the appointment type exists; otherwise, false.</returns>
    Task<bool> ExistsAsync(int id);

    /// <summary>
    /// Checks if an appointment type exists by its name.
    /// </summary>
    /// <param name="name">The name to check.</param>
    /// <returns>True if an appointment type with the given name exists; otherwise, false.</returns>
    Task<bool> ExistsByNameAsync(string name);
}