using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using MediatR;

namespace ElectroHuila.Application.Features.Users.Commands.DeleteLogicalUser;

/// <summary>
/// Manejador del comando para eliminación lógica de usuarios
/// </summary>
public class DeleteLogicalUserCommandHandler : IRequestHandler<DeleteLogicalUserCommand, Result>
{
    private readonly IUserRepository _userRepository;

    public DeleteLogicalUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result> Handle(DeleteLogicalUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(request.Id);
            if (user == null)
            {
                return Result.Failure($"Usuario con ID {request.Id} no encontrado");
            }

            if (!user.IsActive)
            {
                return Result.Failure($"El usuario con ID {request.Id} ya está inactivo");
            }

            // Eliminación lógica
            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error al eliminar lógicamente el usuario: {ex.Message}");
        }
    }
}
