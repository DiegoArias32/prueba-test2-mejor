using ElectroHuila.Application.DTOs.Notifications;
using ElectroHuila.Application.Features.NotificationTemplates.Commands.CreateNotificationTemplate;
using ElectroHuila.Application.Features.NotificationTemplates.Commands.UpdateNotificationTemplate;
using ElectroHuila.Application.Features.NotificationTemplates.Commands.DeleteNotificationTemplate;
using ElectroHuila.Application.Features.NotificationTemplates.Queries.GetAllNotificationTemplates;
using ElectroHuila.Application.Features.NotificationTemplates.Queries.GetNotificationTemplateById;
using ElectroHuila.Application.Features.NotificationTemplates.Queries.GetNotificationTemplateByCode;
using ElectroHuila.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectroHuila.WebApi.Controllers.V1;

/// <summary>
/// Controlador para gestionar las plantillas de notificaciones.
/// Permite administrar templates para emails, SMS y notificaciones push.
/// Requiere autenticación JWT y permisos de administrador.
/// </summary>
[Authorize]
public class NotificationTemplatesController : ApiController
{

    /// <summary>
    /// Obtiene todas las plantillas de notificación.
    /// Incluye plantillas activas e inactivas.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación para la operación asíncrona</param>
    /// <returns>Lista completa de plantillas de notificación</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetAllNotificationTemplatesQuery();
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Obtiene una plantilla por su ID.
    /// </summary>
    /// <param name="id">ID de la plantilla</param>
    /// <param name="cancellationToken">Token de cancelación para la operación asíncrona</param>
    /// <returns>Datos de la plantilla encontrada</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var query = new GetNotificationTemplateByIdQuery(id);
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Busca una plantilla por su código único.
    /// </summary>
    /// <param name="code">Código de la plantilla (ejemplo: "APPOINTMENT_CREATED")</param>
    /// <param name="cancellationToken">Token de cancelación para la operación asíncrona</param>
    /// <returns>Datos de la plantilla con el código especificado</returns>
    [HttpGet("by-code/{code}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetByCode(string code, CancellationToken cancellationToken)
    {
        var query = new GetNotificationTemplateByCodeQuery(code);
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Crea una nueva plantilla de notificación.
    /// Solo administradores pueden crear plantillas.
    /// </summary>
    /// <param name="dto">Datos de la plantilla a crear</param>
    /// <param name="cancellationToken">Token de cancelación para la operación asíncrona</param>
    /// <returns>La plantilla creada con su ID asignado</returns>
    [HttpPost]
    [Authorize(Roles = "Admin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CreateNotificationTemplateDto dto, CancellationToken cancellationToken)
    {
        var command = new CreateNotificationTemplateCommand(dto);
        var result = await Mediator.Send(command, cancellationToken);
        return CreatedResult(result, nameof(GetById), new { id = result.Data?.Id });
    }

    /// <summary>
    /// Actualiza completamente una plantilla existente.
    /// </summary>
    /// <param name="id">ID de la plantilla a actualizar</param>
    /// <param name="dto">Datos completos de la plantilla</param>
    /// <param name="cancellationToken">Token de cancelación para la operación asíncrona</param>
    /// <returns>La plantilla actualizada</returns>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateNotificationTemplateDto dto, CancellationToken cancellationToken)
    {
        if (id != dto.Id)
        {
            return BadRequest(new { error = "ID mismatch between route and body" });
        }

        var command = new UpdateNotificationTemplateCommand(dto);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Elimina (desactiva) una plantilla del sistema.
    /// Realiza eliminación lógica para preservar integridad de datos.
    /// </summary>
    /// <param name="id">ID de la plantilla a eliminar</param>
    /// <param name="cancellationToken">Token de cancelación para la operación asíncrona</param>
    /// <returns>Confirmación de eliminación exitosa</returns>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var command = new DeleteNotificationTemplateCommand(id);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }
}
