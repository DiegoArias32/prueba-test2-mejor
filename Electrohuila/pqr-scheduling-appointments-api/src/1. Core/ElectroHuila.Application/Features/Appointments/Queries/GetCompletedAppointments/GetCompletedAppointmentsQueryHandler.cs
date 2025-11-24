using AutoMapper;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Appointments;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Appointments.Queries.GetCompletedAppointments;

public class GetCompletedAppointmentsQueryHandler : IRequestHandler<GetCompletedAppointmentsQuery, Result<IEnumerable<AppointmentDto>>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IMapper _mapper;

    public GetCompletedAppointmentsQueryHandler(IAppointmentRepository appointmentRepository, IMapper mapper)
    {
        _appointmentRepository = appointmentRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<AppointmentDto>>> Handle(GetCompletedAppointmentsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // AppointmentStatusIds: 4=COMPLETED
            const int COMPLETED_STATUS_ID = 4;

            var appointments = await _appointmentRepository.GetAllAsync();
            var completedAppointments = appointments.Where(a => a.StatusId == COMPLETED_STATUS_ID);
            var appointmentDtos = _mapper.Map<IEnumerable<AppointmentDto>>(completedAppointments);

            return Result.Success(appointmentDtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<IEnumerable<AppointmentDto>>($"Error retrieving completed appointments: {ex.Message}");
        }
    }
}
