using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Catalogs;
using MediatR;

namespace ElectroHuila.Application.Features.Catalogs.AppointmentStatuses.Queries.GetAppointmentStatusById;

/// <summary>
/// Query para obtener un estado de cita por ID
/// </summary>
public record GetAppointmentStatusByIdQuery(int Id) : IRequest<Result<AppointmentStatusDto>>;
