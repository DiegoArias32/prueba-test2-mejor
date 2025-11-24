using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Appointments;
using ElectroHuila.Application.Features.AppointmentDocuments.Commands.CreateAppointmentDocument;
using ElectroHuila.Application.Features.AppointmentDocuments.Commands.UpdateAppointmentDocument;
using ElectroHuila.Application.Features.AppointmentDocuments.Commands.DeleteAppointmentDocument;
using ElectroHuila.Application.Features.AppointmentDocuments.Queries.GetDocumentsByAppointmentId;
using ElectroHuila.Application.Features.AppointmentDocuments.Queries.GetDocumentById;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectroHuila.WebApi.Controllers.V1;

/// <summary>
/// Controlador para gestionar documentos adjuntos a citas.
/// Permite subir, listar y gestionar archivos asociados a citas.
/// Requiere autenticación JWT.
/// </summary>
[Authorize]
public class AppointmentDocumentsController : ApiController
{
    private readonly IAppointmentDocumentRepository _repository;

    public AppointmentDocumentsController(IAppointmentDocumentRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Obtiene un documento por su ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var query = new GetDocumentByIdQuery(id);
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Obtiene todos los documentos de una cita específica.
    /// </summary>
    [HttpGet("appointment/{appointmentId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByAppointment(int appointmentId, CancellationToken cancellationToken)
    {
        var query = new GetDocumentsByAppointmentIdQuery(appointmentId);
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Obtiene estadísticas de documentos de una cita específica.
    /// </summary>
    [HttpGet("appointment/{appointmentId:int}/stats")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStatsByAppointment(int appointmentId, CancellationToken cancellationToken)
    {
        var documents = await _repository.GetByAppointmentIdAsync(appointmentId);
        var documentsList = documents.ToList();

        var totalSize = await _repository.GetTotalSizeByAppointmentIdAsync(appointmentId);
        var stats = new AppointmentDocumentsStatsDto
        {
            AppointmentId = appointmentId,
            TotalDocuments = documentsList.Count,
            TotalSizeBytes = totalSize,
            TotalSizeFormatted = FormatFileSize(totalSize),
            ImageCount = documentsList.Count(d => d.IsImage()),
            PdfCount = documentsList.Count(d => d.IsPdf()),
            OtherCount = documentsList.Count(d => !d.IsImage() && !d.IsPdf())
        };

        return HandleResult(Result<AppointmentDocumentsStatsDto>.Success(stats));
    }

    /// <summary>
    /// Crea un nuevo documento adjunto a una cita.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateAppointmentDocumentDto dto, CancellationToken cancellationToken)
    {
        var command = new CreateAppointmentDocumentCommand(dto);
        var result = await Mediator.Send(command, cancellationToken);
        return CreatedResult(result, nameof(GetById), new { id = result.Data?.Id });
    }

    /// <summary>
    /// Actualiza la descripción de un documento.
    /// </summary>
    [HttpPatch("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateDescription(int id, [FromBody] UpdateAppointmentDocumentDto dto, CancellationToken cancellationToken)
    {
        if (id != dto.Id)
            return BadRequest(new { error = "ID mismatch" });

        var command = new UpdateAppointmentDocumentCommand(dto);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Elimina (desactiva) un documento del sistema.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var command = new DeleteAppointmentDocumentCommand(id);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    private static string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = bytes;
        int order = 0;

        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len /= 1024;
        }

        return $"{len:0.##} {sizes[order]}";
    }
}
