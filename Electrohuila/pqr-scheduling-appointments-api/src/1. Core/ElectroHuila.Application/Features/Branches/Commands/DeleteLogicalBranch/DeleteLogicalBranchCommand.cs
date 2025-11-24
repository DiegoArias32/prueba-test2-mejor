using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Branches.Commands.DeleteLogicalBranch;

/// <summary>
/// Comando para realizar eliminación lógica de una sucursal
/// </summary>
public record DeleteLogicalBranchCommand(int Id) : IRequest<Result>;
