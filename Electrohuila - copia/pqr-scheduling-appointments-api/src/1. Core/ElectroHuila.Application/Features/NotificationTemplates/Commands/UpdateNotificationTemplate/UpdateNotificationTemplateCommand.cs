using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Notifications;
using MediatR;

namespace ElectroHuila.Application.Features.NotificationTemplates.Commands.UpdateNotificationTemplate;

/// <summary>
/// Comando para actualizar una plantilla de notificación
/// </summary>
public record UpdateNotificationTemplateCommand(UpdateNotificationTemplateDto Dto) : IRequest<Result<NotificationTemplateDto>>;
