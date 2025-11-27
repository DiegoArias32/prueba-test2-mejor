// ESTE ES UN ARCHIVO DE EJEMPLO - NO REEMPLAZAR NotificationService.cs
// Muestra cómo integrar los servicios de WhatsApp y Gmail en NotificationService

using ElectroHuila.Application.Contracts.Notifications;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.Contracts.Services;
using ElectroHuila.Infrastructure.DTOs.ExternalApis;
using ElectroHuila.Infrastructure.Services.ExternalApis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ElectroHuila.Infrastructure.Services;

/// <summary>
/// EJEMPLO de cómo integrar los servicios externos en NotificationService
/// NOTE: NotificationTemplateRepository was removed - templates are now handled by external services
/// </summary>
public class NotificationServiceExample : INotificationService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IBranchRepository _branchRepository;
    private readonly ISignalRNotificationService _signalRService;
    private readonly IWhatsAppApiService _whatsappService;  // NUEVO
    private readonly IGmailApiService _gmailService;        // NUEVO
    private readonly IConfiguration _configuration;         // NUEVO
    private readonly ILogger<NotificationService> _logger;

    public NotificationServiceExample(
        IAppointmentRepository appointmentRepository,
        IClientRepository clientRepository,
        IBranchRepository branchRepository,
        ISignalRNotificationService signalRService,
        IWhatsAppApiService whatsappService,               // NUEVO - Inyectar servicio
        IGmailApiService gmailService,                     // NUEVO - Inyectar servicio
        IConfiguration configuration,                      // NUEVO - Para leer configuración
        ILogger<NotificationService> logger)
    {
        _appointmentRepository = appointmentRepository;
        _clientRepository = clientRepository;
        _branchRepository = branchRepository;
        _signalRService = signalRService;
        _whatsappService = whatsappService;                // NUEVO
        _gmailService = gmailService;                      // NUEVO
        _configuration = configuration;                    // NUEVO
        _logger = logger;
    }

    public async Task SendAppointmentConfirmationAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
            if (appointment == null)
            {
                _logger.LogWarning("Appointment {AppointmentId} not found for confirmation", appointmentId);
                return;
            }

            var client = await _clientRepository.GetByIdAsync(appointment.ClientId);
            var branch = await _branchRepository.GetByIdAsync(appointment.BranchId);

            if (client == null)
            {
                _logger.LogWarning("Client not found for appointment {AppointmentId}", appointmentId);
                return;
            }

            // PREPARAR DATOS PARA APIS EXTERNAS
            var confirmationData = new AppointmentConfirmationData
            {
                NumeroCita = appointment.AppointmentNumber ?? string.Empty,
                NombreCliente = client.FullName ?? string.Empty,
                Fecha = appointment.AppointmentDate.ToString("yyyy-MM-dd"),
                Hora = appointment.AppointmentDate.ToString("HH:mm"),
                Profesional = "Profesional Asignado", // Obtener del appointment si existe
                Ubicacion = branch?.Name ?? "No especificada",
                Direccion = branch?.Address,
                TipoCita = appointment.AppointmentTypeId.ToString()
            };

            // ENVIAR POR EMAIL (usando Gmail API)
            if (_configuration.GetValue<bool>("Notifications:EnableEmail", true))
            {
                if (!string.IsNullOrEmpty(client.Email))
                {
                    var emailSent = await _gmailService.SendAppointmentConfirmationAsync(
                        client.Email,
                        confirmationData,
                        cancellationToken);

                    if (emailSent)
                    {
                        _logger.LogInformation("Email confirmation sent successfully to {Email}", client.Email);
                    }
                    else
                    {
                        _logger.LogWarning("Failed to send email confirmation to {Email}", client.Email);
                    }
                }
            }

            // ENVIAR POR WHATSAPP
            if (_configuration.GetValue<bool>("Notifications:EnableWhatsApp", true))
            {
                var phoneNumber = GetFormattedPhoneNumber(client);
                if (!string.IsNullOrEmpty(phoneNumber))
                {
                    var whatsappSent = await _whatsappService.SendAppointmentConfirmationAsync(
                        phoneNumber,
                        confirmationData,
                        cancellationToken);

                    if (whatsappSent)
                    {
                        _logger.LogInformation("WhatsApp confirmation sent successfully to {PhoneNumber}", phoneNumber);
                    }
                    else
                    {
                        _logger.LogWarning("Failed to send WhatsApp confirmation to {PhoneNumber}", phoneNumber);
                    }
                }
            }

            // ENVIAR NOTIFICACION EN TIEMPO REAL (SignalR) - Mantener existente
            if (_configuration.GetValue<bool>("Notifications:EnableSignalR", true))
            {
                await _signalRService.SendNotificationToUserAsync(
                    client.Id.ToString(),
                    new
                    {
                        type = "appointment_confirmed",
                        title = "Cita Confirmada",
                        message = $"Tu cita ha sido confirmada para el {appointment.AppointmentDate:dd/MM/yyyy} a las {appointment.AppointmentDate:HH:mm}",
                        data = new
                        {
                            appointmentId = appointment.Id,
                            appointmentNumber = appointment.AppointmentNumber,
                            appointmentDate = appointment.AppointmentDate,
                            branchName = branch?.Name,
                            clientName = client.FullName
                        },
                        timestamp = System.DateTime.UtcNow
                    },
                    cancellationToken
                );
            }

            _logger.LogInformation("All confirmation notifications processed for appointment {AppointmentId}", appointmentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending confirmation notifications for appointment {AppointmentId}", appointmentId);
        }
    }

    public async Task SendAppointmentReminderAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
            if (appointment == null)
            {
                _logger.LogWarning("Appointment {AppointmentId} not found for reminder", appointmentId);
                return;
            }

            var client = await _clientRepository.GetByIdAsync(appointment.ClientId);
            var branch = await _branchRepository.GetByIdAsync(appointment.BranchId);

            if (client == null)
            {
                _logger.LogWarning("Client not found for appointment {AppointmentId}", appointmentId);
                return;
            }

            // PREPARAR DATOS PARA RECORDATORIO
            var reminderData = new AppointmentReminderData
            {
                NombreCliente = client.FullName ?? string.Empty,
                Fecha = appointment.AppointmentDate.ToString("yyyy-MM-dd"),
                Hora = appointment.AppointmentDate.ToString("HH:mm"),
                Ubicacion = branch?.Name ?? "No especificada",
                Direccion = branch?.Address,
                NumeroCita = appointment.AppointmentNumber,
                HorasAntes = 24 // Configurable
            };

            // ENVIAR POR EMAIL
            if (_configuration.GetValue<bool>("Notifications:EnableEmail", true) && !string.IsNullOrEmpty(client.Email))
            {
                await _gmailService.SendAppointmentReminderAsync(client.Email, reminderData, cancellationToken);
            }

            // ENVIAR POR WHATSAPP
            if (_configuration.GetValue<bool>("Notifications:EnableWhatsApp", true))
            {
                var phoneNumber = GetFormattedPhoneNumber(client);
                if (!string.IsNullOrEmpty(phoneNumber))
                {
                    await _whatsappService.SendAppointmentReminderAsync(phoneNumber, reminderData, cancellationToken);
                }
            }

            // ENVIAR POR SIGNALR
            if (_configuration.GetValue<bool>("Notifications:EnableSignalR", true))
            {
                await _signalRService.SendNotificationToUserAsync(
                    client.Id.ToString(),
                    new
                    {
                        type = "appointment_reminder",
                        title = "Recordatorio de Cita",
                        message = $"Recuerda tu cita programada para el {appointment.AppointmentDate:dd/MM/yyyy} a las {appointment.AppointmentDate:HH:mm}",
                        data = new
                        {
                            appointmentId = appointment.Id,
                            appointmentNumber = appointment.AppointmentNumber,
                            appointmentDate = appointment.AppointmentDate,
                            branchName = branch?.Name,
                            branchAddress = branch?.Address,
                            clientName = client.FullName
                        },
                        timestamp = System.DateTime.UtcNow
                    },
                    cancellationToken
                );
            }

            _logger.LogInformation("All reminder notifications processed for appointment {AppointmentId}", appointmentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending reminder notifications for appointment {AppointmentId}", appointmentId);
        }
    }

    public async Task SendAppointmentCancellationAsync(int appointmentId, string reason, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
            if (appointment == null)
            {
                _logger.LogWarning("Appointment {AppointmentId} not found for cancellation", appointmentId);
                return;
            }

            var client = await _clientRepository.GetByIdAsync(appointment.ClientId);
            var branch = await _branchRepository.GetByIdAsync(appointment.BranchId);

            if (client == null)
            {
                _logger.LogWarning("Client not found for appointment {AppointmentId}", appointmentId);
                return;
            }

            // PREPARAR DATOS PARA CANCELACION
            var cancellationData = new AppointmentCancellationData
            {
                NombreCliente = client.FullName ?? string.Empty,
                Fecha = appointment.AppointmentDate.ToString("yyyy-MM-dd"),
                Hora = appointment.AppointmentDate.ToString("HH:mm"),
                Motivo = reason ?? "No especificado",
                UrlReagendar = "https://electrohuila.com/reagendar", // URL de la aplicación
                Ubicacion = branch?.Name,
                NumeroCita = appointment.AppointmentNumber
            };

            // ENVIAR POR EMAIL
            if (_configuration.GetValue<bool>("Notifications:EnableEmail", true) && !string.IsNullOrEmpty(client.Email))
            {
                await _gmailService.SendAppointmentCancellationAsync(client.Email, cancellationData, cancellationToken);
            }

            // ENVIAR POR WHATSAPP
            if (_configuration.GetValue<bool>("Notifications:EnableWhatsApp", true))
            {
                var phoneNumber = GetFormattedPhoneNumber(client);
                if (!string.IsNullOrEmpty(phoneNumber))
                {
                    await _whatsappService.SendAppointmentCancellationAsync(phoneNumber, cancellationData, cancellationToken);
                }
            }

            // ENVIAR POR SIGNALR
            if (_configuration.GetValue<bool>("Notifications:EnableSignalR", true))
            {
                await _signalRService.SendNotificationToUserAsync(
                    client.Id.ToString(),
                    new
                    {
                        type = "appointment_cancelled",
                        title = "Cita Cancelada",
                        message = $"Tu cita del {appointment.AppointmentDate:dd/MM/yyyy} a las {appointment.AppointmentDate:HH:mm} ha sido cancelada",
                        data = new
                        {
                            appointmentId = appointment.Id,
                            appointmentNumber = appointment.AppointmentNumber,
                            appointmentDate = appointment.AppointmentDate,
                            branchName = branch?.Name,
                            clientName = client.FullName,
                            cancellationReason = reason ?? "No especificado"
                        },
                        timestamp = System.DateTime.UtcNow
                    },
                    cancellationToken
                );
            }

            _logger.LogInformation("All cancellation notifications processed for appointment {AppointmentId}", appointmentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending cancellation notifications for appointment {AppointmentId}", appointmentId);
        }
    }

    // METODO HELPER PARA FORMATEAR NUMERO DE TELEFONO
    private string? GetFormattedPhoneNumber(Domain.Entities.Clients.Client client)
    {
        // Usar Mobile primero, si no existe usar Phone
        var phone = client.Mobile ?? client.Phone;

        if (string.IsNullOrEmpty(phone))
            return null;

        // Eliminar espacios y caracteres especiales
        phone = phone.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");

        // Si no comienza con +57, agregarlo (asumiendo Colombia)
        if (!phone.StartsWith("+"))
        {
            phone = "+57" + phone;
        }

        return phone;
    }

    // METODOS ORIGINALES (mantener compatibilidad con interfaz INotificationService)
    public Task<bool> SendEmailAsync(string to, string templateCode, Dictionary<string, string> data, CancellationToken cancellationToken = default)
    {
        // Implementación original o usar _gmailService según sea necesario
        throw new NotImplementedException();
    }

    public Task<bool> SendSmsAsync(string phone, string templateCode, Dictionary<string, string> data, CancellationToken cancellationToken = default)
    {
        // Implementación original
        throw new NotImplementedException();
    }

    public Task<(string subject, string body)> RenderTemplateAsync(string templateCode, Dictionary<string, string> data, CancellationToken cancellationToken = default)
    {
        // Implementación original
        throw new NotImplementedException();
    }
}
