using ElectroHuila.Application.DTOs.Appointments;
using ElectroHuila.Application.Features.Branches.Queries.GetActiveBranches;
using ElectroHuila.Application.Features.AppointmentTypes.Queries.GetActiveAppointmentTypes;
using ElectroHuila.Application.Features.Clients.Queries.GetClientByNumber;
using ElectroHuila.Application.Features.Appointments.Queries.GetPublicAvailableTimes;
using ElectroHuila.Application.Features.Appointments.Queries.GetOccupiedTimes;
using ElectroHuila.Application.Features.Appointments.Queries.GetAppointmentByNumberPublic;
using ElectroHuila.Application.Features.Appointments.Queries.GetClientAppointments;
using ElectroHuila.Application.Features.Appointments.Queries.VerifyAppointmentByQR;
using ElectroHuila.Application.Features.Appointments.Commands.SchedulePublicAppointment;
using ElectroHuila.Application.Features.Appointments.Commands.ScheduleSimpleAppointment;
using ElectroHuila.Application.Features.Appointments.Commands.CancelPublicAppointment;
using ElectroHuila.Application.Features.AvailableTimes.Queries.GetConfiguredTimesByBranch;
using ElectroHuila.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace ElectroHuila.WebApi.Controllers.V1;

/// <summary>
/// Controlador público para endpoints accesibles sin autenticación.
/// Permite a los clientes agendar citas, consultar horarios disponibles y verificar citas.
/// TODOS los endpoints de este controlador son públicos (NO requieren JWT).
/// </summary>
public class PublicController : ApiController
{
    /// <summary>
    /// Obtiene todas las sucursales activas disponibles para agendamiento.
    /// </summary>
    /// <returns>Lista de sucursales activas.</returns>
    [HttpGet("branches")]
    public async Task<IActionResult> GetActiveBranches()
    {
        var query = new GetActiveBranchesQuery();
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Obtiene todos los tipos de citas activos disponibles para agendamiento público.
    /// </summary>
    /// <returns>Lista de tipos de citas activos</returns>
    [HttpGet("appointment-types")]
    public async Task<IActionResult> GetActiveAppointmentTypes()
    {
        var query = new GetActiveAppointmentTypesQuery();
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Valida un cliente y devuelve información pública básica.
    /// No requiere autenticación para consulta de datos básicos.
    /// </summary>
    /// <param name="clientNumber">Número de cliente a validar (ejemplo: "CLI-20241002-001")</param>
    /// <returns>Información pública del cliente validado</returns>
    [HttpGet("client/validate/{clientNumber}")]
    public async Task<IActionResult> ValidatePublicClient(string clientNumber)
    {
        var query = new GetClientByNumberQuery(clientNumber);
        var result = await Mediator.Send(query);

        if (result.IsFailure)
        {
            return NotFound(new { message = "Cliente no encontrado" });
        }

        // Return only public information
        var client = result.Data;
        return Ok(new
        {
            clientNumber = client!.ClientNumber,
            fullName = client.FullName,
            email = client.Email,
            documentNumber = client.DocumentNumber,
            phone = client.Phone,
            mobile = client.Mobile,
            id = client.Id
        });
    }

    /// <summary>
    /// Obtiene los horarios disponibles para agendamiento público en una fecha y sucursal específica.
    /// </summary>
    /// <param name="date">Fecha para consultar disponibilidad</param>
    /// <param name="branchId">ID de la sucursal (requerido)</param>
    /// <returns>Lista de horarios disponibles para la fecha y sucursal</returns>
    [HttpGet("available-times")]
    public async Task<IActionResult> GetPublicAvailableTimes([FromQuery] DateTime date, [FromQuery] int branchId)
    {
        if (branchId <= 0)
        {
            return BadRequest(new { message = "ID de sede requerido" });
        }

        var query = new GetPublicAvailableTimesQuery(date, branchId);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Endpoint de depuración para obtener horarios configurados de una sucursal.
    /// Útil para verificar configuración de horarios.
    /// </summary>
    /// <param name="branchId">ID de la sucursal</param>
    /// <returns>Horarios configurados para la sucursal</returns>
    [HttpGet("debug/configured-times/{branchId}")]
    public async Task<IActionResult> GetConfiguredTimes(int branchId)
    {
        var query = new GetConfiguredTimesByBranchQuery(branchId);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Endpoint de depuración para obtener horarios ocupados en una fecha y sucursal.
    /// Útil para verificar por qué ciertos horarios no están disponibles.
    /// </summary>
    /// <param name="date">Fecha a consultar</param>
    /// <param name="branchId">ID de la sucursal</param>
    /// <returns>Lista de horarios ocupados</returns>
    [HttpGet("debug/occupied-times")]
    public async Task<IActionResult> GetOccupiedTimesDebug([FromQuery] DateTime date, [FromQuery] int branchId)
    {
        var query = new GetOccupiedTimesQuery(date, branchId);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Agenda una cita pública sin autenticación.
    /// </summary>
    /// <param name="dto">Datos de la cita a agendar.</param>
    /// <returns>Cita creada con número de confirmación.</returns>
    [HttpPost("schedule-appointment")]
    public async Task<IActionResult> SchedulePublicAppointment([FromBody] SchedulePublicAppointmentDto dto)
    {
        var command = new SchedulePublicAppointmentCommand(dto);
        var result = await Mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return CreatedAtAction(nameof(ConsultPublicAppointment),
            new { appointmentNumber = result.Data!.AppointmentNumber, clientNumber = dto.ClientNumber },
            result.Data);
    }

    /// <summary>
    /// Agenda una cita simple creando el cliente automáticamente si no existe.
    /// Este endpoint simplifica el proceso de agendamiento al manejar tanto la creación
    /// del cliente como el agendamiento de la cita en una sola operación.
    /// </summary>
    /// <param name="command">Datos del cliente y la cita a agendar</param>
    /// <returns>Confirmación con el número de cliente y número de cita generados</returns>
    /// <response code="200">Cita agendada exitosamente</response>
    /// <response code="400">Error en la validación de datos o en el proceso de agendamiento</response>
    [HttpPost("schedule-simple-appointment")]
    [ProducesResponseType(typeof(ScheduleSimpleAppointmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ScheduleSimpleAppointment([FromBody] ScheduleSimpleAppointmentCommand command)
    {
        var result = await Mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Consulta una cita específica usando el número de cita y cliente.
    /// Permite verificar el estado de una cita agendada.
    /// </summary>
    /// <param name="appointmentNumber">Número de la cita (ejemplo: "APT-20241002-001")</param>
    /// <param name="clientNumber">Número del cliente (requerido para validación)</param>
    /// <returns>Detalles de la cita consultada</returns>
    [HttpGet("appointment/{appointmentNumber}")]
    public async Task<IActionResult> ConsultPublicAppointment(string appointmentNumber, [FromQuery] string clientNumber)
    {
        if (string.IsNullOrWhiteSpace(appointmentNumber))
        {
            return BadRequest(new { message = "Número de cita requerido" });
        }

        if (string.IsNullOrWhiteSpace(clientNumber))
        {
            return BadRequest(new { message = "Número de cliente requerido para consultar la cita" });
        }

        var query = new GetAppointmentByNumberPublicQuery(appointmentNumber, clientNumber);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Obtiene todas las citas de un cliente específico.
    /// Permite al cliente consultar su historial de citas.
    /// </summary>
    /// <param name="clientNumber">Número del cliente (requerido)</param>
    /// <returns>Lista de citas del cliente</returns>
    [HttpGet("client/{clientNumber}/appointments")]
    public async Task<IActionResult> GetPublicClientAppointments(string clientNumber)
    {
        if (string.IsNullOrWhiteSpace(clientNumber))
        {
            return BadRequest(new { message = "El número de cliente es requerido" });
        }

        // First get client to validate
        var clientQuery = new GetClientByNumberQuery(clientNumber);
        var clientResult = await Mediator.Send(clientQuery);

        if (clientResult.IsFailure)
        {
            return NotFound(new { message = "Cliente no encontrado" });
        }

        var appointmentsQuery = new GetClientAppointmentsQuery(clientNumber);
        var result = await Mediator.Send(appointmentsQuery);
        return HandleResult(result);
    }

    /// <summary>
    /// Cancela una cita específica del cliente.
    /// Permite al cliente cancelar sus propias citas proporcionando un motivo.
    /// </summary>
    /// <param name="clientNumber">Número del cliente</param>
    /// <param name="appointmentId">ID de la cita a cancelar</param>
    /// <param name="dto">Datos de cancelación incluyendo motivo</param>
    /// <returns>Confirmación de cancelación</returns>
    [HttpPatch("client/{clientNumber}/appointment/{appointmentId}/cancel")]
    public async Task<IActionResult> CancelPublicAppointment(string clientNumber, int appointmentId, [FromBody] CancelAppointmentDto dto)
    {
        if (string.IsNullOrWhiteSpace(clientNumber))
        {
            return BadRequest(new { message = "El número de cliente es requerido" });
        }

        var command = new CancelPublicAppointmentCommand(clientNumber, appointmentId, dto.Reason ?? "");
        var result = await Mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(new { message = "Cita cancelada exitosamente" });
    }

    /// <summary>
    /// Verifica una cita usando código QR o parámetros de consulta.
    /// Útil para validación rápida en puntos de atención.
    /// </summary>
    /// <param name="number">Número de la cita</param>
    /// <param name="client">Número del cliente</param>
    /// <returns>Resultado de verificación de la cita</returns>
    [HttpGet("verify-appointment")]
    public async Task<IActionResult> VerifyAppointmentByQR([FromQuery] string number, [FromQuery] string client)
    {
        if (string.IsNullOrWhiteSpace(number) || string.IsNullOrWhiteSpace(client))
        {
            return BadRequest(new { message = "Número de cita y cliente requeridos", valid = false });
        }

        var query = new VerifyAppointmentByQRQuery(number, client);
        var result = await Mediator.Send(query);

        if (result.IsFailure)
        {
            return NotFound(new { message = result.Error, valid = false });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Endpoint de health check para verificar que el servicio está funcionando.
    /// </summary>
    /// <returns>Estado del servicio y timestamp.</returns>
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new {
            status = "Healthy",
            timestamp = DateTime.UtcNow,
            version = "1.0.0"
        });
    }
}