using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Permissions;
using MediatR;

namespace ElectroHuila.Application.Features.Permissions.Commands.UpdateUserTabs;

public record UpdateUserTabsCommand(UpdateUserTabsDto Dto) : IRequest<Result<bool>>;