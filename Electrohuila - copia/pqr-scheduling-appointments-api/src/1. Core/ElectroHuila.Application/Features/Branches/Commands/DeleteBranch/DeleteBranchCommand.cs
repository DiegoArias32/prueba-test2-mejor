using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Branches.Commands.DeleteBranch;

public record DeleteBranchCommand(int Id) : IRequest<Result<bool>>;