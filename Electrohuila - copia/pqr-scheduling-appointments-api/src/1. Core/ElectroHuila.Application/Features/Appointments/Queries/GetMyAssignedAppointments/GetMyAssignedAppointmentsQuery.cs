using ElectroHuila.Application.DTOs.Appointments;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Appointments.Queries.GetMyAssignedAppointments;

/// <summary>
/// Query para obtener las citas asignadas al usuario actual basadas en sus tipos de cita asignados
/// </summary>
public record GetMyAssignedAppointmentsQuery(int UserId) : IRequest<Result<List<AppointmentDetailDto>>>;
