using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.AppointmentTypes;
using MediatR;

namespace ElectroHuila.Application.Features.AppointmentTypes.Commands.CreateAppointmentType;

public record CreateAppointmentTypeCommand(CreateAppointmentTypeDto AppointmentTypeDto) : IRequest<Result<AppointmentTypeDto>>;