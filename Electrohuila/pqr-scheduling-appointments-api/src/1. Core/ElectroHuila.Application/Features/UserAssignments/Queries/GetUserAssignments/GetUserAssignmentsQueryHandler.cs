using AutoMapper;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Assignments;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.UserAssignments.Queries.GetUserAssignments;

/// <summary>
/// Handler para obtener las asignaciones de un usuario
/// </summary>
public class GetUserAssignmentsQueryHandler : IRequestHandler<GetUserAssignmentsQuery, Result<List<UserAssignmentDto>>>
{
    private readonly IUserAssignmentRepository _assignmentRepository;
    private readonly IMapper _mapper;

    public GetUserAssignmentsQueryHandler(IUserAssignmentRepository assignmentRepository, IMapper mapper)
    {
        _assignmentRepository = assignmentRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<UserAssignmentDto>>> Handle(GetUserAssignmentsQuery request, CancellationToken cancellationToken)
    {
        var assignments = await _assignmentRepository.GetByUserIdAsync(request.UserId);
        var assignmentDtos = _mapper.Map<List<UserAssignmentDto>>(assignments);

        return Result.Success(assignmentDtos);
    }
}
