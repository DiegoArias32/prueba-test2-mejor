using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Catalogs;
using MediatR;

namespace ElectroHuila.Application.Features.Catalogs.AppointmentStatuses.Queries.GetAllAppointmentStatuses;

/// <summary>
/// Query para obtener todos los estados de citas activos
/// </summary>
public record GetAllAppointmentStatusesQuery : IRequest<Result<IEnumerable<AppointmentStatusDto>>>;
