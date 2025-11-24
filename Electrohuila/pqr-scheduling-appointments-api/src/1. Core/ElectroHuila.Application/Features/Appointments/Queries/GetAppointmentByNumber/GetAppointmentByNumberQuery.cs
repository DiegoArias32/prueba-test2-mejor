using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Appointments;
using MediatR;

namespace ElectroHuila.Application.Features.Appointments.Queries.GetAppointmentByNumber;

public record GetAppointmentByNumberQuery(string AppointmentNumber) : IRequest<Result<AppointmentDto>>;
