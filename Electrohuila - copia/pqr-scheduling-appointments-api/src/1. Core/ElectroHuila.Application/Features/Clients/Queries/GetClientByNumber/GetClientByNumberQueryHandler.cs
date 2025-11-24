using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Clients;
using MediatR;

namespace ElectroHuila.Application.Features.Clients.Queries.GetClientByNumber;

public class GetClientByNumberQueryHandler : IRequestHandler<GetClientByNumberQuery, Result<ClientDto>>
{
    private readonly IClientRepository _clientRepository;
    private readonly IMapper _mapper;

    public GetClientByNumberQueryHandler(IClientRepository clientRepository, IMapper mapper)
    {
        _clientRepository = clientRepository;
        _mapper = mapper;
    }

    public async Task<Result<ClientDto>> Handle(GetClientByNumberQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var client = await _clientRepository.GetByClientNumberAsync(request.ClientNumber);
            if (client == null)
            {
                return Result.Failure<ClientDto>($"Client with number {request.ClientNumber} not found");
            }

            var clientDto = _mapper.Map<ClientDto>(client);
            return Result.Success(clientDto);
        }
        catch (Exception ex)
        {
            return Result.Failure<ClientDto>($"Error retrieving client: {ex.Message}");
        }
    }
}
