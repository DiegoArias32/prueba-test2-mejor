using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Roles;
using MediatR;

namespace ElectroHuila.Application.Features.Roles.Commands.UpdateRol;

public class UpdateRolCommandHandler : IRequestHandler<UpdateRolCommand, Result<RolDto>>
{
    private readonly IRolRepository _rolRepository;
    private readonly IMapper _mapper;

    public UpdateRolCommandHandler(IRolRepository rolRepository, IMapper mapper)
    {
        _rolRepository = rolRepository;
        _mapper = mapper;
    }

    public async Task<Result<RolDto>> Handle(UpdateRolCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingRole = await _rolRepository.GetByIdAsync(request.Id);
            if (existingRole == null)
            {
                return Result.Failure<RolDto>($"Role with ID {request.Id} not found");
            }

            if (!string.IsNullOrEmpty(request.RolDto.Code) && request.RolDto.Code != existingRole.Code)
            {
                var codeExists = await _rolRepository.ExistsByCodeAsync(request.RolDto.Code);
                if (codeExists)
                {
                    return Result.Failure<RolDto>($"Role with code '{request.RolDto.Code}' already exists");
                }
            }

            if (!string.IsNullOrEmpty(request.RolDto.Name) && request.RolDto.Name != existingRole.Name)
            {
                var nameExists = await _rolRepository.ExistsByNameAsync(request.RolDto.Name);
                if (nameExists)
                {
                    return Result.Failure<RolDto>($"Role with name '{request.RolDto.Name}' already exists");
                }
            }

            existingRole.Name = request.RolDto.Name;
            existingRole.Code = request.RolDto.Code;
            existingRole.UpdatedAt = DateTime.UtcNow;

            await _rolRepository.UpdateAsync(existingRole);
            var rolDto = _mapper.Map<RolDto>(existingRole);

            return Result.Success(rolDto);
        }
        catch (Exception ex)
        {
            return Result.Failure<RolDto>($"Error updating role: {ex.Message}");
        }
    }
}