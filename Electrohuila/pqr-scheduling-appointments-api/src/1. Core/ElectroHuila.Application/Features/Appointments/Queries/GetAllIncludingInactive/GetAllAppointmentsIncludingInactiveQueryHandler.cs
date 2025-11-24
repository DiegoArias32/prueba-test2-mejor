using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Appointments;
using MediatR;

namespace ElectroHuila.Application.Features.Appointments.Queries.GetAllIncludingInactive;

/// <summary>
/// Handler para obtener todas las citas incluyendo las inactivas.
/// </summary>
public class GetAllAppointmentsIncludingInactiveQueryHandler
    : IRequestHandler<GetAllAppointmentsIncludingInactiveQuery, Result<List<AppointmentDto>>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IMapper _mapper;

    public GetAllAppointmentsIncludingInactiveQueryHandler(
        IAppointmentRepository appointmentRepository,
        IMapper mapper)
    {
        _appointmentRepository = appointmentRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<AppointmentDto>>> Handle(
        GetAllAppointmentsIncludingInactiveQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var appointments = await _appointmentRepository.GetAllIncludingInactiveAsync();
            var appointmentDtos = _mapper.Map<List<AppointmentDto>>(appointments);

            return Result.Success(appointmentDtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<List<AppointmentDto>>($"Error retrieving appointments including inactive: {ex.Message}");
        }
    }
}
