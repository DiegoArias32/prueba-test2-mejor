using ElectroHuila.Application.DTOs.Roles;
using ElectroHuila.Application.Features.Roles.Commands.CreateRol;
using ElectroHuila.Application.Features.Roles.Commands.UpdateRol;
using ElectroHuila.Application.Features.Roles.Commands.DeleteRol;
using ElectroHuila.Application.Features.Roles.Commands.DeleteLogicalRol;
using ElectroHuila.Application.Features.Roles.Queries.GetAllRoles;
using ElectroHuila.Application.Features.Roles.Queries.GetRolById;
using ElectroHuila.Application.Features.Roles.Queries.GetRolByCode;
using ElectroHuila.Application.Features.Roles.Queries.GetRolesByUser;
using ElectroHuila.Application.Features.Roles.Queries.GetAllIncludingInactive;
using ElectroHuila.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectroHuila.WebApi.Controllers.V1;

/// <summary>
/// Controlador para gestionar roles y perfiles de usuario del sistema.
/// Permite administrar roles, asignaciones y permisos asociados.
/// Requiere autenticación JWT para todos los endpoints.
/// </summary>
[Authorize]
public class RolesController : ApiController
{

    /// <summary>
    /// Obtiene todos los roles registrados en el sistema.
    /// Incluye roles activos e inactivos para administración completa.
    /// </summary>
    /// <returns>Lista completa de roles del sistema</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await Mediator.Send(new GetAllRolesQuery());
        return HandleResult(result);
    }

    /// <summary>
    /// Obtiene todos los roles incluyendo los inactivos.
    /// </summary>
    /// <returns>Lista de todos los roles (activos e inactivos)</returns>
    [HttpGet("all-including-inactive")]
    public async Task<IActionResult> GetAllIncludingInactive()
    {
        var result = await Mediator.Send(new GetAllRolesIncludingInactiveQuery());
        return HandleResult(result);
    }

    /// <summary>
    /// Busca un rol específico por su identificador único.
    /// </summary>
    /// <param name="id">ID del rol a buscar (ejemplo: 1)</param>
    /// <returns>Datos completos del rol encontrado</returns>
    [HttpGet("{id:int}", Name = "GetRolById")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await Mediator.Send(new GetRolByIdQuery(id));
        return HandleResult(result);
    }

    /// <summary>
    /// Busca un rol por su código único.
    /// Útil para identificación de roles por nombre clave.
    /// </summary>
    /// <param name="code">Código del rol (ejemplo: "ADMIN", "USER")</param>
    /// <returns>Datos del rol con el código especificado</returns>
    [HttpGet("by-code/{code}")]
    public async Task<IActionResult> GetByCode(string code)
    {
        var result = await Mediator.Send(new GetRolByCodeQuery(code));
        return HandleResult(result);
    }

    /// <summary>
    /// Obtiene todos los roles asignados a un usuario específico.
    /// Útil para determinar permisos y capacidades del usuario.
    /// </summary>
    /// <param name="userId">ID del usuario (ejemplo: 1)</param>
    /// <returns>Lista de roles asignados al usuario</returns>
    [HttpGet("by-user/{userId:int}")]
    public async Task<IActionResult> GetByUser(int userId)
    {
        var result = await Mediator.Send(new GetRolesByUserQuery(userId));
        return HandleResult(result);
    }

    /// <summary>
    /// Crea un nuevo rol en el sistema.
    /// Define un nuevo perfil de usuario con permisos específicos.
    /// </summary>
    /// <param name="dto">Datos del rol a crear (nombre, código, descripción, permisos)</param>
    /// <returns>El rol creado con su ID asignado</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRolDto dto)
    {
        var result = await Mediator.Send(new CreateRolCommand(dto));
        return CreatedResult(result, "GetRolById", new { id = result.Data?.Id });
    }

    /// <summary>
    /// Actualiza un rol existente en el sistema.
    /// Permite modificar permisos y configuraciones del rol.
    /// </summary>
    /// <param name="id">ID del rol a actualizar</param>
    /// <param name="dto">Nuevos datos para el rol</param>
    /// <returns>El rol actualizado</returns>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRolDto dto)
    {
        var result = await Mediator.Send(new UpdateRolCommand(id, dto));
        return HandleResult(result);
    }

    /// <summary>
    /// Elimina permanentemente un rol del sistema.
    /// Eliminación física que remueve completamente el rol.
    /// </summary>
    /// <param name="id">ID del rol a eliminar</param>
    /// <returns>Confirmación de eliminación exitosa</returns>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await Mediator.Send(new DeleteRolCommand(id));
        return HandleResult(result);
    }

    /// <summary>
    /// Elimina lógicamente un rol del sistema.
    /// Desactiva el rol preservando integridad de datos y auditoría.
    /// </summary>
    /// <param name="id">ID del rol a eliminar lógicamente</param>
    /// <returns>Mensaje de confirmación de eliminación lógica</returns>
    [HttpPatch("delete-logical/{id:int}")]
    public async Task<IActionResult> DeleteLogical(int id)
    {
        var result = await Mediator.Send(new DeleteLogicalRolCommand(id));
        return HandleResult(result);
    }
}