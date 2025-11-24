using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Clients;
using MediatR;

namespace ElectroHuila.Application.Features.Clients.Queries.GetClientByDocument;

public class GetClientByDocumentQueryHandler : IRequestHandler<GetClientByDocumentQuery, Result<ClientDto>>
{
    private readonly IClientRepository _clientRepository;
    private readonly IMapper _mapper;

    public GetClientByDocumentQueryHandler(IClientRepository clientRepository, IMapper mapper)
    {
        _clientRepository = clientRepository;
        _mapper = mapper;
    }

    public async Task<Result<ClientDto>> Handle(GetClientByDocumentQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var client = await _clientRepository.GetByDocumentNumberAsync(request.DocumentNumber);
            if (client == null)
            {
                return Result.Failure<ClientDto>($"Client with document number {request.DocumentNumber} not found");
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
