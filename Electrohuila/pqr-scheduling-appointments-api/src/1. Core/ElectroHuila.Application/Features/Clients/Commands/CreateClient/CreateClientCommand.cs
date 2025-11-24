using ElectroHuila.Application.DTOs.Clients;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Clients.Commands.CreateClient;

public record CreateClientCommand(CreateClientDto ClientDto) : IRequest<Result<ClientDto>>;