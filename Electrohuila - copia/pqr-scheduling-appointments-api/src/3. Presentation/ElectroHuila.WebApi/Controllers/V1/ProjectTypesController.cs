using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace ElectroHuila.WebApi.Controllers.V1;

/// <summary>
/// Controller para gestionar los tipos de proyecto
/// </summary>
public class ProjectTypesController : ApiController
{
    private readonly IProjectTypeRepository _repository;

    public ProjectTypesController(IProjectTypeRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Obtiene todos los tipos de proyecto activos
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var projectTypes = await _repository.GetAllAsync();
        return Ok(projectTypes);
    }

    /// <summary>
    /// Obtiene un tipo de proyecto por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var projectType = await _repository.GetByIdAsync(id);

        if (projectType == null)
            return NotFound($"Tipo de proyecto con ID {id} no encontrado");

        return Ok(projectType);
    }

    /// <summary>
    /// Obtiene un tipo de proyecto por código
    /// </summary>
    [HttpGet("code/{code}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCode(string code)
    {
        var projectType = await _repository.GetByCodeAsync(code);

        if (projectType == null)
            return NotFound($"Tipo de proyecto con código {code} no encontrado");

        return Ok(projectType);
    }
}
