using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Appointments;
using MediatR;

namespace ElectroHuila.Application.Features.Appointments.Queries.GetAppointmentsByStatus;

public record GetAppointmentsByStatusQuery(string Status) : IRequest<Result<IEnumerable<AppointmentDto>>>;
