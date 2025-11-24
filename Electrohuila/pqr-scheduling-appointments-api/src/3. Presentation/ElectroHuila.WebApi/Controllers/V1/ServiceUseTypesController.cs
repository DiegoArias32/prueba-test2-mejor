using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace ElectroHuila.WebApi.Controllers.V1;

/// <summary>
/// Controller para gestionar los tipos de uso de servicio
/// </summary>
public class ServiceUseTypesController : ApiController
{
    private readonly IServiceUseTypeRepository _repository;

    public ServiceUseTypesController(IServiceUseTypeRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Obtiene todos los tipos de uso de servicio activos
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var serviceUseTypes = await _repository.GetAllAsync();
        return Ok(serviceUseTypes);
    }

    /// <summary>
    /// Obtiene un tipo de uso de servicio por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var serviceUseType = await _repository.GetByIdAsync(id);

        if (serviceUseType == null)
            return NotFound($"Tipo de uso de servicio con ID {id} no encontrado");

        return Ok(serviceUseType);
    }

    /// <summary>
    /// Obtiene un tipo de uso de servicio por código
    /// </summary>
    [HttpGet("code/{code}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCode(string code)
    {
        var serviceUseType = await _repository.GetByCodeAsync(code);

        if (serviceUseType == null)
            return NotFound($"Tipo de uso de servicio con código {code} no encontrado");

        return Ok(serviceUseType);
    }
}
