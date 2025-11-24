using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Clients;
using MediatR;

namespace ElectroHuila.Application.Features.Clients.Queries.GetAllIncludingInactive;

/// <summary>
/// Handler para obtener todos los clientes incluyendo los inactivos.
/// </summary>
public class GetAllClientsIncludingInactiveQueryHandler
    : IRequestHandler<GetAllClientsIncludingInactiveQuery, Result<IEnumerable<ClientDto>>>
{
    private readonly IClientRepository _clientRepository;
    private readonly IMapper _mapper;

    public GetAllClientsIncludingInactiveQueryHandler(
        IClientRepository clientRepository,
        IMapper mapper)
    {
        _clientRepository = clientRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<ClientDto>>> Handle(
        GetAllClientsIncludingInactiveQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var clients = await _clientRepository.GetAllIncludingInactiveAsync();
            var clientDtos = _mapper.Map<IEnumerable<ClientDto>>(clients);

            return Result.Success(clientDtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<IEnumerable<ClientDto>>($"Error retrieving clients including inactive: {ex.Message}");
        }
    }
}
