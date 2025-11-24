using AutoMapper;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Appointments;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Appointments.Queries.GetAllAppointments;

public class GetAllAppointmentsQueryHandler : IRequestHandler<GetAllAppointmentsQuery, Result<PagedList<AppointmentDto>>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IMapper _mapper;

    public GetAllAppointmentsQueryHandler(IAppointmentRepository appointmentRepository, IMapper mapper)
    {
        _appointmentRepository = appointmentRepository;
        _mapper = mapper;
    }

    public async Task<Result<PagedList<AppointmentDto>>> Handle(GetAllAppointmentsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var appointments = await _appointmentRepository.GetAllAsync();
            var appointmentDtos = _mapper.Map<IEnumerable<AppointmentDto>>(appointments);

            var pagedAppointments = PagedList<AppointmentDto>.Create(appointmentDtos, request.PageNumber, request.PageSize);

            return Result.Success(pagedAppointments);
        }
        catch (Exception ex)
        {
            return Result.Failure<PagedList<AppointmentDto>>($"Error retrieving appointments: {ex.Message}");
        }
    }
}