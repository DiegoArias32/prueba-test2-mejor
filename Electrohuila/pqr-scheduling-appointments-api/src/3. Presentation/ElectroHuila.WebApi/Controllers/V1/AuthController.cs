using ElectroHuila.Application.Features.Auth.Commands.Login;
using ElectroHuila.Application.Features.Auth.Commands.RefreshToken;
using ElectroHuila.Application.Features.Auth.Commands.Logout;
using ElectroHuila.Application.Features.Auth.Queries.ValidateToken;
using ElectroHuila.Application.Features.Auth.Queries.GetUserInfo;
using ElectroHuila.Application.Features.Auth.Queries.GetCurrentUserPermissions;
using ElectroHuila.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectroHuila.WebApi.Controllers.V1;

/// <summary>
/// Controlador de autenticación y autorización.
/// Gestiona login, logout, validación de tokens, refresh tokens y permisos de usuario.
/// </summary>
public class AuthController : ApiController
{
    /// <summary>
    /// Autentica un usuario y genera un token JWT.
    /// </summary>
    /// <param name="command">Comando con credenciales de usuario (username y password).</param>
    /// <returns>Token JWT, refresh token e información del usuario autenticado.</returns>
    /// <response code="200">Login exitoso, retorna token y datos del usuario.</response>
    /// <response code="400">Credenciales inválidas o error de autenticación.</response>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Renueva un token JWT expirado usando un refresh token válido.
    /// </summary>
    /// <param name="command">Comando con el refresh token.</param>
    /// <returns>Nuevo token JWT y refresh token.</returns>
    /// <response code="200">Refresh exitoso, retorna nuevos tokens.</response>
    /// <response code="400">Refresh token inválido o expirado.</response>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Cierra la sesión del usuario autenticado.
    /// </summary>
    /// <param name="command">Comando de logout.</param>
    /// <returns>Confirmación de cierre de sesión exitoso.</returns>
    /// <response code="200">Logout exitoso.</response>
    /// <response code="401">Usuario no autenticado.</response>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] LogoutCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Valida si un token JWT es válido y no ha expirado.
    /// </summary>
    /// <returns>Información de validez del token.</returns>
    /// <response code="200">Token válido, retorna información del token.</response>
    /// <response code="401">Token inválido, expirado o no proporcionado.</response>
    [HttpGet("validate-token")]
    [Authorize]
    public async Task<IActionResult> ValidateToken()
    {
        var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized(new { message = "Token not found" });
        }

        var query = new ValidateTokenQuery(token);
        var result = await Mediator.Send(query);

        if (result.IsFailure)
        {
            return Unauthorized(result.Error);
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Obtiene la información del usuario autenticado actualmente.
    /// </summary>
    /// <returns>Datos del usuario (username, email, roles, etc).</returns>
    /// <response code="200">Información del usuario obtenida exitosamente.</response>
    /// <response code="401">Usuario no autenticado.</response>
    [HttpGet("user-info")]
    [Authorize]
    public async Task<IActionResult> GetUserInfo()
    {
        var query = new GetUserInfoQuery();
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Obtiene los permisos del usuario autenticado actual sobre todos los formularios del sistema.
    /// </summary>
    /// <returns>Lista de permisos (lectura, creación, actualización, eliminación) por formulario.</returns>
    /// <response code="200">Permisos del usuario obtenidos exitosamente.</response>
    /// <response code="401">Usuario no autenticado.</response>
    [HttpGet("permissions")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUserPermissions()
    {
        var query = new GetCurrentUserPermissionsQuery();
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }
}