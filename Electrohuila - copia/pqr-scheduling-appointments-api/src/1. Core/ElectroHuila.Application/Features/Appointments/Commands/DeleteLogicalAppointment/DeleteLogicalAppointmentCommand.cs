using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Appointments.Commands.DeleteLogicalAppointment;

/// <summary>
/// Comando para realizar eliminación lógica de una cita
/// </summary>
public record DeleteLogicalAppointmentCommand(int Id) : IRequest<Result>;
