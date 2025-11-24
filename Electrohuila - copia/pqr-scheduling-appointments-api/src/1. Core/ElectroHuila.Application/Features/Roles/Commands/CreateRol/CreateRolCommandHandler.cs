using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Roles;
using ElectroHuila.Domain.Entities.Security;
using MediatR;

namespace ElectroHuila.Application.Features.Roles.Commands.CreateRol;

public class CreateRolCommandHandler : IRequestHandler<CreateRolCommand, Result<RolDto>>
{
    private readonly IRolRepository _rolRepository;
    private readonly IMapper _mapper;

    public CreateRolCommandHandler(IRolRepository rolRepository, IMapper mapper)
    {
        _rolRepository = rolRepository;
        _mapper = mapper;
    }

    public async Task<Result<RolDto>> Handle(CreateRolCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var codeExists = await _rolRepository.ExistsByCodeAsync(request.RolDto.Code);
            if (codeExists)
            {
                return Result.Failure<RolDto>($"Role with code '{request.RolDto.Code}' already exists");
            }

            var nameExists = await _rolRepository.ExistsByNameAsync(request.RolDto.Name);
            if (nameExists)
            {
                return Result.Failure<RolDto>($"Role with name '{request.RolDto.Name}' already exists");
            }

            var role = _mapper.Map<Rol>(request.RolDto);
            var createdRole = await _rolRepository.AddAsync(role);
            var rolDto = _mapper.Map<RolDto>(createdRole);

            return Result.Success(rolDto);
        }
        catch (Exception ex)
        {
            return Result.Failure<RolDto>($"Error creating role: {ex.Message}");
        }
    }
}