using AutoMapper;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.Contracts.Services;
using ElectroHuila.Application.DTOs.Appointments;
using ElectroHuila.Application.Common.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ElectroHuila.Application.Features.Appointments.Commands.UpdateAppointment;

public class UpdateAppointmentCommandHandler : IRequestHandler<UpdateAppointmentCommand, Result<AppointmentDto>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IHolidayRepository _holidayRepository;
    private readonly INotificationService _notificationService;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateAppointmentCommandHandler> _logger;

    public UpdateAppointmentCommandHandler(
        IAppointmentRepository appointmentRepository,
        IHolidayRepository holidayRepository,
        INotificationService notificationService,
        IMapper mapper,
        ILogger<UpdateAppointmentCommandHandler> logger)
    {
        _appointmentRepository = appointmentRepository;
        _holidayRepository = holidayRepository;
        _notificationService = notificationService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<AppointmentDto>> Handle(UpdateAppointmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var appointment = await _appointmentRepository.GetByIdAsync(request.Id);
            if (appointment == null)
            {
                return Result.Failure<AppointmentDto>("Appointment not found");
            }

            // VALIDACIÓN CRÍTICA: Si se está actualizando la fecha, verificar que no sea festivo
            if (request.AppointmentDto.AppointmentDate != appointment.AppointmentDate)
            {
                var newDate = request.AppointmentDto.AppointmentDate;

                // Verificar si es domingo
                if (newDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    return Result.Failure<AppointmentDto>("No se pueden agendar citas los domingos");
                }

                // Verificar si es festivo
                var branchId = request.AppointmentDto.BranchId != 0 ? request.AppointmentDto.BranchId : appointment.BranchId;
                var isHoliday = await _holidayRepository.IsHolidayAsync(newDate, branchId, cancellationToken);

                if (isHoliday)
                {
                    var holiday = await _holidayRepository.GetByDateAsync(newDate, branchId, cancellationToken);
                    return Result.Failure<AppointmentDto>(
                        $"No se pueden agendar citas en días festivos. {holiday?.HolidayName ?? "Día festivo"}");
                }

                // Verificar que la fecha no esté en el pasado
                if (newDate.Date < DateTime.UtcNow.Date)
                {
                    return Result.Failure<AppointmentDto>("No se pueden agendar citas en fechas pasadas");
                }
            }

            // StatusIds: 1=PENDING, 2=CONFIRMED, 3=NO_SHOW, 4=COMPLETED, 5=CANCELLED
            const int NO_SHOW_STATUS_ID = 3;
            const int COMPLETED_STATUS_ID = 4;
            const int CANCELLED_STATUS_ID = 5;

            // Capturar el estado anterior para detectar cambios
            var previousStatusId = appointment.StatusId;

            // Update appointment properties
            appointment.AppointmentDate = request.AppointmentDto.AppointmentDate;
            appointment.AppointmentTime = request.AppointmentDto.AppointmentTime;
            appointment.StatusId = request.AppointmentDto.StatusId;
            appointment.Notes = request.AppointmentDto.Notes;
            appointment.BranchId = request.AppointmentDto.BranchId;
            appointment.AppointmentTypeId = request.AppointmentDto.AppointmentTypeId;
            appointment.UpdatedAt = DateTime.UtcNow;

            await _appointmentRepository.UpdateAsync(appointment);

            // Enviar notificación si el estado cambió a NO_SHOW, COMPLETED o CANCELLED
            if (previousStatusId != request.AppointmentDto.StatusId)
            {
                if (request.AppointmentDto.StatusId == COMPLETED_STATUS_ID)
                {
                    // Cita completada - Cliente asistió y fue atendido
                    try
                    {
                        _logger.LogInformation(
                            "Enviando notificación de cita completada para cita {AppointmentId}",
                            request.Id);

                        await _notificationService.SendAppointmentCompletedAsync(
                            request.Id,
                            cancellationToken);

                        _logger.LogInformation(
                            "Notificación de cita completada enviada exitosamente para cita {AppointmentId}",
                            request.Id);
                    }
                    catch (Exception notificationEx)
                    {
                        _logger.LogError(notificationEx,
                            "Error al enviar notificación de cita completada para cita {AppointmentId}",
                            request.Id);
                    }
                }
                else if (request.AppointmentDto.StatusId == NO_SHOW_STATUS_ID)
                {
                    // Cliente no asistió a la cita
                    try
                    {
                        _logger.LogInformation(
                            "Enviando notificación de no asistencia para cita {AppointmentId}",
                            request.Id);

                        await _notificationService.SendAppointmentCancellationAsync(
                            request.Id,
                            "No asistió a la cita programada",
                            cancellationToken);

                        _logger.LogInformation(
                            "Notificación de no asistencia enviada exitosamente para cita {AppointmentId}",
                            request.Id);
                    }
                    catch (Exception notificationEx)
                    {
                        _logger.LogError(notificationEx,
                            "Error al enviar notificación de no asistencia para cita {AppointmentId}",
                            request.Id);
                    }
                }
                else if (request.AppointmentDto.StatusId == CANCELLED_STATUS_ID)
                {
                    // Cita cancelada (desde panel administrativo)
                    try
                    {
                        _logger.LogInformation(
                            "Enviando notificación de cancelación para cita {AppointmentId}",
                            request.Id);

                        await _notificationService.SendAppointmentCancellationAsync(
                            request.Id,
                            request.AppointmentDto.Notes ?? "Cita cancelada por el administrador",
                            cancellationToken);

                        _logger.LogInformation(
                            "Notificación de cancelación enviada exitosamente para cita {AppointmentId}",
                            request.Id);
                    }
                    catch (Exception notificationEx)
                    {
                        _logger.LogError(notificationEx,
                            "Error al enviar notificación de cancelación para cita {AppointmentId}",
                            request.Id);
                    }
                }
            }

            var appointmentDto = _mapper.Map<AppointmentDto>(appointment);
            return Result.Success(appointmentDto);
        }
        catch (Exception ex)
        {
            return Result.Failure<AppointmentDto>($"Error updating appointment: {ex.Message}");
        }
    }
}