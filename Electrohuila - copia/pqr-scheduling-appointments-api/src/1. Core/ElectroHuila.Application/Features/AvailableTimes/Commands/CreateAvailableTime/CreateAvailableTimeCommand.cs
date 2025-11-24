using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.AvailableTimes;
using MediatR;

namespace ElectroHuila.Application.Features.AvailableTimes.Commands.CreateAvailableTime;

public record CreateAvailableTimeCommand(CreateAvailableTimeDto AvailableTimeDto) : IRequest<Result<AvailableTimeDto>>;