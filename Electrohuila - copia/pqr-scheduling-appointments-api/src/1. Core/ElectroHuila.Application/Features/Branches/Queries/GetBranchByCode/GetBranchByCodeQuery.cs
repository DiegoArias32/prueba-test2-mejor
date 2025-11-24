using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Branches;
using MediatR;

namespace ElectroHuila.Application.Features.Branches.Queries.GetBranchByCode;

public record GetBranchByCodeQuery(string Code) : IRequest<Result<BranchDto>>;