using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using MediatR;

namespace ElectroHuila.Application.Features.Roles.Commands.DeleteRol;

public class DeleteRolCommandHandler : IRequestHandler<DeleteRolCommand, Result<bool>>
{
    private readonly IRolRepository _rolRepository;

    public DeleteRolCommandHandler(IRolRepository rolRepository)
    {
        _rolRepository = rolRepository;
    }

    public async Task<Result<bool>> Handle(DeleteRolCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var role = await _rolRepository.GetByIdAsync(request.Id);
            if (role == null)
            {
                return Result.Failure<bool>($"Role with ID {request.Id} not found");
            }

            // Soft delete
            role.IsActive = false;
            role.UpdatedAt = DateTime.UtcNow;
            await _rolRepository.UpdateAsync(role);

            return Result.Success(true);
        }
        catch (Exception ex)
        {
            return Result.Failure<bool>($"Error deleting role: {ex.Message}");
        }
    }
}