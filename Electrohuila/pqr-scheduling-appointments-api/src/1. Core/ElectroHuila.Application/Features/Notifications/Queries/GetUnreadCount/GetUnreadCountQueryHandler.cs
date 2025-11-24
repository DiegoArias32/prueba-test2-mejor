using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ElectroHuila.Application.Features.Notifications.Queries.GetUnreadCount;

/// <summary>
/// Handler for retrieving the count of unread notifications for a user.
/// Returns the total number of unread IN_APP notifications.
/// </summary>
public class GetUnreadCountQueryHandler : IRequestHandler<GetUnreadCountQuery, Result<int>>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ILogger<GetUnreadCountQueryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the GetUnreadCountQueryHandler.
    /// </summary>
    /// <param name="notificationRepository">Repository for notification operations.</param>
    /// <param name="logger">Logger for diagnostic information.</param>
    public GetUnreadCountQueryHandler(
        INotificationRepository notificationRepository,
        ILogger<GetUnreadCountQueryHandler> logger)
    {
        _notificationRepository = notificationRepository;
        _logger = logger;
    }

    /// <summary>
    /// Handles the retrieval of unread notification count.
    /// </summary>
    /// <param name="request">Query containing user identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result containing the unread count or error information.</returns>
    public async Task<Result<int>> Handle(GetUnreadCountQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Retrieving unread notification count for user {UserId}", request.UserId);

            // Get unread count from repository
            var unreadCount = await _notificationRepository.GetUnreadCountAsync(request.UserId, cancellationToken);

            _logger.LogInformation(
                "User {UserId} has {Count} unread notifications",
                request.UserId,
                unreadCount
            );

            return Result.Success(unreadCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving unread count for user {UserId}", request.UserId);
            return Result.Failure<int>($"Error al obtener el contador de notificaciones no leídas: {ex.Message}");
        }
    }
}
