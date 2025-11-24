using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Appointments.Commands.CancelAppointment;

public class CancelAppointmentCommandHandler : IRequestHandler<CancelAppointmentCommand, Result>
{
    private readonly IAppointmentRepository _appointmentRepository;

    public CancelAppointmentCommandHandler(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<Result> Handle(CancelAppointmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var appointment = await _appointmentRepository.GetByIdAsync(request.Id);
            if (appointment == null)
            {
                return Result.Failure("Appointment not found");
            }

            // StatusIds: 4=COMPLETED, 5=CANCELLED
            const int COMPLETED_STATUS_ID = 4;
            const int CANCELLED_STATUS_ID = 5;

            if (appointment.StatusId == CANCELLED_STATUS_ID)
            {
                return Result.Failure("Appointment is already cancelled");
            }

            if (appointment.StatusId == COMPLETED_STATUS_ID)
            {
                return Result.Failure("Cannot cancel a completed appointment");
            }

            appointment.StatusId = CANCELLED_STATUS_ID;
            appointment.CancellationReason = request.CancellationReason;
            appointment.UpdatedAt = DateTime.UtcNow;

            await _appointmentRepository.UpdateAsync(appointment);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error cancelling appointment: {ex.Message}");
        }
    }
}