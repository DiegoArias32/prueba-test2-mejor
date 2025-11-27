using ElectroHuila.Application.DTOs.Appointments;
using ElectroHuila.Application.Features.Appointments.Commands.CreateAppointment;
using ElectroHuila.Application.Features.Appointments.Commands.UpdateAppointment;
using ElectroHuila.Application.Features.Appointments.Commands.CancelAppointment;
using ElectroHuila.Application.Features.Appointments.Commands.ScheduleAppointment;
using ElectroHuila.Application.Features.Appointments.Commands.CompleteAppointment;
using ElectroHuila.Application.Features.Appointments.Commands.DeleteLogicalAppointment;
using ElectroHuila.Application.Features.Appointments.Queries.GetAppointmentById;
using ElectroHuila.Application.Features.Appointments.Queries.GetAppointmentByNumber;
using ElectroHuila.Application.Features.Appointments.Queries.GetAppointmentsByClientNumber;
using ElectroHuila.Application.Features.Appointments.Queries.GetAppointmentsByDate;
using ElectroHuila.Application.Features.Appointments.Queries.GetAppointmentsByBranch;
using ElectroHuila.Application.Features.Appointments.Queries.GetAppointmentsByStatus;
using ElectroHuila.Application.Features.Appointments.Queries.GetPendingAppointments;
using ElectroHuila.Application.Features.Appointments.Queries.GetCompletedAppointments;
using ElectroHuila.Application.Features.Appointments.Queries.ValidateAvailability;
using ElectroHuila.Application.Features.Appointments.Queries.GetAvailableTimes;
using ElectroHuila.Application.Features.Appointments.Queries.GetAllIncludingInactive;
using ElectroHuila.Application.Features.Appointments.Queries.GetMyAssignedAppointments;
using ElectroHuila.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ElectroHuila.WebApi.Controllers.V1;

/// <summary>
/// Controlador para gestión de citas (Appointments).
/// Proporciona endpoints para crear, consultar, actualizar, cancelar y completar citas.
/// Requiere autenticación JWT para todos los endpoints.
/// </summary>
[Authorize]
public class AppointmentsController : ApiController
{
    private readonly ILogger<AppointmentsController> _logger;

    public AppointmentsController(ILogger<AppointmentsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Obtiene una cita por su ID.
    /// </summary>
    /// <param name="id">ID de la cita.</param>
    /// <returns>Datos de la cita encontrada.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await Mediator.Send(new GetAppointmentByIdQuery(id));
        return HandleResult(result);
    }

    /// <summary>
    /// Obtiene una cita por su número único.
    /// </summary>
    /// <param name="appointmentNumber">Número de la cita (ej: APT-20241002-ABC123)</param>
    /// <returns>Datos de la cita encontrada</returns>
    [HttpGet("number/{appointmentNumber}")]
    public async Task<IActionResult> GetByNumber(string appointmentNumber)
    {
        var result = await Mediator.Send(new GetAppointmentByNumberQuery(appointmentNumber));
        return HandleResult(result);
    }

    /// <summary>
    /// Obtiene todas las citas de un cliente específico.
    /// </summary>
    /// <param name="clientNumber">Número del cliente (ej: CLI-20241002-123456)</param>
    /// <returns>Lista de citas del cliente</returns>
    [HttpGet("client/{clientNumber}")]
    public async Task<IActionResult> GetByClientNumber(string clientNumber)
    {
        var result = await Mediator.Send(new GetAppointmentsByClientNumberQuery(clientNumber));
        return HandleResult(result);
    }

    /// <summary>
    /// Obtiene todas las citas programadas para una fecha específica.
    /// </summary>
    /// <param name="date">Fecha a consultar</param>
    /// <returns>Lista de citas de la fecha</returns>
    [HttpGet("date/{date}")]
    public async Task<IActionResult> GetByDate(DateTime date)
    {
        var result = await Mediator.Send(new GetAppointmentsByDateQuery(date));
        return HandleResult(result);
    }

    /// <summary>
    /// Obtiene todas las citas de una sucursal específica.
    /// </summary>
    /// <param name="branchId">ID de la sucursal</param>
    /// <returns>Lista de citas de la sucursal</returns>
    [HttpGet("branch/{branchId}")]
    public async Task<IActionResult> GetByBranch(int branchId)
    {
        var result = await Mediator.Send(new GetAppointmentsByBranchQuery(branchId));
        return HandleResult(result);
    }

    /// <summary>
    /// Obtiene todas las citas con un estado específico.
    /// </summary>
    /// <param name="status">Estado de las citas (Pending, Completed, Cancelled)</param>
    /// <returns>Lista de citas con el estado especificado</returns>
    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetByStatus(string status)
    {
        var result = await Mediator.Send(new GetAppointmentsByStatusQuery(status));
        return HandleResult(result);
    }

    /// <summary>
    /// Obtiene todas las citas pendientes (sin completar).
    /// </summary>
    /// <returns>Lista de citas pendientes</returns>
    [HttpGet("pending")]
    public async Task<IActionResult> GetPending()
    {
        var result = await Mediator.Send(new GetPendingAppointmentsQuery());
        return HandleResult(result);
    }

    /// <summary>
    /// Obtiene todas las citas completadas.
    /// </summary>
    /// <returns>Lista de citas completadas</returns>
    [HttpGet("completed")]
    public async Task<IActionResult> GetCompleted()
    {
        var result = await Mediator.Send(new GetCompletedAppointmentsQuery());
        return HandleResult(result);
    }

    /// <summary>
    /// Obtiene todas las citas incluyendo las inactivas.
    /// </summary>
    /// <returns>Lista de todas las citas (activas e inactivas)</returns>
    [HttpGet("all-including-inactive")]
    public async Task<IActionResult> GetAllIncludingInactive()
    {
        var result = await Mediator.Send(new GetAllAppointmentsIncludingInactiveQuery());
        return HandleResult(result);
    }

