using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.AppointmentTypes;
using MediatR;

namespace ElectroHuila.Application.Features.AppointmentTypes.Queries.GetAllIncludingInactive;

/// <summary>
/// Query para obtener todos los tipos de citas incluyendo los inactivos.
/// </summary>
public record GetAllAppointmentTypesIncludingInactiveQuery : IRequest<Result<IEnumerable<AppointmentTypeDto>>>;
