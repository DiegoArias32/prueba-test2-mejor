using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Auth.Commands.Logout;

public record LogoutCommand(string Token) : IRequest<Result<bool>>;
