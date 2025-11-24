using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Branches;
using ElectroHuila.Domain.Entities.Locations;
using MediatR;

namespace ElectroHuila.Application.Features.Branches.Commands.UpdateBranch;

public class UpdateBranchCommandHandler : IRequestHandler<UpdateBranchCommand, Result<BranchDto>>
{
    private readonly IBranchRepository _branchRepository;
    private readonly IMapper _mapper;

    public UpdateBranchCommandHandler(IBranchRepository branchRepository, IMapper mapper)
    {
        _branchRepository = branchRepository;
        _mapper = mapper;
    }

    public async Task<Result<BranchDto>> Handle(UpdateBranchCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingBranch = await _branchRepository.GetByIdAsync(request.Id);
            if (existingBranch == null)
            {
                return Result.Failure<BranchDto>($"Branch with ID {request.Id} not found");
            }

            if (!string.IsNullOrEmpty(request.BranchDto.Code) && request.BranchDto.Code != existingBranch.Code)
            {
                var codeExists = await _branchRepository.ExistsByCodeAsync(request.BranchDto.Code);
                if (codeExists)
                {
                    return Result.Failure<BranchDto>($"Branch with code '{request.BranchDto.Code}' already exists");
                }
            }

            existingBranch.Name = request.BranchDto.Name;
            existingBranch.Code = request.BranchDto.Code;
            existingBranch.Address = request.BranchDto.Address;
            existingBranch.Phone = request.BranchDto.Phone;
            existingBranch.City = request.BranchDto.City;
            existingBranch.State = request.BranchDto.State;
            existingBranch.IsMain = request.BranchDto.IsMain;
            existingBranch.UpdatedAt = DateTime.UtcNow;

            await _branchRepository.UpdateAsync(existingBranch);
            var branchDto = _mapper.Map<BranchDto>(existingBranch);

            return Result.Success(branchDto);
        }
        catch (Exception ex)
        {
            return Result.Failure<BranchDto>($"Error updating branch: {ex.Message}");
        }
    }
}