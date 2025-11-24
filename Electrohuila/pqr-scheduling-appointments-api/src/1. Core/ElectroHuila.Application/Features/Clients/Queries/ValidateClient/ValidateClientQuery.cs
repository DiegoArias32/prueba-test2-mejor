using ElectroHuila.Application.DTOs.Clients;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Clients.Queries.ValidateClient;

public record ValidateClientQuery(string ClientNumber) : IRequest<Result<ClientDto>>;
