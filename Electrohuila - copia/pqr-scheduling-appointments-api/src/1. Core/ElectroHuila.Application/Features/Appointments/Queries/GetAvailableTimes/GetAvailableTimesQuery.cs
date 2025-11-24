using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Appointments.Queries.GetAvailableTimes;

public record GetAvailableTimesQuery(DateTime Date, int BranchId) : IRequest<Result<IEnumerable<string>>>;
