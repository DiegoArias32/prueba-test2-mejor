using ElectroHuila.Application.Contracts.Notifications;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.Contracts.Services;
using ElectroHuila.Domain.Entities.Notifications;
using ElectroHuila.Infrastructure.DTOs.ExternalApis;
using ElectroHuila.Infrastructure.Services.ExternalApis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ElectroHuila.Infrastructure.Services;

/// <summary>
/// Implementación del servicio de notificaciones
/// </summary>
public class NotificationService : INotificationService
{
    private readonly INotificationTemplateRepository _templateRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IBranchRepository _branchRepository;
    private readonly ISignalRNotificationService _signalRService;
    private readonly IWhatsAppApiService _whatsAppService;
    private readonly IGmailApiService _gmailService;
    private readonly INotificationRepository _notificationRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<NotificationService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public NotificationService(
        INotificationTemplateRepository templateRepository,
        IAppointmentRepository appointmentRepository,
        IClientRepository clientRepository,
        IBranchRepository branchRepository,
        ISignalRNotificationService signalRService,
        IWhatsAppApiService whatsAppService,
        IGmailApiService gmailService,
        INotificationRepository notificationRepository,
        IConfiguration configuration,
        ILogger<NotificationService> logger,
        IServiceProvider serviceProvider)
    {
        _templateRepository = templateRepository;
        _appointmentRepository = appointmentRepository;
        _clientRepository = clientRepository;
        _branchRepository = branchRepository;
        _signalRService = signalRService;
        _whatsAppService = whatsAppService;
        _gmailService = gmailService;
        _notificationRepository = notificationRepository;
        _configuration = configuration;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task SendAppointmentConfirmationAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        // IMPORTANTE: Crear un NUEVO scope de DbContext para evitar conflictos con el DbContext
        // que creó la cita. Esto evita el NullReferenceException de Oracle.
        using var scope = _serviceProvider.CreateScope();
        var appointmentRepo = scope.ServiceProvider.GetRequiredService<IAppointmentRepository>();
        var clientRepo = scope.ServiceProvider.GetRequiredService<IClientRepository>();
        var branchRepo = scope.ServiceProvider.GetRequiredService<IBranchRepository>();
        var appointmentTypeRepo = scope.ServiceProvider.GetRequiredService<IAppointmentTypeRepository>();
        var notificationRepo = scope.ServiceProvider.GetRequiredService<INotificationRepository>();

        try
        {
            // 1. Obtener datos con el NUEVO DbContext (sin conflictos)
            var appointment = await appointmentRepo.GetByIdAsync(appointmentId);
            if (appointment == null)
            {
                _logger.LogWarning("Appointment {AppointmentId} not found for confirmation", appointmentId);
                return;
            }

            var client = await clientRepo.GetByIdAsync(appointment.ClientId);
            var branch = await branchRepo.GetByIdAsync(appointment.BranchId);
            var appointmentType = await appointmentTypeRepo.GetByIdAsync(appointment.AppointmentTypeId);

            if (client == null)
            {
                _logger.LogWarning("Client not found for appointment {AppointmentId}", appointmentId);
                return;
            }

            _logger.LogInformation("Processing confirmation notifications for appointment {AppointmentId}, client {ClientId}",
                appointmentId, client.Id);

            // Preparar datos para las APIs externas con todos los campos mejorados
            var confirmationData = new AppointmentConfirmationData
            {
                // Campos requeridos
                NumeroCita = appointment.AppointmentNumber ?? string.Empty,
                NombreCliente = client.FullName ?? string.Empty,
                Fecha = appointment.AppointmentDate.ToString("yyyy-MM-dd"),
                Hora = appointment.AppointmentTime ?? "00:00", // AppointmentTime es string "HH:mm"
                Profesional = "Técnico ElectroHuila",
                Ubicacion = branch?.Name ?? "No especificada",

                // Campos opcionales originales
                Direccion = branch?.Address ?? string.Empty,
                TipoCita = appointmentType?.Name ?? "No especificado",

                // Campos adicionales para WhatsApp mejorado
                ClienteId = client.ClientNumber ?? client.Id.ToString(),
                Telefono = client.Mobile ?? client.Phone ?? string.Empty,
                DireccionCliente = client.Address ?? string.Empty,
                Observaciones = appointment.Notes ?? string.Empty,
                QrUrl = null // TODO: Generar URL del código QR si se implementa
            };

            // 2. Enviar EMAIL
            if (!string.IsNullOrEmpty(client.Email))
            {
                var emailNotification = Notification.Create(
                    type: "EMAIL",
                    title: "Cita Confirmada",
                    message: $"Tu cita para el {appointment.AppointmentDate:dd/MM/yyyy} a las {appointment.AppointmentDate:HH:mm} ha sido confirmada",
                    clientId: client.Id,
                    appointmentId: appointmentId
                );

                await notificationRepo.CreateAsync(emailNotification, cancellationToken);
                _logger.LogInformation("EMAIL notification record created with ID {NotificationId} for client {ClientId}",
                    emailNotification.Id, client.Id);

                try
                {
                    var emailSent = await _gmailService.SendAppointmentConfirmationAsync(
                        client.Email,
                        confirmationData,
                        cancellationToken
                    );

                    if (emailSent)
                    {
                        emailNotification.MarkAsSent();
                        _logger.LogInformation("Email confirmation sent successfully to {Email} for appointment {AppointmentId}",
                            client.Email, appointmentId);
                    }
                    else
                    {
                        emailNotification.MarkAsFailed("No se pudo enviar el email");
                        _logger.LogWarning("Failed to send email confirmation to {Email} for appointment {AppointmentId}",
                            client.Email, appointmentId);
                    }
                }
                catch (Exception ex)
                {
                    emailNotification.MarkAsFailed(ex.Message);
                    _logger.LogError(ex, "Error sending email confirmation to {Email} for appointment {AppointmentId}",
                        client.Email, appointmentId);
                }
                finally
                {
                    await notificationRepo.UpdateAsync(emailNotification, cancellationToken);
                    _logger.LogDebug("EMAIL notification {NotificationId} updated with status {Status}",
                        emailNotification.Id, emailNotification.Status);
                }
            }

            // 3. Enviar WHATSAPP (si está habilitado)
            if (_configuration.GetValue<bool>("ExternalApis:WhatsApp:Enabled", false))
            {
                var phoneNumber = GetFormattedPhoneNumber(client);
                if (!string.IsNullOrEmpty(phoneNumber))
                {
                    var whatsappNotification = Notification.Create(
                        type: "WHATSAPP",
                        title: "Cita Confirmada",
                        message: $"Tu cita para el {appointment.AppointmentDate:dd/MM/yyyy} a las {appointment.AppointmentDate:HH:mm} ha sido confirmada",
                        clientId: client.Id,
                        appointmentId: appointmentId
                    );

                    await notificationRepo.CreateAsync(whatsappNotification, cancellationToken);
                    _logger.LogInformation("WHATSAPP notification record created with ID {NotificationId} for client {ClientId}",
                        whatsappNotification.Id, client.Id);

                    try
                    {
                        var whatsappSent = await _whatsAppService.SendAppointmentConfirmationAsync(
                            phoneNumber,
                            confirmationData,
                            cancellationToken
                        );

                        if (whatsappSent)
                        {
                            whatsappNotification.MarkAsSent();
                            _logger.LogInformation("WhatsApp confirmation sent successfully to {Phone} for appointment {AppointmentId}",
                                phoneNumber, appointmentId);
                        }
                        else
                        {
                            whatsappNotification.MarkAsFailed("No se pudo enviar el mensaje de WhatsApp");
                            _logger.LogWarning("Failed to send WhatsApp confirmation to {Phone} for appointment {AppointmentId}",
                                phoneNumber, appointmentId);
                        }
                    }
                    catch (Exception ex)
                    {
                        whatsappNotification.MarkAsFailed(ex.Message);
                        _logger.LogError(ex, "Error sending WhatsApp confirmation to {Phone} for appointment {AppointmentId}",
                            phoneNumber, appointmentId);
                    }
                    finally
                    {
                        await notificationRepo.UpdateAsync(whatsappNotification, cancellationToken);
                        _logger.LogDebug("WHATSAPP notification {NotificationId} updated with status {Status}",
                            whatsappNotification.Id, whatsappNotification.Status);
                    }
                }
                else
                {
                    _logger.LogWarning("No valid phone number found for client {ClientId}, skipping WhatsApp notification", client.Id);
                }
            }

            // 4. Crear notificaciones IN_APP para usuarios asignados al tipo de cita
            var userAssignmentRepo = scope.ServiceProvider.GetRequiredService<IUserAssignmentRepository>();
            var assignedUsers = await userAssignmentRepo.GetByAppointmentTypeIdAsync(appointment.AppointmentTypeId);

            foreach (var assignment in assignedUsers.Where(a => a.IsActive))
            {
                try
                {
                    var inAppNotification = Notification.Create(
                        type: "IN_APP",
                        title: "Nueva Cita Creada",
                        message: $"Nueva cita #{appointment.AppointmentNumber} para {client.FullName} el {appointment.AppointmentDate:dd/MM/yyyy} a las {appointment.AppointmentTime}",
                        userId: assignment.UserId,
                        appointmentId: appointmentId
                    );

                    inAppNotification.MarkAsSent(); // IN_APP notifications are always "sent" immediately
                    await notificationRepo.CreateAsync(inAppNotification, cancellationToken);

                    _logger.LogInformation("IN_APP notification created for user {UserId} about appointment {AppointmentId}",
                        assignment.UserId, appointmentId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating IN_APP notification for user {UserId}", assignment.UserId);
                }
            }

            // NOTE: NO creamos notificaciones IN_APP para clientes porque:
            // - Los clientes NO son usuarios (no tienen UserId)
            // - Los clientes NO tienen portal web para ver notificaciones IN_APP
            // - Las notificaciones IN_APP son solo para ADMINS (tabla USERS)
            // - Los clientes reciben confirmaciones por Email y WhatsApp solamente

            _logger.LogInformation("All confirmation notifications processed for appointment {AppointmentId}", appointmentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing confirmation notifications for appointment {AppointmentId}", appointmentId);
        }
    }

    public async Task SendAppointmentReminderAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        var appointmentRepo = scope.ServiceProvider.GetRequiredService<IAppointmentRepository>();
        var clientRepo = scope.ServiceProvider.GetRequiredService<IClientRepository>();
        var branchRepo = scope.ServiceProvider.GetRequiredService<IBranchRepository>();
        var notificationRepo = scope.ServiceProvider.GetRequiredService<INotificationRepository>();

        try
        {
            // 1. Obtener datos de la cita
            var appointment = await appointmentRepo.GetByIdAsync(appointmentId);
            if (appointment == null)
            {
                _logger.LogWarning("Appointment {AppointmentId} not found for reminder", appointmentId);
                return;
            }

            var client = await clientRepo.GetByIdAsync(appointment.ClientId);
            var branch = await branchRepo.GetByIdAsync(appointment.BranchId);

            if (client == null)
            {
                _logger.LogWarning("Client not found for appointment {AppointmentId}", appointmentId);
                return;
            }

            _logger.LogInformation("Processing reminder notifications for appointment {AppointmentId}, client {ClientId}",
                appointmentId, client.Id);

            // Calcular horas antes de la cita
            var hoursUntilAppointment = (appointment.AppointmentDate - System.DateTime.Now).TotalHours;
            var reminderData = new AppointmentReminderData
            {
                NombreCliente = client.FullName ?? string.Empty,
                Fecha = appointment.AppointmentDate.ToString("yyyy-MM-dd"),
                Hora = appointment.AppointmentTime ?? "00:00",
                Ubicacion = branch?.Name ?? "No especificada",
                Direccion = branch?.Address ?? string.Empty,
                NumeroCita = appointment.AppointmentNumber,
                HorasAntes = (int)hoursUntilAppointment
            };

            // 2. Enviar EMAIL
            if (!string.IsNullOrEmpty(client.Email))
            {
                var emailNotification = Notification.Create(
                    type: "EMAIL",
                    title: "Recordatorio de Cita",
                    message: $"Recuerda tu cita programada para el {appointment.AppointmentDate:dd/MM/yyyy} a las {appointment.AppointmentDate:HH:mm}",
                    clientId: client.Id,
                    appointmentId: appointmentId
                );

                await notificationRepo.CreateAsync(emailNotification, cancellationToken);
                _logger.LogInformation("EMAIL reminder notification record created with ID {NotificationId} for client {ClientId}",
                    emailNotification.Id, client.Id);

                try
                {
                    var emailSent = await _gmailService.SendAppointmentReminderAsync(
                        client.Email,
                        reminderData,
                        cancellationToken
                    );

                    if (emailSent)
                    {
                        emailNotification.MarkAsSent();
                        _logger.LogInformation("Email reminder sent successfully to {Email} for appointment {AppointmentId}",
                            client.Email, appointmentId);
                    }
                    else
                    {
                        emailNotification.MarkAsFailed("No se pudo enviar el email de recordatorio");
                        _logger.LogWarning("Failed to send email reminder to {Email} for appointment {AppointmentId}",
                            client.Email, appointmentId);
                    }
                }
                catch (Exception ex)
                {
                    emailNotification.MarkAsFailed(ex.Message);
                    _logger.LogError(ex, "Error sending email reminder to {Email} for appointment {AppointmentId}",
                        client.Email, appointmentId);
                }
                finally
                {
                    await notificationRepo.UpdateAsync(emailNotification, cancellationToken);
                    _logger.LogDebug("EMAIL reminder notification {NotificationId} updated with status {Status}",
                        emailNotification.Id, emailNotification.Status);
                }
            }

            // 3. Enviar WHATSAPP (si está habilitado)
            if (_configuration.GetValue<bool>("ExternalApis:WhatsApp:Enabled", false))
            {
                var phoneNumber = GetFormattedPhoneNumber(client);
                if (!string.IsNullOrEmpty(phoneNumber))
                {
                    var whatsappNotification = Notification.Create(
                        type: "WHATSAPP",
                        title: "Recordatorio de Cita",
                        message: $"Recuerda tu cita programada para el {appointment.AppointmentDate:dd/MM/yyyy} a las {appointment.AppointmentDate:HH:mm}",
                        clientId: client.Id,
                        appointmentId: appointmentId
                    );

                    await notificationRepo.CreateAsync(whatsappNotification, cancellationToken);
                    _logger.LogInformation("WHATSAPP reminder notification record created with ID {NotificationId} for client {ClientId}",
                        whatsappNotification.Id, client.Id);

                    try
                    {
                        var whatsappSent = await _whatsAppService.SendAppointmentReminderAsync(
                            phoneNumber,
                            reminderData,
                            cancellationToken
                        );

                        if (whatsappSent)
                        {
                            whatsappNotification.MarkAsSent();
                            _logger.LogInformation("WhatsApp reminder sent successfully to {Phone} for appointment {AppointmentId}",
                                phoneNumber, appointmentId);
                        }
                        else
                        {
                            whatsappNotification.MarkAsFailed("No se pudo enviar el mensaje de WhatsApp");
                            _logger.LogWarning("Failed to send WhatsApp reminder to {Phone} for appointment {AppointmentId}",
                                phoneNumber, appointmentId);
                        }
                    }
                    catch (Exception ex)
                    {
                        whatsappNotification.MarkAsFailed(ex.Message);
                        _logger.LogError(ex, "Error sending WhatsApp reminder to {Phone} for appointment {AppointmentId}",
                            phoneNumber, appointmentId);
                    }
                    finally
                    {
                        await notificationRepo.UpdateAsync(whatsappNotification, cancellationToken);
                        _logger.LogDebug("WHATSAPP reminder notification {NotificationId} updated with status {Status}",
                            whatsappNotification.Id, whatsappNotification.Status);
                    }
                }
                else
                {
                    _logger.LogWarning("No valid phone number found for client {ClientId}, skipping WhatsApp reminder", client.Id);
                }
            }

            // NOTE: NO creamos notificaciones IN_APP para clientes porque:
            // - Los clientes NO son usuarios (no tienen UserId)
            // - Los clientes NO tienen portal web para ver notificaciones IN_APP
            // - Las notificaciones IN_APP son solo para ADMINS (tabla USERS)
            // - Los clientes reciben recordatorios por Email y WhatsApp solamente

            _logger.LogInformation("All reminder notifications processed for appointment {AppointmentId}", appointmentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing reminder notifications for appointment {AppointmentId}", appointmentId);
        }
    }

    public async Task SendAppointmentCancellationAsync(int appointmentId, string reason, CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        var appointmentRepo = scope.ServiceProvider.GetRequiredService<IAppointmentRepository>();
        var clientRepo = scope.ServiceProvider.GetRequiredService<IClientRepository>();
        var branchRepo = scope.ServiceProvider.GetRequiredService<IBranchRepository>();
        var notificationRepo = scope.ServiceProvider.GetRequiredService<INotificationRepository>();

        try
        {
            // 1. Obtener datos de la cita
            var appointment = await appointmentRepo.GetByIdAsync(appointmentId);
            if (appointment == null)
            {
                _logger.LogWarning("Appointment {AppointmentId} not found for cancellation", appointmentId);
                return;
            }

            var client = await clientRepo.GetByIdAsync(appointment.ClientId);
            var branch = await branchRepo.GetByIdAsync(appointment.BranchId);

            if (client == null)
            {
                _logger.LogWarning("Client not found for appointment {AppointmentId}", appointmentId);
                return;
            }

            _logger.LogInformation("Processing cancellation notifications for appointment {AppointmentId}, client {ClientId}",
                appointmentId, client.Id);

            var cancellationData = new AppointmentCancellationData
            {
                NombreCliente = client.FullName ?? string.Empty,
                Fecha = appointment.AppointmentDate.ToString("yyyy-MM-dd"),
                Hora = appointment.AppointmentTime ?? "00:00",
                Motivo = reason ?? "No especificado",
                UrlReagendar = _configuration["ExternalApis:RescheduleUrl"] ?? "https://electrohuila.com/reagendar",
                Ubicacion = branch?.Name,
                NumeroCita = appointment.AppointmentNumber
            };

            // 2. Enviar EMAIL
            if (!string.IsNullOrEmpty(client.Email))
            {
                var emailNotification = Notification.Create(
                    type: "EMAIL",
                    title: "Cita Cancelada",
                    message: $"Tu cita del {appointment.AppointmentDate:dd/MM/yyyy} a las {appointment.AppointmentDate:HH:mm} ha sido cancelada. Motivo: {reason ?? "No especificado"}",
                    clientId: client.Id,
                    appointmentId: appointmentId
                );

                await notificationRepo.CreateAsync(emailNotification, cancellationToken);
                _logger.LogInformation("EMAIL cancellation notification record created with ID {NotificationId} for client {ClientId}",
                    emailNotification.Id, client.Id);

                try
                {
                    var emailSent = await _gmailService.SendAppointmentCancellationAsync(
                        client.Email,
                        cancellationData,
                        cancellationToken
                    );

                    if (emailSent)
                    {
                        emailNotification.MarkAsSent();
                        _logger.LogInformation("Email cancellation sent successfully to {Email} for appointment {AppointmentId}",
                            client.Email, appointmentId);
                    }
                    else
                    {
                        emailNotification.MarkAsFailed("No se pudo enviar el email de cancelación");
                        _logger.LogWarning("Failed to send email cancellation to {Email} for appointment {AppointmentId}",
                            client.Email, appointmentId);
                    }
                }
                catch (Exception ex)
                {
                    emailNotification.MarkAsFailed(ex.Message);
                    _logger.LogError(ex, "Error sending email cancellation to {Email} for appointment {AppointmentId}",
                        client.Email, appointmentId);
                }
                finally
                {
                    await notificationRepo.UpdateAsync(emailNotification, cancellationToken);
                    _logger.LogDebug("EMAIL cancellation notification {NotificationId} updated with status {Status}",
                        emailNotification.Id, emailNotification.Status);
                }
            }

            // 3. Enviar WHATSAPP (si está habilitado)
            if (_configuration.GetValue<bool>("ExternalApis:WhatsApp:Enabled", false))
            {
                var phoneNumber = GetFormattedPhoneNumber(client);
                if (!string.IsNullOrEmpty(phoneNumber))
                {
                    var whatsappNotification = Notification.Create(
                        type: "WHATSAPP",
                        title: "Cita Cancelada",
                        message: $"Tu cita del {appointment.AppointmentDate:dd/MM/yyyy} a las {appointment.AppointmentDate:HH:mm} ha sido cancelada. Motivo: {reason ?? "No especificado"}",
                        clientId: client.Id,
                        appointmentId: appointmentId
                    );

                    await notificationRepo.CreateAsync(whatsappNotification, cancellationToken);
                    _logger.LogInformation("WHATSAPP cancellation notification record created with ID {NotificationId} for client {ClientId}",
                        whatsappNotification.Id, client.Id);

                    try
                    {
                        var whatsappSent = await _whatsAppService.SendAppointmentCancellationAsync(
                            phoneNumber,
                            cancellationData,
                            cancellationToken
                        );

                        if (whatsappSent)
                        {
                            whatsappNotification.MarkAsSent();
                            _logger.LogInformation("WhatsApp cancellation sent successfully to {Phone} for appointment {AppointmentId}",
                                phoneNumber, appointmentId);
                        }
                        else
                        {
                            whatsappNotification.MarkAsFailed("No se pudo enviar el mensaje de WhatsApp");
                            _logger.LogWarning("Failed to send WhatsApp cancellation to {Phone} for appointment {AppointmentId}",
                                phoneNumber, appointmentId);
                        }
                    }
                    catch (Exception ex)
                    {
                        whatsappNotification.MarkAsFailed(ex.Message);
                        _logger.LogError(ex, "Error sending WhatsApp cancellation to {Phone} for appointment {AppointmentId}",
                            phoneNumber, appointmentId);
                    }
                    finally
                    {
                        await notificationRepo.UpdateAsync(whatsappNotification, cancellationToken);
                        _logger.LogDebug("WHATSAPP cancellation notification {NotificationId} updated with status {Status}",
                            whatsappNotification.Id, whatsappNotification.Status);
                    }
                }
                else
                {
                    _logger.LogWarning("No valid phone number found for client {ClientId}, skipping WhatsApp cancellation", client.Id);
                }
            }

            // NOTE: NO creamos notificaciones IN_APP para clientes porque:
            // - Los clientes NO son usuarios (no tienen UserId)
            // - Los clientes NO tienen portal web para ver notificaciones IN_APP
            // - Las notificaciones IN_APP son solo para ADMINS (tabla USERS)
            // - Los clientes reciben cancelaciones por Email y WhatsApp solamente

            _logger.LogInformation("All cancellation notifications processed for appointment {AppointmentId}", appointmentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing cancellation notifications for appointment {AppointmentId}", appointmentId);
        }
    }

    public async Task<bool> SendEmailAsync(string to, string templateCode, Dictionary<string, string> data, CancellationToken cancellationToken = default)
    {
        try
        {
            var template = await _templateRepository.GetByCodeAsync(templateCode, cancellationToken);
            if (template == null)
            {
                _logger.LogWarning("Email template {TemplateCode} not found", templateCode);
                return false;
            }

            if (template.TemplateType != "EMAIL")
            {
                _logger.LogWarning("Template {TemplateCode} is not an EMAIL template", templateCode);
                return false;
            }

            var subject = ReplacePlaceholders(template.Subject ?? string.Empty, data);
            var body = ReplacePlaceholders(template.BodyTemplate, data);

            // TODO: Implementar integración con servicio de email (SendGrid, AWS SES, etc.)
            _logger.LogInformation("Email would be sent to {To} with subject: {Subject}", to, subject);
            _logger.LogDebug("Email body: {Body}", body);

            // Por ahora solo logeamos, pero aquí iría la integración real
            // await _emailService.SendAsync(to, subject, body);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email to {To} with template {TemplateCode}", to, templateCode);
            return false;
        }
    }

    public async Task<bool> SendSmsAsync(string phone, string templateCode, Dictionary<string, string> data, CancellationToken cancellationToken = default)
    {
        try
        {
            var template = await _templateRepository.GetByCodeAsync(templateCode, cancellationToken);
            if (template == null)
            {
                _logger.LogWarning("SMS template {TemplateCode} not found", templateCode);
                return false;
            }

            if (template.TemplateType != "SMS")
            {
                _logger.LogWarning("Template {TemplateCode} is not an SMS template", templateCode);
                return false;
            }

            var message = ReplacePlaceholders(template.BodyTemplate, data);

            // TODO: Implementar integración con servicio de SMS (Twilio, AWS SNS, etc.)
            _logger.LogInformation("SMS would be sent to {Phone}: {Message}", phone, message);

            // Por ahora solo logeamos, pero aquí iría la integración real
            // await _smsService.SendAsync(phone, message);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending SMS to {Phone} with template {TemplateCode}", phone, templateCode);
            return false;
        }
    }

    public async Task<(string subject, string body)> RenderTemplateAsync(string templateCode, Dictionary<string, string> data, CancellationToken cancellationToken = default)
    {
        var template = await _templateRepository.GetByCodeAsync(templateCode, cancellationToken);
        if (template == null)
        {
            throw new InvalidOperationException($"Template {templateCode} not found");
        }

        var subject = ReplacePlaceholders(template.Subject ?? string.Empty, data);
        var body = ReplacePlaceholders(template.BodyTemplate, data);

        return (subject, body);
    }

    private static Dictionary<string, string> BuildAppointmentData(
        Domain.Entities.Appointments.Appointment appointment,
        Domain.Entities.Clients.Client client,
        Domain.Entities.Locations.Branch? branch)
    {
        return new Dictionary<string, string>
        {
            ["CLIENT_NAME"] = client.FullName ?? string.Empty,
            ["CLIENT_EMAIL"] = client.Email ?? string.Empty,
            ["CLIENT_PHONE"] = client.Phone ?? client.Mobile ?? string.Empty,
            ["APPOINTMENT_NUMBER"] = appointment.AppointmentNumber ?? string.Empty,
            ["APPOINTMENT_DATE"] = appointment.AppointmentDate.ToString("dd/MM/yyyy"),
            ["APPOINTMENT_TIME"] = appointment.AppointmentDate.ToString("HH:mm"),
            ["APPOINTMENT_TYPE"] = appointment.AppointmentTypeId.ToString(),
            ["BRANCH_NAME"] = branch?.Name ?? "No especificada",
            ["BRANCH_ADDRESS"] = branch?.Address ?? string.Empty,
            ["BRANCH_PHONE"] = branch?.Phone ?? string.Empty
        };
    }

    private static string ReplacePlaceholders(string template, Dictionary<string, string> data)
    {
        var result = template;
        foreach (var kvp in data)
        {
            var placeholder = $"{{{{{kvp.Key}}}}}"; // {{KEY}}
            result = result.Replace(placeholder, kvp.Value);
        }
        return result;
    }

    /// <summary>
    /// Formatea el número de teléfono del cliente para WhatsApp (formato internacional)
    /// </summary>
    private static string? GetFormattedPhoneNumber(Domain.Entities.Clients.Client client)
    {
        // Usar Mobile primero, si no existe usar Phone
        var phone = client.Mobile ?? client.Phone;

        if (string.IsNullOrEmpty(phone))
            return null;

        // Eliminar espacios y caracteres especiales
        phone = phone.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "").Replace("+", "");

        // Si tiene 10 dígitos (número local colombiano), agregar código de país
        if (phone.Length == 10 && phone.StartsWith("3"))
        {
            phone = "57" + phone; // Código de Colombia
        }

        // Agregar el prefijo +
        if (!phone.StartsWith("+"))
        {
            phone = "+" + phone;
        }

        return phone;
    }
}
