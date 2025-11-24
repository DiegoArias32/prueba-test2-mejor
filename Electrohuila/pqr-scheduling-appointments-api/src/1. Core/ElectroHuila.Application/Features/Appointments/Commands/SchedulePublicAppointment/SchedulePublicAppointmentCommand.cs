using ElectroHuila.Application.DTOs.Appointments;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Appointments.Commands.SchedulePublicAppointment;

public record SchedulePublicAppointmentCommand(SchedulePublicAppointmentDto AppointmentDto) : IRequest<Result<AppointmentDto>>;
