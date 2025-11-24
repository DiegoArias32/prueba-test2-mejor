using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ElectroHuila.Application.Features.Notifications.Commands.MarkNotificationAsRead;

/// <summary>
/// Handler for marking a notification as read.
/// Validates that the notification belongs to the user before updating.
/// </summary>
public class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand, Result>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ILogger<MarkNotificationAsReadCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the MarkNotificationAsReadCommandHandler.
    /// </summary>
    /// <param name="notificationRepository">Repository for notification operations.</param>
    /// <param name="logger">Logger for diagnostic information.</param>
    public MarkNotificationAsReadCommandHandler(
        INotificationRepository notificationRepository,
        ILogger<MarkNotificationAsReadCommandHandler> logger)
    {
        _notificationRepository = notificationRepository;
        _logger = logger;
    }

    /// <summary>
    /// Handles the marking of a notification as read.
    /// </summary>
    /// <param name="request">Command containing notification and user identifiers.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result indicating success or failure of the operation.</returns>
    public async Task<Result> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Retrieve the notification
            var notification = await _notificationRepository.GetByIdAsync(request.NotificationId, cancellationToken);

            if (notification == null)
            {
                _logger.LogWarning("Notification with ID {NotificationId} not found", request.NotificationId);
                return Result.Failure($"La notificación con ID {request.NotificationId} no existe.");
            }

            // Verify that the notification belongs to the requesting user
            if (notification.UserId != request.UserId)
            {
                _logger.LogWarning(
                    "User {UserId} attempted to mark notification {NotificationId} as read, but it belongs to user {OwnerId}",
                    request.UserId,
                    request.NotificationId,
                    notification.UserId
                );
                return Result.Failure("No tiene permisos para marcar esta notificación como leída.");
            }

            // Check if already read
            if (notification.IsRead)
            {
                _logger.LogInformation("Notification {NotificationId} already marked as read", request.NotificationId);
                return Result.Success();
            }

            // Mark as read
            notification.MarkAsRead();

            // Update in database
            await _notificationRepository.UpdateAsync(notification, cancellationToken);

            _logger.LogInformation(
                "Notification {NotificationId} marked as read by user {UserId}",
                request.NotificationId,
                request.UserId
            );

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error marking notification {NotificationId} as read for user {UserId}",
                request.NotificationId,
                request.UserId
            );
            return Result.Failure($"Error al marcar la notificación como leída: {ex.Message}");
        }
    }
}
