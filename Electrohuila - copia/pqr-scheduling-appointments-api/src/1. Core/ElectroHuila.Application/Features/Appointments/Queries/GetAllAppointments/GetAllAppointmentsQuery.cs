using ElectroHuila.Application.DTOs.Appointments;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Appointments.Queries.GetAllAppointments;

public record GetAllAppointmentsQuery(int PageNumber = 1, int PageSize = 10) : IRequest<Result<PagedList<AppointmentDto>>>;