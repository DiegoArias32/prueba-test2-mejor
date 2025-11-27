using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Appointments.Queries.GetPublicAvailableTimes;

public class GetPublicAvailableTimesQueryHandler : IRequestHandler<GetPublicAvailableTimesQuery, Result<IEnumerable<string>>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IAvailableTimeRepository _availableTimeRepository;
    private readonly IHolidayRepository _holidayRepository;

    public GetPublicAvailableTimesQueryHandler(
        IAppointmentRepository appointmentRepository,
        IAvailableTimeRepository availableTimeRepository,
        IHolidayRepository holidayRepository)
    {
        _appointmentRepository = appointmentRepository;
        _availableTimeRepository = availableTimeRepository;
        _holidayRepository = holidayRepository;
    }

    public async Task<Result<IEnumerable<string>>> Handle(GetPublicAvailableTimesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.BranchId <= 0)
                return Result.Failure<IEnumerable<string>>("ID de sede requerido");

            // Validate date - Check if Sunday
            if (request.Date.DayOfWeek == DayOfWeek.Sunday)
            {
                return Result.Failure<IEnumerable<string>>("SUNDAY_NOT_AVAILABLE|Los domingos no se atienden citas");
            }

            // Validate date - Check if holiday
            var holiday = await _holidayRepository.GetByDateAsync(
                request.Date,
                request.BranchId,
                cancellationToken);

            if (holiday != null)
            {
                var holidayName = holiday.HolidayName ?? "Día festivo";
                return Result.Failure<IEnumerable<string>>($"HOLIDAY_NOT_AVAILABLE|No se puede agendar porque es {holidayName}");
            }

            // Validate date - Check if past date
            if (request.Date.Date < DateTime.UtcNow.Date)
            {
                return Result.Failure<IEnumerable<string>>("PAST_DATE_NOT_AVAILABLE|No se pueden agendar citas en fechas pasadas");
            }

            // Get configured times for the branch
            var configuredTimes = await _availableTimeRepository.GetByBranchIdAsync(request.BranchId);

            if (!configuredTimes.Any())
                return Result.Success(Enumerable.Empty<string>());

            // AppointmentStatusIds: 4=COMPLETED, 5=CANCELLED
            const int COMPLETED_STATUS_ID = 4;
            const int CANCELLED_STATUS_ID = 5;

            // Get all appointments for the date and branch
            var allAppointments = await _appointmentRepository.GetAllAsync();
            var occupiedAppointments = allAppointments
                .Where(a => a.AppointmentDate.Date == request.Date.Date
                           && a.BranchId == request.BranchId
                           && a.IsEnabled
                           && a.StatusId != CANCELLED_STATUS_ID
                           && a.StatusId != COMPLETED_STATUS_ID)
                .ToList();

            // Get occupied time slots
            var occupiedTimes = occupiedAppointments
                .Select(a => a.AppointmentTime)
                .Where(t => !string.IsNullOrEmpty(t))
                .ToHashSet();

            // Filter available times
            var availableTimes = configuredTimes
                .Where(ct => ct.IsActive && !occupiedTimes.Contains(ct.Time))
                .Select(ct => ct.Time)
                .OrderBy(t => t)
                .ToList();

            return Result.Success<IEnumerable<string>>(availableTimes);
        }
        catch (Exception ex)
        {
            return Result.Failure<IEnumerable<string>>($"Error al obtener horas disponibles: {ex.Message}");
        }
    }
}
