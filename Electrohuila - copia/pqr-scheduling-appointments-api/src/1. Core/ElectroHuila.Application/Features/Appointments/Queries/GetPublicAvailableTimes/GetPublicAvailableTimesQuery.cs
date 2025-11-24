using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Appointments.Queries.GetPublicAvailableTimes;

public record GetPublicAvailableTimesQuery(DateTime Date, int BranchId) : IRequest<Result<IEnumerable<string>>>;
