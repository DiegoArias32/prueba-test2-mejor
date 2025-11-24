using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.AppointmentTypes.Commands.DeleteAppointmentType;

public record DeleteAppointmentTypeCommand(int Id) : IRequest<Result<bool>>;