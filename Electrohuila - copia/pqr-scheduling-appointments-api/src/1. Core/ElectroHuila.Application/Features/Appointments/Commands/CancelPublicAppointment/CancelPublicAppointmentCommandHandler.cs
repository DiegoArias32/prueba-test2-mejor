using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Appointments.Commands.CancelPublicAppointment;

public class CancelPublicAppointmentCommandHandler : IRequestHandler<CancelPublicAppointmentCommand, Result<bool>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IClientRepository _clientRepository;

    public CancelPublicAppointmentCommandHandler(
        IAppointmentRepository appointmentRepository,
        IClientRepository clientRepository)
    {
        _appointmentRepository = appointmentRepository;
        _clientRepository = clientRepository;
    }

    public async Task<Result<bool>> Handle(CancelPublicAppointmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.ClientNumber))
                return Result.Failure<bool>("El número de cliente es requerido");

            if (string.IsNullOrWhiteSpace(request.Reason))
                return Result.Failure<bool>("El motivo de cancelación es requerido");

            var client = await _clientRepository.GetByClientNumberAsync(request.ClientNumber);
            if (client == null)
                return Result.Failure<bool>("Cliente no encontrado");

            var appointment = await _appointmentRepository.GetByIdAsync(request.AppointmentId);
            if (appointment == null || appointment.ClientId != client.Id)
                return Result.Failure<bool>("Cita no encontrada");

            // StatusId: 5=CANCELLED
            const int CANCELLED_STATUS_ID = 5;

            // Update appointment status
            appointment.StatusId = CANCELLED_STATUS_ID;
            appointment.CancellationReason = request.Reason;
            appointment.UpdatedAt = DateTime.UtcNow;

            await _appointmentRepository.UpdateAsync(appointment);

            return Result.Success(true);
        }
        catch (Exception ex)
        {
            return Result.Failure<bool>($"Error al cancelar cita: {ex.Message}");
        }
    }
}
