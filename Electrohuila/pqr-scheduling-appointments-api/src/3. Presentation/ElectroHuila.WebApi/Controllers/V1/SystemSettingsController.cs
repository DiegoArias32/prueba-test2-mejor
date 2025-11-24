using ElectroHuila.Application.DTOs.Settings;
using ElectroHuila.Application.Features.Settings.Commands.CreateSystemSetting;
using ElectroHuila.Application.Features.Settings.Commands.UpdateSystemSetting;
using ElectroHuila.Application.Features.Settings.Commands.UpdateSystemSettingValue;
using ElectroHuila.Application.Features.Settings.Queries.GetAllSystemSettings;
using ElectroHuila.Application.Features.Settings.Queries.GetSystemSettingByKey;
using ElectroHuila.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectroHuila.WebApi.Controllers.V1;

/// <summary>
/// Controlador para gestionar las configuraciones del sistema.
/// Permite administrar settings, parámetros y configuraciones globales de la aplicación.
/// Requiere autenticación JWT y permisos de administrador.
/// </summary>
[Authorize]
public class SystemSettingsController : ApiController
{
    /// <summary>
    /// Obtiene todas las configuraciones del sistema.
    /// Incluye configuraciones activas e inactivas.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación para la operación asíncrona</param>
    /// <returns>Lista completa de configuraciones del sistema</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetAllSystemSettingsQuery();
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Obtiene una configuración del sistema por su clave única.
    /// </summary>
    /// <param name="key">Clave de la configuración (ejemplo: "MAX_APPOINTMENT_DURATION")</param>
    /// <param name="cancellationToken">Token de cancelación para la operación asíncrona</param>
    /// <returns>Datos de la configuración con la clave especificada</returns>
    [HttpGet("{key}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetByKey(string key, CancellationToken cancellationToken)
    {
        var query = new GetSystemSettingByKeyQuery(key);
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Crea una nueva configuración del sistema.
    /// Solo administradores pueden crear nuevas configuraciones.
    /// </summary>
    /// <param name="dto">Datos de la configuración a crear (clave, valor, tipo, descripción, etc.)</param>
    /// <param name="cancellationToken">Token de cancelación para la operación asíncrona</param>
    /// <returns>La configuración creada con su ID asignado</returns>
    [HttpPost]
    [Authorize(Roles = "Admin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CreateSystemSettingDto dto, CancellationToken cancellationToken)
    {
        var command = new CreateSystemSettingCommand(dto);
        var result = await Mediator.Send(command, cancellationToken);
        return CreatedResult(result, nameof(GetByKey), new { key = dto.SettingKey });
    }

    /// <summary>
    /// Actualiza completamente una configuración del sistema existente.
    /// </summary>
    /// <param name="id">ID de la configuración a actualizar</param>
    /// <param name="dto">Datos completos de la configuración a actualizar</param>
    /// <param name="cancellationToken">Token de cancelación para la operación asíncrona</param>
    /// <returns>La configuración actualizada</returns>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSystemSettingDto dto, CancellationToken cancellationToken)
    {
        if (id != dto.Id)
        {
            return BadRequest(new { error = "ID mismatch between route and body" });
        }

        var command = new UpdateSystemSettingCommand(dto);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Actualiza únicamente el valor de una configuración existente.
    /// Endpoint optimizado para cambios rápidos de valores sin modificar metadatos.
    /// </summary>
    /// <param name="dto">Clave de la configuración y nuevo valor</param>
    /// <param name="cancellationToken">Token de cancelación para la operación asíncrona</param>
    /// <returns>La configuración actualizada con el nuevo valor</returns>
    [HttpPatch("value")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateValue([FromBody] UpdateSystemSettingValueDto dto, CancellationToken cancellationToken)
    {
        var command = new UpdateSystemSettingValueCommand(dto);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }
}
