using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Clients;
using MediatR;

namespace ElectroHuila.Application.Features.Clients.Queries.GetAllClients;

public class GetAllClientsQueryHandler : IRequestHandler<GetAllClientsQuery, Result<List<ClientDto>>>
{
    private readonly IClientRepository _clientRepository;
    private readonly IMapper _mapper;

    public GetAllClientsQueryHandler(IClientRepository clientRepository, IMapper mapper)
    {
        _clientRepository = clientRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<ClientDto>>> Handle(GetAllClientsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var clients = await _clientRepository.GetAllAsync();
            var clientDtos = _mapper.Map<List<ClientDto>>(clients);
            return Result.Success(clientDtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<List<ClientDto>>($"Error retrieving clients: {ex.Message}");
        }
    }
}
