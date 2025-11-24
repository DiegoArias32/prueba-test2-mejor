using AutoMapper;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Appointments;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Appointments.Queries.GetAppointmentByNumber;

public class GetAppointmentByNumberQueryHandler : IRequestHandler<GetAppointmentByNumberQuery, Result<AppointmentDto>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IMapper _mapper;

    public GetAppointmentByNumberQueryHandler(IAppointmentRepository appointmentRepository, IMapper mapper)
    {
        _appointmentRepository = appointmentRepository;
        _mapper = mapper;
    }

    public async Task<Result<AppointmentDto>> Handle(GetAppointmentByNumberQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var appointments = await _appointmentRepository.GetAllAsync();
            var appointment = appointments.FirstOrDefault(a => a.AppointmentNumber == request.AppointmentNumber);

            if (appointment == null)
            {
                return Result.Failure<AppointmentDto>($"Appointment with number {request.AppointmentNumber} not found");
            }

            var appointmentDto = _mapper.Map<AppointmentDto>(appointment);
            return Result.Success(appointmentDto);
        }
        catch (Exception ex)
        {
            return Result.Failure<AppointmentDto>($"Error retrieving appointment: {ex.Message}");
        }
    }
}
