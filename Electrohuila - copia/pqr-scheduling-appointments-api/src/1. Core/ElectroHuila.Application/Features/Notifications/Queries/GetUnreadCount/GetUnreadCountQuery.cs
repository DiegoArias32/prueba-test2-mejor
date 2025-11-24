using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Notifications.Queries.GetUnreadCount;

/// <summary>
/// Query to retrieve the count of unread notifications for a specific user.
/// Only counts IN_APP notifications that have not been read.
/// </summary>
/// <param name="UserId">Identifier of the user whose unread count to retrieve.</param>
public record GetUnreadCountQuery(int UserId) : IRequest<Result<int>>;
