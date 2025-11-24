using AutoMapper;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Appointments;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Appointments.Queries.GetPendingAppointments;

public class GetPendingAppointmentsQueryHandler : IRequestHandler<GetPendingAppointmentsQuery, Result<IEnumerable<AppointmentDto>>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IMapper _mapper;

    public GetPendingAppointmentsQueryHandler(IAppointmentRepository appointmentRepository, IMapper mapper)
    {
        _appointmentRepository = appointmentRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<AppointmentDto>>> Handle(GetPendingAppointmentsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // AppointmentStatusIds: 1=PENDING, 2=CONFIRMED
            const int PENDING_STATUS_ID = 1;
            const int CONFIRMED_STATUS_ID = 2;

            var appointments = await _appointmentRepository.GetAllAsync();
            var pendingAppointments = appointments.Where(a => a.StatusId == PENDING_STATUS_ID ||
                                                              a.StatusId == CONFIRMED_STATUS_ID);
            var appointmentDtos = _mapper.Map<IEnumerable<AppointmentDto>>(pendingAppointments);

            return Result.Success(appointmentDtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<IEnumerable<AppointmentDto>>($"Error retrieving pending appointments: {ex.Message}");
        }
    }
}
