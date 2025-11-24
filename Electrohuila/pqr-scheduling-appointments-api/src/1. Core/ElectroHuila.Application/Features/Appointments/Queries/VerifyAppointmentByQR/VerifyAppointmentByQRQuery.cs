using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Appointments.Queries.VerifyAppointmentByQR;

public record VerifyAppointmentByQRQuery(string AppointmentNumber, string ClientNumber) : IRequest<Result<object>>;
