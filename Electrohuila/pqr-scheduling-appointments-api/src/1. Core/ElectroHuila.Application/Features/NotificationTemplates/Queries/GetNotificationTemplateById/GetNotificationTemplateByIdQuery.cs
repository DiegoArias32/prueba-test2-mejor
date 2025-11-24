using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Notifications;
using MediatR;

namespace ElectroHuila.Application.Features.NotificationTemplates.Queries.GetNotificationTemplateById;

/// <summary>
/// Query para obtener una plantilla de notificación por ID
/// </summary>
public record GetNotificationTemplateByIdQuery(int Id) : IRequest<Result<NotificationTemplateDto>>;
