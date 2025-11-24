using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace ElectroHuila.WebApi.Extensions;

/// <summary>
/// Extensiones para configurar servicios de la Web API en el contenedor de dependencias.
/// Centraliza la configuración de controladores, autenticación, autorización y documentación.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configura todos los servicios necesarios para la Web API de ElectroHuila.
    /// Incluye controladores, Swagger, autenticación JWT, autorización y CORS.
    /// </summary>
    /// <param name="services">Colección de servicios del contenedor DI</param>
    /// <param name="configuration">Configuración de la aplicación</param>
    /// <returns>Colección de servicios configurada</returns>
    public static IServiceCollection AddWebApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configuración de controladores y API Explorer
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        
        // Configuración de Swagger para documentación de API
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "ElectroHuila API",
                Version = "v1",
                Description = "API for ElectroHuila Appointment Management System"
            });

            // Configuración de autenticación Bearer en Swagger
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            // Requerimiento de seguridad global para todos los endpoints
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        // Configuración de autenticación JWT
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var jwtSettings = configuration.GetSection("Jwt");
                var key = jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key is not configured");

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        // Configuración de autorización
        services.AddAuthorization();

        // Configuración de CORS para permitir orígenes específicos
        services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigins", policy =>
            {
                policy.WithOrigins(configuration.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>())
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials();
            });
        });

        return services;
    }
}