using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Clients;
using MediatR;

namespace ElectroHuila.Application.Features.Clients.Queries.GetClientById;

public record GetClientByIdQuery(int Id) : IRequest<Result<ClientDto>>;
