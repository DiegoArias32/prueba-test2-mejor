using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.Common.Models;
using ElectroHuila.Application.Common.Interfaces.Services.Common;
using ElectroHuila.Domain.Entities.Appointments;
using ElectroHuila.Domain.Entities.Clients;
using ElectroHuila.Domain.Enums;
using MediatR;
using ElectroHuila.Application.Contracts.Notifications;
using ElectroHuila.Application.Contracts.Services;

namespace ElectroHuila.Application.Features.Appointments.Commands.ScheduleSimpleAppointment;

/// <summary>
/// Handler para el comando de agendamiento simple de cita.
/// Implementa la lógica de negocio para crear un cliente (si no existe) y agendar su cita en una sola operación.
/// </summary>
public class ScheduleSimpleAppointmentCommandHandler
    : IRequestHandler<ScheduleSimpleAppointmentCommand, Result<ScheduleSimpleAppointmentResponse>>
{
    private readonly IClientRepository _clientRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IBranchRepository _branchRepository;
    private readonly IHolidayRepository _holidayRepository;
    private readonly IUserAssignmentRepository _userAssignmentRepository;
    private readonly IAppointmentNumberGenerator _numberGenerator;
    private readonly ISignalRNotificationService _signalRService;
    private readonly INotificationService _notificationService;

    public ScheduleSimpleAppointmentCommandHandler(
        IClientRepository clientRepository,
        IAppointmentRepository appointmentRepository,
        IBranchRepository branchRepository,
        IHolidayRepository holidayRepository,
        IUserAssignmentRepository userAssignmentRepository,
        IAppointmentNumberGenerator numberGenerator,
        ISignalRNotificationService signalRService,
        INotificationService notificationService)
    {
        _clientRepository = clientRepository;
        _appointmentRepository = appointmentRepository;
        _branchRepository = branchRepository;
        _holidayRepository = holidayRepository;
        _userAssignmentRepository = userAssignmentRepository;
        _numberGenerator = numberGenerator;
        _signalRService = signalRService;
        _notificationService = notificationService;
    }

    /// <summary>
    /// Procesa el comando de agendamiento simple.
    /// Flujo: Validar sucursal → Buscar/Crear cliente → Crear cita → Retornar confirmación
    /// </summary>
    public async Task<Result<ScheduleSimpleAppointmentResponse>> Handle(
        ScheduleSimpleAppointmentCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // ========== PASO 1: VALIDAR SUCURSAL ==========
            var branch = await _branchRepository.GetByIdAsync(request.BranchId);
            if (branch == null)
            {
                return Result.Failure<ScheduleSimpleAppointmentResponse>("La sucursal especificada no existe o no está activa");
            }

            // ========== PASO 2: BUSCAR O CREAR CLIENTE ==========
            var existingClient = await _clientRepository.GetByDocumentNumberAsync(request.DocumentNumber);

            Client client;
            bool isNewClient = false;

            if (existingClient == null)
            {
                // El cliente NO existe - Crear nuevo cliente
                var documentTypeEnum = ParseDocumentType(request.DocumentType);

                client = new Client
                {
                    DocumentType = documentTypeEnum,
                    DocumentNumber = request.DocumentNumber,
                    FullName = request.FullName,
                    Phone = request.Phone,
                    Mobile = request.Mobile,
                    Email = request.Email,
                    Address = request.Address,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Generar ClientNumber único usando el servicio
                client.ClientNumber = await _numberGenerator.GenerateClientNumberAsync();

                // Agregar cliente usando el repositorio (que maneja SaveChanges internamente)
                client = await _clientRepository.AddAsync(client);
                isNewClient = true;
            }
            else
            {
                // El cliente YA existe - Usar el existente
                client = existingClient;
            }

            // ========== PASO 3: VALIDAR FECHA DE CITA ==========
            if (!DateTime.TryParse(request.AppointmentDate, out var appointmentDate))
            {
                return Result.Failure<ScheduleSimpleAppointmentResponse>("La fecha de la cita no es válida");
            }

            // VALIDACIÓN CRÍTICA: Verificar fecha y hora de la cita
            // Verificar si es domingo
            if (appointmentDate.DayOfWeek == DayOfWeek.Sunday)
            {
                return Result.Failure<ScheduleSimpleAppointmentResponse>("No se pueden agendar citas los domingos");
            }

            // Verificar si es festivo
            var isHoliday = await _holidayRepository.IsHolidayAsync(
                appointmentDate,
                request.BranchId,
                cancellationToken);

            if (isHoliday)
            {
                var holiday = await _holidayRepository.GetByDateAsync(
                    appointmentDate,
                    request.BranchId,
                    cancellationToken);

                return Result.Failure<ScheduleSimpleAppointmentResponse>(
                    $"No se pueden agendar citas en días festivos. {holiday?.HolidayName ?? "Día festivo"}");
            }

            // Verificar que la fecha no esté en el pasado
            if (appointmentDate.Date < DateTime.UtcNow.Date)
            {
                return Result.Failure<ScheduleSimpleAppointmentResponse>("No se pueden agendar citas en fechas pasadas");
            }

            // ========== PASO 4: CREAR LA CITA ==========
            var appointment = new Appointment
            {
                ClientId = client.Id,
                BranchId = request.BranchId,
                AppointmentTypeId = request.AppointmentTypeId,
                AppointmentDate = appointmentDate,
                AppointmentTime = request.AppointmentTime,
                Notes = request.Observations,
                StatusId = 1, // 1 = PENDIENTE (estado por defecto)
                IsEnabled = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Generar AppointmentNumber único usando el servicio
            appointment.AppointmentNumber = await _numberGenerator.GenerateAppointmentNumberAsync();

            // Agregar cita usando el repositorio (que maneja SaveChanges internamente)
            appointment = await _appointmentRepository.AddAsync(appointment);

            // Send real-time notification ONLY to users assigned to this appointment type via SignalR
            var assignedUsers = await _userAssignmentRepository.GetByAppointmentTypeIdAsync(appointment.AppointmentTypeId);
            var notificationData = new
            {
                type = "appointment_created",
                data = new
                {
                    id = appointment.Id,
                    appointmentNumber = appointment.AppointmentNumber,
                    clientId = appointment.ClientId,
                    appointmentDate = appointment.AppointmentDate,
                    appointmentTypeId = appointment.AppointmentTypeId,
                    timestamp = DateTime.UtcNow
                }
            };

            foreach (var assignment in assignedUsers.Where(a => a.IsActive))
            {
                await _signalRService.SendNotificationToUserAsync(
                    assignment.UserId.ToString(),
                    notificationData,
                    cancellationToken);
            }

            // Send appointment confirmation notifications (Email, WhatsApp, IN_APP)
            _ = Task.Run(async () =>
            {
                try
                {
                    await _notificationService.SendAppointmentConfirmationAsync(
                        appointment.Id,
                        CancellationToken.None);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error sending confirmation notifications: {ex.Message}");
                }
            });

            // ========== PASO 5: RETORNAR RESPUESTA ==========
            var response = new ScheduleSimpleAppointmentResponse
            {
                ClientNumber = client.ClientNumber,
                AppointmentNumber = appointment.AppointmentNumber,
                Message = isNewClient
                    ? "Cliente creado y cita agendada exitosamente"
                    : "Cita agendada exitosamente",
                AppointmentDate = appointment.AppointmentDate,
                AppointmentTime = appointment.AppointmentTime ?? string.Empty,
                BranchName = branch.Name
            };

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            return Result.Failure<ScheduleSimpleAppointmentResponse>(
                $"Error al agendar la cita: {ex.Message}");
        }
    }

    /// <summary>
    /// Convierte el string de tipo de documento al enum correspondiente
    /// </summary>
    private static DocumentType ParseDocumentType(string documentType)
    {
        return documentType.ToUpper() switch
        {
            "CC" => DocumentType.CC,
            "TI" => DocumentType.TI,
            "CE" => DocumentType.CE,
            "RC" => DocumentType.RC,
            _ => DocumentType.CC // Por defecto CC si no se reconoce
        };
    }
}
