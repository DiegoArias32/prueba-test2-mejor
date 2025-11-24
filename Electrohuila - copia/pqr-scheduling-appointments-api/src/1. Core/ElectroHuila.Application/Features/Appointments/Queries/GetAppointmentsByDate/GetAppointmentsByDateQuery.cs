using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Appointments;
using MediatR;

namespace ElectroHuila.Application.Features.Appointments.Queries.GetAppointmentsByDate;

public record GetAppointmentsByDateQuery(DateTime Date) : IRequest<Result<IEnumerable<AppointmentDto>>>;
