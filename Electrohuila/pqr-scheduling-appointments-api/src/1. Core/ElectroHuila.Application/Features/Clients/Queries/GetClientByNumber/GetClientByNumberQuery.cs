using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Clients;
using MediatR;

namespace ElectroHuila.Application.Features.Clients.Queries.GetClientByNumber;

public record GetClientByNumberQuery(string ClientNumber) : IRequest<Result<ClientDto>>;
