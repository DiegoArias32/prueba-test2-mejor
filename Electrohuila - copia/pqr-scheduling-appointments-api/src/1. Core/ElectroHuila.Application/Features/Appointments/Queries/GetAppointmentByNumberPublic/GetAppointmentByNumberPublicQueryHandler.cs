using AutoMapper;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Appointments;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Appointments.Queries.GetAppointmentByNumberPublic;

public class GetAppointmentByNumberPublicQueryHandler : IRequestHandler<GetAppointmentByNumberPublicQuery, Result<AppointmentDto>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IMapper _mapper;

    public GetAppointmentByNumberPublicQueryHandler(
        IAppointmentRepository appointmentRepository,
        IClientRepository clientRepository,
        IMapper mapper)
    {
        _appointmentRepository = appointmentRepository;
        _clientRepository = clientRepository;
        _mapper = mapper;
    }

    public async Task<Result<AppointmentDto>> Handle(GetAppointmentByNumberPublicQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.AppointmentNumber))
                return Result.Failure<AppointmentDto>("Número de cita requerido");

            if (string.IsNullOrWhiteSpace(request.ClientNumber))
                return Result.Failure<AppointmentDto>("Número de cliente requerido para consultar la cita");

            // Get all appointments and find by appointment number
            var allAppointments = await _appointmentRepository.GetAllAsync();
            var appointment = allAppointments.FirstOrDefault(a => a.AppointmentNumber == request.AppointmentNumber);

            if (appointment == null)
                return Result.Failure<AppointmentDto>("Cita no encontrada");

            // Verify client owns this appointment
            var client = await _clientRepository.GetByIdAsync(appointment.ClientId);
            if (client?.ClientNumber != request.ClientNumber)
                return Result.Failure<AppointmentDto>("Cita no encontrada para este cliente");

            var appointmentDto = _mapper.Map<AppointmentDto>(appointment);

            return Result.Success(appointmentDto);
        }
        catch (Exception ex)
        {
            return Result.Failure<AppointmentDto>($"Error al consultar cita pública: {ex.Message}");
        }
    }
}
