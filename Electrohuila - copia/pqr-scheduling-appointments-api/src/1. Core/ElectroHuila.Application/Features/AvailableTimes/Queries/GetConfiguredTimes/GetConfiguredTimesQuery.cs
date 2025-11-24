using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.AvailableTimes;
using MediatR;

namespace ElectroHuila.Application.Features.AvailableTimes.Queries.GetConfiguredTimes;

public record GetConfiguredTimesQuery(int BranchId, int? AppointmentTypeId = null) : IRequest<Result<IEnumerable<AvailableTimeDto>>>;