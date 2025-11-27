using AutoMapper;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Appointments;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Domain.Entities.Appointments;
using MediatR;
using ElectroHuila.Application.Contracts.Notifications;
using ElectroHuila.Application.Contracts.Services;

namespace ElectroHuila.Application.Features.Appointments.Commands.CreateAppointment;

public class CreateAppointmentCommandHandler : IRequestHandler<CreateAppointmentCommand, Result<AppointmentDto>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IHolidayRepository _holidayRepository;
    private readonly IBranchRepository _branchRepository;
    private readonly IUserAssignmentRepository _userAssignmentRepository;
    private readonly IMapper _mapper;
    private readonly ISignalRNotificationService _signalRService;
    private readonly INotificationService _notificationService;

    public CreateAppointmentCommandHandler(
        IAppointmentRepository appointmentRepository,
        IHolidayRepository holidayRepository,
        IBranchRepository branchRepository,
        IUserAssignmentRepository userAssignmentRepository,
        IMapper mapper,
        ISignalRNotificationService signalRService,
        INotificationService notificationService)
    {
        _appointmentRepository = appointmentRepository;
        _holidayRepository = holidayRepository;
        _branchRepository = branchRepository;
        _userAssignmentRepository = userAssignmentRepository;
        _mapper = mapper;
        _signalRService = signalRService;
        _notificationService = notificationService;
    }

    public async Task<Result<AppointmentDto>> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var appointment = _mapper.Map<Appointment>(request.AppointmentDto);

            // VALIDACIÓN CRÍTICA: Verificar si la fecha es festivo
            var appointmentDate = appointment.AppointmentDate;

            // Verificar si es domingo
            if (appointmentDate.DayOfWeek == DayOfWeek.Sunday)
            {
                return Result.Failure<AppointmentDto>("No se pueden agendar citas los domingos");
            }

            // Verificar si es festivo
            var isHoliday = await _holidayRepository.IsHolidayAsync(
                appointmentDate,
                appointment.BranchId,
                cancellationToken);

            if (isHoliday)
            {
                var holiday = await _holidayRepository.GetByDateAsync(
                    appointmentDate,
                    appointment.BranchId,
                    cancellationToken);

                return Result.Failure<AppointmentDto>(
                    $"No se pueden agendar citas en días festivos. {holiday?.HolidayName ?? "Día festivo"}");
            }

            // Verificar que la fecha no esté en el pasado
            if (appointmentDate.Date < DateTime.UtcNow.Date)
            {
                return Result.Failure<AppointmentDto>("No se pueden agendar citas en fechas pasadas");
            }

            appointment.AppointmentNumber = GenerateAppointmentNumber();

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
            // This runs in background - don't await to avoid blocking the response
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
                    // Log error but don't fail the appointment creation
                    Console.Error.WriteLine($"Error sending confirmation notifications: {ex.Message}");
                }
            });

            return Result.Success(appointmentDto);
        }
        catch (Exception ex)
        {
            return Result.Failure<AppointmentDto>($"Error creating appointment: {ex.Message}");
        }
    }

    private static string GenerateAppointmentNumber()
    {
        return $"APT-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid():N}";
    }
}