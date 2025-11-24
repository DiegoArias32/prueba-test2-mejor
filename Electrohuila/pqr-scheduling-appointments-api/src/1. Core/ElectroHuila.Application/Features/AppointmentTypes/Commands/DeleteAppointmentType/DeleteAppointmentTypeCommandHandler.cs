using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using MediatR;

namespace ElectroHuila.Application.Features.AppointmentTypes.Commands.DeleteAppointmentType;

public class DeleteAppointmentTypeCommandHandler : IRequestHandler<DeleteAppointmentTypeCommand, Result<bool>>
{
    private readonly IAppointmentTypeRepository _appointmentTypeRepository;

    public DeleteAppointmentTypeCommandHandler(IAppointmentTypeRepository appointmentTypeRepository)
    {
        _appointmentTypeRepository = appointmentTypeRepository;
    }

    public async Task<Result<bool>> Handle(DeleteAppointmentTypeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var appointmentType = await _appointmentTypeRepository.GetByIdAsync(request.Id);
            if (appointmentType == null)
            {
                return Result.Failure<bool>($"Appointment type with ID {request.Id} not found");
            }

            // Soft delete
            appointmentType.IsActive = false;
            await _appointmentTypeRepository.UpdateAsync(appointmentType);

            return Result.Success(true);
        }
        catch (Exception ex)
        {
            return Result.Failure<bool>($"Error deleting appointment type: {ex.Message}");
        }
    }
}