using ElectroHuila.Application.DTOs.Settings;
using ElectroHuila.Application.Features.Settings.Queries.GetActiveTheme;
using ElectroHuila.Application.Features.Settings.Commands.UpdateThemeSettings;
using ElectroHuila.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectroHuila.WebApi.Controllers.V1;

/// <summary>
/// Controlador para gestionar la configuración de tema y colores de la aplicación.
/// Permite obtener y actualizar los colores personalizables del sistema.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class ThemeController : ApiController
{
    /// <summary>
    /// Obtiene la configuración del tema activo de la aplicación.
    /// Este endpoint es público para permitir que el frontend obtenga los colores sin autenticación.
    /// </summary>
    /// <returns>Configuración completa del tema incluyendo colores principales, estados, fondos, etc.</returns>
    [HttpGet("active")]
    [AllowAnonymous]
    public async Task<IActionResult> GetActiveTheme()
    {
        var query = new GetActiveThemeQuery();
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Actualiza la configuración del tema activo.
    /// Requiere autenticación de administrador.
    /// </summary>
    /// <param name="id">ID del tema a actualizar</param>
    /// <param name="dto">Datos del tema a actualizar (solo se actualizan los campos proporcionados)</param>
    /// <returns>Tema actualizado con todos los colores</returns>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> UpdateTheme(int id, [FromBody] UpdateThemeSettingsDto dto)
    {
        var command = new UpdateThemeSettingsCommand(id, dto);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Actualiza parcialmente el tema activo.
    /// Permite modificar solo algunos colores sin enviar toda la configuración.
    /// Requiere autenticación de administrador.
    /// </summary>
    /// <param name="id">ID del tema a actualizar</param>
    /// <param name="dto">Datos parciales del tema a actualizar</param>
    /// <returns>Confirmación de actualización exitosa</returns>
    [HttpPatch("{id:int}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> PatchTheme(int id, [FromBody] UpdateThemeSettingsDto dto)
    {
        var command = new UpdateThemeSettingsCommand(id, dto);
        var result = await Mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(new { success = true, message = "Tema actualizado exitosamente", data = result.Data });
    }
}
