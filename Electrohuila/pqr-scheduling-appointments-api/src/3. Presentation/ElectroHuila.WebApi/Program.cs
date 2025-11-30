using ElectroHuila.Application;
using ElectroHuila.Infrastructure;
using ElectroHuila.WebApi.Middleware;
using Microsoft.OpenApi.Models;

/// <summary>
/// Punto de entrada principal de la aplicación ElectroHuila WebAPI.
/// Configura servicios, middleware, autenticación, autorización y Swagger.
/// </summary>

var builder = WebApplication.CreateBuilder(args);


// ========== REGISTRO DE SERVICIOS BASE ==========
// Servicios esenciales de ASP.NET Core
builder.Services.AddControllers(); // Habilita soporte para controladores MVC
builder.Services.AddEndpointsApiExplorer(); // Habilita exploración de endpoints para Swagger
builder.Services.AddHttpContextAccessor(); // Permite inyectar IHttpContextAccessor para acceder al contexto HTTP

// ========== CONFIGURACIÓN DE COMPRESIÓN DE RESPUESTAS (PERFORMANCE) ==========
// Habilita compresión GZIP/Brotli para reducir el tamaño de las respuestas HTTP en 70-80%
// Mejora significativamente el rendimiento en clientes móviles y conexiones lentas
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true; // IMPORTANTE: Habilitar para HTTPS (por defecto está deshabilitado por seguridad)
    options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProvider>();
    options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.BrotliCompressionProvider>();
});

// Configurar nivel de compresión Brotli (más eficiente que GZIP)
builder.Services.Configure<Microsoft.AspNetCore.ResponseCompression.BrotliCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.Fastest; // Fastest para mejor balance performance/compresión
});

// Configurar nivel de compresión GZIP (fallback para clientes que no soporten Brotli)
builder.Services.Configure<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.Fastest; // Fastest para mejor performance
});

// ========== CONFIGURACIÓN DE COMPORTAMIENTO DE API ==========
// Deshabilitar transformación automática de ProblemDetails
// Permite que los controladores retornen formatos de error personalizados sin wrapping automático
builder.Services.Configure<Microsoft.AspNetCore.Mvc.ApiBehaviorOptions>(options =>
{
    // Deshabilitar mapeo automático de errores de cliente a ProblemDetails
    // Esto preserva los formatos de error personalizados como BadRequest(new { error = "..." })
    options.SuppressMapClientErrors = true;
});

// ========== CONFIGURACIÓN DE SIGNALR ==========
// SignalR para notificaciones en tiempo real con autenticación JWT
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true; // Habilitar errores detallados en desarrollo
});

// ========== CONFIGURACIÓN DE MIDDLEWARE PARA SELECCIÓN DINÁMICA DE BD ==========
// Este middleware permite cambiar el proveedor de base de datos mediante el header "X-Database-Provider"
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// ========== CONFIGURACIÓN DE SWAGGER/OPENAPI ==========
// Swagger para documentación interactiva de la API con soporte JWT
builder.Services.AddSwaggerGen(options =>
{
    // Información general de la API - Super Profesional
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1.0.0",
        Title = "ElectroHuila PQR - API de Agendamiento de Citas",
        Description = @"
## Sistema Integral de Gestión de Citas PQR - ElectroHuila

API REST profesional para la gestión completa del sistema de agendamiento de citas, proyectos nuevos, 
y atención de Peticiones, Quejas y Reclamos (PQR) de ElectroHuila.

### Características Principales

- **Multi-Database**: Soporte dinámico para Oracle, SQL Server y PostgreSQL
- **Multi-Sucursal**: Gestión de citas en múltiples sucursales
- **Agendamiento Inteligente**: Sistema de citas con horarios disponibles
- **Gestión de Clientes**: CRUD completo de clientes y documentos
- **Autenticación JWT**: Seguridad mediante tokens Bearer
- **Health Checks**: Monitoreo de conexiones y estado del sistema
- **Temas Personalizables**: Configuración de colores y estilos

### Proveedores de Base de Datos

Utiliza el parámetro `?database=` en cualquier endpoint para seleccionar:
- `oracle` - Oracle Database (AWS RDS) - **Por Defecto**
- `sqlserver` - Microsoft SQL Server
- `postgresql` - PostgreSQL
- `mysql` - MySQL Database

### Documentación Adicional

- PROXIMAMENTE

### Entorno Disponible

- **AWS PROD**: https://8papi9muvp.us-east-2.awsapprunner.com
        "
    });

    // Configuración del servidor (AWS App Runner)
    options.AddServer(new OpenApiServer
    {
        Url = "https://8papi9muvp.us-east-2.awsapprunner.com",
        Description = "AWS App Runner - Producción"
    });

    // Incluir comentarios XML para documentación detallada
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    }

    // Agregar el filtro que añade el parámetro 'database' a todos los endpoints
    options.OperationFilter<ElectroHuila.WebApi.Filters.DatabaseParameterOperationFilter>();

    // Personalización de la UI de Swagger - Ordenar endpoints alfabéticamente
    options.OrderActionsBy(apiDesc => $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.HttpMethod}");

    // Configuración de autenticación JWT en Swagger UI
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = @"
**Autenticación JWT Bearer Token**

