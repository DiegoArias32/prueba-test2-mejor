using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.AvailableTimes;
using MediatR;

namespace ElectroHuila.Application.Features.AvailableTimes.Queries.GetAllIncludingInactive;

/// <summary>
/// Query para obtener todos los horarios disponibles incluyendo los inactivos.
/// </summary>
public record GetAllAvailableTimesIncludingInactiveQuery : IRequest<Result<IEnumerable<AvailableTimeDto>>>;
