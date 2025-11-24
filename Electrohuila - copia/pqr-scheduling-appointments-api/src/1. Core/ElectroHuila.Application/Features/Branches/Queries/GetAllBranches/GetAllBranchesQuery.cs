using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Branches;
using MediatR;

namespace ElectroHuila.Application.Features.Branches.Queries.GetAllBranches;

public record GetAllBranchesQuery() : IRequest<Result<IEnumerable<BranchDto>>>;