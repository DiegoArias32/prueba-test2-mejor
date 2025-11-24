using AutoMapper;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Assignments;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.UserAssignments.Queries.GetAllAssignments;

/// <summary>
/// Handler para obtener todas las asignaciones del sistema
/// </summary>
public class GetAllAssignmentsQueryHandler : IRequestHandler<GetAllAssignmentsQuery, Result<List<UserAssignmentDto>>>
{
    private readonly IUserAssignmentRepository _assignmentRepository;
    private readonly IMapper _mapper;

    public GetAllAssignmentsQueryHandler(IUserAssignmentRepository assignmentRepository, IMapper mapper)
    {
        _assignmentRepository = assignmentRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<UserAssignmentDto>>> Handle(GetAllAssignmentsQuery request, CancellationToken cancellationToken)
    {
        var assignments = await _assignmentRepository.GetAllAsync();
        var assignmentDtos = _mapper.Map<List<UserAssignmentDto>>(assignments);

        return Result.Success(assignmentDtos);
    }
}
