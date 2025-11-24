using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Users.Commands.DeleteLogicalUser;

/// <summary>
/// Comando para realizar eliminación lógica de un usuario
/// </summary>
public record DeleteLogicalUserCommand(int Id) : IRequest<Result>;
