using AutoMapper;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Appointments;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Appointments.Queries.GetClientAppointments;

public class GetClientAppointmentsQueryHandler : IRequestHandler<GetClientAppointmentsQuery, Result<IEnumerable<AppointmentDto>>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IMapper _mapper;

    public GetClientAppointmentsQueryHandler(
        IAppointmentRepository appointmentRepository,
        IClientRepository clientRepository,
        IMapper mapper)
    {
        _appointmentRepository = appointmentRepository;
        _clientRepository = clientRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<AppointmentDto>>> Handle(GetClientAppointmentsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.ClientNumber))
                return Result.Failure<IEnumerable<AppointmentDto>>("El número de cliente es requerido");

            var client = await _clientRepository.GetByClientNumberAsync(request.ClientNumber);
            if (client == null)
                return Result.Failure<IEnumerable<AppointmentDto>>("Cliente no encontrado");

            // Get all appointments for the client with navigation properties loaded
            var clientAppointments = await _appointmentRepository.GetByClientIdAsync(client.Id);

            var appointmentDtos = _mapper.Map<IEnumerable<AppointmentDto>>(clientAppointments);

            return Result.Success(appointmentDtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<IEnumerable<AppointmentDto>>($"Error al obtener citas del cliente: {ex.Message}");
        }
    }
}
