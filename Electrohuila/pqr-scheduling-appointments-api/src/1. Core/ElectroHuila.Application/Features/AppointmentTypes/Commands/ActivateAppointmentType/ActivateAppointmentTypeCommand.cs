using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.AppointmentTypes.Commands.ActivateAppointmentType;

/// <summary>
/// Comando para activar (reactivar) un tipo de cita que fue desactivado previamente
/// </summary>
public record ActivateAppointmentTypeCommand(int Id) : IRequest<Result>;
