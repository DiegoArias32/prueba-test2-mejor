using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.AppointmentTypes;
using MediatR;

namespace ElectroHuila.Application.Features.AppointmentTypes.Queries.GetActiveAppointmentTypes;

/// <summary>
/// Query to retrieve all active appointment types.
/// Returns only appointment types that are currently active and available for scheduling.
/// </summary>
public record GetActiveAppointmentTypesQuery() : IRequest<Result<IEnumerable<AppointmentTypeDto>>>;