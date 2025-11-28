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
using ElectroHuila.Application.Contracts.Repositories;
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
    private readonly IHolidayRepository _holidayRepository;

    public HolidaysController(IHolidayRepository holidayRepository)
    {
        _holidayRepository = holidayRepository;
    }

    /// <summary>
    /// Obtiene todos los festivos registrados en el sistema con paginación.
    /// Incluye festivos nacionales, locales y de empresa.
    /// </summary>
    /// <param name="pageNumber">Número de página (predeterminado: 1)</param>
    /// <param name="pageSize">Cantidad de registros por página (predeterminado: 20)</param>
    /// <param name="cancellationToken">Token de cancelación para la operación asíncrona</param>
    /// <returns>Lista paginada de festivos</returns>
    [HttpGet]
    [ResponseCache(Duration = 300)] // Cache 5 minutos
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllHolidaysQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };
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

    /// <summary>
    /// Activa un festivo previamente desactivado.
    /// Marca el festivo como activo en el sistema.
    /// </summary>
    /// <param name="id">ID del festivo a activar</param>
    /// <returns>Confirmación de activación exitosa</returns>
    [HttpPatch("{id:int}/activate")]
    public async Task<IActionResult> Activate(int id)
    {
        var holiday = await _holidayRepository.GetByIdAsync(id);
        if (holiday == null)
            return NotFound(new { message = $"Holiday with ID {id} not found" });

        holiday.IsActive = true;
        holiday.UpdatedAt = DateTime.UtcNow;
        await _holidayRepository.UpdateAsync(holiday);

        return Ok(new { success = true, message = "Holiday activated successfully" });
    }

    /// <summary>
    /// Desactiva un festivo del sistema.
    /// Marca el festivo como inactivo sin eliminarlo físicamente.
    /// </summary>
    /// <param name="id">ID del festivo a desactivar</param>
    /// <returns>Confirmación de desactivación exitosa</returns>
    [HttpPatch("{id:int}/deactivate")]
    public async Task<IActionResult> Deactivate(int id)
    {
        var holiday = await _holidayRepository.GetByIdAsync(id);
        if (holiday == null)
            return NotFound(new { message = $"Holiday with ID {id} not found" });

        holiday.IsActive = false;
        holiday.UpdatedAt = DateTime.UtcNow;
        await _holidayRepository.UpdateAsync(holiday);

        return Ok(new { success = true, message = "Holiday deactivated successfully" });
    }
}
