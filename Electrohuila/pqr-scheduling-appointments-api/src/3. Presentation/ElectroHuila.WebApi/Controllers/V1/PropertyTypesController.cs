using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace ElectroHuila.WebApi.Controllers.V1;

/// <summary>
/// Controller para gestionar los tipos de propiedad
/// </summary>
public class PropertyTypesController : ApiController
{
    private readonly IPropertyTypeRepository _repository;

    public PropertyTypesController(IPropertyTypeRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Obtiene todos los tipos de propiedad activos
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var propertyTypes = await _repository.GetAllAsync();
        return Ok(propertyTypes);
    }

    /// <summary>
    /// Obtiene un tipo de propiedad por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var propertyType = await _repository.GetByIdAsync(id);

        if (propertyType == null)
            return NotFound($"Tipo de propiedad con ID {id} no encontrado");

        return Ok(propertyType);
    }

    /// <summary>
    /// Obtiene un tipo de propiedad por código
    /// </summary>
    [HttpGet("code/{code}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCode(string code)
    {
        var propertyType = await _repository.GetByCodeAsync(code);

        if (propertyType == null)
            return NotFound($"Tipo de propiedad con código {code} no encontrado");

        return Ok(propertyType);
    }
}
