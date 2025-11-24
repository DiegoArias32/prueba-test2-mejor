using ElectroHuila.Application.DTOs.Appointments;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Appointments.Commands.UpdateAppointment;

public record UpdateAppointmentCommand(int Id, UpdateAppointmentDto AppointmentDto) : IRequest<Result<AppointmentDto>>;