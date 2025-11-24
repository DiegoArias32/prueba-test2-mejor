using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Notifications;
using MediatR;

namespace ElectroHuila.Application.Features.Notifications.Commands.CreateNotification;

/// <summary>
/// Command to create a new notification in the system.
/// </summary>
/// <param name="NotificationDto">Data transfer object containing notification information.</param>
public record CreateNotificationCommand(CreateNotificationDto NotificationDto) : IRequest<Result<NotificationDto>>;
