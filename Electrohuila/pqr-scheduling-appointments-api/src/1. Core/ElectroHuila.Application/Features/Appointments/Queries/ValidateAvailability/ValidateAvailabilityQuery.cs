using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Appointments.Queries.ValidateAvailability;

public record ValidateAvailabilityQuery(DateTime Date, TimeSpan Time, int BranchId) : IRequest<Result<bool>>;
