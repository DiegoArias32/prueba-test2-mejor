using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.Common.Models;
using MediatR;

namespace ElectroHuila.Application.Features.Appointments.Queries.VerifyAppointmentByQR;

public class VerifyAppointmentByQRQueryHandler : IRequestHandler<VerifyAppointmentByQRQuery, Result<object>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IClientRepository _clientRepository;

    public VerifyAppointmentByQRQueryHandler(
        IAppointmentRepository appointmentRepository,
        IClientRepository clientRepository)
    {
        _appointmentRepository = appointmentRepository;
        _clientRepository = clientRepository;
    }

    public async Task<Result<object>> Handle(VerifyAppointmentByQRQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.AppointmentNumber))
                return Result.Failure<object>("Número de cita requerido");

            if (string.IsNullOrWhiteSpace(request.ClientNumber))
                return Result.Failure<object>("Número de cliente requerido");

            // Get all appointments and find by appointment number
            var allAppointments = await _appointmentRepository.GetAllAsync();
            var appointment = allAppointments.FirstOrDefault(a => a.AppointmentNumber == request.AppointmentNumber);

            if (appointment == null)
            {
                return Result.Failure<object>("Cita no encontrada");
            }

            var clientEntity = await _clientRepository.GetByIdAsync(appointment.ClientId);
            if (clientEntity?.ClientNumber != request.ClientNumber)
            {
                return Result.Failure<object>("Cita no válida para este cliente");
            }

            // StatusIds: 1=PENDING, 4=COMPLETED, 5=CANCELLED
            const int PENDING_STATUS_ID = 1;
            const int COMPLETED_STATUS_ID = 4;
            const int CANCELLED_STATUS_ID = 5;

            var isValid = appointment.IsEnabled && appointment.StatusId != CANCELLED_STATUS_ID;
            var statusCode = appointment.Status?.Code ?? "UNKNOWN";
            var statusName = appointment.Status?.Name ?? "Desconocido";

            var statusDescription = appointment.StatusId switch
            {
                PENDING_STATUS_ID => "Cita programada, pendiente de asistir",
                COMPLETED_STATUS_ID => "Cita completada exitosamente",
                CANCELLED_STATUS_ID => "Cita cancelada",
                _ => $"Estado: {statusName}"
            };

            var verification = new
            {
                isValid = isValid,
                appointmentNumber = appointment.AppointmentNumber,
                appointmentDate = appointment.AppointmentDate.ToString("yyyy-MM-dd"),
                appointmentTime = appointment.AppointmentTime,
                status = statusCode,
                statusDescription = statusDescription,
                client = new
                {
                    clientNumber = clientEntity.ClientNumber,
                    fullName = clientEntity.FullName
                },
                branch = appointment.Branch != null ? new
                {
                    id = appointment.Branch.Id,
                    name = appointment.Branch.Name,
                    address = appointment.Branch.Address,
                    phone = appointment.Branch.Phone,
                    city = appointment.Branch.City
                } : null,
                appointmentType = appointment.AppointmentType != null ? new
                {
                    id = appointment.AppointmentType.Id,
                    name = appointment.AppointmentType.Name,
                    description = appointment.AppointmentType.Description,
                    code = appointment.AppointmentType.Code
                } : null,
                creationDate = appointment.CreatedAt.ToString(),
                observations = appointment.Notes,
                message = isValid ? "Cita verificada correctamente" : "Cita encontrada pero no está activa"
            };

            return Result.Success<object>(verification);
        }
        catch (Exception ex)
        {
            return Result.Failure<object>($"Error al verificar cita por QR: {ex.Message}");
        }
    }
}
