using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.AppointmentTypes;
using MediatR;

namespace ElectroHuila.Application.Features.AppointmentTypes.Queries.GetActiveAppointmentTypes;

/// <summary>
/// Handles the GetActiveAppointmentTypesQuery query.
/// Retrieves all active appointment types from the repository and maps them to DTOs.
/// </summary>
public class GetActiveAppointmentTypesQueryHandler : IRequestHandler<GetActiveAppointmentTypesQuery, Result<IEnumerable<AppointmentTypeDto>>>
{
    private readonly IAppointmentTypeRepository _appointmentTypeRepository;
    private readonly IMapper _mapper;

    public GetActiveAppointmentTypesQueryHandler(IAppointmentTypeRepository appointmentTypeRepository, IMapper mapper)
    {
        _appointmentTypeRepository = appointmentTypeRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the query execution.
    /// </summary>
    /// <param name="request">The query parameters.</param>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    /// <returns>A result containing the list of active appointment types.</returns>
    public async Task<Result<IEnumerable<AppointmentTypeDto>>> Handle(GetActiveAppointmentTypesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var appointmentTypes = await _appointmentTypeRepository.GetActiveAsync();
            var appointmentTypeDtos = _mapper.Map<IEnumerable<AppointmentTypeDto>>(appointmentTypes);

            return Result.Success(appointmentTypeDtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<IEnumerable<AppointmentTypeDto>>($"Error retrieving active appointment types: {ex.Message}");
        }
    }
}