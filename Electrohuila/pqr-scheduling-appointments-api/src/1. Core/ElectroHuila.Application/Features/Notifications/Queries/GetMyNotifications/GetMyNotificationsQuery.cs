using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Notifications;
using MediatR;

namespace ElectroHuila.Application.Features.Notifications.Queries.GetMyNotifications;

/// <summary>
/// Query para obtener las notificaciones del usuario autenticado.
/// Incluye:
/// - Notificaciones IN_APP directas para el usuario (UserId)
/// - Notificaciones EMAIL/WHATSAPP de clientes con citas en los tipos asignados al usuario
/// </summary>
/// <param name="PageNumber">N�mero de p�gina (default: 1).</param>
/// <param name="PageSize">Tama�o de p�gina (default: 20).</param>
public record GetMyNotificationsQuery(
    int PageNumber = 1,
    int PageSize = 20
) : IRequest<Result<PagedList<NotificationListDto>>>;
