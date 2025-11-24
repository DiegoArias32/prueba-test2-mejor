using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Roles;
using MediatR;

namespace ElectroHuila.Application.Features.Roles.Queries.GetRolByCode;

public class GetRolByCodeQueryHandler : IRequestHandler<GetRolByCodeQuery, Result<RolDto>>
{
    private readonly IRolRepository _rolRepository;
    private readonly IMapper _mapper;

    public GetRolByCodeQueryHandler(IRolRepository rolRepository, IMapper mapper)
    {
        _rolRepository = rolRepository;
        _mapper = mapper;
    }

    public async Task<Result<RolDto>> Handle(GetRolByCodeQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Code))
            {
                return Result.Failure<RolDto>("Role code cannot be empty");
            }

            var role = await _rolRepository.GetByCodeAsync(request.Code);
            if (role == null)
            {
                return Result.Failure<RolDto>($"Role with code '{request.Code}' not found");
            }

            var rolDto = _mapper.Map<RolDto>(role);
            return Result.Success(rolDto);
        }
        catch (Exception ex)
        {
            return Result.Failure<RolDto>($"Error retrieving role: {ex.Message}");
        }
    }
}