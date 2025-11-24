using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Clients;
using MediatR;

namespace ElectroHuila.Application.Features.Clients.Queries.GetAllClients;

public record GetAllClientsQuery : IRequest<Result<List<ClientDto>>>;
