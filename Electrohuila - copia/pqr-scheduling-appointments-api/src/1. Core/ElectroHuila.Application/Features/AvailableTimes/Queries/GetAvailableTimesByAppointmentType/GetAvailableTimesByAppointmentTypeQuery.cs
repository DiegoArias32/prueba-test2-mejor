using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.AvailableTimes;
using MediatR;

namespace ElectroHuila.Application.Features.AvailableTimes.Queries.GetAvailableTimesByAppointmentType;

public record GetAvailableTimesByAppointmentTypeQuery(int AppointmentTypeId) : IRequest<Result<IEnumerable<AvailableTimeDto>>>;