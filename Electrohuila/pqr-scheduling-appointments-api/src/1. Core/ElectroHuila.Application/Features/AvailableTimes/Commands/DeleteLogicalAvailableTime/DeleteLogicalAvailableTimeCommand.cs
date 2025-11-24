using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.AvailableTimes.Commands.DeleteLogicalAvailableTime;

/// <summary>
/// Comando para realizar eliminación lógica de un horario disponible
/// </summary>
public record DeleteLogicalAvailableTimeCommand(int Id) : IRequest<Result>;
