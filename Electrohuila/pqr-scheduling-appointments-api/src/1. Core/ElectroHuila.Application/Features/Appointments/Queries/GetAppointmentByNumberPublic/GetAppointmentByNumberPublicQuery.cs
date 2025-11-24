using ElectroHuila.Application.DTOs.Appointments;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Appointments.Queries.GetAppointmentByNumberPublic;

public record GetAppointmentByNumberPublicQuery(string AppointmentNumber, string ClientNumber) : IRequest<Result<AppointmentDto>>;
