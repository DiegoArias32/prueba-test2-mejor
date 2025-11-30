using ElectroHuila.Application.Common.Interfaces.Persistence;
using ElectroHuila.Application.Common.Interfaces.Services.Common;
using ElectroHuila.Application.Common.Interfaces.Services.Security;
using ElectroHuila.Application.Contracts.Repositories;
using ElectroHuila.Application.Contracts.Services;
using ElectroHuila.Infrastructure.Identity;
using ElectroHuila.Infrastructure.Persistence;
using ElectroHuila.Infrastructure.Persistence.Repositories;
using ElectroHuila.Infrastructure.Persistence.Seeds;
using ElectroHuila.Infrastructure.Services;
using ElectroHuila.Infrastructure.Services.ExternalApis;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ElectroHuila.Infrastructure;

/// <summary>
/// Clase estática que configura la inyección de dependencias para la capa de infraestructura.
/// Registra servicios de persistencia, seguridad, autenticación y acceso a base de datos Oracle.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Agrega los servicios de infraestructura al contenedor de dependencias.
    /// </summary>
    /// <param name="services">Colección de servicios donde se registrarán las dependencias.</param>
    /// <param name="configuration">Configuración de la aplicación con cadenas de conexión y settings.</param>
    /// <returns>La colección de servicios modificada para permitir encadenamiento fluido.</returns>
    /// <remarks>
    /// Este método configura:
    /// 1. Base de datos (Oracle, SQL Server o PostgreSQL): Configura Entity Framework Core dinámicamente
    /// 2. Repositorios: Registra todos los repositorios con patrón Repository para acceso a datos
    /// 3. Servicios de seguridad: JWT token generator, password hasher, token service, current user service
    /// 4. Autenticación JWT: Configura Bearer token con validación de firma, issuer, audience y lifetime
    /// 5. Autorización: Registra el handler de permisos personalizados para control de acceso
    /// </remarks>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // ========== REGISTRO DE HTTP CONTEXT ACCESSOR ==========
        // Requerido por DatabaseProviderService para acceder al HttpContext
        services.AddHttpContextAccessor();

        // ========== OBTENER PROVEEDOR DE BASE DE DATOS ==========
        var databaseProvider = configuration["DatabaseProvider"] ?? "Oracle";

        // ========== CONFIGURACIÓN DE BASE DE DATOS ORACLE (SI APLICA) ==========
        if (databaseProvider.Equals("Oracle", StringComparison.OrdinalIgnoreCase))
        {
            // NOTA: Oracle RDS no requiere wallet. Solo Oracle Cloud Autonomous Database requiere wallet.
            var walletPath = configuration["ApplicationSettings:WalletPath"];
            if (!string.IsNullOrEmpty(walletPath))
            {
                Environment.SetEnvironmentVariable("TNS_ADMIN", walletPath);
            }
        }

        // ========== CONFIGURACIÓN DINÁMICA DE BASE DE DATOS ==========
        string? connectionString = null;

        switch (databaseProvider.ToUpperInvariant())
        {
            case "ORACLE":
                connectionString = configuration.GetConnectionString("OracleConnection");
                break;

            case "SQLSERVER":
                connectionString = configuration.GetConnectionString("SqlServerConnection");
                break;

            case "POSTGRESQL":
                connectionString = configuration.GetConnectionString("PostgreSqlConnection");
                break;

            case "MYSQL":
                connectionString = configuration.GetConnectionString("MySqlConnection");
                break;

            default:
                throw new InvalidOperationException($"Unsupported database provider: {databaseProvider}. Supported providers: Oracle, SqlServer, PostgreSQL, MySQL");
        }

        // Validar cadena de conexión
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException($"{databaseProvider}Connection string is not configured in appsettings.json!");
        }

        // ========== REGISTRO DEL DBCONTEXT FACTORY ==========
        // Registrar el factory que creará DbContext dinámicamente por request
        services.AddScoped<IDbContextFactory, ApplicationDbContextFactory>();

        // ========== REGISTRO DEL DBCONTEXT CON RESOLUCIÓN DINÁMICA ==========
        // El DbContext se crea usando el factory en cada request
        services.AddScoped<ApplicationDbContext>(serviceProvider =>
        {
            var factory = serviceProvider.GetRequiredService<IDbContextFactory>();
            return factory.CreateDbContext();
        });

        // Registrar Unit of Work
        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());

        // ========== REGISTRO DE REPOSITORIOS ==========
        // Todos los repositorios se registran con alcance Scoped (una instancia por request HTTP)
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<IAppointmentTypeRepository, AppointmentTypeRepository>();
        services.AddScoped<IAvailableTimeRepository, AvailableTimeRepository>();
        services.AddScoped<IBranchRepository, BranchRepository>();
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IFormRepository, FormRepository>();
        services.AddScoped<IModuleRepository, ModuleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IRolRepository, RolRepository>();
        services.AddScoped<IRolFormPermissionRepository, RolFormPermissionRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        // ========== REPOSITORIOS DE CATÁLOGOS ==========
        // Repositorios para las tablas de catálogo (enums convertidos a tablas)
        services.AddScoped<IAppointmentStatusRepository, AppointmentStatusRepository>();
        // services.AddScoped<IDocumentTypeRepository, DocumentTypeRepository>(); // TODO: Implementar cuando se necesite
        services.AddScoped<IProjectTypeRepository, ProjectTypeRepository>();
        services.AddScoped<IPropertyTypeRepository, PropertyTypeRepository>();
        services.AddScoped<IServiceUseTypeRepository, ServiceUseTypeRepository>();

        // ========== REPOSITORIOS DE CONFIGURACIÓN ==========
        // Repositorios para configuración de la aplicación
        services.AddScoped<IThemeSettingsRepository, ThemeSettingsRepository>();
        services.AddScoped<ISystemSettingRepository, SystemSettingRepository>();

        // ========== REPOSITORIOS DE NOTIFICACIONES ==========
        // Repositorio para gestión de notificaciones
        services.AddScoped<INotificationRepository, NotificationRepository>();

        // ========== REPOSITORIOS DE FESTIVOS Y DOCUMENTOS ==========
        // Repositorios para festivos y documentos adjuntos a citas
        services.AddScoped<IHolidayRepository, HolidayRepository>();
        services.AddScoped<IAppointmentDocumentRepository, AppointmentDocumentRepository>();

        // ========== REPOSITORIOS DE ASIGNACIONES ==========
        // Repositorio para asignaciones de usuarios a tipos de cita
        services.AddScoped<IUserAssignmentRepository, UserAssignmentRepository>();

        // ========== SERVICIOS DE INICIALIZACIÓN ==========
        // Servicio para inicialización de datos base
        services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();

        // ========== SERVICIOS DE SEGURIDAD ==========
        // Servicios para autenticación, autorización y manejo de contraseñas
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // ========== SERVICIO DE GENERACIÓN DE NÚMEROS ==========
        // Servicio para generar números únicos de citas
        services.AddScoped<IAppointmentNumberGenerator, AppointmentNumberGenerator>();

        // ========== SERVICIO DE PROVEEDOR DE BASE DE DATOS ==========
        // Servicio para gestionar el proveedor de base de datos dinámicamente
        // IMPORTANTE: Debe ser Scoped para permitir cambios por request HTTP
        services.AddScoped<IDatabaseProviderService, DatabaseProviderService>();

        // ========== SERVICIO DE NOTIFICACIONES ==========
        // Servicio para envío de notificaciones (Email, SMS, Push) usando templates
        services.AddScoped<INotificationService, NotificationService>();

        // Servicio de notificaciones en tiempo real con SignalR
        services.AddScoped<ElectroHuila.Application.Contracts.Notifications.ISignalRNotificationService, SignalRNotificationService>();

        // ========== HTTPCLIENTS PARA APIS EXTERNAS ==========
        // HttpClient para WhatsApp API con configuración de timeout y base URL
        services.AddHttpClient<IWhatsAppApiService, WhatsAppApiService>(client =>
        {
            var baseUrl = configuration["ExternalApis:WhatsApp:BaseUrl"] ?? "http://localhost:3000";
            client.BaseAddress = new Uri(baseUrl);

            var timeoutSeconds = configuration.GetValue<int>("ExternalApis:WhatsApp:TimeoutSeconds", 30);
            client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);

            // Agregar API Key si está configurada
            var apiKey = configuration["ExternalApis:WhatsApp:ApiKey"];
            if (!string.IsNullOrEmpty(apiKey))
            {
                client.DefaultRequestHeaders.Add("X-API-Key", apiKey);
            }
        });

        // HttpClient para Gmail API con configuración de timeout y base URL
        services.AddHttpClient<IGmailApiService, GmailApiService>(client =>
        {
            var baseUrl = configuration["ExternalApis:Gmail:BaseUrl"] ?? "http://localhost:4000";
            client.BaseAddress = new Uri(baseUrl);

            var timeoutSeconds = configuration.GetValue<int>("ExternalApis:Gmail:TimeoutSeconds", 30);
            client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
        });

        // ========== CONFIGURACIÓN DE AUTENTICACIÓN JWT ==========
        // Obtener configuración JWT desde appsettings.json
        var jwtKey = configuration["Jwt:Key"];
        var jwtIssuer = configuration["Jwt:Issuer"];
        var jwtAudience = configuration["Jwt:Audience"];

        // Configurar esquema de autenticación Bearer
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!)),
                ValidateIssuer = true,
                ValidIssuer = jwtIssuer,
                ValidateAudience = true,
                ValidAudience = jwtAudience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero // Sin tolerancia de tiempo para tokens expirados
            };

            // Configuración para SignalR: Permitir token desde query string
            // SignalR no puede enviar headers en WebSocket, así que el token viene en ?access_token=xxx
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];

                    // Si la petición es para un hub de SignalR y tiene token en query string
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                    {
                        // Leer el token desde la query string
                        context.Token = accessToken;
                    }

                    return Task.CompletedTask;
                }
            };
        });

        // ========== CONFIGURACIÓN DE AUTORIZACIÓN ==========
        // Registrar servicios de autorización y handler personalizado para permisos
        services.AddAuthorization();
        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

        return services;
    }
}