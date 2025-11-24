using ElectroHuila.Application.DTOs.Catalogs;
using ElectroHuila.Domain.Enums;
using ElectroHuila.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace ElectroHuila.WebApi.Controllers.V1;

/// <summary>
/// Controller para gestionar los tipos de documento
/// </summary>
public class DocumentTypesController : ApiController
{
    /// <summary>
    /// Obtiene todos los tipos de documento disponibles
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        var documentTypes = Enum.GetValues<DocumentType>()
            .Select(dt => new DocumentTypeDto
            {
                Id = (int)dt,
                Code = dt.ToString(),
                Name = dt.ToDisplayName()
            })
            .OrderBy(dt => dt.Id)
            .ToList();

        return Ok(documentTypes);
    }

    /// <summary>
    /// Obtiene un tipo de documento por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(int id)
    {
        if (!Enum.IsDefined(typeof(DocumentType), id))
            return NotFound($"Tipo de documento con ID {id} no encontrado");

        var documentType = (DocumentType)id;
        var dto = new DocumentTypeDto
        {
            Id = (int)documentType,
            Code = documentType.ToString(),
            Name = documentType.ToDisplayName()
        };

        return Ok(dto);
    }

    /// <summary>
    /// Obtiene un tipo de documento por código
    /// </summary>
    [HttpGet("code/{code}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetByCode(string code)
    {
        if (!Enum.TryParse<DocumentType>(code, true, out var documentType))
            return NotFound($"Tipo de documento con código {code} no encontrado");

        var dto = new DocumentTypeDto
        {
            Id = (int)documentType,
            Code = documentType.ToString(),
            Name = documentType.ToDisplayName()
        };

        return Ok(dto);
    }
}
