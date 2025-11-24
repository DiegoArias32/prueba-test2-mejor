using ElectroHuila.Application.DTOs.Appointments;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Appointments.Commands.CompleteAppointment;

public record CompleteAppointmentCommand(int AppointmentId, string? Notes) : IRequest<Result<AppointmentDto>>;
