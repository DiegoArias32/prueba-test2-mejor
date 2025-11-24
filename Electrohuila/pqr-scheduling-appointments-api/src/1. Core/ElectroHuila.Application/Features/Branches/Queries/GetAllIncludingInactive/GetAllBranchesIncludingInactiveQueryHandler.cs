using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Branches;
using MediatR;

namespace ElectroHuila.Application.Features.Branches.Queries.GetAllIncludingInactive;

/// <summary>
/// Handler para obtener todas las sucursales incluyendo las inactivas.
/// </summary>
public class GetAllBranchesIncludingInactiveQueryHandler
    : IRequestHandler<GetAllBranchesIncludingInactiveQuery, Result<IEnumerable<BranchDto>>>
{
    private readonly IBranchRepository _branchRepository;
    private readonly IMapper _mapper;

    public GetAllBranchesIncludingInactiveQueryHandler(
        IBranchRepository branchRepository,
        IMapper mapper)
    {
        _branchRepository = branchRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<BranchDto>>> Handle(
        GetAllBranchesIncludingInactiveQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var branches = await _branchRepository.GetAllIncludingInactiveAsync();
            var branchDtos = _mapper.Map<IEnumerable<BranchDto>>(branches);

            return Result.Success(branchDtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<IEnumerable<BranchDto>>($"Error retrieving branches including inactive: {ex.Message}");
        }
    }
}
