using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Branches;
using MediatR;

namespace ElectroHuila.Application.Features.Branches.Queries.GetAllBranches;

public class GetAllBranchesQueryHandler : IRequestHandler<GetAllBranchesQuery, Result<IEnumerable<BranchDto>>>
{
    private readonly IBranchRepository _branchRepository;
    private readonly IMapper _mapper;

    public GetAllBranchesQueryHandler(IBranchRepository branchRepository, IMapper mapper)
    {
        _branchRepository = branchRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<BranchDto>>> Handle(GetAllBranchesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var branches = await _branchRepository.GetAllAsync();
            var branchDtos = _mapper.Map<IEnumerable<BranchDto>>(branches);

            return Result.Success(branchDtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<IEnumerable<BranchDto>>($"Error retrieving branches: {ex.Message}");
        }
    }
}