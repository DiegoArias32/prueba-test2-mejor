using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Appointments.Queries.GetOccupiedTimes;

public record GetOccupiedTimesQuery(DateTime Date, int BranchId) : IRequest<Result<object>>;
