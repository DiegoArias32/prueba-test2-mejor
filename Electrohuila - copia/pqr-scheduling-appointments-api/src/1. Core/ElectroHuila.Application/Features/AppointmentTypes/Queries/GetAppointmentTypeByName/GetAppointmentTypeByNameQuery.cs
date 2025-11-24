using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.AppointmentTypes;
using MediatR;

namespace ElectroHuila.Application.Features.AppointmentTypes.Queries.GetAppointmentTypeByName;

public record GetAppointmentTypeByNameQuery(string Name) : IRequest<Result<AppointmentTypeDto>>;