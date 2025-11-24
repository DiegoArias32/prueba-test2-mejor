using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Appointments.Queries.GetAvailableTimes;

public class GetAvailableTimesQueryHandler : IRequestHandler<GetAvailableTimesQuery, Result<IEnumerable<string>>>
{
    private readonly IAppointmentRepository _appointmentRepository;

    // Define business hours and time slots
    private static readonly string[] AllTimeSlots = new[]
    {
        "08:00", "08:30", "09:00", "09:30", "10:00", "10:30", "11:00", "11:30",
        "12:00", "12:30", "13:00", "13:30", "14:00", "14:30", "15:00", "15:30",
        "16:00", "16:30", "17:00", "17:30"
    };

    public GetAvailableTimesQueryHandler(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<Result<IEnumerable<string>>> Handle(GetAvailableTimesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var appointments = await _appointmentRepository.GetByBranchIdAsync(request.BranchId);

            // AppointmentStatusIds: 4=COMPLETED, 5=CANCELLED
            const int COMPLETED_STATUS_ID = 4;
            const int CANCELLED_STATUS_ID = 5;

            var bookedTimes = appointments
                .Where(a => a.AppointmentDate.Date == request.Date.Date &&
                           a.StatusId != CANCELLED_STATUS_ID &&
                           a.StatusId != COMPLETED_STATUS_ID)
                .Select(a => a.AppointmentTime)
                .Where(t => !string.IsNullOrEmpty(t))
                .ToHashSet();

            var availableTimes = AllTimeSlots.Where(time => !bookedTimes.Contains(time)).ToList();

            return Result.Success<IEnumerable<string>>(availableTimes);
        }
        catch (Exception ex)
        {
            return Result.Failure<IEnumerable<string>>($"Error retrieving available times: {ex.Message}");
        }
    }
}
