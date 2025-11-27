using AutoMapper;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Appointments;
using ElectroHuila.Application.Common.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ElectroHuila.Application.Features.Appointments.Queries.GetMyAssignedAppointments;

/// <summary>
/// Handler para obtener las citas asignadas al usuario actual
/// Filtra las citas según los tipos de cita que tiene asignados el usuario
/// </summary>
public class GetMyAssignedAppointmentsQueryHandler : IRequestHandler<GetMyAssignedAppointmentsQuery, Result<List<AppointmentDetailDto>>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IUserAssignmentRepository _assignmentRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetMyAssignedAppointmentsQueryHandler> _logger;

    public GetMyAssignedAppointmentsQueryHandler(
        IAppointmentRepository appointmentRepository,
        IUserAssignmentRepository assignmentRepository,
        IMapper mapper,
        ILogger<GetMyAssignedAppointmentsQueryHandler> logger)
    {
        _appointmentRepository = appointmentRepository;
        _assignmentRepository = assignmentRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<List<AppointmentDetailDto>>> Handle(GetMyAssignedAppointmentsQuery request, CancellationToken cancellationToken)
    {
        // Obtener los tipos de cita asignados al usuario
        var assignedAppointmentTypeIds = await _assignmentRepository.GetAssignedAppointmentTypeIdsAsync(request.UserId);

        // Si no tiene asignaciones, retornar lista vacía
        if (!assignedAppointmentTypeIds.Any())
        {
            return Result.Success(new List<AppointmentDetailDto>());
        }

        // Obtener todas las citas con los datos relacionados
        var appointments = await _appointmentRepository.GetAppointmentsWithDetailsAsync(assignedAppointmentTypeIds);

        // Mapear a DTOs - el repositorio ya trae los datos completos
        var appointmentDtos = _mapper.Map<List<AppointmentDetailDto>>(appointments);

        return Result.Success(appointmentDtos);
    }
}