Ingresa tu token JWT en el formato: `Bearer {tu-token-aqui}`

**Pasos para autenticarte:**
1. Ejecuta el endpoint `/api/v1/Auth/Login` con tus credenciales
2. Copia el token de la respuesta
3. Haz clic en el botón **'Authorize'** (arriba a la derecha)
4. Pega el token en el formato: `Bearer {token}`
5. Haz clic en **'Authorize'** y luego **'Close'**

**Ejemplo:**
```
Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

Una vez autenticado, podrás acceder a todos los endpoints protegidos.
        "
    });

    // Requerimiento de seguridad global para todos los endpoints
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// ========== REGISTRO DE CAPAS DE LA APLICACIÓN ==========
// Registrar servicios de la capa de aplicación (MediatR, AutoMapper, Validators)
builder.Services.AddApplication();
// Registrar servicios de infraestructura (DbContext, Repositorios, Autenticación)
builder.Services.AddInfrastructure(builder.Configuration);

// ========== CONFIGURACIÓN DE CORS ==========
// Política permisiva de CORS para permitir cualquier origen, método y header
// IMPORTANTE: SignalR requiere AllowCredentials, por lo que no se puede usar AllowAnyOrigin
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",
                "http://localhost:3001",
                "http://localhost:4200",
                "https://localhost:3000",
                "https://localhost:3001",
                "https://localhost:4200",
                "https://master.dvhx1nlzb7qbn.amplifyapp.com", // Dominio correcto de Amplify
                "https://master.d2okx7truwtsji.amplifyapp.com"
              )
              .SetIsOriginAllowedToAllowWildcardSubdomains()
              .AllowAnyMethod()
              .AllowAnyHeader()
              .WithExposedHeaders("*") // Permitir todos los headers de respuesta (requerido para WebSocket)
              .AllowCredentials(); // Requerido para SignalR
    });
});

// ========== CONSTRUCCIÓN DE LA APLICACIÓN ==========
var app = builder.Build();

// ========== CONFIGURACIÓN DE SWAGGER UI ==========
// Habilitar Swagger en todos los entornos
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "ElectroHuila API v1");
    options.RoutePrefix = "swagger"; // Swagger disponible en /swagger
});

// ========== CONFIGURACIÓN DEL PIPELINE DE MIDDLEWARE ==========

// ========== HABILITAR COMPRESIÓN DE RESPUESTAS (DEBE ESTAR PRIMERO) ==========
// IMPORTANTE: UseResponseCompression debe estar ANTES de UseStaticFiles, UseCors, UseRouting
// para que comprima todas las respuestas HTTP (JSON, HTML, etc.)
app.UseResponseCompression();

// Middleware para manejo global de excepciones (debe estar primero para capturar todas las excepciones)
app.UseExceptionHandling();

// Middleware para capturar y cambiar dinámicamente el proveedor de base de datos
app.UseDatabaseProviderSelector();

// ========== CONFIGURACIÓN DE WEBSOCKETS ==========
// Habilitar WebSockets ANTES de CORS y Routing para soporte de SignalR
app.UseWebSockets(new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2),
    AllowedOrigins = {
        "http://localhost:3000",
        "http://localhost:3001",
        "http://localhost:4200",
        "https://localhost:3000",
        "https://localhost:3001",
        "https://localhost:4200"
    }
});

// IMPORTANT: CORS must come BEFORE HttpsRedirection to handle preflight requests
app.UseCors(); // Habilitar CORS

// Only redirect to HTTPS in production
// In development, we use HTTP to avoid certificate issues
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection(); // Redirigir HTTP a HTTPS solo en producción
}

// IMPORTANTE: El orden de Authentication y Authorization es crítico
app.UseAuthentication(); // Autenticación: Identifica al usuario mediante JWT
app.UseAuthorization();  // Autorización: Verifica permisos del usuario autenticado

// ========== MAPEO DE CONTROLADORES ==========
app.MapControllers(); // Registrar todos los controladores de la aplicación

// ========== MAPEO DE SIGNALR HUBS ==========
// Mapear hub de SignalR con autenticación opcional para permitir conexiones
app.MapHub<ElectroHuila.Infrastructure.Hubs.NotificationHub>("/hubs/notifications");

// ========== INICIO DE LA APLICACIÓN ==========
app.Run();