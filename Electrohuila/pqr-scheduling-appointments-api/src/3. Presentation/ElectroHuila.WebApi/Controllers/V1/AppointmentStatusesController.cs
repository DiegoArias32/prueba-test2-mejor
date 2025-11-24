using ElectroHuila.Application.Features.Catalogs.AppointmentStatuses.Commands.UpdateAppointmentStatusDesign;
using ElectroHuila.Application.Features.Catalogs.AppointmentStatuses.Queries.GetAllAppointmentStatuses;
using ElectroHuila.Application.Features.Catalogs.AppointmentStatuses.Queries.GetAppointmentStatusById;
using ElectroHuila.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectroHuila.WebApi.Controllers.V1;

/// <summary>
/// Controller para gestionar los estados de citas
/// </summary>
public class AppointmentStatusesController : ApiController
{
    /// <summary>
    /// Obtiene todos los estados de citas activos
    /// </summary>
    /// <returns>Lista de estados ordenados por DisplayOrder</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll()
    {
        var query = new GetAllAppointmentStatusesQuery();
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Obtiene un estado de cita por su ID
    /// </summary>
    /// <param name="id">ID del estado</param>
    /// <returns>Estado encontrado</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new GetAppointmentStatusByIdQuery(id);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Actualiza el diseño (colores e icono) de un estado
    /// </summary>
    /// <param name="id">ID del estado</param>
    /// <param name="command">Datos de diseño a actualizar</param>
    /// <returns>Estado actualizado</returns>
    [HttpPatch("{id}/design")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateDesign(
        int id,
        [FromBody] UpdateAppointmentStatusDesignCommand command)
    {
        if (id != command.Id)
            return BadRequest("El ID de la ruta no coincide con el ID del comando");

        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}
