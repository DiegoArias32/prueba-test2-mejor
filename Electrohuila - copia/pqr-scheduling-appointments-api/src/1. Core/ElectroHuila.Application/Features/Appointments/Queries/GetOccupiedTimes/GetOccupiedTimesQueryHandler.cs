using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Appointments.Queries.GetOccupiedTimes;

public class GetOccupiedTimesQueryHandler : IRequestHandler<GetOccupiedTimesQuery, Result<object>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IAvailableTimeRepository _availableTimeRepository;

    public GetOccupiedTimesQueryHandler(
        IAppointmentRepository appointmentRepository,
        IAvailableTimeRepository availableTimeRepository)
    {
        _appointmentRepository = appointmentRepository;
        _availableTimeRepository = availableTimeRepository;
    }

    public async Task<Result<object>> Handle(GetOccupiedTimesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Get all appointments for the date
            var allAppointments = await _appointmentRepository.GetAllAsync();
            var branchAppointments = allAppointments
                .Where(a => a.AppointmentDate.Date == request.Date.Date && a.BranchId == request.BranchId)
                .ToList();

            // AppointmentStatusIds: 4=COMPLETED, 5=CANCELLED
            const int COMPLETED_STATUS_ID = 4;
            const int CANCELLED_STATUS_ID = 5;

            // Get configured times for the branch
            var configuredTimes = await _availableTimeRepository.GetByBranchIdAsync(request.BranchId);

            // Get occupied times (excluding Cancelada and Completada)
            var occupiedTimes = branchAppointments
                .Where(a => a.IsEnabled && a.StatusId != CANCELLED_STATUS_ID && a.StatusId != COMPLETED_STATUS_ID)
                .Select(a => a.AppointmentTime)
                .Where(t => !string.IsNullOrEmpty(t))
                .ToHashSet();

            // Calculate available times
            var availableTimes = configuredTimes
                .Where(ct => ct.IsActive && !occupiedTimes.Contains(ct.Time))
                .Select(ct => ct.Time)
                .OrderBy(t => t)
                .ToList();

            var debug = new
            {
                date = request.Date.ToString("yyyy-MM-dd"),
                branchId = request.BranchId,
                allAppointmentsOfDate = branchAppointments.Select(a => new
                {
                    id = a.Id,
                    appointmentNumber = a.AppointmentNumber,
                    time = a.AppointmentTime,
                    statusId = a.StatusId,
                    statusCode = a.Status?.Code,
                    statusName = a.Status?.Name,
                    isEnabled = a.IsEnabled
                }),
                calculatedAvailableTimes = availableTimes,
                appliedLogic = "Excluye: Cancelada + Completada"
            };

            return Result.Success<object>(debug);
        }
        catch (Exception ex)
        {
            return Result.Failure<object>($"Error al obtener horas ocupadas: {ex.Message}");
        }
    }
}
