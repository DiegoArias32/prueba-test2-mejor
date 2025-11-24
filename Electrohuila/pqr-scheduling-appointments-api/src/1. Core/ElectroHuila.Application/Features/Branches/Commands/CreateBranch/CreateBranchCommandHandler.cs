using AutoMapper;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Branches;
using ElectroHuila.Domain.Entities.Locations;
using MediatR;

namespace ElectroHuila.Application.Features.Branches.Commands.CreateBranch;

public class CreateBranchCommandHandler : IRequestHandler<CreateBranchCommand, Result<BranchDto>>
{
    private readonly IBranchRepository _branchRepository;
    private readonly IMapper _mapper;

    public CreateBranchCommandHandler(IBranchRepository branchRepository, IMapper mapper)
    {
        _branchRepository = branchRepository;
        _mapper = mapper;
    }

    public async Task<Result<BranchDto>> Handle(CreateBranchCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var codeExists = await _branchRepository.ExistsByCodeAsync(request.BranchDto.Code);
            if (codeExists)
            {
                return Result.Failure<BranchDto>($"Branch with code '{request.BranchDto.Code}' already exists");
            }

            var branch = new Branch
            {
                Name = request.BranchDto.Name,
                Code = request.BranchDto.Code,
                Address = request.BranchDto.Address,
                Phone = request.BranchDto.Phone,
                City = request.BranchDto.City,
                State = request.BranchDto.State,
                IsMain = request.BranchDto.IsMain,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _branchRepository.AddAsync(branch);
            var branchDto = _mapper.Map<BranchDto>(branch);

            return Result.Success(branchDto);
        }
        catch (Exception ex)
        {
            return Result.Failure<BranchDto>($"Error creating branch: {ex.Message}");
        }
    }
}