using ElectroHuila.Application.Features.Users.Commands.CreateUser;
using ElectroHuila.Application.Features.Users.Commands.UpdateUser;
using ElectroHuila.Application.Features.Users.Commands.DeleteUser;
using ElectroHuila.Application.Features.Users.Commands.DeleteLogicalUser;
using ElectroHuila.Application.Features.Users.Commands.AssignRolesToUser;
using ElectroHuila.Application.Features.Users.Queries.GetAllUsers;
using ElectroHuila.Application.Features.Users.Queries.GetUserById;
using ElectroHuila.Application.Features.Users.Queries.GetUserByUsername;
using ElectroHuila.Application.Features.Users.Queries.GetUserRoles;
using ElectroHuila.Application.Features.Users.Queries.GetUserPermissions;
using ElectroHuila.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectroHuila.WebApi.Controllers.V1;

/// <summary>
/// Controlador para gestionar usuarios del sistema ElectroHuila.
/// Administra usuarios, roles, permisos y autorizaciones.
/// Requiere autenticación JWT para todos los endpoints.
/// </summary>
[Authorize]
public class UsersController : ApiController
{
    /// <summary>
    /// Obtiene todos los usuarios registrados en el sistema.
    /// </summary>
    /// <returns>Lista completa de usuarios</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var query = new GetAllUsersQuery();
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Busca un usuario específico por su identificador único.
    /// </summary>
    /// <param name="id">ID del usuario a buscar (ejemplo: 1)</param>
    /// <returns>Datos completos del usuario encontrado</returns>
    [HttpGet("{id:int}", Name = "GetUserById")]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new GetUserByIdQuery(id);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Busca un usuario por su nombre de usuario único.
    /// </summary>
    /// <param name="username">Nombre de usuario (ejemplo: "admin", "jperez")</param>
    /// <returns>Datos del usuario con el username especificado</returns>
    [HttpGet("by-username/{username}")]
    public async Task<IActionResult> GetByUsername(string username)
    {
        var query = new GetUserByUsernameQuery(username);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Obtiene todos los roles asignados a un usuario específico.
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <returns>Lista de roles del usuario</returns>
    [HttpGet("{userId:int}/roles")]
    public async Task<IActionResult> GetUserRoles(int userId)
    {
        var query = new GetUserRolesQuery(userId);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Obtiene todos los permisos efectivos de un usuario.
    /// Incluye permisos directos y heredados de roles.
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <returns>Lista de permisos efectivos del usuario</returns>
    [HttpGet("{userId:int}/permissions")]
    public async Task<IActionResult> GetUserPermissions(int userId)
    {
        var query = new GetUserPermissionsQuery(userId);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Verifica si un usuario tiene un permiso específico en un formulario.
    /// Funcionalidad pendiente de implementación completa.
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <param name="formCode">Código del formulario</param>
    /// <param name="action">Acción a verificar (crear, leer, actualizar, eliminar)</param>
    /// <returns>Resultado de verificación de permisos</returns>
    [HttpGet("{userId:int}/has-permission")]
    public async Task<IActionResult> HasPermission(int userId, [FromQuery] string formCode, [FromQuery] string action)
    {
        if (string.IsNullOrEmpty(formCode) || string.IsNullOrEmpty(action))
        {
            return BadRequest("formCode and action are required");
        }

        // This will need proper implementation in Application layer
        return Ok(new { hasPermission = false, userId, formCode, action, message = "Permission check pending implementation" });
    }

    /// <summary>
    /// Crea un nuevo usuario en el sistema.
    /// </summary>
    /// <param name="command">Datos del usuario a crear (username, email, nombre, etc.)</param>
    /// <returns>El usuario creado con su ID asignado</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
    {
        var result = await Mediator.Send(command);
        return CreatedResult(result, "GetUserById", new { id = result.Data?.Id });
    }

    /// <summary>
    /// Crea un nuevo usuario incluyendo asignación de roles.
    /// Permite crear usuario y asignar roles en una sola operación.
    /// </summary>
    /// <param name="command">Datos del usuario incluyendo roles a asignar</param>
    /// <returns>El usuario creado con roles asignados</returns>
    [HttpPost("create-with-roles")]
    public async Task<IActionResult> CreateWithRoles([FromBody] CreateUserCommand command)
    {
        // This uses the same command but assumes roles are included in the command
        var result = await Mediator.Send(command);
        return CreatedResult(result, "GetUserById", new { id = result.Data?.Id });
    }

    /// <summary>
    /// Actualiza un usuario existente.
    /// Valida que el ID de la URL coincida con el del comando.
    /// </summary>
    /// <param name="id">ID del usuario a actualizar</param>
    /// <param name="command">Comando con los nuevos datos del usuario</param>
    /// <returns>El usuario actualizado</returns>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("ID mismatch");
        }

        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Actualiza un usuario incluyendo modificación de roles.
    /// Permite actualizar datos del usuario y sus roles asignados.
    /// </summary>
    /// <param name="id">ID del usuario a actualizar</param>
    /// <param name="command">Comando con datos del usuario y roles</param>
    /// <returns>El usuario actualizado con roles modificados</returns>
    [HttpPut("{id:int}/update-with-roles")]
    public async Task<IActionResult> UpdateWithRoles(int id, [FromBody] UpdateUserCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("ID mismatch");
        }

        // This uses the same command but assumes roles are included in the command
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Elimina un usuario del sistema.
    /// Realiza eliminación lógica para preservar integridad de datos.
    /// </summary>
    /// <param name="id">ID del usuario a eliminar</param>
    /// <returns>Confirmación de eliminación exitosa</returns>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var command = new DeleteUserCommand(id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Realiza eliminación lógica de un usuario.
    /// Marca el usuario como inactivo sin eliminarlo físicamente de la base de datos.
    /// </summary>
    /// <param name="id">ID del usuario a desactivar</param>
    /// <returns>Confirmación de desactivación exitosa</returns>
    [HttpPatch("delete-logical/{id:int}")]
    public async Task<IActionResult> DeleteLogical(int id)
    {
        var command = new DeleteLogicalUserCommand(id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Asigna roles específicos a un usuario.
    /// Permite gestionar los roles del usuario de forma independiente.
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <param name="command">Comando con la lista de roles a asignar</param>
    /// <returns>Confirmación de asignación de roles</returns>
    [HttpPost("{userId:int}/roles")]
    public async Task<IActionResult> AssignRoles(int userId, [FromBody] AssignRolesToUserCommand command)
    {
        if (userId != command.UserId)
        {
            return BadRequest("User ID mismatch");
        }

        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}