using ElectroHuila.Application.DTOs.AppointmentTypes;
using ElectroHuila.Application.Features.AppointmentTypes.Commands.CreateAppointmentType;
using ElectroHuila.Application.Features.AppointmentTypes.Commands.UpdateAppointmentType;
using ElectroHuila.Application.Features.AppointmentTypes.Commands.DeleteAppointmentType;
using ElectroHuila.Application.Features.AppointmentTypes.Commands.DeleteLogicalAppointmentType;
using ElectroHuila.Application.Features.AppointmentTypes.Commands.ActivateAppointmentType;
using ElectroHuila.Application.Features.AppointmentTypes.Queries.GetAllAppointmentTypes;
using ElectroHuila.Application.Features.AppointmentTypes.Queries.GetActiveAppointmentTypes;
using ElectroHuila.Application.Features.AppointmentTypes.Queries.GetAppointmentTypeById;
using ElectroHuila.Application.Features.AppointmentTypes.Queries.GetAppointmentTypeByName;
using ElectroHuila.Application.Features.AppointmentTypes.Queries.GetAllIncludingInactive;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectroHuila.WebApi.Controllers.V1;

/// <summary>
/// Controlador para gestionar los tipos de citas disponibles en el sistema.
/// Permite crear, consultar, actualizar y eliminar tipos de citas.
/// Requiere autenticación JWT.
/// </summary>
[Authorize]
public class AppointmentTypesController : ApiController
{
    private readonly IAppointmentTypeRepository _appointmentTypeRepository;

    public AppointmentTypesController(IAppointmentTypeRepository appointmentTypeRepository)
    {
        _appointmentTypeRepository = appointmentTypeRepository;
    }

