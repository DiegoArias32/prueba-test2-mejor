using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Clients;
using ElectroHuila.Domain.Enums;
using MediatR;

namespace ElectroHuila.Application.Features.Clients.Commands.UpdateClient;

public class UpdateClientCommandHandler : IRequestHandler<UpdateClientCommand, Result<ClientDto>>
{
    private readonly IClientRepository _clientRepository;
    private readonly IMapper _mapper;

    public UpdateClientCommandHandler(IClientRepository clientRepository, IMapper mapper)
    {
        _clientRepository = clientRepository;
        _mapper = mapper;
    }

    public async Task<Result<ClientDto>> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var client = await _clientRepository.GetByIdAsync(request.Id);
            if (client == null)
            {
                return Result.Failure<ClientDto>($"Client with ID {request.Id} not found");
            }

            client.DocumentType = Enum.Parse<DocumentType>(request.ClientDto.DocumentType);
            client.DocumentNumber = request.ClientDto.DocumentNumber;
            client.FullName = request.ClientDto.FullName;
            client.Email = request.ClientDto.Email;
            client.Phone = request.ClientDto.Phone;
            client.Mobile = request.ClientDto.Mobile;
            client.Address = request.ClientDto.Address;
            client.UpdatedAt = DateTime.UtcNow;

            // Update IsActive if provided
            if (request.ClientDto.IsActive.HasValue)
            {
                client.IsActive = request.ClientDto.IsActive.Value;
            }

            await _clientRepository.UpdateAsync(client);
            var dto = _mapper.Map<ClientDto>(client);

            return Result.Success(dto);
        }
        catch (Exception ex)
        {
            return Result.Failure<ClientDto>($"Error updating client: {ex.Message}");
        }
    }
}
