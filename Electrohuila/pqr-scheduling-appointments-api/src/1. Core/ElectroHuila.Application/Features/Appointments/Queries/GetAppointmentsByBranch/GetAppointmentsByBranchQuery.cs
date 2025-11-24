using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.DTOs.Appointments;
using MediatR;

namespace ElectroHuila.Application.Features.Appointments.Queries.GetAppointmentsByBranch;

public record GetAppointmentsByBranchQuery(int BranchId) : IRequest<Result<IEnumerable<AppointmentDto>>>;
