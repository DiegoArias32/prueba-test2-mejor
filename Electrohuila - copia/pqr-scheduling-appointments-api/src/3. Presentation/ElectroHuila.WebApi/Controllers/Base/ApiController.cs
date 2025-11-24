using ElectroHuila.Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ElectroHuila.WebApi.Controllers.Base;

/// <summary>
/// Controlador base abstracto para todos los controladores de la API.
/// Proporciona funcionalidad común para manejo de resultados y acceso a MediatR.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public abstract class ApiController : ControllerBase
{
    private ISender _mediator = null!;

    /// <summary>
    /// Obtiene una instancia de ISender (MediatR) mediante inyección de dependencias lazy.
    /// </summary>
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    /// <summary>
    /// Maneja un resultado genérico y retorna la respuesta HTTP apropiada.
    /// </summary>
    /// <typeparam name="T">Tipo de dato del resultado.</typeparam>
    /// <param name="result">Resultado de la operación.</param>
    /// <returns>200 OK con datos si es exitoso, 400 BadRequest con error si falla.</returns>
    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }

        return BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Maneja un resultado sin datos y retorna la respuesta HTTP apropiada.
    /// </summary>
    /// <param name="result">Resultado de la operación.</param>
    /// <returns>200 OK si es exitoso, 400 BadRequest con error si falla.</returns>
    protected IActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
        {
            return Ok();
        }

        return BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Maneja un resultado de creación y retorna 201 Created con la ubicación del recurso.
    /// </summary>
    /// <typeparam name="T">Tipo de dato del resultado.</typeparam>
    /// <param name="result">Resultado de la operación de creación.</param>
    /// <param name="routeName">Nombre de la ruta para generar la ubicación del recurso.</param>
    /// <param name="routeValues">Valores de ruta para generar la URL del recurso creado.</param>
    /// <returns>201 Created con datos y ubicación si es exitoso, 400 BadRequest con error si falla.</returns>
    protected IActionResult CreatedResult<T>(Result<T> result, string routeName = "", object routeValues = null!)
    {
        if (result.IsSuccess)
        {
            if (!string.IsNullOrEmpty(routeName))
            {
                return CreatedAtRoute(routeName, routeValues, result.Data);
            }
            return Created("", result.Data);
        }

        return BadRequest(new { error = result.Error });
    }
}