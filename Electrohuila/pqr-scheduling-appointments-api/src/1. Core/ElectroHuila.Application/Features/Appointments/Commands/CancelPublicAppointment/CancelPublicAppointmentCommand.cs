using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Appointments.Commands.CancelPublicAppointment;

public record CancelPublicAppointmentCommand(string ClientNumber, int AppointmentId, string Reason) : IRequest<Result<bool>>;
