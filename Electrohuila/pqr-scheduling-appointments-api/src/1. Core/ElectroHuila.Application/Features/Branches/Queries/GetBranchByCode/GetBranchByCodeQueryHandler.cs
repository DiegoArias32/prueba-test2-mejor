using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Branches;
using MediatR;

namespace ElectroHuila.Application.Features.Branches.Queries.GetBranchByCode;

public class GetBranchByCodeQueryHandler : IRequestHandler<GetBranchByCodeQuery, Result<BranchDto>>
{
    private readonly IBranchRepository _branchRepository;
    private readonly IMapper _mapper;

    public GetBranchByCodeQueryHandler(IBranchRepository branchRepository, IMapper mapper)
    {
        _branchRepository = branchRepository;
        _mapper = mapper;
    }

    public async Task<Result<BranchDto>> Handle(GetBranchByCodeQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Code))
            {
                return Result.Failure<BranchDto>("Branch code cannot be empty");
            }

            var branch = await _branchRepository.GetByCodeAsync(request.Code);
            if (branch == null)
            {
                return Result.Failure<BranchDto>($"Branch with code '{request.Code}' not found");
            }

            var branchDto = _mapper.Map<BranchDto>(branch);
            return Result.Success(branchDto);
        }
        catch (Exception ex)
        {
            return Result.Failure<BranchDto>($"Error retrieving branch: {ex.Message}");
        }
    }
}