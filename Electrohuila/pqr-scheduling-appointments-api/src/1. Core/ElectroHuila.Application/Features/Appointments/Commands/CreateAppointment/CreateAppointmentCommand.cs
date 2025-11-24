using ElectroHuila.Application.DTOs.Appointments;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Appointments.Commands.CreateAppointment;

public record CreateAppointmentCommand(CreateAppointmentDto AppointmentDto) : IRequest<Result<AppointmentDto>>;