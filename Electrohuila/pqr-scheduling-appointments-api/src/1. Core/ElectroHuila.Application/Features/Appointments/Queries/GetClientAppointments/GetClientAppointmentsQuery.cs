using ElectroHuila.Application.DTOs.Appointments;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Appointments.Queries.GetClientAppointments;

public record GetClientAppointmentsQuery(string ClientNumber) : IRequest<Result<IEnumerable<AppointmentDto>>>;
