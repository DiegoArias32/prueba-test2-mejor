using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Roles;
using MediatR;

namespace ElectroHuila.Application.Features.Roles.Queries.GetAllIncludingInactive;

/// <summary>
/// Handler para obtener todos los roles incluyendo los inactivos.
/// </summary>
public class GetAllRolesIncludingInactiveQueryHandler
    : IRequestHandler<GetAllRolesIncludingInactiveQuery, Result<IEnumerable<RolDto>>>
{
    private readonly IRolRepository _rolRepository;
    private readonly IMapper _mapper;

    public GetAllRolesIncludingInactiveQueryHandler(
        IRolRepository rolRepository,
        IMapper mapper)
    {
        _rolRepository = rolRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<RolDto>>> Handle(
        GetAllRolesIncludingInactiveQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var roles = await _rolRepository.GetAllIncludingInactiveAsync();
            var rolDtos = _mapper.Map<IEnumerable<RolDto>>(roles);

            return Result.Success(rolDtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<IEnumerable<RolDto>>($"Error retrieving roles including inactive: {ex.Message}");
        }
    }
}
