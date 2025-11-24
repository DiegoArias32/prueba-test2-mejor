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
        _logger.LogInformation("=== DEBUG: GetMyAssignedAppointments - UserId: {UserId} ===", request.UserId);

        // Obtener los tipos de cita asignados al usuario
        var assignedAppointmentTypeIds = await _assignmentRepository.GetAssignedAppointmentTypeIdsAsync(request.UserId);

        _logger.LogInformation("DEBUG: Assigned AppointmentType IDs: [{TypeIds}] - Count: {Count}",
            string.Join(", ", assignedAppointmentTypeIds),
            assignedAppointmentTypeIds.Count);

        // Si no tiene asignaciones, retornar lista vacía
        if (!assignedAppointmentTypeIds.Any())
        {
            _logger.LogWarning("DEBUG: Usuario {UserId} no tiene tipos de cita asignados", request.UserId);
            return Result.Success(new List<AppointmentDetailDto>());
        }

        // Obtener todas las citas con los datos relacionados
        var appointments = await _appointmentRepository.GetAppointmentsWithDetailsAsync(assignedAppointmentTypeIds);

        _logger.LogInformation("DEBUG: Appointments returned from repository: {Count}", appointments.Count());

        foreach (var apt in appointments.Take(5))
        {
            _logger.LogInformation("DEBUG: Appointment - Id: {Id}, TypeId: {TypeId}, Date: {Date}",
                apt.Id, apt.AppointmentTypeId, apt.AppointmentDate);
        }

        // Mapear a DTOs - el repositorio ya trae los datos completos
        var appointmentDtos = _mapper.Map<List<AppointmentDetailDto>>(appointments);

        _logger.LogInformation("DEBUG: Final DTOs count: {Count}", appointmentDtos.Count);

        return Result.Success(appointmentDtos);
    }
}
