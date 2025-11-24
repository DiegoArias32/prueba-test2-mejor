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
    private readonly IMapper _mapper;

    public ScheduleAppointmentCommandHandler(IAppointmentRepository appointmentRepository, IMapper mapper)
    {
        _appointmentRepository = appointmentRepository;
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

            // Check availability first
            var appointments = await _appointmentRepository.GetByBranchIdAsync(request.AppointmentDto.BranchId);
            var hasConflict = appointments.Any(a =>
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
