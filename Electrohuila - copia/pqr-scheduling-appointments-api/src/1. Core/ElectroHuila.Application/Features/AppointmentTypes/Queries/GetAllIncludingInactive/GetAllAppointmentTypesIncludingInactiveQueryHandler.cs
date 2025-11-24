using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.AppointmentTypes;
using MediatR;

namespace ElectroHuila.Application.Features.AppointmentTypes.Queries.GetAllIncludingInactive;

/// <summary>
/// Handler para obtener todos los tipos de citas incluyendo los inactivos.
/// </summary>
public class GetAllAppointmentTypesIncludingInactiveQueryHandler
    : IRequestHandler<GetAllAppointmentTypesIncludingInactiveQuery, Result<IEnumerable<AppointmentTypeDto>>>
{
    private readonly IAppointmentTypeRepository _appointmentTypeRepository;
    private readonly IMapper _mapper;

    public GetAllAppointmentTypesIncludingInactiveQueryHandler(
        IAppointmentTypeRepository appointmentTypeRepository,
        IMapper mapper)
    {
        _appointmentTypeRepository = appointmentTypeRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<AppointmentTypeDto>>> Handle(
        GetAllAppointmentTypesIncludingInactiveQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var appointmentTypes = await _appointmentTypeRepository.GetAllIncludingInactiveAsync();
            var appointmentTypeDtos = _mapper.Map<IEnumerable<AppointmentTypeDto>>(appointmentTypes);

            return Result.Success(appointmentTypeDtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<IEnumerable<AppointmentTypeDto>>($"Error retrieving appointment types including inactive: {ex.Message}");
        }
    }
}
