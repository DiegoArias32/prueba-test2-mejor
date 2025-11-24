using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.AvailableTimes;
using MediatR;

namespace ElectroHuila.Application.Features.AvailableTimes.Commands.BulkCreateAvailableTimes;

public record BulkCreateAvailableTimesCommand(BulkCreateAvailableTimesDto Dto) : IRequest<Result<List<AvailableTimeDto>>>;