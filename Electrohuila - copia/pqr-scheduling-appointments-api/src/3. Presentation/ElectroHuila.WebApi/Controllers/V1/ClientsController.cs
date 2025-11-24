using ElectroHuila.Application.Features.Clients.Commands.CreateClient;
using ElectroHuila.Application.Features.Clients.Commands.UpdateClient;
using ElectroHuila.Application.Features.Clients.Commands.DeleteClient;
using ElectroHuila.Application.Features.Clients.Commands.DeleteLogicalClient;
using ElectroHuila.Application.Features.Clients.Queries.GetAllClients;
using ElectroHuila.Application.Features.Clients.Queries.GetClientById;
using ElectroHuila.Application.Features.Clients.Queries.GetClientByNumber;
using ElectroHuila.Application.Features.Clients.Queries.GetClientByDocument;
using ElectroHuila.Application.Features.Clients.Queries.ValidateClient;
using ElectroHuila.Application.Features.Clients.Queries.GetAllIncludingInactive;
using ElectroHuila.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectroHuila.WebApi.Controllers.V1;

/// <summary>
/// Controlador para gestionar los clientes de ElectroHuila.
/// Permite administrar información de clientes, validaciones y consultas.
/// Requiere autenticación JWT.
/// </summary>
[Authorize]
public class ClientsController : ApiController
{
    /// <summary>
    /// Obtiene todos los clientes registrados en el sistema.
    /// </summary>
    /// <returns>Lista completa de clientes</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var query = new GetAllClientsQuery();
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Obtiene todos los clientes incluyendo los inactivos.
    /// </summary>
    /// <returns>Lista de todos los clientes (activos e inactivos)</returns>
    [HttpGet("all-including-inactive")]
    public async Task<IActionResult> GetAllIncludingInactive()
    {
        var query = new GetAllClientsIncludingInactiveQuery();
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Busca un cliente específico por su identificador único.
    /// </summary>
    /// <param name="id">ID del cliente a buscar (ejemplo: 1)</param>
    /// <returns>Datos completos del cliente encontrado</returns>
    [HttpGet("{id:int}", Name = "GetClientById")]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new GetClientByIdQuery(id);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Busca un cliente por su número de cliente único.
    /// </summary>
    /// <param name="clientNumber">Número de cliente (ejemplo: "CLI-20241002-001")</param>
    /// <returns>Datos del cliente con el número especificado</returns>
    [HttpGet("by-number/{clientNumber}")]
    public async Task<IActionResult> GetByNumber(string clientNumber)
    {
        var query = new GetClientByNumberQuery(clientNumber);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Busca un cliente por su número de documento de identidad.
    /// </summary>
    /// <param name="documentNumber">Número de documento (ejemplo: "12345678")</param>
    /// <returns>Datos del cliente con el documento especificado</returns>
    [HttpGet("by-document/{documentNumber}")]
    public async Task<IActionResult> GetByDocument(string documentNumber)
    {
        var query = new GetClientByDocumentQuery(documentNumber);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Valida si un cliente existe y está activo para agendar citas.
    /// </summary>
    /// <param name="clientNumber">Número de cliente a validar</param>
    /// <returns>Resultado de validación con estado del cliente</returns>
    [HttpGet("validate/{clientNumber}")]
    public async Task<IActionResult> ValidateClient(string clientNumber)
    {
        var query = new ValidateClientQuery(clientNumber);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Verifica si un número de cliente ya existe en el sistema.
    /// Útil para validaciones en formularios de registro.
    /// </summary>
    /// <param name="clientNumber">Número de cliente a verificar</param>
    /// <returns>Objeto con propiedad 'exists' indicando si existe</returns>
    [HttpGet("exists/number/{clientNumber}")]
    public async Task<IActionResult> ExistsClientNumber(string clientNumber)
    {
        var query = new GetClientByNumberQuery(clientNumber);
        var result = await Mediator.Send(query);
        return Ok(new { exists = result.IsSuccess });
    }

    /// <summary>
    /// Verifica si un número de documento ya está registrado.
    /// Previene duplicados de documentos de identidad.
    /// </summary>
    /// <param name="documentNumber">Número de documento a verificar</param>
    /// <returns>Objeto con propiedad 'exists' indicando si existe</returns>
    [HttpGet("exists/document/{documentNumber}")]
    public async Task<IActionResult> ExistsDocumentNumber(string documentNumber)
    {
        var query = new GetClientByDocumentQuery(documentNumber);
        var result = await Mediator.Send(query);
        return Ok(new { exists = result.IsSuccess });
    }

    /// <summary>
    /// Verifica si un correo electrónico ya está registrado.
    /// Funcionalidad pendiente de implementación en la capa de aplicación.
    /// </summary>
    /// <param name="email">Correo electrónico a verificar</param>
    /// <returns>Objeto temporal indicando que la validación está pendiente</returns>
    [HttpGet("exists/email/{email}")]
    public async Task<IActionResult> ExistsEmail(string email)
    {
        // This will need to be implemented in the Application layer
        // For now, return a basic response
        return Ok(new { exists = false, message = "Email validation pending implementation" });
    }

    /// <summary>
    /// Crea un nuevo cliente en el sistema.
    /// Genera automáticamente el número de cliente único.
    /// </summary>
    /// <param name="command">Datos del cliente a crear (nombre, documento, contacto, etc.)</param>
    /// <returns>El cliente creado con su ID y número asignados</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateClientCommand command)
    {
        var result = await Mediator.Send(command);
        return CreatedResult(result, "GetClientById", new { id = result.Data?.Id });
    }

    /// <summary>
    /// Actualiza completamente un cliente existente.
    /// Valida que el ID de la URL coincida con el del comando.
    /// </summary>
    /// <param name="id">ID del cliente a actualizar</param>
    /// <param name="command">Comando con los nuevos datos del cliente</param>
    /// <returns>El cliente actualizado</returns>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateClientCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("ID mismatch");
        }

        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Elimina un cliente del sistema.
    /// Realiza eliminación lógica para preservar integridad de datos.
    /// </summary>
    /// <param name="id">ID del cliente a eliminar</param>
    /// <returns>Confirmación de eliminación exitosa</returns>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var command = new DeleteClientCommand(id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Realiza eliminación lógica de un cliente.
    /// Marca el cliente como inactivo sin eliminarlo físicamente de la base de datos.
    /// </summary>
    /// <param name="id">ID del cliente a desactivar</param>
    /// <returns>Confirmación de desactivación exitosa</returns>
    [HttpPatch("delete-logical/{id:int}")]
    public async Task<IActionResult> DeleteLogical(int id)
    {
        var command = new DeleteLogicalClientCommand(id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}