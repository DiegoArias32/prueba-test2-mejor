using ElectroHuila.Application.DTOs.Permissions;
using ElectroHuila.Application.Features.Permissions.Commands.CreatePermission;
using ElectroHuila.Application.Features.Permissions.Commands.AssignPermissionToRol;
using ElectroHuila.Application.Features.Permissions.Commands.RemovePermissionFromRol;
using ElectroHuila.Application.Features.Permissions.Commands.UpdateUserTabs;
using ElectroHuila.Application.Features.Permissions.Commands.UpdateRolFormPermission;
using ElectroHuila.Application.Features.Permissions.Queries.GetAllPermissions;
using ElectroHuila.Application.Features.Permissions.Queries.GetRolPermissionsSummary;
using ElectroHuila.Application.Features.Permissions.Queries.GetAllRolPermissionsSummary;
using ElectroHuila.Application.Features.Permissions.Queries.GetRolFormPermissions;
using ElectroHuila.Application.Features.Permissions.Queries.GetRolFormPermissionsAssignments;
using ElectroHuila.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectroHuila.WebApi.Controllers.V1;

/// <summary>
/// Controlador para gestionar permisos y autorizaciones del sistema.
/// Administra permisos por roles, formularios y pestañas de usuario.
/// Requiere autenticación JWT para todos los endpoints.
/// </summary>
[Authorize]
public class PermissionsController : ApiController
{

    /// <summary>
    /// Obtiene todos los permisos disponibles en el sistema.
    /// Lista completa de permisos configurables.
    /// </summary>
    /// <returns>Lista de todos los permisos del sistema</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await Mediator.Send(new GetAllPermissionsQuery());
        return HandleResult(result);
    }

    /// <summary>
    /// Obtiene un resumen de permisos para todos los roles del sistema.
    /// Vista general de la matriz de permisos por rol.
    /// </summary>
    /// <returns>Resumen completo de permisos por cada rol</returns>
    [HttpGet("roles-permissions-summary")]
    public async Task<IActionResult> GetAllRolPermissionsSummary()
    {
        var result = await Mediator.Send(new GetAllRolPermissionsSummaryQuery());
        return HandleResult(result);
    }

    /// <summary>
    /// Obtiene el resumen de permisos para un rol específico.
    /// Muestra todos los permisos asignados al rol.
    /// </summary>
    /// <param name="rolId">ID del rol a consultar (ejemplo: 1)</param>
    /// <returns>Resumen de permisos del rol especificado</returns>
    [HttpGet("rol/{rolId:int}/summary")]
    public async Task<IActionResult> GetRolSummary(int rolId)
    {
        var result = await Mediator.Send(new GetRolPermissionsSummaryQuery(rolId));
        return HandleResult(result);
    }

    /// <summary>
    /// Obtiene las asignaciones de permisos entre roles y formularios.
    /// Permite filtrar por rol específico o formulario específico.
    /// </summary>
    /// <param name="rolId">ID del rol para filtrar (opcional)</param>
    /// <param name="formId">ID del formulario para filtrar (opcional)</param>
    /// <returns>Lista de asignaciones de permisos rol-formulario</returns>
    [HttpGet("assignments")]
    public async Task<IActionResult> GetRolFormPermissionsAssignments([FromQuery] int? rolId = null, [FromQuery] int? formId = null)
    {
        var result = await Mediator.Send(new GetRolFormPermissionsAssignmentsQuery(rolId, formId));
        return HandleResult(result);
    }

    /// <summary>
    /// Obtiene los permisos específicos de un rol en un formulario particular.
    /// Detalla qué acciones puede realizar el rol en el formulario.
    /// </summary>
    /// <param name="rolId">ID del rol</param>
    /// <param name="formId">ID del formulario</param>
    /// <returns>Permisos específicos del rol en el formulario</returns>
    [HttpGet("rol/{rolId:int}/form/{formId:int}")]
    public async Task<IActionResult> GetRolFormPermissions(int rolId, int formId)
    {
        var result = await Mediator.Send(new GetRolFormPermissionsQuery(rolId, formId));
        return HandleResult(result);
    }

    /// <summary>
    /// Crea un nuevo permiso en el sistema.
    /// Define una nueva acción que puede ser asignada a roles.
    /// </summary>
    /// <param name="dto">Datos del permiso a crear (nombre, descripción, módulo)</param>
    /// <returns>El permiso creado</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePermissionDto dto)
    {
        var result = await Mediator.Send(new CreatePermissionCommand(dto));
        return HandleResult(result);
    }

    /// <summary>
    /// Asigna un permiso existente a un rol específico.
    /// Otorga nuevas capacidades a un rol del sistema.
    /// </summary>
    /// <param name="dto">Datos de asignación (rol, permiso, formulario)</param>
    /// <returns>Confirmación de la asignación</returns>
    [HttpPost("assign-to-rol")]
    public async Task<IActionResult> AssignToRol([FromBody] AssignPermissionToRolDto dto)
    {
        var result = await Mediator.Send(new AssignPermissionToRolCommand(dto));
        return HandleResult(result);
    }

    /// <summary>
    /// Remueve un permiso de un rol específico.
    /// Revoca capacidades previamente otorgadas a un rol.
    /// </summary>
    /// <param name="dto">Datos de remoción (rol, permiso, formulario)</param>
    /// <returns>Confirmación de la remoción</returns>
    [HttpPost("remove-from-rol")]
    public async Task<IActionResult> RemoveFromRol([FromBody] RemovePermissionFromRolDto dto)
    {
        var result = await Mediator.Send(new RemovePermissionFromRolCommand(dto));
        return HandleResult(result);
    }

    /// <summary>
    /// Actualiza los permisos de un rol en un formulario específico.
    /// Modifica las capacidades existentes del rol en el formulario.
    /// </summary>
    /// <param name="dto">Nuevos datos de permisos para el rol-formulario</param>
    /// <returns>Mensaje de confirmación de actualización</returns>
    [HttpPut("update")]
    public async Task<IActionResult> UpdateRolFormPermission([FromBody] UpdateRolFormPermissionDto dto)
    {
        var result = await Mediator.Send(new UpdateRolFormPermissionCommand(dto));
        return HandleResult(result);
    }

    /// <summary>
    /// Actualiza las pestañas disponibles para un usuario.
    /// Configura qué secciones de la interfaz puede acceder el usuario.
    /// </summary>
    /// <param name="dto">Configuración de pestañas del usuario</param>
    /// <returns>Configuración actualizada de pestañas</returns>
    [HttpPut("user-tabs")]
    public async Task<IActionResult> UpdateUserTabs([FromBody] UpdateUserTabsDto dto)
    {
        var result = await Mediator.Send(new UpdateUserTabsCommand(dto));
        return HandleResult(result);
    }
}