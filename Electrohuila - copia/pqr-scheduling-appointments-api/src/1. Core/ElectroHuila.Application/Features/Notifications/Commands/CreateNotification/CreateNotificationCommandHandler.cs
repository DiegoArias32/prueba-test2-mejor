using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Notifications;
using ElectroHuila.Domain.Entities.Notifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ElectroHuila.Application.Features.Notifications.Commands.CreateNotification;

/// <summary>
/// Handler for creating a new notification.
/// Validates the input data and creates the notification in the database.
/// </summary>
public class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommand, Result<NotificationDto>>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateNotificationCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the CreateNotificationCommandHandler.
    /// </summary>
    /// <param name="notificationRepository">Repository for notification operations.</param>
    /// <param name="userRepository">Repository for user operations.</param>
    /// <param name="mapper">AutoMapper instance for object mapping.</param>
    /// <param name="logger">Logger for diagnostic information.</param>
    public CreateNotificationCommandHandler(
        INotificationRepository notificationRepository,
        IUserRepository userRepository,
        IMapper mapper,
        ILogger<CreateNotificationCommandHandler> logger)
    {
        _notificationRepository = notificationRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Handles the creation of a new notification.
    /// </summary>
    /// <param name="request">Command containing notification data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result containing the created notification or error information.</returns>
    public async Task<Result<NotificationDto>> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var dto = request.NotificationDto;

            // Validate that the user exists
            var userExists = await _userRepository.ExistsAsync(dto.UserId);
            if (!userExists)
            {
                _logger.LogWarning("Attempted to create notification for non-existent user {UserId}", dto.UserId);
                return Result.Failure<NotificationDto>($"El usuario con ID {dto.UserId} no existe.");
            }

            // Create notification entity using factory method
            var notification = Notification.Create(
                userId: dto.UserId,
                type: dto.Type,
                title: dto.Title,
                message: dto.Message,
                appointmentId: dto.AppointmentId,
                metadata: dto.Metadata
            );

            // Save to database
            var createdNotification = await _notificationRepository.CreateAsync(notification, cancellationToken);

            // Load related data for complete DTO
            var notificationWithDetails = await _notificationRepository.GetByIdAsync(createdNotification.Id, cancellationToken);

            if (notificationWithDetails == null)
            {
                _logger.LogError("Failed to retrieve created notification with ID {Id}", createdNotification.Id);
                return Result.Failure<NotificationDto>("Error al recuperar la notificación creada.");
            }

            var notificationDto = _mapper.Map<NotificationDto>(notificationWithDetails);

            _logger.LogInformation(
                "Notification created successfully - ID: {Id}, Type: {Type}, UserId: {UserId}",
                notificationDto.Id,
                notificationDto.Type,
                notificationDto.UserId
            );

            return Result.Success(notificationDto);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation error creating notification");
            return Result.Failure<NotificationDto>($"Error de validación: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating notification for user {UserId}", request.NotificationDto.UserId);
            return Result.Failure<NotificationDto>($"Error al crear la notificación: {ex.Message}");
        }
    }
}
