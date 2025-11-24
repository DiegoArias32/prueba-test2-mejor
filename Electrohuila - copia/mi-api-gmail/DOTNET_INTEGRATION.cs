// ============================================================================
// INTEGRACIÓN GMAIL API CON BACKEND .NET - ELECTROHUILA
// ============================================================================

using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// ============================================================================
// 1. MODELOS / DTOs
// ============================================================================

namespace ElectroHuila.EmailService.Models
{
    // Request para confirmación de cita
    public class AppointmentConfirmationRequest
    {
        public string To { get; set; }
        public string Name { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Professional { get; set; }
        public string Location { get; set; }
    }

    // Request para recordatorio de cita
    public class AppointmentReminderRequest
    {
        public string To { get; set; }
        public string Name { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Location { get; set; }
        public string Address { get; set; }
        public string AppointmentNumber { get; set; }
    }

    // Request para cancelación de cita
    public class AppointmentCancellationRequest
    {
        public string To { get; set; }
        public string Name { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Professional { get; set; }
        public string Location { get; set; }
        public string Reason { get; set; }
        public string SchedulingUrl { get; set; }
    }

    // Request para recuperación de contraseña
    public class PasswordResetRequest
    {
        public string To { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }

    // Request para bienvenida
    public class WelcomeEmailRequest
    {
        public string To { get; set; }
        public string Name { get; set; }
        public string DashboardUrl { get; set; }
    }

    // Respuesta genérica de la API
    public class EmailApiResponse
    {
        public bool Ok { get; set; }
        public string Mensaje { get; set; }
        public string MessageId { get; set; }
        public string Error { get; set; }
    }

    // Estadísticas
    public class EmailStats
    {
        public int Total { get; set; }
        public int Success { get; set; }
        public int Failed { get; set; }
        public Dictionary<string, TemplateStats> ByTemplate { get; set; }
    }

    public class TemplateStats
    {
        public int Total { get; set; }
        public int Success { get; set; }
        public int Failed { get; set; }
    }
}

// ============================================================================
// 2. INTERFAZ DEL SERVICIO
// ============================================================================

namespace ElectroHuila.EmailService.Interfaces
{
    public interface IGmailApiService
    {
        Task<bool> EnviarConfirmacionCita(string email, string nombre, DateTime fecha, TimeSpan hora, string profesional, string ubicacion);
        Task<bool> EnviarRecordatorioCita(string email, string nombre, DateTime fecha, TimeSpan hora, string ubicacion, string direccion = null, string numeroCita = null);
        Task<bool> EnviarCancelacionCita(string email, string nombre, DateTime fecha, TimeSpan hora, string profesional, string ubicacion, string motivo = null, string urlAgendamiento = null);
        Task<bool> EnviarRecuperacionPassword(string email, string nombre, string codigo);
        Task<bool> EnviarBienvenida(string email, string nombre, string dashboardUrl = null);
        Task<EmailStats> ObtenerEstadisticas(DateTime? fecha = null);
        Task<bool> VerificarEstado();
    }
}

// ============================================================================
// 3. IMPLEMENTACIÓN DEL SERVICIO
// ============================================================================

namespace ElectroHuila.EmailService.Services
{
    using ElectroHuila.EmailService.Interfaces;
    using ElectroHuila.EmailService.Models;

    public class GmailApiService : IGmailApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GmailApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["GmailApi:ApiKey"];

            // Configurar base URL
            var baseUrl = configuration["GmailApi:BaseUrl"];
            if (!string.IsNullOrEmpty(baseUrl))
            {
                _httpClient.BaseAddress = new Uri(baseUrl);
            }
        }

        // Método privado para enviar requests POST con autenticación
        private async Task<EmailApiResponse> EnviarRequest<T>(string endpoint, T requestData)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
                request.Headers.Add("X-API-Key", _apiKey);
                request.Content = JsonContent.Create(requestData);

                var response = await _httpClient.SendAsync(request);
                var result = await response.Content.ReadFromJsonAsync<EmailApiResponse>();

                return result;
            }
            catch (Exception ex)
            {
                return new EmailApiResponse
                {
                    Ok = false,
                    Error = ex.Message
                };
            }
        }

        public async Task<bool> EnviarConfirmacionCita(
            string email,
            string nombre,
            DateTime fecha,
            TimeSpan hora,
            string profesional,
            string ubicacion)
        {
            var request = new AppointmentConfirmationRequest
            {
                To = email,
                Name = nombre,
                Date = fecha.ToString("dd/MM/yyyy"),
                Time = DateTime.Today.Add(hora).ToString("hh:mm tt"),
                Professional = profesional,
                Location = ubicacion
            };

            var response = await EnviarRequest("/email/appointment-confirmation", request);
            return response?.Ok ?? false;
        }

