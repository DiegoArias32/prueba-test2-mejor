using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.AppointmentTypes;
using MediatR;

namespace ElectroHuila.Application.Features.AppointmentTypes.Queries.GetAllAppointmentTypes;

/// <summary>
/// Query to retrieve all appointment types.
/// Returns all appointment types regardless of their active status.
/// </summary>
public record GetAllAppointmentTypesQuery() : IRequest<Result<IEnumerable<AppointmentTypeDto>>>;