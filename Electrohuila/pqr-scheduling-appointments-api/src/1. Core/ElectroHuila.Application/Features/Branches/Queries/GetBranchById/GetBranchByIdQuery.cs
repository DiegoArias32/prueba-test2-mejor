using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Branches;
using MediatR;

namespace ElectroHuila.Application.Features.Branches.Queries.GetBranchById;

public record GetBranchByIdQuery(int Id) : IRequest<Result<BranchDto>>;