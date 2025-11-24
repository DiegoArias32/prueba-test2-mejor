using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using MediatR;

namespace ElectroHuila.Application.Features.Clients.Commands.DeleteLogicalClient;

/// <summary>
/// Manejador del comando para eliminación lógica de clientes
/// </summary>
public class DeleteLogicalClientCommandHandler : IRequestHandler<DeleteLogicalClientCommand, Result>
{
    private readonly IClientRepository _clientRepository;

    public DeleteLogicalClientCommandHandler(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    public async Task<Result> Handle(DeleteLogicalClientCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var client = await _clientRepository.GetByIdAsync(request.Id);
            if (client == null)
            {
                return Result.Failure($"Cliente con ID {request.Id} no encontrado");
            }

            if (!client.IsActive)
            {
                return Result.Failure($"El cliente con ID {request.Id} ya está inactivo");
            }

            // Eliminación lógica
            client.IsActive = false;
            client.UpdatedAt = DateTime.UtcNow;
            await _clientRepository.UpdateAsync(client);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error al eliminar lógicamente el cliente: {ex.Message}");
        }
    }
}