        public async Task<bool> EnviarRecordatorioCita(
            string email,
            string nombre,
            DateTime fecha,
            TimeSpan hora,
            string ubicacion,
            string direccion = null,
            string numeroCita = null)
        {
            var request = new AppointmentReminderRequest
            {
                To = email,
                Name = nombre,
                Date = fecha.ToString("dd/MM/yyyy"),
                Time = DateTime.Today.Add(hora).ToString("hh:mm tt"),
                Location = ubicacion,
                Address = direccion,
                AppointmentNumber = numeroCita
            };

            var response = await EnviarRequest("/email/appointment-reminder", request);
            return response?.Ok ?? false;
        }

        public async Task<bool> EnviarCancelacionCita(
            string email,
            string nombre,
            DateTime fecha,
            TimeSpan hora,
            string profesional,
            string ubicacion,
            string motivo = null,
            string urlAgendamiento = null)
        {
            var request = new AppointmentCancellationRequest
            {
                To = email,
                Name = nombre,
                Date = fecha.ToString("dd/MM/yyyy"),
                Time = DateTime.Today.Add(hora).ToString("hh:mm tt"),
                Professional = profesional,
                Location = ubicacion,
                Reason = motivo,
                SchedulingUrl = urlAgendamiento ?? "https://electrohuila.com/citas"
            };

            var response = await EnviarRequest("/email/appointment-cancellation", request);
            return response?.Ok ?? false;
        }

        public async Task<bool> EnviarRecuperacionPassword(string email, string nombre, string codigo)
        {
            var request = new PasswordResetRequest
            {
                To = email,
                Name = nombre,
                Code = codigo
            };

            var response = await EnviarRequest("/email/password-reset", request);
            return response?.Ok ?? false;
        }

        public async Task<bool> EnviarBienvenida(string email, string nombre, string dashboardUrl = null)
        {
            var request = new WelcomeEmailRequest
            {
                To = email,
                Name = nombre,
                DashboardUrl = dashboardUrl ?? "https://electrohuila.com/dashboard"
            };

            var response = await EnviarRequest("/email/welcome", request);
            return response?.Ok ?? false;
        }

        public async Task<EmailStats> ObtenerEstadisticas(DateTime? fecha = null)
        {
            try
            {
                var endpoint = "/email/stats";
                if (fecha.HasValue)
                {
                    endpoint += $"?date={fecha.Value:yyyy-MM-dd}";
                }

                var response = await _httpClient.GetAsync(endpoint);
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var result = await response.Content.ReadFromJsonAsync<JsonElement>();
                var stats = result.GetProperty("stats").Deserialize<EmailStats>();

                return stats;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> VerificarEstado()
        {
            try
            {
                var response = await _httpClient.GetAsync("/email/status");
                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }

                var result = await response.Content.ReadFromJsonAsync<EmailApiResponse>();
                return result?.Ok ?? false;
            }
            catch
            {
                return false;
            }
        }
    }
}

// ============================================================================
// 4. CONFIGURACIÓN EN PROGRAM.CS
// ============================================================================

namespace ElectroHuila.Api
{
    using ElectroHuila.EmailService.Interfaces;
    using ElectroHuila.EmailService.Services;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Agregar servicios
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // ============= CONFIGURAR GMAIL API SERVICE =============
            builder.Services.AddHttpClient<IGmailApiService, GmailApiService>((serviceProvider, client) =>
            {
                var config = serviceProvider.GetRequiredService<IConfiguration>();
                var baseUrl = config["GmailApi:BaseUrl"];
                client.BaseAddress = new Uri(baseUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
            });
            // ========================================================

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}

// ============================================================================
// 5. CONFIGURACIÓN EN APPSETTINGS.JSON
// ============================================================================

/*
{
  "GmailApi": {
    "BaseUrl": "http://localhost:4000",
    "ApiKey": "tu-api-key-super-segura-aqui"
  }
}
*/

// ============================================================================
// 6. EJEMPLO DE USO EN CONTROLADOR DE CITAS
// ============================================================================

namespace ElectroHuila.Api.Controllers
{
    using ElectroHuila.EmailService.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api/[controller]")]
    public class CitasController : ControllerBase
    {
        private readonly IGmailApiService _gmailService;
        // private readonly ICitaRepository _citaRepository;

