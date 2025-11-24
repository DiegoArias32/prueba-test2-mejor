using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Branches;
using MediatR;

namespace ElectroHuila.Application.Features.Branches.Queries.GetMainBranch;

public class GetMainBranchQueryHandler : IRequestHandler<GetMainBranchQuery, Result<BranchDto>>
{
    private readonly IBranchRepository _branchRepository;
    private readonly IMapper _mapper;

    public GetMainBranchQueryHandler(IBranchRepository branchRepository, IMapper mapper)
    {
        _branchRepository = branchRepository;
        _mapper = mapper;
    }

    public async Task<Result<BranchDto>> Handle(GetMainBranchQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var branch = await _branchRepository.GetMainBranchAsync();
            if (branch == null)
            {
                return Result.Failure<BranchDto>("No main branch found");
            }

            var branchDto = _mapper.Map<BranchDto>(branch);
            return Result.Success(branchDto);
        }
        catch (Exception ex)
        {
            return Result.Failure<BranchDto>($"Error retrieving main branch: {ex.Message}");
        }
    }
}