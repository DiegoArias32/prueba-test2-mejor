using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Branches;
using MediatR;

namespace ElectroHuila.Application.Features.Branches.Commands.CreateBranch;

public record CreateBranchCommand(CreateBranchDto BranchDto) : IRequest<Result<BranchDto>>;