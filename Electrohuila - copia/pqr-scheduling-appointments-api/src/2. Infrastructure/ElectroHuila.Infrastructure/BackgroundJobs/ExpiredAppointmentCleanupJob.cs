using ElectroHuila.Application.Contracts.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ElectroHuila.Infrastructure.BackgroundJobs;

/// <summary>
/// Servicio en segundo plano que gestiona la limpieza automática de citas expiradas.
/// Hereda de <see cref="BackgroundService"/> para ejecutarse como un servicio persistente.
/// </summary>
/// <remarks>
/// Este job se ejecuta cada 6 horas verificando citas confirmadas que han expirado
/// (2 horas después de su fecha programada) y las marca como "NoShow" automáticamente.
/// Esto ayuda a mantener la integridad de los datos y liberar espacios no utilizados.
/// </remarks>
public class ExpiredAppointmentCleanupJob : BackgroundService
{
    private readonly ILogger<ExpiredAppointmentCleanupJob> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    
    /// <summary>
    /// Intervalo de tiempo entre cada verificación de citas expiradas.
    /// </summary>
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(6);
    
    /// <summary>
    /// Tiempo de tolerancia después de la fecha de la cita para considerarla expirada.
    /// </summary>
    private readonly TimeSpan _expirationThreshold = TimeSpan.FromHours(2);

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="ExpiredAppointmentCleanupJob"/>.
    /// </summary>
    /// <param name="logger">Logger para registrar eventos y errores del job.</param>
    /// <param name="scopeFactory">Factory para crear scopes de servicios con inyección de dependencias.</param>
    public ExpiredAppointmentCleanupJob(
        ILogger<ExpiredAppointmentCleanupJob> logger,
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
    /// <remarks>
    /// Incluye un retraso inicial de 1 minuto antes de la primera ejecución para permitir
    /// que otros servicios se inicialicen correctamente.
    /// </remarks>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Expired Appointment Cleanup Job started");

        // Wait 1 minute before first execution
        await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupExpiredAppointments();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up expired appointments");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("Expired Appointment Cleanup Job stopped");
    }

    /// <summary>
    /// Procesa y marca como "NoShow" todas las citas confirmadas que han expirado.
    /// </summary>
    /// <remarks>
    /// Una cita se considera expirada cuando:
    /// - Su estado es "CONFIRMED" (StatusId = 2)
    /// - Ha pasado más tiempo del umbral de expiración (2 horas) desde su fecha programada
    ///
    /// Las citas expiradas se marcan como "NO_SHOW" (StatusId = 6) y se actualiza su timestamp.
    /// </remarks>
    /// <returns>Task que representa la operación asíncrona.</returns>
    private async Task CleanupExpiredAppointments()
    {
        using var scope = _scopeFactory.CreateScope();
        var appointmentRepository = scope.ServiceProvider.GetRequiredService<IAppointmentRepository>();

        _logger.LogInformation("Checking for expired appointments...");

        var appointments = await appointmentRepository.GetAllAsync();

        // StatusId: 2 = CONFIRMED, 6 = NO_SHOW (según AppointmentStatuses)
        const int CONFIRMED_STATUS_ID = 2;
        const int NO_SHOW_STATUS_ID = 6;

        var expiredAppointments = appointments
            .Where(a => a.StatusId == CONFIRMED_STATUS_ID &&
                       a.AppointmentDate < DateTime.UtcNow.Subtract(_expirationThreshold))
            .ToList();

        if (expiredAppointments.Count == 0)
        {
            _logger.LogInformation("No expired appointments found");
            return;
        }

        _logger.LogInformation($"Found {expiredAppointments.Count} expired appointments to mark as no-show");

        foreach (var appointment in expiredAppointments)
        {
            try
            {
                appointment.StatusId = NO_SHOW_STATUS_ID;
                appointment.UpdatedAt = DateTime.UtcNow;

                await appointmentRepository.UpdateAsync(appointment);

                _logger.LogInformation(
                    $"Appointment {appointment.Id} (Number: {appointment.AppointmentNumber}) marked as no-show");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    $"Error marking appointment {appointment.Id} as no-show");
            }
        }

        _logger.LogInformation($"Expired appointments cleanup completed. Processed {expiredAppointments.Count} appointments");
    }

    /// <summary>
    /// Método llamado cuando el servicio está siendo detenido.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación para la operación de detención.</param>
    /// <returns>Task que representa la operación asíncrona de detención.</returns>
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Expired Appointment Cleanup Job is stopping");
        await base.StopAsync(cancellationToken);
    }
}