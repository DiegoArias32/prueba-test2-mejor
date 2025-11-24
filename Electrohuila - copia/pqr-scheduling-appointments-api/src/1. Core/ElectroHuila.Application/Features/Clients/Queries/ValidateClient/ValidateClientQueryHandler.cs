using AutoMapper;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Clients;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Clients.Queries.ValidateClient;

public class ValidateClientQueryHandler : IRequestHandler<ValidateClientQuery, Result<ClientDto>>
{
    private readonly IClientRepository _clientRepository;
    private readonly IMapper _mapper;

    public ValidateClientQueryHandler(IClientRepository clientRepository, IMapper mapper)
    {
        _clientRepository = clientRepository;
        _mapper = mapper;
    }

    public async Task<Result<ClientDto>> Handle(ValidateClientQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.ClientNumber))
                return Result.Failure<ClientDto>("El número de cliente es requerido");

            var client = await _clientRepository.GetByClientNumberAsync(request.ClientNumber);

            if (client == null)
                return Result.Failure<ClientDto>("Cliente no encontrado");

            var clientDto = _mapper.Map<ClientDto>(client);

            return Result.Success(clientDto);
        }
        catch (Exception ex)
        {
            return Result.Failure<ClientDto>($"Error al validar cliente: {ex.Message}");
        }
    }
}
