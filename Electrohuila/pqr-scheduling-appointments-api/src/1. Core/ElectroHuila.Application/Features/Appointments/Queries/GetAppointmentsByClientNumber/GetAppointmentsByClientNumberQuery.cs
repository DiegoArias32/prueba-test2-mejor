using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Appointments;
using MediatR;

namespace ElectroHuila.Application.Features.Appointments.Queries.GetAppointmentsByClientNumber;

public record GetAppointmentsByClientNumberQuery(string ClientNumber) : IRequest<Result<IEnumerable<AppointmentDto>>>;
