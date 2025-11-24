using ElectroHuila.Application.DTOs.Appointments;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Appointments.Commands.ScheduleAppointment;

public record ScheduleAppointmentCommand(CreateAppointmentDto AppointmentDto) : IRequest<Result<AppointmentDto>>;
