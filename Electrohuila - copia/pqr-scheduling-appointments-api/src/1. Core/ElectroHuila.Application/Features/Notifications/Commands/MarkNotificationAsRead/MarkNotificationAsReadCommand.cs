using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Notifications.Commands.MarkNotificationAsRead;

/// <summary>
/// Command to mark a notification as read by the user.
/// Only applicable for IN_APP notifications.
/// </summary>
/// <param name="NotificationId">Unique identifier of the notification to mark as read.</param>
/// <param name="UserId">Identifier of the user marking the notification as read.</param>
public record MarkNotificationAsReadCommand(int NotificationId, int UserId) : IRequest<Result>;
