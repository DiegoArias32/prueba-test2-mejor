using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Appointments.Commands.CancelAppointment;

public record CancelAppointmentCommand(int Id, string CancellationReason) : IRequest<Result>;