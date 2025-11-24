using ElectroHuila.Domain.Entities.Appointments;

namespace ElectroHuila.Application.Contracts.Repositories;

/// <summary>
/// Repository interface for managing available time slots for appointments.
/// Provides methods for CRUD operations and queries on available time entities.
/// </summary>
public interface IAvailableTimeRepository
{
    /// <summary>
    /// Retrieves an available time by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the available time.</param>
    /// <returns>The available time if found; otherwise, null.</returns>
    Task<AvailableTime?> GetByIdAsync(int id);

    /// <summary>
    /// Retrieves all available times from the system.
    /// </summary>
    /// <returns>A collection of all available times.</returns>
    Task<IEnumerable<AvailableTime>> GetAllAsync();

    /// <summary>
    /// Retrieves all available times including inactive ones from the system.
    /// </summary>
    /// <returns>A collection of all available times (active and inactive).</returns>
    Task<IEnumerable<AvailableTime>> GetAllIncludingInactiveAsync();

    /// <summary>
    /// Retrieves all available times for a specific branch.
    /// </summary>
    /// <param name="branchId">The unique identifier of the branch.</param>
    /// <returns>A collection of available times for the specified branch.</returns>
    Task<IEnumerable<AvailableTime>> GetByBranchIdAsync(int branchId);

    /// <summary>
    /// Retrieves all available times for a specific appointment type.
    /// </summary>
    /// <param name="appointmentTypeId">The unique identifier of the appointment type.</param>
    /// <returns>A collection of available times for the specified appointment type.</returns>
    Task<IEnumerable<AvailableTime>> GetByAppointmentTypeIdAsync(int appointmentTypeId);

    /// <summary>
    /// Retrieves available times filtered by branch and optionally by appointment type.
    /// </summary>
    /// <param name="branchId">The unique identifier of the branch.</param>
    /// <param name="appointmentTypeId">Optional unique identifier of the appointment type.</param>
    /// <returns>A collection of available times matching the criteria.</returns>
    Task<IEnumerable<AvailableTime>> GetAvailableTimesAsync(int branchId, int? appointmentTypeId = null);

    /// <summary>
    /// Adds a new available time to the system.
    /// </summary>
    /// <param name="availableTime">The available time to add.</param>
    /// <returns>The added available time with generated values.</returns>
    Task<AvailableTime> AddAsync(AvailableTime availableTime);

    /// <summary>
    /// Updates an existing available time in the system.
    /// </summary>
    /// <param name="availableTime">The available time with updated values.</param>
    Task UpdateAsync(AvailableTime availableTime);

    /// <summary>
    /// Deletes an available time by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the available time to delete.</param>
    Task DeleteAsync(int id);

    /// <summary>
    /// Checks if an available time exists by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier to check.</param>
    /// <returns>True if the available time exists; otherwise, false.</returns>
    Task<bool> ExistsAsync(int id);

    /// <summary>
    /// Checks if a specific time slot is available for booking.
    /// </summary>
    /// <param name="branchId">The unique identifier of the branch.</param>
    /// <param name="time">The time slot to check.</param>
    /// <param name="appointmentTypeId">Optional unique identifier of the appointment type.</param>
    /// <returns>True if the time slot is available; otherwise, false.</returns>
    Task<bool> IsTimeSlotAvailableAsync(int branchId, string time, int? appointmentTypeId = null);
}