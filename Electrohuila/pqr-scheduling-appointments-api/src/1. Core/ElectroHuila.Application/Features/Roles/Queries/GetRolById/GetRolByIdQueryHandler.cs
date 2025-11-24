using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Roles;
using MediatR;

namespace ElectroHuila.Application.Features.Roles.Queries.GetRolById;

public class GetRolByIdQueryHandler : IRequestHandler<GetRolByIdQuery, Result<RolDto>>
{
    private readonly IRolRepository _rolRepository;
    private readonly IMapper _mapper;

    public GetRolByIdQueryHandler(IRolRepository rolRepository, IMapper mapper)
    {
        _rolRepository = rolRepository;
        _mapper = mapper;
    }

    public async Task<Result<RolDto>> Handle(GetRolByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var role = await _rolRepository.GetByIdAsync(request.Id);
            if (role == null)
            {
                return Result.Failure<RolDto>($"Role with ID {request.Id} not found");
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