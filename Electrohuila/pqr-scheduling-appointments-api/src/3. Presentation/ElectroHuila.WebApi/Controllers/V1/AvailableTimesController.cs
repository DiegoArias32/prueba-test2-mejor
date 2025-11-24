using ElectroHuila.Application.DTOs.AvailableTimes;
using ElectroHuila.Application.Features.AvailableTimes.Commands.CreateAvailableTime;
using ElectroHuila.Application.Features.AvailableTimes.Commands.UpdateAvailableTime;
using ElectroHuila.Application.Features.AvailableTimes.Commands.DeleteAvailableTime;
using ElectroHuila.Application.Features.AvailableTimes.Commands.DeleteLogicalAvailableTime;
using ElectroHuila.Application.Features.AvailableTimes.Commands.BulkCreateAvailableTimes;
using ElectroHuila.Application.Features.AvailableTimes.Queries.GetAvailableTimesByBranch;
using ElectroHuila.Application.Features.AvailableTimes.Queries.GetAvailableTimesByAppointmentType;
using ElectroHuila.Application.Features.AvailableTimes.Queries.GetConfiguredTimes;
using ElectroHuila.Application.Features.AvailableTimes.Queries.GetAllIncludingInactive;
using ElectroHuila.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectroHuila.WebApi.Controllers.V1;

/// <summary>
/// Controlador para gestionar los horarios disponibles para citas.
/// Permite configurar, consultar y administrar los horarios de atención
/// por sucursal y tipo de cita. Requiere autenticación JWT.
/// </summary>
[Authorize]
public class AvailableTimesController : ApiController
{

    /// <summary>
    /// Obtiene todos los horarios disponibles incluyendo los inactivos.
    /// </summary>
    /// <returns>Lista de todos los horarios disponibles (activos e inactivos)</returns>
    [HttpGet("all-including-inactive")]
    public async Task<IActionResult> GetAllIncludingInactive()
    {
        var result = await Mediator.Send(new GetAllAvailableTimesIncludingInactiveQuery());
        return HandleResult(result);
    }

    /// <summary>
    /// Obtiene todos los horarios disponibles configurados para una sucursal específica.
    /// </summary>
    /// <param name="branchId">ID de la sucursal (ejemplo: 1)</param>
    /// <returns>Lista de horarios disponibles en la sucursal</returns>
    [HttpGet("branch/{branchId:int}")]
    public async Task<IActionResult> GetByBranch(int branchId)
    {
        var result = await Mediator.Send(new GetAvailableTimesByBranchQuery(branchId));
        return HandleResult(result);
    }

    /// <summary>
    /// Obtiene los horarios disponibles para un tipo de cita específico.
    /// Útil para mostrar horarios según el servicio solicitado.
    /// </summary>
    /// <param name="appointmentTypeId">ID del tipo de cita (ejemplo: 2)</param>
    /// <returns>Lista de horarios disponibles para el tipo de cita</returns>
    [HttpGet("appointment-type/{appointmentTypeId:int}")]
    public async Task<IActionResult> GetByAppointmentType(int appointmentTypeId)
    {
        var result = await Mediator.Send(new GetAvailableTimesByAppointmentTypeQuery(appointmentTypeId));
        return HandleResult(result);
    }

    /// <summary>
    /// Obtiene los horarios configurados para una combinación específica
    /// de sucursal y tipo de cita.
    /// </summary>
    /// <param name="branchId">ID de la sucursal</param>
    /// <param name="appointmentTypeId">ID del tipo de cita</param>
    /// <returns>Horarios configurados para la combinación especificada</returns>
    [HttpGet("configured")]
    public async Task<IActionResult> GetConfiguredTimes([FromQuery] int branchId, [FromQuery] int appointmentTypeId)
    {
        var result = await Mediator.Send(new GetConfiguredTimesQuery(branchId, appointmentTypeId));
        return HandleResult(result);
    }

    /// <summary>
    /// Obtiene todos los horarios configurados para una sucursal específica.
    /// Endpoint alternativo para consultar horarios por sucursal.
    /// </summary>
    /// <param name="branchId">ID de la sucursal (ejemplo: 1)</param>
    /// <returns>Lista de horarios configurados en la sucursal</returns>
    [HttpGet("configured-times/{branchId:int}")]
    public async Task<IActionResult> GetConfiguredTimesByBranch(int branchId)
    {
        var result = await Mediator.Send(new GetAvailableTimesByBranchQuery(branchId));
        return HandleResult(result);
    }

    /// <summary>
    /// Crea un nuevo horario disponible en el sistema.
    /// </summary>
    /// <param name="dto">Datos del horario a crear (sucursal, tipo de cita, hora, días)</param>
    /// <returns>El horario disponible creado</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAvailableTimeDto dto)
    {
        var result = await Mediator.Send(new CreateAvailableTimeCommand(dto));
        return HandleResult(result);
    }

    /// <summary>
    /// Crea múltiples horarios disponibles en una sola operación.
    /// Útil para configurar horarios de trabajo completos.
    /// </summary>
    /// <param name="dto">Lista de horarios a crear en lote</param>
    /// <returns>Lista de horarios creados exitosamente</returns>
    [HttpPost("bulk")]
    public async Task<IActionResult> BulkCreate([FromBody] BulkCreateAvailableTimesDto dto)
    {
        var result = await Mediator.Send(new BulkCreateAvailableTimesCommand(dto));
        return HandleResult(result);
    }

    /// <summary>
    /// Actualiza un horario disponible existente.
    /// Permite modificar horarios, días de atención o configuraciones.
    /// </summary>
    /// <param name="id">ID del horario disponible a actualizar</param>
    /// <param name="dto">Nuevos datos para el horario</param>
    /// <returns>El horario disponible actualizado</returns>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAvailableTimeDto dto)
    {
        var result = await Mediator.Send(new UpdateAvailableTimeCommand(id, dto));
        return HandleResult(result);
    }

    /// <summary>
    /// Elimina un horario disponible del sistema.
    /// Realiza eliminación lógica para preservar historial.
    /// </summary>
    /// <param name="id">ID del horario disponible a eliminar</param>
    /// <returns>Confirmación de eliminación exitosa</returns>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await Mediator.Send(new DeleteAvailableTimeCommand(id));
        return HandleResult(result);
    }

    /// <summary>
    /// Realiza eliminación lógica de un horario disponible.
    /// Marca el horario como inactivo sin eliminarlo físicamente de la base de datos.
    /// </summary>
    /// <param name="id">ID del horario disponible a desactivar</param>
    /// <returns>Confirmación de desactivación exitosa</returns>
    [HttpPatch("delete-logical/{id:int}")]
    public async Task<IActionResult> DeleteLogical(int id)
    {
        var command = new DeleteLogicalAvailableTimeCommand(id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}