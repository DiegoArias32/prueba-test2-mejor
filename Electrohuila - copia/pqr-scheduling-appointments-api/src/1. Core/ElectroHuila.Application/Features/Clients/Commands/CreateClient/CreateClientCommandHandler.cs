using AutoMapper;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Clients;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Domain.Entities.Clients;
using MediatR;

namespace ElectroHuila.Application.Features.Clients.Commands.CreateClient;

public class CreateClientCommandHandler : IRequestHandler<CreateClientCommand, Result<ClientDto>>
{
    private readonly IClientRepository _clientRepository;
    private readonly IMapper _mapper;

    public CreateClientCommandHandler(IClientRepository clientRepository, IMapper mapper)
    {
        _clientRepository = clientRepository;
        _mapper = mapper;
    }

    public async Task<Result<ClientDto>> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if client already exists
            var existingClient = await _clientRepository.GetByDocumentNumberAsync(request.ClientDto.DocumentNumber);
            if (existingClient != null)
            {
                return Result.Failure<ClientDto>("A client with this document number already exists");
            }

            var client = _mapper.Map<Client>(request.ClientDto);
            var createdClient = await _clientRepository.AddAsync(client);
            var clientDto = _mapper.Map<ClientDto>(createdClient);

            return Result.Success(clientDto);
        }
        catch (Exception ex)
        {
            return Result.Failure<ClientDto>($"Error creating client: {ex.Message}");
        }
    }
}