using ElectroHuila.Application.DTOs.Auth;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Auth.Commands.Login;

public record LoginCommand(LoginDto LoginDto) : IRequest<Result<LoginResponseDto>>;