        public CitasController(IGmailApiService gmailService)
        {
            _gmailService = gmailService;
            // _citaRepository = citaRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CrearCita([FromBody] CitaDto cita)
        {
            // 1. Validar datos
            if (string.IsNullOrEmpty(cita.PacienteEmail))
            {
                return BadRequest("Email del paciente es requerido");
            }

            // 2. Guardar cita en base de datos
            // var citaGuardada = await _citaRepository.Guardar(cita);

            // 3. Enviar email de confirmación
            var emailEnviado = await _gmailService.EnviarConfirmacionCita(
                email: cita.PacienteEmail,
                nombre: cita.PacienteNombre,
                fecha: cita.Fecha,
                hora: cita.Hora,
                profesional: cita.ProfesionalNombre,
                ubicacion: cita.Ubicacion
            );

            if (!emailEnviado)
            {
                // Log: Email no pudo enviarse pero la cita sí se creó
                Console.WriteLine($"⚠️ Cita {cita.Id} creada pero email no enviado");
            }

            return Ok(new
            {
                cita = cita,
                emailEnviado = emailEnviado
            });
        }

        [HttpPost("{id}/enviar-recordatorio")]
        public async Task<IActionResult> EnviarRecordatorio(int id)
        {
            // 1. Obtener cita de la base de datos
            // var cita = await _citaRepository.ObtenerPorId(id);

            // Ejemplo de cita (reemplazar con datos reales)
            var cita = new CitaDto
            {
                Id = id,
                PacienteEmail = "paciente@example.com",
                PacienteNombre = "Juan Pérez",
                Fecha = DateTime.Now.AddDays(1),
                Hora = new TimeSpan(10, 0, 0),
                Ubicacion = "Sede Principal",
                NumeroCita = $"APT-{DateTime.Now.Year}-{id:D6}"
            };

            // 2. Enviar recordatorio
            var emailEnviado = await _gmailService.EnviarRecordatorioCita(
                email: cita.PacienteEmail,
                nombre: cita.PacienteNombre,
                fecha: cita.Fecha,
                hora: cita.Hora,
                ubicacion: cita.Ubicacion,
                direccion: "Calle 123 #45-67",
                numeroCita: cita.NumeroCita
            );

            if (!emailEnviado)
            {
                return StatusCode(500, "Error al enviar recordatorio");
            }

            return Ok(new { mensaje = "Recordatorio enviado exitosamente" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelarCita(int id, [FromBody] CancelacionDto cancelacion)
        {
            // 1. Obtener cita
            // var cita = await _citaRepository.ObtenerPorId(id);

            // Ejemplo
            var cita = new CitaDto
            {
                Id = id,
                PacienteEmail = "paciente@example.com",
                PacienteNombre = "Juan Pérez",
                Fecha = DateTime.Now.AddDays(1),
                Hora = new TimeSpan(10, 0, 0),
                ProfesionalNombre = "Dr. María González",
                Ubicacion = "Sede Principal"
            };

            // 2. Cancelar en BD
            // await _citaRepository.Cancelar(id);

            // 3. Enviar email de cancelación
            var emailEnviado = await _gmailService.EnviarCancelacionCita(
                email: cita.PacienteEmail,
                nombre: cita.PacienteNombre,
                fecha: cita.Fecha,
                hora: cita.Hora,
                profesional: cita.ProfesionalNombre,
                ubicacion: cita.Ubicacion,
                motivo: cancelacion.Motivo,
                urlAgendamiento: "https://electrohuila.com/citas"
            );

            return Ok(new
            {
                mensaje = "Cita cancelada exitosamente",
                emailEnviado = emailEnviado
            });
        }
    }

    // DTOs
    public class CitaDto
    {
        public int Id { get; set; }
        public string PacienteEmail { get; set; }
        public string PacienteNombre { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan Hora { get; set; }
        public string ProfesionalNombre { get; set; }
        public string Ubicacion { get; set; }
        public string NumeroCita { get; set; }
    }

    public class CancelacionDto
    {
        public string Motivo { get; set; }
    }
}

// ============================================================================
// 7. EJEMPLO DE USO EN CONTROLADOR DE AUTENTICACIÓN
// ============================================================================

namespace ElectroHuila.Api.Controllers
{
    using ElectroHuila.EmailService.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IGmailApiService _gmailService;
        // private readonly IUserRepository _userRepository;

        public AuthController(IGmailApiService gmailService)
        {
            _gmailService = gmailService;
        }

        [HttpPost("registro")]
        public async Task<IActionResult> Registrar([FromBody] RegistroDto registro)
        {
            // 1. Crear usuario en BD
            // var usuario = await _userRepository.Crear(registro);

            // 2. Enviar email de bienvenida
            var emailEnviado = await _gmailService.EnviarBienvenida(
                email: registro.Email,
                nombre: registro.Nombre,
                dashboardUrl: "https://electrohuila.com/dashboard"
            );

            return Ok(new
            {
                mensaje = "Usuario registrado exitosamente",
                emailEnviado = emailEnviado
            });
        }

        [HttpPost("recuperar-password")]
        public async Task<IActionResult> RecuperarPassword([FromBody] RecuperacionDto recuperacion)
        {
            // 1. Generar código de 6 dígitos
            var codigo = new Random().Next(100000, 999999).ToString();

            // 2. Guardar código en BD (con expiración de 30 minutos)
            // await _userRepository.GuardarCodigoRecuperacion(recuperacion.Email, codigo);

            // 3. Enviar email con código
            var emailEnviado = await _gmailService.EnviarRecuperacionPassword(
                email: recuperacion.Email,
                nombre: "Usuario", // Obtener de BD
                codigo: codigo
            );

            if (!emailEnviado)
            {
                return StatusCode(500, "Error al enviar email de recuperación");
            }

            return Ok(new { mensaje = "Código enviado a tu email" });
        }
    }

    public class RegistroDto
    {
        public string Email { get; set; }
        public string Nombre { get; set; }
        public string Password { get; set; }
    }

    public class RecuperacionDto
    {
        public string Email { get; set; }
    }
}

// ============================================================================
// 8. HEALTH CHECK Y MONITOREO
// ============================================================================

namespace ElectroHuila.Api.Controllers
{
    using ElectroHuila.EmailService.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly IGmailApiService _gmailService;

        public HealthController(IGmailApiService gmailService)
        {
            _gmailService = gmailService;
        }

        [HttpGet]
        public async Task<IActionResult> Check()
        {
            var gmailApiStatus = await _gmailService.VerificarEstado();

            return Ok(new
            {
                status = "healthy",
                timestamp = DateTime.UtcNow,
                services = new
                {
                    gmailApi = gmailApiStatus ? "online" : "offline"
                }
            });
        }

        [HttpGet("email-stats")]
        public async Task<IActionResult> EmailStats([FromQuery] DateTime? fecha = null)
        {
            var stats = await _gmailService.ObtenerEstadisticas(fecha);

            if (stats == null)
            {
                return StatusCode(500, "No se pudieron obtener las estadísticas");
            }

            return Ok(stats);
        }
    }
}

// ============================================================================
// 9. EJEMPLO DE TAREA PROGRAMADA (RECORDATORIOS AUTOMÁTICOS)
// ============================================================================

namespace ElectroHuila.BackgroundServices
{
    using ElectroHuila.EmailService.Interfaces;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class RecordatoriosCitasService : BackgroundService
    {
        private readonly IGmailApiService _gmailService;
        private readonly ILogger<RecordatoriosCitasService> _logger;
        // private readonly ICitaRepository _citaRepository;

        public RecordatoriosCitasService(
            IGmailApiService gmailService,
            ILogger<RecordatoriosCitasService> logger)
        {
            _gmailService = gmailService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Iniciando envío de recordatorios...");

                    // 1. Obtener citas del día siguiente
                    var manana = DateTime.Today.AddDays(1);
                    // var citas = await _citaRepository.ObtenerPorFecha(manana);

                    // 2. Enviar recordatorio a cada paciente
                    // foreach (var cita in citas)
                    // {
                    //     await _gmailService.EnviarRecordatorioCita(
                    //         email: cita.PacienteEmail,
                    //         nombre: cita.PacienteNombre,
                    //         fecha: cita.Fecha,
                    //         hora: cita.Hora,
                    //         ubicacion: cita.Ubicacion,
                    //         numeroCita: cita.NumeroCita
                    //     );
                    // }

                    _logger.LogInformation("Recordatorios enviados exitosamente");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error enviando recordatorios");
                }

                // Ejecutar cada 24 horas
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
    }

    // Registrar en Program.cs:
    // builder.Services.AddHostedService<RecordatoriosCitasService>();
}

// ============================================================================
// FIN DE LA INTEGRACIÓN
// ============================================================================
