using ElectroHuila.Domain.Entities.Notifications;

namespace ElectroHuila.Application.Contracts.Repositories;

/// <summary>
/// Repository interface for managing notifications.
/// Provides methods to create, retrieve, update, and query notifications.
/// </summary>
public interface INotificationRepository
{
    /// <summary>
    /// Creates a new notification in the database.
    /// </summary>
    /// <param name="notification">Notification entity to create.</param>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    /// <returns>Created notification with its assigned ID.</returns>
    Task<Notification> CreateAsync(Notification notification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves notifications for a specific user with pagination.
    /// Returns notifications ordered by creation date (newest first).
    /// </summary>
    /// <param name="userId">ID of the user whose notifications to retrieve.</param>
    /// <param name="pageNumber">Page number (1-based).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    /// <returns>Collection of notifications for the specified user and page.</returns>
    Task<IEnumerable<Notification>> GetUserNotificationsAsync(
        int userId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the count of unread notifications for a specific user.
    /// Only counts IN_APP notifications that have not been read.
    /// </summary>
    /// <param name="userId">ID of the user whose unread count to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    /// <returns>Number of unread notifications.</returns>
    Task<int> GetUnreadCountAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a notification by its unique identifier.
    /// Includes related User and Appointment entities.
    /// </summary>
    /// <param name="id">ID of the notification to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    /// <returns>Notification entity if found, null otherwise.</returns>
    Task<Notification?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing notification in the database.
    /// </summary>
    /// <param name="notification">Notification entity with updated data.</param>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    /// <returns>Task representing the asynchronous operation.</returns>
    Task UpdateAsync(Notification notification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all notifications for a specific appointment.
    /// Useful for tracking notification history related to an appointment.
    /// </summary>
    /// <param name="appointmentId">ID of the appointment.</param>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    /// <returns>Collection of notifications associated with the appointment.</returns>
    Task<IEnumerable<Notification>> GetByAppointmentIdAsync(int appointmentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves notifications by status (PENDING, SENT, FAILED).
    /// Useful for retry mechanisms and monitoring.
    /// </summary>
    /// <param name="status">Status to filter by.</param>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    /// <returns>Collection of notifications with the specified status.</returns>
    Task<IEnumerable<Notification>> GetByStatusAsync(string status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves notifications for a user (admin) based on their assigned appointment types.
    /// Includes:
    /// - IN_APP notifications directed to the user (UserId = userId)
    /// - EMAIL/WHATSAPP notifications for clients with appointments in the user's assigned types
    /// </summary>
    /// <param name="userId">ID of the user (admin) requesting notifications.</param>
    /// <param name="pageNumber">Page number (1-based).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    /// <returns>Paginated collection of notifications relevant to the user.</returns>
    Task<(IEnumerable<Notification> Notifications, int TotalCount)> GetNotificationsForUserByAssignedTypesAsync(
        int userId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
}
