using ElectroHuila.Application.DTOs.AvailableTimes;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.AvailableTimes.Queries.GetConfiguredTimesByBranch;

public record GetConfiguredTimesByBranchQuery(int BranchId) : IRequest<Result<IEnumerable<AvailableTimeDto>>>;
