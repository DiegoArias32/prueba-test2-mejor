using AutoMapper;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Appointments;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Appointments.Queries.GetAppointmentsByClientNumber;

public class GetAppointmentsByClientNumberQueryHandler : IRequestHandler<GetAppointmentsByClientNumberQuery, Result<IEnumerable<AppointmentDto>>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IMapper _mapper;

    public GetAppointmentsByClientNumberQueryHandler(IAppointmentRepository appointmentRepository, IClientRepository clientRepository, IMapper mapper)
    {
        _appointmentRepository = appointmentRepository;
        _clientRepository = clientRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<AppointmentDto>>> Handle(GetAppointmentsByClientNumberQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var client = await _clientRepository.GetByClientNumberAsync(request.ClientNumber);
            if (client == null)
            {
                return Result.Failure<IEnumerable<AppointmentDto>>("Cliente no encontrado");
            }

            var appointments = await _appointmentRepository.GetByClientIdAsync(client.Id);
            var appointmentDtos = _mapper.Map<IEnumerable<AppointmentDto>>(appointments);

            return Result.Success(appointmentDtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<IEnumerable<AppointmentDto>>($"Error retrieving appointments for client: {ex.Message}");
        }
    }
}
