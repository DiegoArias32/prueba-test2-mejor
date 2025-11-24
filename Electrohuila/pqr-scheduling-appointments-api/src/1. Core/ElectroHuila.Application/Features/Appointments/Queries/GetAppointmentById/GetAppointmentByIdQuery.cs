using ElectroHuila.Application.DTOs.Appointments;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Appointments.Queries.GetAppointmentById;

public record GetAppointmentByIdQuery(int Id) : IRequest<Result<AppointmentDto>>;