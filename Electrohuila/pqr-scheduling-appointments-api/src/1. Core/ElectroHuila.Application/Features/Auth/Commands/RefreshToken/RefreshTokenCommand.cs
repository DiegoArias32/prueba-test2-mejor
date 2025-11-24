using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Auth;
using MediatR;

namespace ElectroHuila.Application.Features.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : IRequest<Result<LoginResponseDto>>;
