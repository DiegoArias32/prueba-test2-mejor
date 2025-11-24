using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.AppointmentTypes;
using MediatR;

namespace ElectroHuila.Application.Features.AppointmentTypes.Queries.GetAppointmentTypeByName;

public class GetAppointmentTypeByNameQueryHandler : IRequestHandler<GetAppointmentTypeByNameQuery, Result<AppointmentTypeDto>>
{
    private readonly IAppointmentTypeRepository _appointmentTypeRepository;
    private readonly IMapper _mapper;

    public GetAppointmentTypeByNameQueryHandler(IAppointmentTypeRepository appointmentTypeRepository, IMapper mapper)
    {
        _appointmentTypeRepository = appointmentTypeRepository;
        _mapper = mapper;
    }

    public async Task<Result<AppointmentTypeDto>> Handle(GetAppointmentTypeByNameQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var appointmentType = await _appointmentTypeRepository.GetByNameAsync(request.Name);
            if (appointmentType == null)
            {
                return Result.Failure<AppointmentTypeDto>($"Appointment type with name '{request.Name}' not found");
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