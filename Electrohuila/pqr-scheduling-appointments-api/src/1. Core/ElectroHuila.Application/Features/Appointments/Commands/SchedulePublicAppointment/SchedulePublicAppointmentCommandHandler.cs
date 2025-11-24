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
    private readonly IMapper _mapper;
    private readonly ISignalRNotificationService _signalRService;
    private readonly INotificationService _notificationService;

    public SchedulePublicAppointmentCommandHandler(
        IAppointmentRepository appointmentRepository,
        IClientRepository clientRepository,
        IMapper mapper,
        ISignalRNotificationService signalRService,
        INotificationService notificationService)
    {
        _appointmentRepository = appointmentRepository;
        _clientRepository = clientRepository;
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

            // Send real-time notification to admin panel via SignalR
            await _signalRService.BroadcastNotificationAsync(new
            {
                type = "appointment_created",
                data = new
                {
                    id = createdAppointment.Id,
                    appointmentNumber = createdAppointment.AppointmentNumber,
                    clientId = createdAppointment.ClientId,
                    appointmentDate = createdAppointment.AppointmentDate,
                    timestamp = DateTime.UtcNow
                }
            }, cancellationToken);

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
