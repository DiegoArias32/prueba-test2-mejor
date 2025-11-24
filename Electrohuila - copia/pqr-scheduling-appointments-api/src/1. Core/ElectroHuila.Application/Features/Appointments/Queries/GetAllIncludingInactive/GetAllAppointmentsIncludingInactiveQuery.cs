using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Appointments;
using MediatR;

namespace ElectroHuila.Application.Features.Appointments.Queries.GetAllIncludingInactive;

/// <summary>
/// Query para obtener todas las citas incluyendo las inactivas.
/// </summary>
public record GetAllAppointmentsIncludingInactiveQuery : IRequest<Result<List<AppointmentDto>>>;
