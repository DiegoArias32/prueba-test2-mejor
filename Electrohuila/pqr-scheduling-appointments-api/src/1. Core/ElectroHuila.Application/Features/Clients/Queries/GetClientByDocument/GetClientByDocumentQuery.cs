using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Clients;
using MediatR;

namespace ElectroHuila.Application.Features.Clients.Queries.GetClientByDocument;

public record GetClientByDocumentQuery(string DocumentNumber) : IRequest<Result<ClientDto>>;
