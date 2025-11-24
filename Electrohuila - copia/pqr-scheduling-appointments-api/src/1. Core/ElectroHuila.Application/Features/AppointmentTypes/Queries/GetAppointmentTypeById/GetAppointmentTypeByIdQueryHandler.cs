using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.AppointmentTypes;
using MediatR;

namespace ElectroHuila.Application.Features.AppointmentTypes.Queries.GetAppointmentTypeById;

public class GetAppointmentTypeByIdQueryHandler : IRequestHandler<GetAppointmentTypeByIdQuery, Result<AppointmentTypeDto>>
{
    private readonly IAppointmentTypeRepository _appointmentTypeRepository;
    private readonly IMapper _mapper;

    public GetAppointmentTypeByIdQueryHandler(IAppointmentTypeRepository appointmentTypeRepository, IMapper mapper)
    {
        _appointmentTypeRepository = appointmentTypeRepository;
        _mapper = mapper;
    }

    public async Task<Result<AppointmentTypeDto>> Handle(GetAppointmentTypeByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var appointmentType = await _appointmentTypeRepository.GetByIdAsync(request.Id);
            if (appointmentType == null)
            {
                return Result.Failure<AppointmentTypeDto>($"Appointment type with ID {request.Id} not found");
            }

            var appointmentTypeDto = _mapper.Map<AppointmentTypeDto>(appointmentType);
            return Result.Success(appointmentTypeDto);
        }
        catch (Exception ex)
        {
            return Result.Failure<AppointmentTypeDto>($"Error retrieving appointment type: {ex.Message}");
        }
    }
}