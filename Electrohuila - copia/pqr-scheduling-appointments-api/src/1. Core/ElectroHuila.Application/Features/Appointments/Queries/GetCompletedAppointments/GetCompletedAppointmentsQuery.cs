using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Appointments;
using MediatR;

namespace ElectroHuila.Application.Features.Appointments.Queries.GetCompletedAppointments;

public record GetCompletedAppointmentsQuery : IRequest<Result<IEnumerable<AppointmentDto>>>;
