using ElectroHuila.Infrastructure.DTOs.ExternalApis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace ElectroHuila.Infrastructure.Services.ExternalApis;

/// <summary>
/// Implementación del servicio de integración con la API externa de Gmail.
/// Gestiona el envío de correos electrónicos a través de Gmail API.
/// </summary>
public class GmailApiService : IGmailApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GmailApiService> _logger;
    private readonly IConfiguration _configuration;
    private readonly bool _isEnabled;

    public GmailApiService(
        HttpClient httpClient,
        ILogger<GmailApiService> logger,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _configuration = configuration;
        _isEnabled = configuration.GetValue<bool>("ExternalApis:Gmail:Enabled", true);
    }

    /// <inheritdoc />
    public async Task<bool> SendAppointmentConfirmationAsync(
        string email,
        AppointmentConfirmationData data,
        CancellationToken cancellationToken = default)
    {
        if (!_isEnabled)
        {
            _logger.LogWarning("Gmail API is disabled. Skipping appointment confirmation to {Email}", email);
            return false;
        }

        try
        {
            _logger.LogInformation("Sending appointment confirmation email to {Email} for appointment {AppointmentNumber}",
                email, data.NumeroCita);

            var payload = new
            {
                email,
                data
            };

            var response = await _httpClient.PostAsJsonAsync(
                "/gmail/appointment-confirmation",
                payload,
                cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Successfully sent appointment confirmation email to {Email}", email);
                return true;
            }

            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning("Failed to send appointment confirmation email to {Email}. Status: {StatusCode}, Response: {Response}",
                email, response.StatusCode, errorContent);

            return false;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error sending appointment confirmation email to {Email}", email);
            return false;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Timeout sending appointment confirmation email to {Email}", email);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error sending appointment confirmation email to {Email}", email);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<bool> SendAppointmentReminderAsync(
        string email,
        AppointmentReminderData data,
        CancellationToken cancellationToken = default)
    {
        if (!_isEnabled)
        {
            _logger.LogWarning("Gmail API is disabled. Skipping appointment reminder to {Email}", email);
            return false;
        }

        try
        {
            _logger.LogInformation("Sending appointment reminder email to {Email} for {Date} at {Time}",
                email, data.Fecha, data.Hora);

            var payload = new
            {
                email,
                data
            };

            var response = await _httpClient.PostAsJsonAsync(
                "/gmail/appointment-reminder",
                payload,
                cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Successfully sent appointment reminder email to {Email}", email);
                return true;
            }

            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning("Failed to send appointment reminder email to {Email}. Status: {StatusCode}, Response: {Response}",
                email, response.StatusCode, errorContent);

            return false;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error sending appointment reminder email to {Email}", email);
            return false;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Timeout sending appointment reminder email to {Email}", email);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error sending appointment reminder email to {Email}", email);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<bool> SendAppointmentCancellationAsync(
        string email,
        AppointmentCancellationData data,
        CancellationToken cancellationToken = default)
    {
        if (!_isEnabled)
        {
            _logger.LogWarning("Gmail API is disabled. Skipping appointment cancellation to {Email}", email);
            return false;
        }

        try
        {
            _logger.LogInformation("Sending appointment cancellation email to {Email} for {Date} at {Time}",
                email, data.Fecha, data.Hora);

            var payload = new
            {
                email,
                data
            };

            var response = await _httpClient.PostAsJsonAsync(
                "/gmail/appointment-cancellation",
                payload,
                cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Successfully sent appointment cancellation email to {Email}", email);
                return true;
            }

            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning("Failed to send appointment cancellation email to {Email}. Status: {StatusCode}, Response: {Response}",
                email, response.StatusCode, errorContent);

            return false;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error sending appointment cancellation email to {Email}", email);
            return false;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Timeout sending appointment cancellation email to {Email}", email);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error sending appointment cancellation email to {Email}", email);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<bool> SendPasswordResetAsync(
        string email,
        PasswordResetData data,
        CancellationToken cancellationToken = default)
    {
        if (!_isEnabled)
        {
            _logger.LogWarning("Gmail API is disabled. Skipping password reset email to {Email}", email);
            return false;
        }

        try
        {
            _logger.LogInformation("Sending password reset email to {Email}", email);

            var payload = new
            {
                email,
                data
            };

            var response = await _httpClient.PostAsJsonAsync(
                "/gmail/password-reset",
                payload,
                cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Successfully sent password reset email to {Email}", email);
                return true;
            }

            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning("Failed to send password reset email to {Email}. Status: {StatusCode}, Response: {Response}",
                email, response.StatusCode, errorContent);

            return false;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error sending password reset email to {Email}", email);
            return false;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Timeout sending password reset email to {Email}", email);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error sending password reset email to {Email}", email);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<bool> SendWelcomeEmailAsync(
        string email,
        WelcomeData data,
        CancellationToken cancellationToken = default)
    {
        if (!_isEnabled)
        {
            _logger.LogWarning("Gmail API is disabled. Skipping welcome email to {Email}", email);
            return false;
        }

        try
        {
            _logger.LogInformation("Sending welcome email to {Email}", email);

            var payload = new
            {
                email,
                data
            };

            var response = await _httpClient.PostAsJsonAsync(
                "/gmail/welcome",
                payload,
                cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Successfully sent welcome email to {Email}", email);
                return true;
            }

            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning("Failed to send welcome email to {Email}. Status: {StatusCode}, Response: {Response}",
                email, response.StatusCode, errorContent);

            return false;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error sending welcome email to {Email}", email);
            return false;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Timeout sending welcome email to {Email}", email);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error sending welcome email to {Email}", email);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<bool> CheckStatusAsync(CancellationToken cancellationToken = default)
    {
        if (!_isEnabled)
        {
            _logger.LogWarning("Gmail API is disabled");
            return false;
        }

        try
        {
            _logger.LogDebug("Checking Gmail API status");

            var response = await _httpClient.GetAsync("/gmail/status", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Gmail API is available and responding");
                return true;
            }

            _logger.LogWarning("Gmail API returned status code: {StatusCode}", response.StatusCode);
            return false;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error checking Gmail API status");
            return false;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Timeout checking Gmail API status");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error checking Gmail API status");
            return false;
        }
    }
}
