using ElectroHuila.Application.Common.Interfaces;
using ElectroHuila.Application.Common.Interfaces.Services.External;
using ElectroHuila.Application.Common.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace ElectroHuila.Infrastructure.External.ElectroHuila;

/// <summary>
/// Servicio para integración con la API externa de ElectroHuila.
/// Implementa <see cref="IElectroHuilaApiService"/> para proporcionar comunicación con sistemas externos.
/// </summary>
/// <remarks>
/// Este servicio maneja todas las comunicaciones HTTP con la API de ElectroHuila,
/// incluyendo autenticación mediante API Key, serialización/deserialización JSON,
/// y manejo de errores. Utiliza el patrón Result para encapsular respuestas exitosas y errores.
/// </remarks>
public class ElectroHuilaApiService : IElectroHuilaApiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ElectroHuilaApiService> _logger;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="ElectroHuilaApiService"/>.
    /// </summary>
    /// <param name="httpClient">Cliente HTTP para realizar las peticiones a la API.</param>
    /// <param name="configuration">Configuración de la aplicación que contiene las credenciales de la API.</param>
    /// <param name="logger">Logger para registrar eventos y errores del servicio.</param>
    /// <remarks>
    /// Durante la inicialización se configuran automáticamente:
    /// - La URL base de la API desde la configuración
    /// - La API Key para autenticación en los headers
    /// </remarks>
    public ElectroHuilaApiService(HttpClient httpClient, IConfiguration configuration, ILogger<ElectroHuilaApiService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;

        var baseUrl = _configuration["ExternalServices:ElectroHuila:BaseUrl"];
        var apiKey = _configuration["ExternalServices:ElectroHuila:ApiKey"];

        _httpClient.BaseAddress = new Uri(baseUrl!);
        _httpClient.DefaultRequestHeaders.Add("X-API-Key", apiKey);
    }

    /// <summary>
    /// Realiza una petición HTTP GET a un endpoint específico de la API de ElectroHuila.
    /// </summary>
    /// <typeparam name="T">Tipo de objeto esperado en la respuesta.</typeparam>
    /// <param name="endpoint">Endpoint relativo de la API a consultar.</param>
    /// <returns>
    /// Un <see cref="Result{T}"/> que contiene el objeto deserializado si es exitoso,
    /// o información del error si falla.
    /// </returns>
    /// <remarks>
    /// La deserialización JSON se realiza con configuración case-insensitive.
    /// Los errores HTTP y excepciones se capturan y encapsulan en el Result.
    /// </remarks>
    public async Task<Result<T>> GetAsync<T>(string endpoint)
    {
        try
        {
            var response = await _httpClient.GetAsync(endpoint);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to get data from ElectroHuila API. Endpoint: {Endpoint}, Status: {Status}",
                    endpoint, response.StatusCode);
                return Result.Failure<T>($"API request failed with status: {response.StatusCode}");
            }

            var jsonContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<T>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return Result.Success(result!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling ElectroHuila API endpoint: {Endpoint}", endpoint);
            return Result.Failure<T>($"Error calling API: {ex.Message}");
        }
    }

    /// <summary>
    /// Realiza una petición HTTP POST a un endpoint específico de la API de ElectroHuila.
    /// </summary>
    /// <typeparam name="T">Tipo de objeto esperado en la respuesta.</typeparam>
    /// <param name="endpoint">Endpoint relativo de la API donde enviar los datos.</param>
    /// <param name="data">Objeto que se serializará y enviará en el cuerpo de la petición.</param>
    /// <returns>
    /// Un <see cref="Result{T}"/> que contiene el objeto deserializado si es exitoso,
    /// o información del error si falla.
    /// </returns>
    /// <remarks>
    /// Los datos se serializan automáticamente a JSON con encoding UTF-8.
    /// El Content-Type se establece como "application/json".
    /// </remarks>
    public async Task<Result<T>> PostAsync<T>(string endpoint, object data)
    {
        try
        {
            var jsonContent = JsonSerializer.Serialize(data);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, content);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to post data to ElectroHuila API. Endpoint: {Endpoint}, Status: {Status}",
                    endpoint, response.StatusCode);
                return Result.Failure<T>($"API request failed with status: {response.StatusCode}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return Result.Success(result!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error posting to ElectroHuila API endpoint: {Endpoint}", endpoint);
            return Result.Failure<T>($"Error calling API: {ex.Message}");
        }
    }

    /// <summary>
    /// Valida si un cliente existe en el sistema de ElectroHuila usando su número de documento.
    /// </summary>
    /// <param name="documentNumber">Número de documento del cliente a validar.</param>
    /// <returns>
    /// Un <see cref="Result"/> que indica si la validación fue exitosa o contiene información del error.
    /// </returns>
    public async Task<Result> ValidateClientAsync(string documentNumber)
    {
        try
        {
            var response = await GetAsync<dynamic>($"clients/validate/{documentNumber}");
            return response.IsSuccess ? Result.Success() : Result.Failure(response.Error);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating client: {DocumentNumber}", documentNumber);
            return Result.Failure($"Error validating client: {ex.Message}");
        }
    }

    /// <summary>
    /// Obtiene la información completa de un cliente desde el sistema de ElectroHuila.
    /// </summary>
    /// <param name="documentNumber">Número de documento del cliente a consultar.</param>
    /// <returns>
    /// Un <see cref="Result{T}"/> que contiene la información del cliente si es exitoso,
    /// o información del error si falla.
    /// </returns>
    public async Task<Result<dynamic>> GetClientInfoAsync(string documentNumber)
    {
        try
        {
            var response = await GetAsync<dynamic>($"clients/{documentNumber}");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting client info: {DocumentNumber}", documentNumber);
            return Result.Failure<dynamic>($"Error getting client info: {ex.Message}");
        }
    }

    /// <summary>
    /// Valida si una cuenta existe y está activa en el sistema de ElectroHuila.
    /// </summary>
    /// <param name="accountNumber">Número de cuenta a validar.</param>
    /// <returns>
    /// Un <see cref="Result{T}"/> que contiene la información de validación de la cuenta
    /// o información del error si falla.
    /// </returns>
    public async Task<Result<dynamic>> ValidateAccountAsync(string accountNumber)
    {
        try
        {
            var response = await GetAsync<dynamic>($"accounts/validate/{accountNumber}");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating account: {AccountNumber}", accountNumber);
            return Result.Failure<dynamic>($"Error validating account: {ex.Message}");
        }
    }

    /// <summary>
    /// Obtiene el número de cuenta asociado a un cliente específico.
    /// </summary>
    /// <param name="documentNumber">Número de documento del cliente.</param>
    /// <returns>
    /// Un <see cref="Result{T}"/> que contiene el número de cuenta como string si es exitoso,
    /// o información del error si falla.
    /// </returns>
    public async Task<Result<string>> GetClientAccountAsync(string documentNumber)
    {
        try
        {
            var response = await GetAsync<dynamic>($"clients/{documentNumber}/account");
            if (response.IsSuccess)
            {
                return Result.Success(response.Data.AccountNumber.ToString());
            }
            return Result.Failure<string>(response.Error);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting client account: {DocumentNumber}", documentNumber);
            return Result.Failure<string>($"Error getting client account: {ex.Message}");
        }
    }
}