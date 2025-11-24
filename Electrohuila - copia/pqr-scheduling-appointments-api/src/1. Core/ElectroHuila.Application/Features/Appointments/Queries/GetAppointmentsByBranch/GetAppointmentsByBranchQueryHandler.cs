using AutoMapper;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Appointments;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Appointments.Queries.GetAppointmentsByBranch;

public class GetAppointmentsByBranchQueryHandler : IRequestHandler<GetAppointmentsByBranchQuery, Result<IEnumerable<AppointmentDto>>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IMapper _mapper;

    public GetAppointmentsByBranchQueryHandler(IAppointmentRepository appointmentRepository, IMapper mapper)
    {
        _appointmentRepository = appointmentRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<AppointmentDto>>> Handle(GetAppointmentsByBranchQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var appointments = await _appointmentRepository.GetByBranchIdAsync(request.BranchId);
            var appointmentDtos = _mapper.Map<IEnumerable<AppointmentDto>>(appointments);

            return Result.Success(appointmentDtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<IEnumerable<AppointmentDto>>($"Error retrieving appointments for branch: {ex.Message}");
        }
    }
}
