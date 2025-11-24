using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Clients.Commands.DeleteLogicalClient;

/// <summary>
/// Comando para realizar eliminación lógica de un cliente
/// </summary>
public record DeleteLogicalClientCommand(int Id) : IRequest<Result>;
