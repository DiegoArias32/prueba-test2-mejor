using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Branches;
using MediatR;

namespace ElectroHuila.Application.Features.Branches.Queries.GetBranchById;

public class GetBranchByIdQueryHandler : IRequestHandler<GetBranchByIdQuery, Result<BranchDto>>
{
    private readonly IBranchRepository _branchRepository;
    private readonly IMapper _mapper;

    public GetBranchByIdQueryHandler(IBranchRepository branchRepository, IMapper mapper)
    {
        _branchRepository = branchRepository;
        _mapper = mapper;
    }

    public async Task<Result<BranchDto>> Handle(GetBranchByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var branch = await _branchRepository.GetByIdAsync(request.Id);
            if (branch == null)
            {
                return Result.Failure<BranchDto>($"Branch with ID {request.Id} not found");
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