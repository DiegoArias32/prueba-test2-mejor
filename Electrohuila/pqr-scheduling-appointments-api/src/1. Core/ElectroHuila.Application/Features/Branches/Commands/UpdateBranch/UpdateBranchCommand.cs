using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Branches;
using MediatR;

namespace ElectroHuila.Application.Features.Branches.Commands.UpdateBranch;

public record UpdateBranchCommand(int Id, UpdateBranchDto BranchDto) : IRequest<Result<BranchDto>>;