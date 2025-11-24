using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.AppointmentTypes;
using MediatR;

namespace ElectroHuila.Application.Features.AppointmentTypes.Queries.GetAppointmentTypeById;

public record GetAppointmentTypeByIdQuery(int Id) : IRequest<Result<AppointmentTypeDto>>;