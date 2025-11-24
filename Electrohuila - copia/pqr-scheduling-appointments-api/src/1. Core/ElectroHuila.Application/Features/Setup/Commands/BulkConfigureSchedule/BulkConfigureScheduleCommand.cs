using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Setup;
using MediatR;

namespace ElectroHuila.Application.Features.Setup.Commands.BulkConfigureSchedule;

public record BulkConfigureScheduleCommand(List<ConfigureScheduleDto> Configurations) : IRequest<Result<object>>;
