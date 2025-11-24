using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Setup;
using MediatR;

namespace ElectroHuila.Application.Features.Setup.Commands.ConfigureSchedule;

public record ConfigureScheduleCommand(ConfigureScheduleDto Dto) : IRequest<Result<object>>;
