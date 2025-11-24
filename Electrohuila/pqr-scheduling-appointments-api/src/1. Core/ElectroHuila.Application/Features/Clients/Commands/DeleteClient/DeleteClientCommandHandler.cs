using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using MediatR;

namespace ElectroHuila.Application.Features.Clients.Commands.DeleteClient;

public class DeleteClientCommandHandler : IRequestHandler<DeleteClientCommand, Result<bool>>
{
    private readonly IClientRepository _clientRepository;

    public DeleteClientCommandHandler(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    public async Task<Result<bool>> Handle(DeleteClientCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var client = await _clientRepository.GetByIdAsync(request.Id);
            if (client == null)
            {
                return Result.Failure<bool>($"Client with ID {request.Id} not found");
            }

            // Soft delete
            client.IsActive = false;
            client.UpdatedAt = DateTime.UtcNow;
            await _clientRepository.UpdateAsync(client);

            return Result.Success(true);
        }
        catch (Exception ex)
        {
            return Result.Failure<bool>($"Error deleting client: {ex.Message}");
        }
    }
}
