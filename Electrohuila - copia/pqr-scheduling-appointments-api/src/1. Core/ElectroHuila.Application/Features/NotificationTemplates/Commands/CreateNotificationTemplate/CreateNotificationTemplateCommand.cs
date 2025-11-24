using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Notifications;
using MediatR;

namespace ElectroHuila.Application.Features.NotificationTemplates.Commands.CreateNotificationTemplate;

/// <summary>
/// Comando para crear una plantilla de notificación
/// </summary>
public record CreateNotificationTemplateCommand(CreateNotificationTemplateDto Dto) : IRequest<Result<NotificationTemplateDto>>;
