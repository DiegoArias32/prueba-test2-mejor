using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Notifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ElectroHuila.Application.Features.Notifications.Queries.GetUserNotifications;

/// <summary>
/// Handler for retrieving user notifications with pagination.
/// Returns notifications ordered by creation date (newest first).
/// </summary>
public class GetUserNotificationsQueryHandler : IRequestHandler<GetUserNotificationsQuery, Result<IEnumerable<NotificationListDto>>>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetUserNotificationsQueryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the GetUserNotificationsQueryHandler.
    /// </summary>
    /// <param name="notificationRepository">Repository for notification operations.</param>
    /// <param name="mapper">AutoMapper instance for object mapping.</param>
    /// <param name="logger">Logger for diagnostic information.</param>
    public GetUserNotificationsQueryHandler(
        INotificationRepository notificationRepository,
        IMapper mapper,
        ILogger<GetUserNotificationsQueryHandler> logger)
    {
        _notificationRepository = notificationRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Handles the retrieval of user notifications.
    /// </summary>
    /// <param name="request">Query containing user ID and pagination parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result containing the list of notifications or error information.</returns>
    public async Task<Result<IEnumerable<NotificationListDto>>> Handle(GetUserNotificationsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Retrieving notifications for user {UserId} - Page: {PageNumber}, PageSize: {PageSize}",
                request.UserId,
                request.PageNumber,
                request.PageSize
            );

            // Validate pagination parameters
            if (request.PageNumber < 1)
            {
                return Result.Failure<IEnumerable<NotificationListDto>>("El número de página debe ser mayor o igual a 1.");
            }

            if (request.PageSize < 1 || request.PageSize > 100)
            {
                return Result.Failure<IEnumerable<NotificationListDto>>("El tamaño de página debe estar entre 1 y 100.");
            }

            // Retrieve notifications from repository
            var notifications = await _notificationRepository.GetUserNotificationsAsync(
                request.UserId,
                request.PageNumber,
                request.PageSize,
                cancellationToken
            );

            var notificationList = notifications.ToList();

            _logger.LogInformation(
                "Retrieved {Count} notifications for user {UserId}",
                notificationList.Count,
                request.UserId
            );

            // Map to DTOs
            var notificationDtos = _mapper.Map<IEnumerable<NotificationListDto>>(notificationList);

            return Result.Success(notificationDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving notifications for user {UserId}", request.UserId);
            return Result.Failure<IEnumerable<NotificationListDto>>($"Error al obtener las notificaciones: {ex.Message}");
        }
    }
}
