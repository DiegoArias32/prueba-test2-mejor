using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Notifications;
using MediatR;

namespace ElectroHuila.Application.Features.Notifications.Queries.GetUserNotifications;

/// <summary>
/// Query to retrieve notifications for a specific user with pagination.
/// </summary>
/// <param name="UserId">Identifier of the user whose notifications to retrieve.</param>
/// <param name="PageNumber">Page number for pagination (default: 1).</param>
/// <param name="PageSize">Number of items per page (default: 20).</param>
public record GetUserNotificationsQuery(
    int UserId,
    int PageNumber = 1,
    int PageSize = 20
) : IRequest<Result<IEnumerable<NotificationListDto>>>;
