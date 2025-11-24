using AutoMapper;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Appointments;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Appointments.Queries.GetAppointmentsByStatus;

public class GetAppointmentsByStatusQueryHandler : IRequestHandler<GetAppointmentsByStatusQuery, Result<IEnumerable<AppointmentDto>>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IMapper _mapper;

    public GetAppointmentsByStatusQueryHandler(IAppointmentRepository appointmentRepository, IMapper mapper)
    {
        _appointmentRepository = appointmentRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<AppointmentDto>>> Handle(GetAppointmentsByStatusQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var appointments = await _appointmentRepository.GetAllAsync();
            var filteredAppointments = appointments.Where(a => a.Status != null && a.Status.Name == request.Status);
            var appointmentDtos = _mapper.Map<IEnumerable<AppointmentDto>>(filteredAppointments);

            return Result.Success(appointmentDtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<IEnumerable<AppointmentDto>>($"Error retrieving appointments by status: {ex.Message}");
        }
    }
}
