using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Clients.Commands.DeleteClient;

public record DeleteClientCommand(int Id) : IRequest<Result<bool>>;
