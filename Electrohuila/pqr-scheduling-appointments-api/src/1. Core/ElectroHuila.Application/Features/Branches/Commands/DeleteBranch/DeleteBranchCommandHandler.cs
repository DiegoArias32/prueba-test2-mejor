using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using MediatR;

namespace ElectroHuila.Application.Features.Branches.Commands.DeleteBranch;

public class DeleteBranchCommandHandler : IRequestHandler<DeleteBranchCommand, Result<bool>>
{
    private readonly IBranchRepository _branchRepository;

    public DeleteBranchCommandHandler(IBranchRepository branchRepository)
    {
        _branchRepository = branchRepository;
    }

    public async Task<Result<bool>> Handle(DeleteBranchCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var branch = await _branchRepository.GetByIdAsync(request.Id);
            if (branch == null)
            {
                return Result.Failure<bool>($"Branch with ID {request.Id} not found");
            }

            if (branch.IsMain)
            {
                return Result.Failure<bool>("Cannot delete main branch");
            }

            // Soft delete
            branch.IsActive = false;
            branch.UpdatedAt = DateTime.UtcNow;
            await _branchRepository.UpdateAsync(branch);

            return Result.Success(true);
        }
        catch (Exception ex)
        {
            return Result.Failure<bool>($"Error deleting branch: {ex.Message}");
        }
    }
}