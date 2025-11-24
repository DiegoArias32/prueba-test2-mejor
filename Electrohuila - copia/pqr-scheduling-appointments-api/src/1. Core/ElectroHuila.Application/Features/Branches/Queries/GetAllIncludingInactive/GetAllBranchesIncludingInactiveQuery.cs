using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Branches;
using MediatR;

namespace ElectroHuila.Application.Features.Branches.Queries.GetAllIncludingInactive;

/// <summary>
/// Query para obtener todas las sucursales incluyendo las inactivas.
/// </summary>
public record GetAllBranchesIncludingInactiveQuery : IRequest<Result<IEnumerable<BranchDto>>>;
