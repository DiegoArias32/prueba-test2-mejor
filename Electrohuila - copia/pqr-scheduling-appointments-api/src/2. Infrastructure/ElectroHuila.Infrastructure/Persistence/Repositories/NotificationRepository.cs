using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Domain.Entities.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ElectroHuila.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository for managing notifications in the database.
/// Provides methods to create, retrieve, update, and query notifications.
/// All queries include related User and Appointment entities when applicable.
/// </summary>
public class NotificationRepository : INotificationRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<NotificationRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the NotificationRepository.
    /// </summary>
    /// <param name="context">Database context.</param>
    /// <param name="logger">Logger for diagnostic information.</param>
    public NotificationRepository(ApplicationDbContext context, ILogger<NotificationRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new notification in the database.
    /// </summary>
    /// <param name="notification">Notification entity to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created notification with its assigned ID.</returns>
    public async Task<Notification> CreateAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Notification created - ID: {Id}, Type: {Type}, UserId: {UserId}, ClientId: {ClientId}",
                notification.Id,
                notification.Type,
                notification.UserId,
                notification.ClientId
            );

            return notification;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating notification for user {UserId} or client {ClientId}",
                notification.UserId, notification.ClientId);
            throw;
        }
    }

    /// <summary>
    /// Retrieves notifications for a specific user with pagination.
    /// Returns notifications ordered by creation date (newest first).
    /// Includes User and Appointment navigation properties.
    /// </summary>
    /// <param name="userId">ID of the user.</param>
    /// <param name="pageNumber">Page number (1-based).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of notifications for the specified page.</returns>
    public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(
        int userId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var skip = (pageNumber - 1) * pageSize;

            var notifications = await _context.Notifications
                .AsNoTracking()
                .Include(n => n.User)
                .Include(n => n.Appointment)
                .Where(n => n.UserId == userId && n.IsActive)
                .OrderByDescending(n => n.CreatedAt)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            _logger.LogInformation(
                "Retrieved {Count} notifications for user {UserId} - Page {Page}",
                notifications.Count,
                userId,
                pageNumber
            );

            return notifications;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving notifications for user {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Gets the count of unread notifications for a specific user.
    /// Only counts IN_APP notifications that have not been read.
    /// </summary>
    /// <param name="userId">ID of the user.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Number of unread notifications.</returns>
    public async Task<int> GetUnreadCountAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var count = await _context.Notifications
                .Where(n => n.UserId == userId
                         && n.IsActive
                         && !n.IsRead
                         && n.Type == "IN_APP")
                .CountAsync(cancellationToken);

            _logger.LogDebug("User {UserId} has {Count} unread notifications", userId, count);

            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting unread count for user {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Retrieves a notification by its unique identifier.
    /// Includes related User and Appointment entities.
    /// </summary>
    /// <param name="id">ID of the notification.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Notification entity if found and active, null otherwise.</returns>
    public async Task<Notification?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var notification = await _context.Notifications
                .AsNoTracking()
                .Include(n => n.User)
                .Include(n => n.Appointment)
                .FirstOrDefaultAsync(n => n.Id == id && n.IsActive, cancellationToken);

            if (notification == null)
            {
                _logger.LogWarning("Notification with ID {Id} not found or inactive", id);
            }

            return notification;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving notification with ID {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing notification in the database.
    /// </summary>
    /// <param name="notification">Notification entity with updated data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task UpdateAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Notification updated - ID: {Id}, Status: {Status}, IsRead: {IsRead}",
                notification.Id,
                notification.Status,
                notification.IsRead
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating notification with ID {Id}", notification.Id);
            throw;
        }
    }

    /// <summary>
    /// Retrieves all notifications for a specific appointment.
    /// Includes related User entity.
    /// </summary>
    /// <param name="appointmentId">ID of the appointment.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of notifications associated with the appointment.</returns>
    public async Task<IEnumerable<Notification>> GetByAppointmentIdAsync(
        int appointmentId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var notifications = await _context.Notifications
                .AsNoTracking()
                .Include(n => n.User)
                .Include(n => n.Appointment)
                .Where(n => n.AppointmentId == appointmentId && n.IsActive)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync(cancellationToken);

            _logger.LogInformation(
                "Retrieved {Count} notifications for appointment {AppointmentId}",
                notifications.Count,
                appointmentId
            );

            return notifications;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving notifications for appointment {AppointmentId}", appointmentId);
            throw;
        }
    }

    /// <summary>
    /// Retrieves notifications by status (PENDING, SENT, FAILED).
    /// Includes related User and Appointment entities.
    /// Useful for retry mechanisms and monitoring.
    /// </summary>
    /// <param name="status">Status to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of notifications with the specified status.</returns>
    public async Task<IEnumerable<Notification>> GetByStatusAsync(
        string status,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var notifications = await _context.Notifications
                .AsNoTracking()
                .Include(n => n.User)
                .Include(n => n.Appointment)
                .Where(n => n.Status == status && n.IsActive)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync(cancellationToken);

            _logger.LogInformation(
                "Retrieved {Count} notifications with status {Status}",
                notifications.Count,
                status
            );

            return notifications;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving notifications with status {Status}", status);
            throw;
        }
    }

    /// <summary>
    /// Retrieves notifications for a user (admin) based on their assigned appointment types.
    /// Includes both IN_APP notifications for the user and EMAIL/WHATSAPP notifications for clients
    /// with appointments in the user's assigned appointment types.
    /// </summary>
    /// <param name="userId">ID of the user (admin).</param>
    /// <param name="pageNumber">Page number (1-based).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Tuple with paginated notifications and total count.</returns>
    public async Task<(IEnumerable<Notification> Notifications, int TotalCount)> GetNotificationsForUserByAssignedTypesAsync(
        int userId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var skip = (pageNumber - 1) * pageSize;

            // Subconsulta: Obtener los tipos de cita asignados al usuario
            var assignedAppointmentTypeIds = _context.UserAppointmentTypeAssignments
                .Where(ua => ua.UserId == userId && ua.IsActive)
                .Select(ua => ua.AppointmentTypeId)
                .Distinct();

            // Query principal: Obtener notificaciones relevantes para el usuario
            var query = _context.Notifications
                .AsNoTracking()
                .Include(n => n.User)
                .Include(n => n.Client)
                .Include(n => n.Appointment)
                    .ThenInclude(a => a!.Client) // Incluir el cliente de la cita
                .Where(n => n.IsActive && (
                    // Notificaciones IN_APP directas para el usuario (admin)
                    (n.UserId == userId) ||
                    // Notificaciones EMAIL/WHATSAPP para clientes con citas en tipos asignados
                    (n.ClientId != null && n.Appointment != null &&
                     assignedAppointmentTypeIds.Contains(n.Appointment.AppointmentTypeId))
                ));

            // Obtener total count
            var totalCount = await query.CountAsync(cancellationToken);

            // Obtener datos paginados ordenados por fecha descendente (más recientes primero)
            var notifications = await query
                .OrderByDescending(n => n.CreatedAt)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            _logger.LogInformation(
                "Retrieved {Count} of {Total} notifications for user {UserId} with assigned types - Page {Page}",
                notifications.Count,
                totalCount,
                userId,
                pageNumber
            );

            return (notifications, totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving notifications for user {UserId} by assigned types", userId);
            throw;
        }
    }
}