    /// <summary>
    /// Obtiene las citas asignadas al usuario actual basándose en sus tipos de cita asignados.
    /// Este endpoint reemplaza el método anterior que retornaba "Unknown" en los datos relacionados.
    /// Incluye datos completos de cliente, sede y tipo de cita mediante JOINs.
    /// </summary>
    /// <returns>Lista de citas asignadas al usuario con datos completos</returns>
    [HttpGet("my-assigned")]
    public async Task<IActionResult> GetMyAssignedAppointments()
    {
        // Obtener el ID del usuario del token JWT
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized(new { message = "Usuario no autenticado o ID inválido" });
        }

        var result = await Mediator.Send(new GetMyAssignedAppointmentsQuery(userId));
        return HandleResult(result);
    }

    /// <summary>
    /// Valida si hay disponibilidad para una fecha, hora y sucursal específica.
    /// </summary>
    /// <param name="date">Fecha a validar</param>
    /// <param name="time">Hora a validar</param>
    /// <param name="branchId">ID de la sucursal</param>
    /// <returns>Objeto indicando si está disponible</returns>
    [HttpGet("availability")]
    public async Task<IActionResult> ValidateAvailability([FromQuery] DateTime date, [FromQuery] TimeSpan time, [FromQuery] int branchId)
    {
        var result = await Mediator.Send(new ValidateAvailabilityQuery(date, time, branchId));
        return result.IsSuccess
            ? Ok(new { disponible = result.Data })
            : HandleResult(result);
    }

    /// <summary>
    /// Obtiene los horarios disponibles para una fecha y sucursal específica.
    /// </summary>
    /// <param name="date">Fecha a consultar</param>
    /// <param name="branchId">ID de la sucursal</param>
    /// <returns>Lista de horarios disponibles</returns>
    [HttpGet("available-times")]
    public async Task<IActionResult> GetAvailableTimes([FromQuery] DateTime date, [FromQuery] int branchId)
    {
        var result = await Mediator.Send(new GetAvailableTimesQuery(date, branchId));
        return HandleResult(result);
    }

    /// <summary>
    /// Crea una nueva cita.
    /// </summary>
    /// <param name="dto">Datos de la cita a crear.</param>
    /// <returns>Cita creada con su ID asignado.</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAppointmentDto dto)
    {
        var result = await Mediator.Send(new CreateAppointmentCommand(dto));
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data)
            : HandleResult(result);
    }

    /// <summary>
    /// Agenda una nueva cita (alternativa a Create con validaciones adicionales).
    /// </summary>
    /// <param name="dto">Datos de la cita a agendar</param>
    /// <returns>Cita agendada con confirmación</returns>
    [HttpPost("schedule")]
    public async Task<IActionResult> Schedule([FromBody] CreateAppointmentDto dto)
    {
        var result = await Mediator.Send(new ScheduleAppointmentCommand(dto));
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data)
            : HandleResult(result);
    }

    /// <summary>
    /// Actualiza los datos de una cita existente.
    /// </summary>
    /// <param name="id">ID de la cita a actualizar</param>
    /// <param name="dto">Nuevos datos de la cita</param>
    /// <returns>Cita actualizada</returns>
    [HttpPatch("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAppointmentDto dto)
    {
        var result = await Mediator.Send(new UpdateAppointmentCommand(id, dto));
        return HandleResult(result);
    }

    /// <summary>
    /// Cancela una cita existente.
    /// </summary>
    /// <param name="appointmentId">ID de la cita a cancelar</param>
    /// <param name="dto">Datos de cancelación (motivo)</param>
    /// <returns>Confirmación de cancelación</returns>
    [HttpPatch("cancel/{appointmentId}")]
    public async Task<IActionResult> Cancel(int appointmentId, [FromBody] CancelAppointmentDto dto)
    {
        var command = new CancelAppointmentCommand(appointmentId, dto.Reason ?? string.Empty);
        var result = await Mediator.Send(command);
        return result.IsSuccess
            ? Ok(new { success = true, message = "Cita cancelada exitosamente" })
            : HandleResult(result);
    }

    /// <summary>
    /// Marca una cita como completada.
    /// </summary>
    /// <param name="appointmentId">ID de la cita a completar</param>
    /// <param name="dto">Datos de completación (notas)</param>
    /// <returns>Confirmación de completación</returns>
    [HttpPatch("complete/{appointmentId}")]
    public async Task<IActionResult> Complete(int appointmentId, [FromBody] CompleteAppointmentDto dto)
    {
        var command = new CompleteAppointmentCommand(appointmentId, dto.Notes);
        var result = await Mediator.Send(command);
        return result.IsSuccess
            ? Ok(new { success = true, message = "Cita completada exitosamente" })
            : HandleResult(result);
    }

    /// <summary>
    /// Realiza eliminación lógica de una cita.
    /// Marca la cita como inactiva y deshabilitada sin eliminarla físicamente de la base de datos.
    /// </summary>
    /// <param name="id">ID de la cita a desactivar</param>
    /// <returns>Confirmación de desactivación exitosa</returns>
    [HttpPatch("delete-logical/{id:int}")]
    public async Task<IActionResult> DeleteLogical(int id)
    {
        var command = new DeleteLogicalAppointmentCommand(id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    // ========================================
    // REMOVED DUPLICATED ENDPOINTS
    // ========================================
    // GetClientAppointments() - DUPLICATED with GetByClientNumber() above (line 87)
    // VerifyAppointmentByQR() - DUPLICATED in PublicController (line 236)
    // These methods were removed to follow DRY principle
}
