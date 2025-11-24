using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Appointments;
using MediatR;

namespace ElectroHuila.Application.Features.Appointments.Queries.GetPendingAppointments;

public record GetPendingAppointmentsQuery : IRequest<Result<IEnumerable<AppointmentDto>>>;
