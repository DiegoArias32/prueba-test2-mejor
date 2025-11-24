using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Setup;
using MediatR;

namespace ElectroHuila.Application.Features.Setup.Commands.ConfigureInitialData;

public record ConfigureInitialDataCommand(ConfigureInitialDataDto Dto) : IRequest<Result<object>>;
