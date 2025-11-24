using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Roles;
using MediatR;

namespace ElectroHuila.Application.Features.Roles.Queries.GetAllRoles;

public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, Result<IEnumerable<RolDto>>>
{
    private readonly IRolRepository _rolRepository;
    private readonly IMapper _mapper;

    public GetAllRolesQueryHandler(IRolRepository rolRepository, IMapper mapper)
    {
        _rolRepository = rolRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<RolDto>>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var roles = await _rolRepository.GetAllAsync();
            var roleDtos = _mapper.Map<IEnumerable<RolDto>>(roles);

            return Result.Success(roleDtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<IEnumerable<RolDto>>($"Error retrieving roles: {ex.Message}");
        }
    }
}