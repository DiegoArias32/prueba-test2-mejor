using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Appointments.Queries.ValidateAvailability;

public class ValidateAvailabilityQueryHandler : IRequestHandler<ValidateAvailabilityQuery, Result<bool>>
{
    private readonly IAppointmentRepository _appointmentRepository;

    public ValidateAvailabilityQueryHandler(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<Result<bool>> Handle(ValidateAvailabilityQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var appointments = await _appointmentRepository.GetByBranchIdAsync(request.BranchId);

            // AppointmentStatusIds: 4=COMPLETED, 5=CANCELLED
            const int COMPLETED_STATUS_ID = 4;
            const int CANCELLED_STATUS_ID = 5;

            var timeString = request.Time.ToString(@"hh\:mm");
            var hasConflict = appointments.Any(a =>
                a.AppointmentDate.Date == request.Date.Date &&
                a.AppointmentTime == timeString &&
                a.StatusId != CANCELLED_STATUS_ID &&
                a.StatusId != COMPLETED_STATUS_ID);

            var isAvailable = !hasConflict;
            return Result.Success(isAvailable);
        }
        catch (Exception ex)
        {
            return Result.Failure<bool>($"Error validating availability: {ex.Message}");
        }
    }
}
