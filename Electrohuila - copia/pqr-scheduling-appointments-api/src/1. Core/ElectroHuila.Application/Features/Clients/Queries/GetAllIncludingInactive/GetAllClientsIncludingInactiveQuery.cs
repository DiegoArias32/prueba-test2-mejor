using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Clients;
using MediatR;

namespace ElectroHuila.Application.Features.Clients.Queries.GetAllIncludingInactive;

/// <summary>
/// Query para obtener todos los clientes incluyendo los inactivos.
/// </summary>
public record GetAllClientsIncludingInactiveQuery : IRequest<Result<IEnumerable<ClientDto>>>;
