using ElectroHuila.Application.DTOs.Catalogs;
using ElectroHuila.Application.Features.Holidays.Commands.CreateNationalHoliday;
using ElectroHuila.Application.Features.Holidays.Commands.CreateLocalHoliday;
using ElectroHuila.Application.Features.Holidays.Commands.CreateCompanyHoliday;
using ElectroHuila.Application.Features.Holidays.Commands.UpdateHoliday;
using ElectroHuila.Application.Features.Holidays.Commands.DeleteHoliday;
using ElectroHuila.Application.Features.Holidays.Queries.GetAllHolidays;
using ElectroHuila.Application.Features.Holidays.Queries.GetHolidayById;
using ElectroHuila.Application.Features.Holidays.Queries.GetHolidaysByDateRange;
using ElectroHuila.Application.Features.Holidays.Queries.CheckIfHoliday;
using ElectroHuila.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectroHuila.WebApi.Controllers.V1;

/// <summary>
/// Controlador para gestionar festivos y días no laborables.
/// Permite administrar festivos nacionales, locales y de empresa.
/// Requiere autenticación JWT.
/// </summary>
[Authorize]
public class HolidaysController : ApiController
{

    /// <summary>
    /// Obtiene todos los festivos registrados en el sistema.
    /// Incluye festivos nacionales, locales y de empresa.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación para la operación asíncrona</param>
    /// <returns>Lista completa de festivos</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetAllHolidaysQuery();
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Obtiene un festivo por su ID.
    /// </summary>
    /// <param name="id">ID del festivo</param>
    /// <param name="cancellationToken">Token de cancelación para la operación asíncrona</param>
    /// <returns>Datos del festivo encontrado</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var query = new GetHolidayByIdQuery(id);
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Obtiene todos los festivos dentro de un rango de fechas.
    /// </summary>
    /// <param name="startDate">Fecha de inicio del rango</param>
    /// <param name="endDate">Fecha de fin del rango</param>
    /// <param name="cancellationToken">Token de cancelación para la operación asíncrona</param>
    /// <returns>Lista de festivos en el rango especificado</returns>
    [HttpGet("range")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, CancellationToken cancellationToken)
    {
        var query = new GetHolidaysByDateRangeQuery(startDate, endDate);
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Verifica si una fecha específica es festivo.
    /// Útil para validar disponibilidad de fechas para citas.
    /// </summary>
    /// <param name="date">Fecha a verificar (formato: YYYY-MM-DD)</param>
    /// <param name="branchId">ID de sucursal (opcional, para verificar festivos locales)</param>
    /// <param name="cancellationToken">Token de cancelación para la operación asíncrona</param>
    /// <returns>Resultado indicando si es festivo y detalles del mismo</returns>
    [HttpGet("check")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CheckHoliday([FromQuery] DateTime date, [FromQuery] int? branchId, CancellationToken cancellationToken)
    {
        var query = new CheckIfHolidayQuery(date, branchId);
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Crea un nuevo festivo nacional.
    /// Solo administradores pueden crear festivos.
    /// </summary>
    /// <param name="dto">Datos del festivo nacional a crear</param>
    /// <param name="cancellationToken">Token de cancelación para la operación asíncrona</param>
    /// <returns>El festivo creado con su ID asignado</returns>
    [HttpPost("national")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateNational([FromBody] CreateNationalHolidayDto dto, CancellationToken cancellationToken)
    {
        var command = new CreateNationalHolidayCommand(dto);
        var result = await Mediator.Send(command, cancellationToken);
        return CreatedResult(result, nameof(GetById), new { id = result.Data?.Id });
    }

    /// <summary>
    /// Crea un nuevo festivo local para una sucursal específica.
    /// Solo administradores pueden crear festivos.
    /// </summary>
    /// <param name="dto">Datos del festivo local a crear</param>
    /// <param name="cancellationToken">Token de cancelación para la operación asíncrona</param>
    /// <returns>El festivo creado con su ID asignado</returns>
    [HttpPost("local")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateLocal([FromBody] CreateLocalHolidayDto dto, CancellationToken cancellationToken)
    {
        var command = new CreateLocalHolidayCommand(dto);
        var result = await Mediator.Send(command, cancellationToken);
        return CreatedResult(result, nameof(GetById), new { id = result.Data?.Id });
    }

    /// <summary>
    /// Crea un nuevo festivo de empresa (aplica para todas las sucursales).
    /// Solo administradores pueden crear festivos.
    /// </summary>
    /// <param name="dto">Datos del festivo de empresa a crear</param>
    /// <param name="cancellationToken">Token de cancelación para la operación asíncrona</param>
    /// <returns>El festivo creado con su ID asignado</returns>
    [HttpPost("company")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateCompany([FromBody] CreateCompanyHolidayDto dto, CancellationToken cancellationToken)
    {
        var command = new CreateCompanyHolidayCommand(dto);
        var result = await Mediator.Send(command, cancellationToken);
        return CreatedResult(result, nameof(GetById), new { id = result.Data?.Id });
    }

    /// <summary>
    /// Actualiza un festivo existente.
    /// </summary>
    /// <param name="id">ID del festivo a actualizar</param>
    /// <param name="dto">Datos del festivo a actualizar</param>
    /// <param name="cancellationToken">Token de cancelación para la operación asíncrona</param>
    /// <returns>El festivo actualizado</returns>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateHolidayDto dto, CancellationToken cancellationToken)
    {
        if (id != dto.Id)
        {
            return BadRequest(new { error = "ID mismatch between route and body" });
        }

        var command = new UpdateHolidayCommand(dto);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Elimina (desactiva) un festivo del sistema.
    /// Realiza eliminación lógica para preservar integridad de datos.
    /// </summary>
    /// <param name="id">ID del festivo a eliminar</param>
    /// <param name="cancellationToken">Token de cancelación para la operación asíncrona</param>
    /// <returns>Confirmación de eliminación exitosa</returns>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var command = new DeleteHolidayCommand(id);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }
}
