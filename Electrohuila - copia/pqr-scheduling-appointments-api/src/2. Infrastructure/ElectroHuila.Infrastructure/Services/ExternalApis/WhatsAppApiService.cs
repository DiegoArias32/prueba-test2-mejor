using ElectroHuila.Infrastructure.DTOs.ExternalApis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;

namespace ElectroHuila.Infrastructure.Services.ExternalApis;

/// <summary>
/// Implementación del servicio de integración con la API externa de WhatsApp.
/// Gestiona el envío de notificaciones de citas a través de WhatsApp.
/// </summary>
public class WhatsAppApiService : IWhatsAppApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WhatsAppApiService> _logger;
    private readonly IConfiguration _configuration;
    private readonly bool _isEnabled;

    public WhatsAppApiService(
        HttpClient httpClient,
        ILogger<WhatsAppApiService> logger,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _configuration = configuration;
        _isEnabled = configuration.GetValue<bool>("ExternalApis:WhatsApp:Enabled", true);
    }

    /// <inheritdoc />
    public async Task<bool> SendAppointmentConfirmationAsync(
        string phoneNumber,
        AppointmentConfirmationData data,
        CancellationToken cancellationToken = default)
    {
        if (!_isEnabled)
        {
            _logger.LogWarning("WhatsApp API is disabled. Skipping appointment confirmation to {PhoneNumber}", phoneNumber);
            return false;
        }

        try
        {
            _logger.LogInformation("Sending appointment confirmation via WhatsApp to {PhoneNumber} for appointment {AppointmentNumber}",
                phoneNumber, data.NumeroCita);

            var payload = new
            {
                phoneNumber,
                data
            };

            var response = await _httpClient.PostAsJsonAsync(
                "/whatsapp/appointment-confirmation",
                payload,
                cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Successfully sent appointment confirmation to {PhoneNumber}", phoneNumber);
                return true;
            }

            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning("Failed to send appointment confirmation to {PhoneNumber}. Status: {StatusCode}, Response: {Response}",
                phoneNumber, response.StatusCode, errorContent);

            return false;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error sending appointment confirmation via WhatsApp to {PhoneNumber}", phoneNumber);
            return false;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Timeout sending appointment confirmation via WhatsApp to {PhoneNumber}", phoneNumber);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error sending appointment confirmation via WhatsApp to {PhoneNumber}", phoneNumber);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<bool> SendAppointmentReminderAsync(
        string phoneNumber,
        AppointmentReminderData data,
        CancellationToken cancellationToken = default)
    {
        if (!_isEnabled)
        {
            _logger.LogWarning("WhatsApp API is disabled. Skipping appointment reminder to {PhoneNumber}", phoneNumber);
            return false;
        }

        try
        {
            _logger.LogInformation("Sending appointment reminder via WhatsApp to {PhoneNumber} for {Date} at {Time}",
                phoneNumber, data.Fecha, data.Hora);

            var payload = new
            {
                phoneNumber,
                data
            };

            var response = await _httpClient.PostAsJsonAsync(
                "/whatsapp/appointment-reminder",
                payload,
                cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Successfully sent appointment reminder to {PhoneNumber}", phoneNumber);
                return true;
            }

            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning("Failed to send appointment reminder to {PhoneNumber}. Status: {StatusCode}, Response: {Response}",
                phoneNumber, response.StatusCode, errorContent);

            return false;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error sending appointment reminder via WhatsApp to {PhoneNumber}", phoneNumber);
            return false;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Timeout sending appointment reminder via WhatsApp to {PhoneNumber}", phoneNumber);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error sending appointment reminder via WhatsApp to {PhoneNumber}", phoneNumber);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<bool> SendAppointmentCancellationAsync(
        string phoneNumber,
        AppointmentCancellationData data,
        CancellationToken cancellationToken = default)
    {
        if (!_isEnabled)
        {
            _logger.LogWarning("WhatsApp API is disabled. Skipping appointment cancellation to {PhoneNumber}", phoneNumber);
            return false;
        }

        try
        {
            _logger.LogInformation("Sending appointment cancellation via WhatsApp to {PhoneNumber} for {Date} at {Time}",
                phoneNumber, data.Fecha, data.Hora);

            var payload = new
            {
                phoneNumber,
                data
            };

            var response = await _httpClient.PostAsJsonAsync(
                "/whatsapp/appointment-cancellation",
                payload,
                cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Successfully sent appointment cancellation to {PhoneNumber}", phoneNumber);
                return true;
            }

            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning("Failed to send appointment cancellation to {PhoneNumber}. Status: {StatusCode}, Response: {Response}",
                phoneNumber, response.StatusCode, errorContent);

            return false;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error sending appointment cancellation via WhatsApp to {PhoneNumber}", phoneNumber);
            return false;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Timeout sending appointment cancellation via WhatsApp to {PhoneNumber}", phoneNumber);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error sending appointment cancellation via WhatsApp to {PhoneNumber}", phoneNumber);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<bool> CheckStatusAsync(CancellationToken cancellationToken = default)
    {
        if (!_isEnabled)
        {
            _logger.LogWarning("WhatsApp API is disabled");
            return false;
        }

        try
        {
            _logger.LogDebug("Checking WhatsApp API status");

            var response = await _httpClient.GetAsync("/whatsapp/status", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("WhatsApp API is available and responding");
                return true;
            }

            _logger.LogWarning("WhatsApp API returned status code: {StatusCode}", response.StatusCode);
            return false;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error checking WhatsApp API status");
            return false;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Timeout checking WhatsApp API status");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error checking WhatsApp API status");
            return false;
        }
    }
}
