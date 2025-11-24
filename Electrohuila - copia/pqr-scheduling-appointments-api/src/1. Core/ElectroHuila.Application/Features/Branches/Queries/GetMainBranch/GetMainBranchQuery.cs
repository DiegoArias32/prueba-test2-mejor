using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Branches;
using MediatR;

namespace ElectroHuila.Application.Features.Branches.Queries.GetMainBranch;

public record GetMainBranchQuery() : IRequest<Result<BranchDto>>;