using AutoMapper;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Appointments;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Appointments.Commands.UpdateAppointment;

public class UpdateAppointmentCommandHandler : IRequestHandler<UpdateAppointmentCommand, Result<AppointmentDto>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IHolidayRepository _holidayRepository;
    private readonly IMapper _mapper;

    public UpdateAppointmentCommandHandler(
        IAppointmentRepository appointmentRepository,
        IHolidayRepository holidayRepository,
        IMapper mapper)
    {
        _appointmentRepository = appointmentRepository;
        _holidayRepository = holidayRepository;
        _mapper = mapper;
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

            // Update appointment properties
            appointment.AppointmentDate = request.AppointmentDto.AppointmentDate;
            appointment.AppointmentTime = request.AppointmentDto.AppointmentTime;
            appointment.StatusId = request.AppointmentDto.StatusId;
            appointment.Notes = request.AppointmentDto.Notes;
            appointment.BranchId = request.AppointmentDto.BranchId;
            appointment.AppointmentTypeId = request.AppointmentDto.AppointmentTypeId;
            appointment.UpdatedAt = DateTime.UtcNow;

            await _appointmentRepository.UpdateAsync(appointment);

            var appointmentDto = _mapper.Map<AppointmentDto>(appointment);
            return Result.Success(appointmentDto);
        }
        catch (Exception ex)
        {
            return Result.Failure<AppointmentDto>($"Error updating appointment: {ex.Message}");
        }
    }
}