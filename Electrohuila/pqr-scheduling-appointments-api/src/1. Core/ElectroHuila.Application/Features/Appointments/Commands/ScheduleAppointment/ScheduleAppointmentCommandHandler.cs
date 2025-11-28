using AutoMapper;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Appointments;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Domain.Entities.Appointments;
using MediatR;

namespace ElectroHuila.Application.Features.Appointments.Commands.ScheduleAppointment;

public class ScheduleAppointmentCommandHandler : IRequestHandler<ScheduleAppointmentCommand, Result<AppointmentDto>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly ISystemSettingRepository _systemSettingRepository;
    private readonly IMapper _mapper;

    public ScheduleAppointmentCommandHandler(IAppointmentRepository appointmentRepository, ISystemSettingRepository systemSettingRepository, IMapper mapper)
    {
        _appointmentRepository = appointmentRepository;
        _systemSettingRepository = systemSettingRepository;
        _mapper = mapper;
    }

    public async Task<Result<AppointmentDto>> Handle(ScheduleAppointmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // AppointmentStatusIds: 2=CONFIRMED, 4=COMPLETED, 5=CANCELLED
            const int CONFIRMED_STATUS_ID = 2;
            const int COMPLETED_STATUS_ID = 4;
            const int CANCELLED_STATUS_ID = 5;

            // Get max appointments per day setting
            var maxAppointmentsSetting = await _systemSettingRepository.GetValueAsync("MAX_APPOINTMENTS_PER_DAY", cancellationToken);
            var maxAppointmentsPerDay = 50; // default value
            if (!string.IsNullOrEmpty(maxAppointmentsSetting) && int.TryParse(maxAppointmentsSetting, out var maxValue))
            {
                maxAppointmentsPerDay = maxValue;
            }

            // Check if max appointments per day would be exceeded
            // Optimizado: Usar CountAsync en lugar de traer todas las citas
            var appointmentsOnSameDayCount = await _appointmentRepository.CountAppointmentsByDateAsync(
                request.AppointmentDto.BranchId,
                request.AppointmentDto.AppointmentDate,
                CANCELLED_STATUS_ID,
                cancellationToken);

            if (appointmentsOnSameDayCount >= maxAppointmentsPerDay)
            {
                return Result.Failure<AppointmentDto>($"No se pueden agendar más citas para este día. Máximo permitido: {maxAppointmentsPerDay} citas por día.");
            }

            // Check availability for specific time slot
            // Obtener solo las citas del día para verificar conflicto de horario
            var appointmentsOnDate = await _appointmentRepository.GetByBranchIdAsync(request.AppointmentDto.BranchId);
            var hasConflict = appointmentsOnDate.Any(a =>
                a.AppointmentDate.Date == request.AppointmentDto.AppointmentDate.Date &&
                a.AppointmentTime == request.AppointmentDto.AppointmentTime &&
                a.StatusId != CANCELLED_STATUS_ID &&
                a.StatusId != COMPLETED_STATUS_ID);

            if (hasConflict)
            {
                return Result.Failure<AppointmentDto>("The selected time slot is not available");
            }

            var appointment = _mapper.Map<Appointment>(request.AppointmentDto);
            appointment.AppointmentNumber = GenerateAppointmentNumber();
            appointment.StatusId = CONFIRMED_STATUS_ID;

            var createdAppointment = await _appointmentRepository.AddAsync(appointment);
            var appointmentDto = _mapper.Map<AppointmentDto>(createdAppointment);

            return Result.Success(appointmentDto);
        }
        catch (Exception ex)
        {
            return Result.Failure<AppointmentDto>($"Error scheduling appointment: {ex.Message}");
        }
    }

    private static string GenerateAppointmentNumber()
    {
        return $"APT-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid():N}";
    }
}
