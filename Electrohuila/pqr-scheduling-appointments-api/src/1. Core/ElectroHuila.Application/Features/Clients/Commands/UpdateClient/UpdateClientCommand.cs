using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Clients;
using MediatR;

namespace ElectroHuila.Application.Features.Clients.Commands.UpdateClient;

public record UpdateClientCommand(int Id, UpdateClientDto ClientDto) : IRequest<Result<ClientDto>>;
