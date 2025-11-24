using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.AppointmentTypes.Commands.DeleteLogicalAppointmentType;

/// <summary>
/// Comando para realizar eliminación lógica de un tipo de cita
/// </summary>
public record DeleteLogicalAppointmentTypeCommand(int Id) : IRequest<Result>;
