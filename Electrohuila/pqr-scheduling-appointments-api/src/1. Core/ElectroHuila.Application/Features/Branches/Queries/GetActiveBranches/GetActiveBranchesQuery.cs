using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Branches;
using MediatR;

namespace ElectroHuila.Application.Features.Branches.Queries.GetActiveBranches;

public record GetActiveBranchesQuery() : IRequest<Result<IEnumerable<BranchDto>>>;