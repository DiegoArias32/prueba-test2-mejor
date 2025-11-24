using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Setup.Commands.InitializeData;

public record InitializeDataCommand() : IRequest<Result<object>>;
