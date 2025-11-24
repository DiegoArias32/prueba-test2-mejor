using AutoMapper;
using ElectroHuila.Application.Common.Interfaces;
using ElectroHuila.Application.Common.Interfaces.Services.Common;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Notifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ElectroHuila.Application.Features.Notifications.Queries.GetMyNotifications;

/// <summary>
/// Handler para obtener notificaciones del usuario autenticado basadas en sus tipos de cita asignados.
/// </summary>
public class GetMyNotificationsQueryHandler : IRequestHandler<GetMyNotificationsQuery, Result<PagedList<NotificationListDto>>>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    private readonly ILogger<GetMyNotificationsQueryHandler> _logger;

    public GetMyNotificationsQueryHandler(
        INotificationRepository notificationRepository,
        ICurrentUserService currentUserService,
        IMapper mapper,
        ILogger<GetMyNotificationsQueryHandler> logger)
    {
        _notificationRepository = notificationRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<PagedList<NotificationListDto>>> Handle(GetMyNotificationsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Obtener el usuario ID del contexto del usuario autenticado
            var currentUserId = _currentUserService.UserId;
            if (!currentUserId.HasValue)
            {
                _logger.LogWarning("No authenticated user found when requesting notifications");
                return Result.Failure<PagedList<NotificationListDto>>("Usuario no autenticado.");
            }

            _logger.LogInformation(
                "Retrieving notifications for user {UserId} - Page: {PageNumber}, PageSize: {PageSize}",
                currentUserId.Value,
                request.PageNumber,
                request.PageSize
            );

            // Validar parametros de paginacion
            if (request.PageNumber < 1)
            {
                return Result.Failure<PagedList<NotificationListDto>>("El numero de pagina debe ser mayor o igual a 1.");
            }

            if (request.PageSize < 1 || request.PageSize > 100)
            {
                return Result.Failure<PagedList<NotificationListDto>>("El tamano de pagina debe estar entre 1 y 100.");
            }

            // Obtener notificaciones del repositorio
            var (notifications, totalCount) = await _notificationRepository.GetNotificationsForUserByAssignedTypesAsync(
                currentUserId.Value,
                request.PageNumber,
                request.PageSize,
                cancellationToken
            );

            var notificationList = notifications.ToList();

            _logger.LogInformation(
                "Retrieved {Count} of {Total} notifications for user {UserId}",
                notificationList.Count,
                totalCount,
                currentUserId.Value
            );

            // Mapear a DTOs
            var notificationDtos = _mapper.Map<List<NotificationListDto>>(notificationList);

            // Crear PagedList
            var pagedResult = new PagedList<NotificationListDto>(
                notificationDtos,
                totalCount,
                request.PageNumber,
                request.PageSize
            );

            return Result.Success(pagedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving notifications for current user");
            return Result.Failure<PagedList<NotificationListDto>>($"Error al obtener las notificaciones: {ex.Message}");
        }
    }
}
