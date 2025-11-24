using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.AvailableTimes;
using MediatR;

namespace ElectroHuila.Application.Features.AvailableTimes.Queries.GetAvailableTimesByBranch;

public record GetAvailableTimesByBranchQuery(int BranchId) : IRequest<Result<IEnumerable<AvailableTimeDto>>>;