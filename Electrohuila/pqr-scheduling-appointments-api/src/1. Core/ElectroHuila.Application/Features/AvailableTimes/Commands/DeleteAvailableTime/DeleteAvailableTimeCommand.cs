using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.AvailableTimes.Commands.DeleteAvailableTime;

public record DeleteAvailableTimeCommand(int Id) : IRequest<Result<bool>>;