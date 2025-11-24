using ElectroHuila.Application.DTOs.Assignments;
using ElectroHuila.Application.Features.UserAssignments.Commands.AssignUserToAppointmentType;
using ElectroHuila.Application.Features.UserAssignments.Commands.RemoveAssignment;
using ElectroHuila.Application.Features.UserAssignments.Commands.BulkAssignUser;
using ElectroHuila.Application.Features.UserAssignments.Queries.GetUserAssignments;
using ElectroHuila.Application.Features.UserAssignments.Queries.GetAllAssignments;
using ElectroHuila.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectroHuila.WebApi.Controllers.V1;

/// <summary>
/// Controlador para gestión de asignaciones de usuarios a tipos de cita.
/// Permite asignar empleados a tipos de cita específicos para filtrar sus vistas.
/// Requiere autenticación JWT para todos los endpoints.
/// </summary>
[Authorize]
[Route("api/v1/user-assignments")]
public class UserAssignmentsController : ApiController
{
    /// <summary>
    /// Obtiene todas las asignaciones de un usuario específico.
    /// </summary>
    /// <param name="userId">ID del usuario.</param>
    /// <returns>Lista de asignaciones del usuario.</returns>
    [HttpGet("users/{userId}/assignments")]
    public async Task<IActionResult> GetUserAssignments(int userId)
    {
        var result = await Mediator.Send(new GetUserAssignmentsQuery(userId));
        return HandleResult(result);
    }

    /// <summary>
    /// Obtiene todas las asignaciones del sistema (solo administradores).
    /// </summary>
    /// <returns>Lista de todas las asignaciones.</returns>
    [HttpGet("assignments")]
    public async Task<IActionResult> GetAllAssignments()
    {
        var result = await Mediator.Send(new GetAllAssignmentsQuery());
        return HandleResult(result);
    }

    /// <summary>
    /// Asigna un tipo de cita a un usuario.
    /// </summary>
    /// <param name="userId">ID del usuario.</param>
    /// <param name="dto">Datos de la asignación.</param>
    /// <returns>Asignación creada.</returns>
    [HttpPost("users/{userId}/assign-appointment-type")]
    public async Task<IActionResult> AssignAppointmentType(int userId, [FromBody] CreateAssignmentDto dto)
    {
        // Validar que el userId del parámetro coincida con el del DTO
        if (dto.UserId != userId)
        {
            dto = new CreateAssignmentDto
            {
                UserId = userId,
                AppointmentTypeId = dto.AppointmentTypeId
            };
        }

        var result = await Mediator.Send(new AssignUserToAppointmentTypeCommand(dto));
        return result.IsSuccess
            ? Created($"/api/v1/assignments/{result.Data!.Id}", result.Data)
            : HandleResult(result);
    }

    /// <summary>
    /// Asigna múltiples tipos de cita a un usuario.
    /// </summary>
    /// <param name="userId">ID del usuario.</param>
    /// <param name="dto">Datos de las asignaciones masivas.</param>
    /// <returns>Lista de asignaciones creadas.</returns>
    [HttpPost("users/{userId}/bulk-assign")]
    public async Task<IActionResult> BulkAssign(int userId, [FromBody] BulkAssignmentDto dto)
    {
        // Validar que el userId del parámetro coincida con el del DTO
        if (dto.UserId != userId)
        {
            dto = new BulkAssignmentDto
            {
                UserId = userId,
                AppointmentTypeIds = dto.AppointmentTypeIds
            };
        }

        var result = await Mediator.Send(new BulkAssignUserCommand(dto));
        return HandleResult(result);
    }

    /// <summary>
    /// Elimina una asignación específica.
    /// </summary>
    /// <param name="assignmentId">ID de la asignación a eliminar.</param>
    /// <returns>Confirmación de eliminación.</returns>
    [HttpDelete("assignments/{assignmentId}")]
    public async Task<IActionResult> RemoveAssignment(int assignmentId)
    {
        var result = await Mediator.Send(new RemoveAssignmentCommand(assignmentId));
        return result.IsSuccess
            ? Ok(new { success = true, message = "Asignación eliminada exitosamente" })
            : HandleResult(result);
    }
}
