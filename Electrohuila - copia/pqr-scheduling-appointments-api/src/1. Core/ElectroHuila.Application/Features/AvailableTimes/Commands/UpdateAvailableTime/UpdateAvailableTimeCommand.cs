using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.AvailableTimes;
using MediatR;

namespace ElectroHuila.Application.Features.AvailableTimes.Commands.UpdateAvailableTime;

public record UpdateAvailableTimeCommand(int Id, UpdateAvailableTimeDto AvailableTimeDto) : IRequest<Result<AvailableTimeDto>>;