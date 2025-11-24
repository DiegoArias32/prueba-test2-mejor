using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.Contracts.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ElectroHuila.Infrastructure.BackgroundJobs;

/// <summary>
/// Servicio en segundo plano que envía recordatorios automáticos de citas a los clientes.
/// Hereda de <see cref="BackgroundService"/> para ejecutarse como un servicio persistente.
/// </summary>
/// <remarks>
/// Este job se ejecuta cada hora verificando citas que requieren recordatorios.
/// Envía notificaciones por email y SMS a los clientes con citas confirmadas
/// que están programadas para las próximas 24 horas.
/// </remarks>
public class AppointmentReminderJob : BackgroundService
{
    private readonly ILogger<AppointmentReminderJob> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    
    /// <summary>
    /// Intervalo de tiempo entre cada verificación de recordatorios.
    /// </summary>
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1);

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="AppointmentReminderJob"/>.
    /// </summary>
    /// <param name="logger">Logger para registrar eventos y errores del job.</param>
    /// <param name="scopeFactory">Factory para crear scopes de servicios con inyección de dependencias.</param>
    public AppointmentReminderJob(
        ILogger<AppointmentReminderJob> logger,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    /// <summary>
    /// Método principal que ejecuta el servicio en segundo plano de manera continua.
    /// </summary>
    /// <param name="stoppingToken">Token de cancelación para detener el servicio.</param>
    /// <returns>Task que representa la operación asíncrona.</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Appointment Reminder Job started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessReminders();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing appointment reminders");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("Appointment Reminder Job stopped");
    }

    /// <summary>
    /// Procesa y envía recordatorios para todas las citas que califican para recibir notificaciones.
    /// </summary>
    /// <remarks>
    /// Busca citas confirmadas programadas para las próximas 24 horas y envía recordatorios
    /// por email y SMS usando el NotificationService con templates de la base de datos.
    /// </remarks>
    /// <returns>Task que representa la operación asíncrona.</returns>
    private async Task ProcessReminders()
    {
        using var scope = _scopeFactory.CreateScope();
        var appointmentRepository = scope.ServiceProvider.GetRequiredService<IAppointmentRepository>();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
        var settingsRepository = scope.ServiceProvider.GetRequiredService<ISystemSettingRepository>();

        _logger.LogInformation("Checking for appointments requiring reminders...");

        // Obtener configuración de cuántas horas antes enviar recordatorio
        var reminderHoursSetting = await settingsRepository.GetByKeyAsync("APPOINTMENT_REMINDER_HOURS");
        var reminderHours = 24; // Default

        if (reminderHoursSetting != null && int.TryParse(reminderHoursSetting.SettingValue, out var parsedHours))
        {
            reminderHours = parsedHours;
        }

        // Verificar si las notificaciones están habilitadas
        var emailEnabledSetting = await settingsRepository.GetByKeyAsync("EMAIL_NOTIFICATIONS_ENABLED");
        if (emailEnabledSetting != null && bool.TryParse(emailEnabledSetting.SettingValue, out var emailEnabled) && !emailEnabled)
        {
            _logger.LogInformation("Email notifications are disabled, skipping reminder job");
            return;
        }

        var appointments = await appointmentRepository.GetAllAsync();

        // StatusId: 1 = PENDING, 2 = CONFIRMED
        var upcomingAppointments = appointments
            .Where(a => (a.StatusId == 1 || a.StatusId == 2) &&
                       a.AppointmentDate > DateTime.UtcNow &&
                       a.AppointmentDate <= DateTime.UtcNow.AddHours(reminderHours + 1))
            .ToList();

        _logger.LogInformation($"Found {upcomingAppointments.Count} appointments requiring reminders");

        foreach (var appointment in upcomingAppointments)
        {
            try
            {
                // Check if reminder was already sent
                if (appointment.UpdatedAt.HasValue &&
                    appointment.UpdatedAt.Value > DateTime.UtcNow.AddHours(-(reminderHours - 1)))
                {
                    continue; // Skip if reminder was sent recently
                }

                // Usar el NotificationService que utiliza templates de la base de datos
                await notificationService.SendAppointmentReminderAsync(appointment.Id);

                // Update appointment to mark reminder sent
                appointment.UpdatedAt = DateTime.UtcNow;
                await appointmentRepository.UpdateAsync(appointment);

                _logger.LogInformation($"Reminder sent successfully for appointment {appointment.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending reminder for appointment {appointment.Id}");
            }
        }
    }
}