    /// <summary>
    /// Obtiene todos los tipos de citas registrados en el sistema.
    /// Incluye tipos activos e inactivos.
    /// </summary>
    /// <returns>Lista completa de tipos de citas</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await Mediator.Send(new GetAllAppointmentTypesQuery());
        return HandleResult(result);
    }

    /// <summary>
    /// Obtiene únicamente los tipos de citas activos.
    /// Útil para formularios y selección de tipos disponibles.
    /// </summary>
    /// <returns>Lista de tipos de citas activos</returns>
    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
    {
        var result = await Mediator.Send(new GetActiveAppointmentTypesQuery());
        return HandleResult(result);
    }

    /// <summary>
    /// Obtiene todos los tipos de citas incluyendo los inactivos.
    /// </summary>
    /// <returns>Lista de todos los tipos de citas (activos e inactivos)</returns>
    [HttpGet("all-including-inactive")]
    public async Task<IActionResult> GetAllIncludingInactive()
    {
        var result = await Mediator.Send(new GetAllAppointmentTypesIncludingInactiveQuery());
        return HandleResult(result);
    }

    /// <summary>
    /// Busca un tipo de cita específico por su identificador único.
    /// </summary>
    /// <param name="id">ID del tipo de cita a buscar (ejemplo: 1)</param>
    /// <returns>Datos del tipo de cita encontrado</returns>
    [HttpGet("{id:int}", Name = "GetAppointmentTypeById")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await Mediator.Send(new GetAppointmentTypeByIdQuery(id));
        return HandleResult(result);
    }

    /// <summary>
    /// Busca un tipo de cita por su nombre exacto.
    /// </summary>
    /// <param name="name">Nombre del tipo de cita (ejemplo: "Consulta General")</param>
    /// <returns>Datos del tipo de cita con el nombre especificado</returns>
    [HttpGet("by-name/{name}")]
    public async Task<IActionResult> GetByName(string name)
    {
        var result = await Mediator.Send(new GetAppointmentTypeByNameQuery(name));
        return HandleResult(result);
    }

    /// <summary>
    /// Crea un nuevo tipo de cita en el sistema.
    /// </summary>
    /// <param name="dto">Datos del tipo de cita a crear (nombre, descripción, duración, etc.)</param>
    /// <returns>El tipo de cita creado con su ID asignado</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAppointmentTypeDto dto)
    {
        var result = await Mediator.Send(new CreateAppointmentTypeCommand(dto));
        return CreatedResult(result, "GetAppointmentTypeById", new { id = result.Data?.Id });
    }

    /// <summary>
    /// Actualiza completamente un tipo de cita existente.
    /// Reemplaza todos los campos del tipo de cita.
    /// </summary>
    /// <param name="id">ID del tipo de cita a actualizar</param>
    /// <param name="dto">Nuevos datos para el tipo de cita</param>
    /// <returns>El tipo de cita actualizado</returns>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAppointmentTypeDto dto)
    {
        var result = await Mediator.Send(new UpdateAppointmentTypeCommand(id, dto));
        return HandleResult(result);
    }

    /// <summary>
    /// Actualiza parcialmente un tipo de cita existente.
    /// Permite modificar solo los campos especificados.
    /// </summary>
    /// <param name="id">ID del tipo de cita a actualizar</param>
    /// <param name="dto">Datos parciales para actualizar</param>
    /// <returns>Mensaje de confirmación de la actualización</returns>
    [HttpPatch("{id:int}")]
    public async Task<IActionResult> UpdateAppointmentType(int id, [FromBody] UpdateAppointmentTypeDto dto)
    {
        var result = await Mediator.Send(new UpdateAppointmentTypeCommand(id, dto));
        return HandleResult(result);
    }

    /// <summary>
    /// Elimina un tipo de cita del sistema.
    /// Realiza eliminación lógica para preservar integridad de datos.
    /// </summary>
    /// <param name="id">ID del tipo de cita a eliminar</param>
    /// <returns>Confirmación de eliminación exitosa</returns>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await Mediator.Send(new DeleteAppointmentTypeCommand(id));
        return HandleResult(result);
    }

    /// <summary>
    /// Realiza eliminación lógica de un tipo de cita.
    /// Marca el tipo de cita como inactivo sin eliminarlo físicamente de la base de datos.
    /// </summary>
    /// <param name="id">ID del tipo de cita a desactivar</param>
    /// <returns>Confirmación de desactivación exitosa</returns>
    [HttpPatch("delete-logical/{id:int}")]
    public async Task<IActionResult> DeleteLogical(int id)
    {
        var command = new DeleteLogicalAppointmentTypeCommand(id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Activa un tipo de cita que fue previamente desactivado.
    /// Marca el tipo de cita como activo nuevamente.
    /// </summary>
    /// <param name="id">ID del tipo de cita a activar</param>
    /// <returns>Confirmación de activación exitosa</returns>
    [HttpPatch("{id:int}/activate")]
    public async Task<IActionResult> Activate(int id)
    {
        var appointmentType = await _appointmentTypeRepository.GetByIdAsync(id);
        if (appointmentType == null)
            return NotFound(new { message = $"AppointmentType with ID {id} not found" });

        appointmentType.IsActive = true;
        appointmentType.UpdatedAt = DateTime.UtcNow;
        await _appointmentTypeRepository.UpdateAsync(appointmentType);

        return Ok(new { success = true, message = "AppointmentType activated successfully" });
    }

    /// <summary>
    /// Desactiva un tipo de cita del sistema.
    /// Marca el tipo de cita como inactivo sin eliminarlo físicamente.
    /// </summary>
    /// <param name="id">ID del tipo de cita a desactivar</param>
    /// <returns>Confirmación de desactivación exitosa</returns>
    [HttpPatch("{id:int}/deactivate")]
    public async Task<IActionResult> Deactivate(int id)
    {
        var appointmentType = await _appointmentTypeRepository.GetByIdAsync(id);
        if (appointmentType == null)
            return NotFound(new { message = $"AppointmentType with ID {id} not found" });

        appointmentType.IsActive = false;
        appointmentType.UpdatedAt = DateTime.UtcNow;
        await _appointmentTypeRepository.UpdateAsync(appointmentType);

        return Ok(new { success = true, message = "AppointmentType deactivated successfully" });
    }
}