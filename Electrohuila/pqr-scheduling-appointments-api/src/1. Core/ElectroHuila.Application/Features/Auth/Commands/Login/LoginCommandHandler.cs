using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.DTOs.Auth;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Common.Interfaces.Services.Security;
using MediatR;

namespace ElectroHuila.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponseDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<Result<LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetByUsernameAsync(request.LoginDto.Username);
            if (user == null)
            {
                return Result.Failure<LoginResponseDto>("Invalid credentials");
            }

            if (!user.IsActive)
            {
                return Result.Failure<LoginResponseDto>("User account is inactive");
            }

            // TEMPORAL: Comparación directa de contraseñas (sin BCrypt)
            // TODO: Restaurar verificación BCrypt en producción
            if (user.Password != request.LoginDto.Password)
            {
                return Result.Failure<LoginResponseDto>("Invalid credentials");
            }

            // Verificación BCrypt (comentada temporalmente)
            // if (!_passwordHasher.VerifyPassword(request.LoginDto.Password, user.Password))
            // {
            //     return Result.Failure<LoginResponseDto>("Invalid credentials");
            // }

            // Get user roles and permissions from database
            var roles = user.RolUsers
                .Where(ru => ru.Rol != null)
                .Select(ru => ru.Rol.Name)
                .ToList();

            var permissions = user.RolUsers
                .SelectMany(ru => ru.Rol.RolFormPermis)
                .Select(rfp => new
                {
                    FormCode = rfp.Form.Code,
                    CanRead = rfp.Permission.CanRead,
                    CanCreate = rfp.Permission.CanCreate,
                    CanUpdate = rfp.Permission.CanUpdate,
                    CanDelete = rfp.Permission.CanDelete
                })
                .SelectMany(p => new[]
                {
                    p.CanRead ? $"{p.FormCode.ToLower()}.read" : null,
                    p.CanCreate ? $"{p.FormCode.ToLower()}.create" : null,
                    p.CanUpdate ? $"{p.FormCode.ToLower()}.update" : null,
                    p.CanDelete ? $"{p.FormCode.ToLower()}.delete" : null
                })
                .Where(p => p != null)
                .Distinct()
                .ToList();

            var accessToken = _jwtTokenGenerator.GenerateToken(user, roles, permissions);
            var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

            var response = new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = _jwtTokenGenerator.GetTokenExpiration(accessToken),
                User = new UserDetailsDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    AllowedTabs = user.AllowedTabs
                },
                Roles = roles,
                Permissions = permissions
            };

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            return Result.Failure<LoginResponseDto>($"Error during login: {ex.Message}");
        }
    }
}