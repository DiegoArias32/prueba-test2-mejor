using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using MediatR;

namespace ElectroHuila.Application.Features.Permissions.Commands.RemovePermissionFromRol;

public class RemovePermissionFromRolCommandHandler : IRequestHandler<RemovePermissionFromRolCommand, Result<bool>>
{
    private readonly IRolRepository _rolRepository;

    public RemovePermissionFromRolCommandHandler(IRolRepository rolRepository)
    {
        _rolRepository = rolRepository;
    }

    public async Task<Result<bool>> Handle(RemovePermissionFromRolCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var role = await _rolRepository.GetByIdAsync(request.Dto.RolId);
            if (role == null)
            {
                return Result.Failure<bool>($"Role with ID {request.Dto.RolId} not found");
            }

            var rolFormPermi = role.RolFormPermis
                .FirstOrDefault(rfp => rfp.FormId == request.Dto.FormId && rfp.IsActive);

            if (rolFormPermi == null)
            {
                return Result.Failure<bool>($"Permission assignment not found for Role {request.Dto.RolId} and Form {request.Dto.FormId}");
            }

            // Soft delete
            rolFormPermi.IsActive = false;
            rolFormPermi.UpdatedAt = DateTime.UtcNow;

            role.UpdatedAt = DateTime.UtcNow;
            await _rolRepository.UpdateAsync(role);

            return Result.Success(true);
        }
        catch (Exception ex)
        {
            return Result.Failure<bool>($"Error removing permission from role: {ex.Message}");
        }
    }
}