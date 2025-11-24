using ElectroHuila.Application.DTOs.Branches;
using ElectroHuila.Application.Features.Branches.Commands.CreateBranch;
using ElectroHuila.Application.Features.Branches.Commands.UpdateBranch;
using ElectroHuila.Application.Features.Branches.Commands.DeleteBranch;
using ElectroHuila.Application.Features.Branches.Commands.DeleteLogicalBranch;
using ElectroHuila.Application.Features.Branches.Queries.GetAllBranches;
using ElectroHuila.Application.Features.Branches.Queries.GetActiveBranches;
using ElectroHuila.Application.Features.Branches.Queries.GetBranchByCode;
using ElectroHuila.Application.Features.Branches.Queries.GetMainBranch;
using ElectroHuila.Application.Features.Branches.Queries.GetAllIncludingInactive;
using ElectroHuila.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectroHuila.WebApi.Controllers.V1;

/// <summary>
/// Controlador para gestionar las sucursales de ElectroHuila.
/// Permite administrar información de sedes, ubicaciones y configuraciones.
/// Requiere autenticación JWT.
/// </summary>
[Authorize]
public class BranchesController : ApiController
{
    /// <summary>
    /// Obtiene todas las sucursales registradas en el sistema.
    /// Incluye sucursales activas e inactivas.
    /// </summary>
    /// <returns>Lista completa de sucursales</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var query = new GetAllBranchesQuery();
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Obtiene únicamente las sucursales activas.
    /// Útil para formularios y selección de sedes disponibles.
    /// </summary>
    /// <returns>Lista de sucursales activas</returns>
    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
    {
        var query = new GetActiveBranchesQuery();
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Obtiene todas las sucursales incluyendo las inactivas.
    /// </summary>
    /// <returns>Lista de todas las sucursales (activas e inactivas)</returns>
    [HttpGet("all-including-inactive")]
    public async Task<IActionResult> GetAllIncludingInactive()
    {
        var query = new GetAllBranchesIncludingInactiveQuery();
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Obtiene la sucursal principal de ElectroHuila.
    /// Sucursal marcada como sede central o principal.
    /// </summary>
    /// <returns>Datos de la sucursal principal</returns>
    [HttpGet("main")]
    public async Task<IActionResult> GetMain()
    {
        var query = new GetMainBranchQuery();
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Busca una sucursal por su código único.
    /// </summary>
    /// <param name="code">Código de la sucursal (ejemplo: "HUI01")</param>
    /// <returns>Datos de la sucursal con el código especificado</returns>
    [HttpGet("by-code/{code}")]
    public async Task<IActionResult> GetByCode(string code)
    {
        var query = new GetBranchByCodeQuery(code);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Crea una nueva sucursal en el sistema.
    /// </summary>
    /// <param name="command">Datos de la sucursal a crear (nombre, dirección, código, etc.)</param>
    /// <returns>La sucursal creada con su ID asignado</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBranchCommand command)
    {
        var result = await Mediator.Send(command);
        return CreatedResult(result);
    }

    /// <summary>
    /// Actualiza completamente una sucursal existente.
    /// Valida que el ID de la URL coincida con el del comando.
    /// </summary>
    /// <param name="id">ID de la sucursal a actualizar</param>
    /// <param name="command">Comando con los nuevos datos de la sucursal</param>
    /// <returns>La sucursal actualizada</returns>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateBranchCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("ID mismatch");
        }

        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Actualiza parcialmente una sucursal existente.
    /// Permite modificar solo los campos especificados.
    /// </summary>
    /// <param name="id">ID de la sucursal a actualizar</param>
    /// <param name="dto">Datos parciales para actualizar</param>
    /// <returns>Mensaje de confirmación de la actualización</returns>
    [HttpPatch("{id:int}")]
    public async Task<IActionResult> UpdateBranch(int id, [FromBody] UpdateBranchDto dto)
    {
        var command = new UpdateBranchCommand(id, dto);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Elimina una sucursal del sistema.
    /// Realiza eliminación lógica para preservar integridad de datos.
    /// </summary>
    /// <param name="id">ID de la sucursal a eliminar</param>
    /// <returns>Confirmación de eliminación exitosa</returns>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var command = new DeleteBranchCommand(id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Realiza eliminación lógica de una sucursal.
    /// Marca la sucursal como inactiva sin eliminarla físicamente de la base de datos.
    /// </summary>
    /// <param name="id">ID de la sucursal a desactivar</param>
    /// <returns>Confirmación de desactivación exitosa</returns>
    [HttpPatch("delete-logical/{id:int}")]
    public async Task<IActionResult> DeleteLogical(int id)
    {
        var command = new DeleteLogicalBranchCommand(id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}