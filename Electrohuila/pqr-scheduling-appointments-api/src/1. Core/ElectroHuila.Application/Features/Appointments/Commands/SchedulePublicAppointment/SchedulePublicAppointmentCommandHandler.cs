using AutoMapper;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Appointments;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Domain.Entities.Appointments;
using MediatR;
using ElectroHuila.Application.Contracts.Notifications;
using ElectroHuila.Application.Contracts.Services;

namespace ElectroHuila.Application.Features.Appointments.Commands.SchedulePublicAppointment;

public class SchedulePublicAppointmentCommandHandler : IRequestHandler<SchedulePublicAppointmentCommand, Result<AppointmentDto>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IHolidayRepository _holidayRepository;
    private readonly IUserAssignmentRepository _userAssignmentRepository;
    private readonly IMapper _mapper;
    private readonly ISignalRNotificationService _signalRService;
    private readonly INotificationService _notificationService;

    public SchedulePublicAppointmentCommandHandler(
        IAppointmentRepository appointmentRepository,
        IClientRepository clientRepository,
        IHolidayRepository holidayRepository,
        IUserAssignmentRepository userAssignmentRepository,
        IMapper mapper,
        ISignalRNotificationService signalRService,
        INotificationService notificationService)
    {
        _appointmentRepository = appointmentRepository;
        _clientRepository = clientRepository;
        _holidayRepository = holidayRepository;
        _userAssignmentRepository = userAssignmentRepository;
        _mapper = mapper;
        _signalRService = signalRService;
        _notificationService = notificationService;
    }

    public async Task<Result<AppointmentDto>> Handle(SchedulePublicAppointmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate client exists
            var client = await _clientRepository.GetByClientNumberAsync(request.AppointmentDto.ClientNumber);
            if (client == null)
                return Result.Failure<AppointmentDto>("Cliente no encontrado");

            // VALIDACIÓN CRÍTICA: Verificar fecha y hora de la cita
            var appointmentDate = request.AppointmentDto.AppointmentDate;

            // Verificar si es domingo
            if (appointmentDate.DayOfWeek == DayOfWeek.Sunday)
            {
                return Result.Failure<AppointmentDto>("No se pueden agendar citas los domingos");
            }

            // Verificar si es festivo
            var isHoliday = await _holidayRepository.IsHolidayAsync(
                appointmentDate,
                request.AppointmentDto.BranchId,
                cancellationToken);

            if (isHoliday)
            {
                var holiday = await _holidayRepository.GetByDateAsync(
                    appointmentDate,
                    request.AppointmentDto.BranchId,
                    cancellationToken);

                return Result.Failure<AppointmentDto>(
                    $"No se pueden agendar citas en días festivos. {holiday?.HolidayName ?? "Día festivo"}");
            }

            // Verificar que la fecha no esté en el pasado
            if (appointmentDate.Date < DateTime.UtcNow.Date)
            {
                return Result.Failure<AppointmentDto>("No se pueden agendar citas en fechas pasadas");
            }

            // StatusId: 1=PENDING
            const int PENDING_STATUS_ID = 1;

            // Create appointment
            var appointment = new Appointment
            {
                AppointmentNumber = GenerateAppointmentNumber(),
                ClientId = client.Id,
                BranchId = request.AppointmentDto.BranchId,
                AppointmentTypeId = request.AppointmentDto.AppointmentTypeId,
                AppointmentDate = request.AppointmentDto.AppointmentDate,
                AppointmentTime = request.AppointmentDto.AppointmentTime.ToString(@"hh\:mm"),
                StatusId = PENDING_STATUS_ID,
                Notes = request.AppointmentDto.Observations,
                CreatedAt = DateTime.UtcNow,
                IsEnabled = true
            };

            var createdAppointment = await _appointmentRepository.AddAsync(appointment);
            var appointmentDto = _mapper.Map<AppointmentDto>(createdAppointment);

            // Send real-time notification ONLY to users assigned to this appointment type via SignalR
            var assignedUsers = await _userAssignmentRepository.GetByAppointmentTypeIdAsync(createdAppointment.AppointmentTypeId);
            var notificationData = new
            {
                type = "appointment_created",
                data = new
                {
                    id = createdAppointment.Id,
                    appointmentNumber = createdAppointment.AppointmentNumber,
                    clientId = createdAppointment.ClientId,
                    appointmentDate = createdAppointment.AppointmentDate,
                    appointmentTypeId = createdAppointment.AppointmentTypeId,
                    timestamp = DateTime.UtcNow
                }
            };

            foreach (var assignment in assignedUsers.Where(a => a.IsActive))
            {
                await _signalRService.SendNotificationToUserAsync(
                    assignment.UserId.ToString(),
                    notificationData,
                    cancellationToken);
            }

            // Send appointment confirmation notifications (Email, WhatsApp, IN_APP)
            _ = Task.Run(async () =>
            {
                try
                {
                    await _notificationService.SendAppointmentConfirmationAsync(
                        createdAppointment.Id,
                        CancellationToken.None);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error sending confirmation notifications: {ex.Message}");
                }
            });

            return Result.Success(appointmentDto);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<AppointmentDto>(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure<AppointmentDto>(ex.Message);
        }
        catch (Exception ex)
        {
            return Result.Failure<AppointmentDto>($"Error al agendar cita pública: {ex.Message}");
        }
    }

    private static string GenerateAppointmentNumber()
    {
        return $"APT-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid():N}".Substring(0, 20);
    }
}
