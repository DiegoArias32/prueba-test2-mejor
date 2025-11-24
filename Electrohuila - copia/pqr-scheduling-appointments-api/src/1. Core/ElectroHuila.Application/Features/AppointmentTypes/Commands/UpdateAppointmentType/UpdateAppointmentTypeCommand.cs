using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.AppointmentTypes;
using MediatR;

namespace ElectroHuila.Application.Features.AppointmentTypes.Commands.UpdateAppointmentType;

public record UpdateAppointmentTypeCommand(int Id, UpdateAppointmentTypeDto AppointmentTypeDto) : IRequest<Result<AppointmentTypeDto>>;