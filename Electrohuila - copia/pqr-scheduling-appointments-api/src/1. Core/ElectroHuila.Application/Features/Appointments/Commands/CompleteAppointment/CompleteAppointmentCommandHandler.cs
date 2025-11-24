using AutoMapper;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Appointments;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Appointments.Commands.CompleteAppointment;

public class CompleteAppointmentCommandHandler : IRequestHandler<CompleteAppointmentCommand, Result<AppointmentDto>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IMapper _mapper;

    public CompleteAppointmentCommandHandler(IAppointmentRepository appointmentRepository, IMapper mapper)
    {
        _appointmentRepository = appointmentRepository;
        _mapper = mapper;
    }

    public async Task<Result<AppointmentDto>> Handle(CompleteAppointmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var appointment = await _appointmentRepository.GetByIdAsync(request.AppointmentId);

            if (appointment == null)
            {
                return Result.Failure<AppointmentDto>("Appointment not found");
            }

            // StatusIds: 4=COMPLETED, 5=CANCELLED
            const int COMPLETED_STATUS_ID = 4;
            const int CANCELLED_STATUS_ID = 5;

            if (appointment.StatusId == COMPLETED_STATUS_ID)
            {
                return Result.Failure<AppointmentDto>("Appointment is already completed");
            }

            if (appointment.StatusId == CANCELLED_STATUS_ID)
            {
                return Result.Failure<AppointmentDto>("Cannot complete a cancelled appointment");
            }

            appointment.StatusId = COMPLETED_STATUS_ID;
            appointment.CompletedDate = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(request.Notes))
            {
                appointment.Notes = request.Notes;
            }

            await _appointmentRepository.UpdateAsync(appointment);

            var appointmentDto = _mapper.Map<AppointmentDto>(appointment);
            return Result.Success(appointmentDto);
        }
        catch (Exception ex)
        {
            return Result.Failure<AppointmentDto>($"Error completing appointment: {ex.Message}");
        }
    }
}
