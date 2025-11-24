# 🗄️ Sistema de Selección Dinámica de Base de Datos

## 📋 Tabla de Contenidos

1. [Descripción General](#descripción-general)
2. [Arquitectura del Sistema](#arquitectura-del-sistema)
3. [Componentes Implementados](#componentes-implementados)
4. [Cómo Funciona](#cómo-funciona)
5. [Guía de Uso](#guía-de-uso)
6. [Configuración](#configuración)
7. [Health Check Endpoints](#health-check-endpoints)
8. [Troubleshooting](#troubleshooting)

---

## Descripción General

El sistema de **Selección Dinámica de Base de Datos** permite cambiar el proveedor de base de datos **por solicitud HTTP** sin necesidad de reiniciar la aplicación. Soporta tres proveedores:

- **Oracle** (por defecto) - AWS RDS
- **SQL Server** - Microsoft SQL Server
- **PostgreSQL** - PostgreSQL

### Características Principales

✅ **Cambio dinámico por request**: Cada solicitud HTTP puede usar un proveedor diferente
✅ **Sin reinicio**: No requiere reiniciar la aplicación
✅ **Interfaz Swagger**: Lista desplegable automática en todos los endpoints
✅ **Health Check**: Endpoints dedicados para verificar conexiones
✅ **Factory Pattern**: Implementación robusta con creación dinámica de DbContext
✅ **Middleware Pipeline**: Captura la selección antes de la resolución de dependencias
✅ **Multi-entorno**: Configuración independiente para DEV, STAGING, QA y MAIN

---

## Arquitectura del Sistema

### Diagrama de Flujo

```
┌─────────────────────────────────────────────────────────────────┐
│                         HTTP Request                            │
│              GET /api/v1/Appointments?database=sqlserver        │
└────────────────────────────┬────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                  DatabaseProviderMiddleware                     │
│  - Captura parámetro ?database= o header X-Database-Provider   │
│  - Valida el valor (oracle|sqlserver|postgresql)               │
│  - Almacena en HttpContext.Items["DatabaseProvider"]           │
└────────────────────────────┬────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                 Dependency Injection Container                  │
│         Resuelve IDbContextFactory (Scoped Service)             │
└────────────────────────────┬────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│              ApplicationDbContextFactory                        │
│  1. Lee DatabaseProviderService.CurrentProvider                 │
│  2. Obtiene connection string correspondiente                   │
│  3. Crea DbContextOptionsBuilder según proveedor               │
│  4. Retorna nueva instancia de ApplicationDbContext            │
└────────────────────────────┬────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│              DatabaseProviderService (Scoped)                   │
│  Prioridad de lectura:                                          │
│  1. Override manual (_overrideProvider)                         │
│  2. HttpContext.Items["DatabaseProvider"]                       │
│  3. Configuration["DatabaseProvider"] (appsettings.json)        │
└────────────────────────────┬────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                   ApplicationDbContext                          │
│      Conectado al proveedor seleccionado dinámicamente          │
└─────────────────────────────────────────────────────────────────┘
```

### Patrón de Diseño

El sistema utiliza el **Factory Pattern** combinado con **Middleware Pipeline**:

1. **Middleware**: Intercepta la request antes de DI resolution
2. **Factory**: Crea DbContext bajo demanda con configuración dinámica
3. **Scoped Services**: Cada request tiene su propio contexto
4. **HttpContext.Items**: Almacén de estado por request

---

## Componentes Implementados

### 1. DatabaseProvider Enum

**Ubicación**: `src/2. Infrastructure/ElectroHuila.Infrastructure/Services/DatabaseProvider.cs`

```csharp
public enum DatabaseProvider
{
    Oracle,
    SqlServer,
    PostgreSQL
}
```

**Propósito**: Define los proveedores de base de datos soportados.

---

### 2. IDatabaseProviderService Interface

**Ubicación**: `src/2. Infrastructure/ElectroHuila.Infrastructure/Services/IDatabaseProviderService.cs`

```csharp
public interface IDatabaseProviderService
{
    DatabaseProvider CurrentProvider { get; }
    string GetConnectionString();
    void SetProvider(DatabaseProvider provider);
}
```

**Propósito**: Contrato para el servicio que gestiona el proveedor actual.

---

### 3. DatabaseProviderService Implementation

**Ubicación**: `src/2. Infrastructure/ElectroHuila.Infrastructure/Services/DatabaseProviderService.cs`

```csharp
public class DatabaseProviderService : IDatabaseProviderService
{
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private DatabaseProvider? _overrideProvider;

    public DatabaseProvider CurrentProvider
    {
        get
        {
            // Prioridad 1: Override manual
            if (_overrideProvider.HasValue)
                return _overrideProvider.Value;

            // Prioridad 2: HttpContext.Items (desde middleware)
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.Items.ContainsKey("DatabaseProvider") == true)
            {
                var providerFromContext = httpContext.Items["DatabaseProvider"]?.ToString();
                var parsed = ParseProvider(providerFromContext);
                if (parsed.HasValue)
                    return parsed.Value;
            }

            // Prioridad 3: Configuración (appsettings.json)
            var providerFromConfig = _configuration["DatabaseProvider"];
            return ParseProvider(providerFromConfig) ?? DatabaseProvider.Oracle;
        }
    }

    public string GetConnectionString()
    {
        return CurrentProvider switch
        {
            DatabaseProvider.Oracle => _configuration.GetConnectionString("OracleConnection"),
            DatabaseProvider.SqlServer => _configuration.GetConnectionString("SqlServerConnection"),
            DatabaseProvider.PostgreSQL => _configuration.GetConnectionString("PostgreSqlConnection"),
            _ => throw new InvalidOperationException($"Unsupported database provider: {CurrentProvider}")
        };
    }

    public void SetProvider(DatabaseProvider provider)
    {
        _overrideProvider = provider;
    }

    private static DatabaseProvider? ParseProvider(string? providerString)
    {
        if (string.IsNullOrWhiteSpace(providerString))
            return null;

        return providerString.ToLowerInvariant() switch
        {
            "oracle" => DatabaseProvider.Oracle,
            "sqlserver" => DatabaseProvider.SqlServer,
            "postgresql" => DatabaseProvider.PostgreSQL,
            _ => null
        };
    }
}
```

**Características**:
- **Scoped Lifetime**: Una instancia por request HTTP
- **Lectura jerárquica**: Override → HttpContext → Configuration
- **Thread-safe**: Usa HttpContext.Items por request
- **Parsing flexible**: Acepta strings en cualquier formato

---

### 4. DatabaseProviderMiddleware

**Ubicación**: `src/3. Presentation/ElectroHuila.WebApi/Middleware/DatabaseProviderMiddleware.cs`

```csharp
public class DatabaseProviderMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<DatabaseProviderMiddleware> _logger;

    public async Task InvokeAsync(HttpContext context)
    {
        string? providerString = null;

        // 1. Desde query parameter 'database'
        if (context.Request.Query.TryGetValue("database", out var dbFromQuery))
        {
            providerString = dbFromQuery.ToString();
            _logger.LogInformation("Database provider from query parameter: {Provider}", providerString);
        }
        // 2. Desde header 'X-Database-Provider'
        else if (context.Request.Headers.TryGetValue("X-Database-Provider", out var dbFromHeader))
        {
            providerString = dbFromHeader.ToString();
            _logger.LogInformation("Database provider from header: {Provider}", providerString);
        }

        // Si se especificó un proveedor, almacenarlo en HttpContext.Items
        if (!string.IsNullOrEmpty(providerString))
        {
            var provider = ParseDatabaseProvider(providerString);
            if (provider.HasValue)
            {
                context.Items["DatabaseProvider"] = provider.Value.ToString();
                _logger.LogInformation("Database provider set to {Provider} for this request", provider.Value);
            }
            else
            {
                _logger.LogWarning("Invalid database provider specified: {Provider}", providerString);
            }
        }

        await _next(context);
    }

    private static DatabaseProvider? ParseDatabaseProvider(string providerString)
    {
        return providerString.ToLowerInvariant() switch
        {
            "oracle" => DatabaseProvider.Oracle,
            "sqlserver" => DatabaseProvider.SqlServer,
            "postgresql" => DatabaseProvider.PostgreSQL,
            _ => null
        };
    }
}
```

**Características**:
- **Posición en pipeline**: Antes de routing y endpoint execution
- **Dos fuentes**: Query parameter (`?database=`) o Header (`X-Database-Provider`)
- **Validación**: Solo acepta valores válidos
- **Logging**: Registra cada cambio de proveedor

**Registro en Program.cs**:
```csharp
app.UseDatabaseProviderSelector(); // Después de app.UseRouting()
```

---

### 5. IDbContextFactory Interface

**Ubicación**: `src/2. Infrastructure/ElectroHuila.Infrastructure/Persistence/IDbContextFactory.cs`

```csharp
public interface IDbContextFactory
{
    ApplicationDbContext CreateDbContext();
}
```

**Propósito**: Define el contrato para la fábrica de DbContext.

---

### 6. ApplicationDbContextFactory Implementation

**Ubicación**: `src/2. Infrastructure/ElectroHuila.Infrastructure/Persistence/ApplicationDbContextFactory.cs`

```csharp
public class ApplicationDbContextFactory : IDbContextFactory
{
    private readonly IDatabaseProviderService _databaseProviderService;
    private readonly ILogger<ApplicationDbContextFactory> _logger;

    public ApplicationDbContext CreateDbContext()
    {
        var provider = _databaseProviderService.CurrentProvider;
        var connectionString = _databaseProviderService.GetConnectionString();

        _logger.LogInformation("[DbContextFactory] Creating DbContext with provider: {Provider}", provider);

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        switch (provider)
        {
            case DatabaseProvider.Oracle:
                optionsBuilder.UseOracle(connectionString, options =>
                {
                    options.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                    options.CommandTimeout(30);
                });
                break;

            case DatabaseProvider.SqlServer:
                optionsBuilder.UseSqlServer(connectionString, options =>
                {
                    options.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                    options.CommandTimeout(30);
                    options.EnableRetryOnFailure(maxRetryCount: 3);
                });
                break;

            case DatabaseProvider.PostgreSQL:
                optionsBuilder.UseNpgsql(connectionString, options =>
                {
                    options.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                    options.CommandTimeout(30);
                    options.EnableRetryOnFailure(maxRetryCount: 3);
                });
                break;

            default:
                throw new InvalidOperationException($"Unsupported database provider: {provider}");
        }

        // Configuraciones comunes
        optionsBuilder.EnableSensitiveDataLogging(false);
        optionsBuilder.EnableDetailedErrors(true);

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
```

**Características**:
- **Lazy Creation**: Crea DbContext solo cuando se necesita
- **Configuración específica**: Cada proveedor tiene sus opciones
- **Retry policies**: SQL Server y PostgreSQL con reintentos automáticos
- **Migrations assembly**: Configurado para Entity Framework Core
- **Logging**: Registra qué proveedor se usa en cada creación

---

### 7. DatabaseParameterOperationFilter (Swagger)

**Ubicación**: `src/3. Presentation/ElectroHuila.WebApi/Filters/DatabaseParameterOperationFilter.cs`

```csharp
public class DatabaseParameterOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<OpenApiParameter>();

        // Verificar si el parámetro 'database' ya existe para evitar duplicados
        if (operation.Parameters.Any(p => p.Name == "database"))
        {
            return; // Ya existe, no lo agregamos de nuevo
        }

        // Agregar el parámetro 'database' como enum con valores predefinidos
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "database",
            In = ParameterLocation.Query,
            Description = "Seleccionar base de datos para esta operación",
            Required = false,
            Schema = new OpenApiSchema
            {
                Type = "string",
                Enum = new List<IOpenApiAny>
                {
                    new OpenApiString("oracle"),
                    new OpenApiString("sqlserver"),
                    new OpenApiString("postgresql")
                },
                Default = new OpenApiString("oracle")
            }
        });
    }
}
```

**Características**:
- **Automático**: Se aplica a todos los endpoints
- **Sin duplicados**: Detecta si el parámetro ya existe
- **Lista desplegable**: Muestra opciones en Swagger UI
- **Valor por defecto**: Oracle preseleccionado

**Registro en Program.cs**:
```csharp
builder.Services.AddSwaggerGen(options =>
{
    // ... otras configuraciones
    options.OperationFilter<DatabaseParameterOperationFilter>();
});
```

---

### 8. DatabaseHealthController

**Ubicación**: `src/3. Presentation/ElectroHuila.WebApi/Controllers/V1/DatabaseHealthController.cs`

Endpoints disponibles:

#### GET /api/v1/DatabaseHealth/check

Verifica la conexión a la base de datos seleccionada.

**Request**:
```http
GET /api/v1/DatabaseHealth/check?database=sqlserver
```

**Response**:
```json
{
  "provider": "SqlServer",
  "isConnected": true,
  "status": "Healthy",
  "message": "Successfully connected to SqlServer database",
  "databaseName": "ElectroHuila_Dev",
  "tableCount": 22,
  "connectionStringPreview": "Server=localhost;Database=***;User Id=***;Password=***",
  "timestamp": "2025-10-20T11:09:30.899213Z"
}
```

#### GET /api/v1/DatabaseHealth/info

Muestra el proveedor configurado actualmente.

**Response**:
```json
{
  "currentProvider": "Oracle",
  "connectionStringPreview": "User Id=***;Password=***;Data Source=...",
  "availableProviders": ["Oracle", "SqlServer", "PostgreSQL"],
  "message": "Currently configured to use Oracle database"
}
```

#### GET /api/v1/DatabaseHealth/test-all

Prueba la conexión con todos los proveedores.

**Response**:
```json
{
  "results": [
    {
      "provider": "Oracle",
      "isConfigured": true,
      "isConnected": true,
      "message": "Connection successful",
      "connectionStringPreview": "User Id=***;Password=***;Data Source=..."
    },
    {
      "provider": "SqlServer",
      "isConfigured": true,
      "isConnected": false,
      "message": "Error: A network-related error occurred...",
      "connectionStringPreview": "Server=localhost;Database=***..."
    },
    {
      "provider": "PostgreSQL",
      "isConfigured": true,
      "isConnected": false,
      "message": "Error: Failed to connect...",
      "connectionStringPreview": "Host=localhost;Database=***..."
    }
  ],
  "testedAt": "2025-10-20T11:10:00.000Z"
}
```

---

### 9. Dependency Injection Configuration

**Ubicación**: `src/2. Infrastructure/ElectroHuila.Infrastructure/DependencyInjection.cs`

```csharp
public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
{
    // Registrar DatabaseProviderService como Scoped
    services.AddScoped<IDatabaseProviderService, DatabaseProviderService>();

    // Registrar la fábrica de DbContext
    services.AddScoped<IDbContextFactory, ApplicationDbContextFactory>();

    // Registrar ApplicationDbContext usando la fábrica
    services.AddScoped<ApplicationDbContext>(serviceProvider =>
    {
        var factory = serviceProvider.GetRequiredService<IDbContextFactory>();
        return factory.CreateDbContext();
    });

    // Otros servicios...
    return services;
}
```

**Puntos clave**:
- **Scoped Lifetime**: Todos los servicios son Scoped (una instancia por request)
- **Factory Pattern**: DbContext se crea bajo demanda
- **Resolución tardía**: El proveedor se determina al crear el DbContext, no al registrarlo

---

## Cómo Funciona

### Flujo de Ejecución Detallado

1. **Request llega al servidor**
   ```http
   GET /api/v1/Appointments?database=sqlserver
   ```

2. **Middleware captura el parámetro**
   ```csharp
   // DatabaseProviderMiddleware
   context.Items["DatabaseProvider"] = "SqlServer";
   ```

3. **Routing ejecuta el endpoint**
   ```csharp
   [HttpGet]
   public async Task<IActionResult> GetAppointments()
   {
       // El controlador necesita ApplicationDbContext
   }
   ```

4. **DI Container resuelve ApplicationDbContext**
   ```csharp
   // DependencyInjection.cs
   var factory = serviceProvider.GetRequiredService<IDbContextFactory>();
   return factory.CreateDbContext(); // ← Se ejecuta AQUÍ
   ```

5. **Factory lee el proveedor actual**
   ```csharp
   // ApplicationDbContextFactory
   var provider = _databaseProviderService.CurrentProvider;
   // Lee de HttpContext.Items["DatabaseProvider"] = "SqlServer"
   ```

6. **Factory crea DbContext con SQL Server**
   ```csharp
   optionsBuilder.UseSqlServer(connectionString, options => {
       // Configuración específica de SQL Server
   });
   return new ApplicationDbContext(optionsBuilder.Options);
   ```

7. **Request ejecuta con SQL Server**
   ```csharp
   var appointments = await _dbContext.Appointments.ToListAsync();
   // Esta query se ejecuta contra SQL Server
   ```

### ¿Por Qué Funciona?

**Problema anterior**: Cuando usábamos `AddDbContext()`, el DbContext se configuraba **al inicio** de cada request scope, antes de que el middleware pudiera capturar el parámetro.

**Solución actual**: Con Factory Pattern, el DbContext se crea **bajo demanda** (lazy), después de que el middleware ya capturó el parámetro y lo guardó en `HttpContext.Items`.

---

## Guía de Uso

### Opción 1: Swagger UI (Recomendado)

1. Abre Swagger: `http://localhost:5000/swagger`
2. Selecciona cualquier endpoint (ej: `GET /api/v1/Appointments`)
3. Click en **"Try it out"**
4. Busca el parámetro **`database`** (lista desplegable)
5. Selecciona: `oracle`, `sqlserver` o `postgresql`
6. Click en **"Execute"**

### Opción 2: URL Directa

```http
GET http://localhost:5000/api/v1/Appointments?database=oracle
GET http://localhost:5000/api/v1/Appointments?database=sqlserver
GET http://localhost:5000/api/v1/Appointments?database=postgresql
```

### Opción 3: Header HTTP

```http
GET /api/v1/Appointments
X-Database-Provider: sqlserver
```

### Opción 4: Verificar con Health Check

```http
GET /api/v1/DatabaseHealth/check?database=oracle
GET /api/v1/DatabaseHealth/check?database=sqlserver
GET /api/v1/DatabaseHealth/check?database=postgresql
```

---

## Configuración

### appsettings.json

```json
{
  "DatabaseProvider": "Oracle",
  "ConnectionStrings": {
    "OracleConnection": "User Id=admin;Password=YourPassword;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=your-oracle-host.rds.amazonaws.com)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=ORCL)));Persist Security Info=True;",
    "SqlServerConnection": "Server=localhost,1433;Database=ElectroHuila_Dev;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;MultipleActiveResultSets=True;",
    "PostgreSqlConnection": "Host=localhost;Port=5432;Database=ElectroHuila_Dev;Username=postgres;Password=YourStrong@Passw0rd;Include Error Detail=True;"
  }
}
```

### Variables de Entorno (.env)

**Ubicación**: `devops/dev/.env`, `devops/staging/.env`, etc.

```bash
# Proveedor por defecto
DATABASE_PROVIDER=Oracle

# Oracle AWS RDS
ORACLE_CONNECTION_STRING=User Id=admin;Password=***;Data Source=...

# SQL Server
SQLSERVER_CONNECTION_STRING=Server=localhost,1433;Database=ElectroHuila_Dev;User Id=sa;Password=***;TrustServerCertificate=True

# PostgreSQL
POSTGRESQL_CONNECTION_STRING=Host=localhost;Port=5432;Database=ElectroHuila_Dev;Username=postgres;Password=***
```

### docker-compose.yml

```yaml
version: '3.8'

services:
  api:
    image: electrohuila-pqr-api-dev-api
    container_name: electrohuila-api-dev
    build:
      context: ../..
      dockerfile: devops/dev/Dockerfile
    ports:
      - "5000:8080"
    env_file:
      - .env
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - DatabaseProvider=${DATABASE_PROVIDER}
      - ConnectionStrings__OracleConnection=${ORACLE_CONNECTION_STRING}
      - ConnectionStrings__SqlServerConnection=${SQLSERVER_CONNECTION_STRING}
      - ConnectionStrings__PostgreSqlConnection=${POSTGRESQL_CONNECTION_STRING}
```

### Configurar SQL Server y PostgreSQL (Docker)

**SQL Server**:
```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Passw0rd" \
  -p 1433:1433 --name sqlserver \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

**PostgreSQL**:
```bash
docker run --name postgres \
  -e POSTGRES_PASSWORD=YourStrong@Passw0rd \
  -e POSTGRES_DB=ElectroHuila_Dev \
  -p 5432:5432 \
  -d postgres:16
```

**Aplicar migraciones**:
```bash
# Para SQL Server
dotnet ef database update --context ApplicationDbContext -- --DatabaseProvider=SqlServer

# Para PostgreSQL
dotnet ef database update --context ApplicationDbContext -- --DatabaseProvider=PostgreSQL
```

---

## Health Check Endpoints

### Casos de Uso

#### 1. Verificar Conexión Antes de Operar

```bash
curl "http://localhost:5000/api/v1/DatabaseHealth/check?database=sqlserver"
```

Si `isConnected: true`, proceder con las operaciones.

#### 2. Monitoreo de Disponibilidad

```bash
curl "http://localhost:5000/api/v1/DatabaseHealth/test-all"
```

Verifica el estado de los tres proveedores simultáneamente.

#### 3. Debugging de Configuración

```bash
curl "http://localhost:5000/api/v1/DatabaseHealth/info"
```

Muestra el proveedor actual y connection strings (enmascarados).

### Respuestas de Error

**Base de datos no configurada**:
```json
{
  "provider": "SqlServer",
  "isConnected": false,
  "status": "Unhealthy",
  "message": "Cannot connect to SqlServer database",
  "errorDetails": "SqlException",
  "timestamp": "2025-10-20T11:09:30Z"
}
```

**Proveedor inválido**:
```json
{
  "provider": "Unknown",
  "isConnected": false,
  "status": "Unhealthy",
  "message": "Invalid database provider",
  "timestamp": "2025-10-20T11:09:30Z"
}
```

---

## Troubleshooting

### Problema: El proveedor no cambia

**Síntoma**: Siempre usa Oracle aunque especifiques `?database=sqlserver`

**Solución**:
1. Verifica que el parámetro esté en la URL correctamente
2. Revisa los logs del contenedor:
   ```bash
   docker logs -f electrohuila-api-dev
   ```
3. Busca estos mensajes:
   ```
   [DatabaseProviderMiddleware] Database provider from query parameter: sqlserver
   [DatabaseProviderMiddleware] Database provider set to SqlServer for this request
   [DbContextFactory] Creating DbContext with provider: SqlServer
   ```
4. Si no aparecen, el middleware no está capturando el parámetro

**Verificación**:
```bash
curl "http://localhost:5000/api/v1/DatabaseHealth/check?database=sqlserver"
```

El campo `provider` en la respuesta debe ser `"SqlServer"`.

---

### Problema: Cannot connect to database

**Síntoma**: `isConnected: false` en health check

**Causas comunes**:
1. **Instancia no está corriendo**: 
   ```bash
   docker ps | grep sqlserver  # Verifica que el contenedor esté corriendo
   docker ps | grep postgres
   ```

2. **Connection string incorrecta**: Revisa en `.env` o `appsettings.json`

3. **Firewall bloqueando**: Verifica puertos 1433 (SQL Server) y 5432 (PostgreSQL)

4. **Base de datos no existe**: Crea la base de datos manualmente

**SQL Server**:
```sql
CREATE DATABASE ElectroHuila_Dev;
```

**PostgreSQL**:
```sql
CREATE DATABASE "ElectroHuila_Dev";
```

---

### Problema: Table doesn't exist

**Síntoma**: Error al ejecutar queries: "Table or view does not exist"

**Causa**: No se han ejecutado las migraciones de Entity Framework para ese proveedor

**Solución**:
```bash
# Configurar el proveedor temporalmente
export DATABASE_PROVIDER=SqlServer

# Ejecutar migraciones
dotnet ef database update --context ApplicationDbContext --project src/2.Infrastructure/ElectroHuila.Infrastructure

# O manualmente con scripts SQL
```

---

### Problema: Parámetro 'database' duplicado en Swagger

**Síntoma**: Aparecen dos listas desplegables `database` en un endpoint

**Causa**: El endpoint tiene `[FromQuery] string? database` definido manualmente

**Solución**: Remover el parámetro del método del controlador. El filtro lo agrega automáticamente.

**Incorrecto**:
```csharp
public async Task<IActionResult> GetData([FromQuery] string? database = null)
```

**Correcto**:
```csharp
public async Task<IActionResult> GetData()
// El parámetro se agrega automáticamente por DatabaseParameterOperationFilter
```

---

### Problema: Error al compilar - UseOracle/UseSqlServer/UseNpgsql no existe

**Síntoma**: 
```
'DbContextOptionsBuilder' does not contain a definition for 'UseOracle'
```

**Causa**: Falta el paquete NuGet correspondiente

**Solución**:
```bash
# Oracle
dotnet add package Oracle.EntityFrameworkCore --version 9.23.60

# SQL Server
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 9.0.0

# PostgreSQL
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 9.0.0
```

---

### Problema: DbContext se crea con el proveedor incorrecto

**Síntoma**: Los logs muestran el proveedor correcto pero las queries fallan

**Causa**: Múltiples instancias de DbContext siendo creadas

**Verificación**: Busca en los logs cuántas veces aparece:
```
[DbContextFactory] Creating DbContext with provider: XXX
```

**Solución**: Asegúrate de que solo se inyecta `ApplicationDbContext` una vez por request. No lo crees manualmente con `new ApplicationDbContext()`.

---

## Logs de Diagnóstico

### Logs Esperados (Request Exitoso)

```
info: ElectroHuila.WebApi.Middleware.DatabaseProviderMiddleware[0]
      Database provider from query parameter: sqlserver
info: ElectroHuila.WebApi.Middleware.DatabaseProviderMiddleware[0]
      Database provider set to SqlServer for this request
info: ElectroHuila.Infrastructure.Persistence.ApplicationDbContextFactory[0]
      [DbContextFactory] Creating DbContext with provider: SqlServer
info: Microsoft.EntityFrameworkCore.Database.Connection[20001]
      Opening connection to database 'ElectroHuila_Dev' on server 'localhost'.
```

### Ver Logs en Docker

```bash
# Logs en tiempo real
docker logs -f electrohuila-api-dev

# Últimas 100 líneas
docker logs --tail 100 electrohuila-api-dev

# Filtrar por palabra clave
docker logs electrohuila-api-dev 2>&1 | grep "DatabaseProvider"
```

---

## Estado Actual del Sistema

| Componente | Estado | Notas |
|------------|--------|-------|
| Oracle | ✅ Funcional | Conectado a AWS RDS |
| SQL Server | ⚙️ Configurado | Requiere instancia local/cloud |
| PostgreSQL | ⚙️ Configurado | Requiere instancia local/cloud |
| Middleware | ✅ Funcional | Captura parámetros correctamente |
| Factory Pattern | ✅ Funcional | Crea DbContext dinámicamente |
| Swagger Filter | ✅ Funcional | Lista desplegable en todos los endpoints |
| Health Check | ✅ Funcional | 3 endpoints disponibles |
| Logs | ✅ Funcional | Trazabilidad completa |

---

## Mejores Prácticas

### 1. Usar Health Check Antes de Operar

```csharp
// Verificar conexión antes de operaciones críticas
var healthResponse = await _httpClient.GetAsync(
    "/api/v1/DatabaseHealth/check?database=sqlserver");

if (healthResponse.IsSuccessStatusCode)
{
    // Proceder con operaciones
}
```

### 2. Logging Consistente

```csharp
_logger.LogInformation("Switching to {Provider} for operation {OperationName}", 
    provider, operationName);
```

### 3. Manejo de Errores

```csharp
try
{
    var result = await _dbContext.Appointments.ToListAsync();
}
catch (SqlException ex) when (ex.Number == -2) // Timeout
{
    _logger.LogWarning("Database timeout on {Provider}", currentProvider);
    // Reintentar o fallback
}
```

### 4. Testing Multi-Provider

```csharp
[Theory]
[InlineData("oracle")]
[InlineData("sqlserver")]
[InlineData("postgresql")]
public async Task GetAppointments_ShouldWork_WithAllProviders(string provider)
{
    // Arrange
    var request = new HttpRequestMessage(HttpMethod.Get, 
        $"/api/v1/Appointments?database={provider}");

    // Act
    var response = await _client.SendAsync(request);

    // Assert
    response.IsSuccessStatusCode.Should().BeTrue();
}
```

---

## Referencias

### Archivos Importantes

```
src/
├── 2. Infrastructure/
│   └── ElectroHuila.Infrastructure/
│       ├── DependencyInjection.cs                    # Registro de servicios
│       ├── Persistence/
│       │   ├── ApplicationDbContext.cs               # DbContext principal
│       │   ├── IDbContextFactory.cs                  # Interfaz factory
│       │   └── ApplicationDbContextFactory.cs        # Implementación factory
│       └── Services/
│           ├── DatabaseProvider.cs                   # Enum de proveedores
│           ├── IDatabaseProviderService.cs           # Interfaz servicio
│           └── DatabaseProviderService.cs            # Implementación servicio
└── 3. Presentation/
    └── ElectroHuila.WebApi/
        ├── Program.cs                                # Configuración Swagger y Middleware
        ├── Middleware/
        │   └── DatabaseProviderMiddleware.cs         # Captura parámetros
        ├── Filters/
        │   └── DatabaseParameterOperationFilter.cs   # Filtro Swagger
        └── Controllers/
            └── V1/
                └── DatabaseHealthController.cs       # Health checks

devops/
├── dev/
│   ├── docker-compose.yml                           # Configuración Docker DEV
│   └── .env                                         # Variables de entorno DEV
├── staging/
│   ├── docker-compose.yml                           # Configuración Docker STAGING
│   └── .env                                         # Variables de entorno STAGING
├── qa/
│   ├── docker-compose.yml                           # Configuración Docker QA
│   └── .env                                         # Variables de entorno QA
└── main/
    ├── docker-compose.yml                           # Configuración Docker MAIN
    └── .env                                         # Variables de entorno MAIN
```

### Dependencias NuGet

```xml
<PackageReference Include="Oracle.EntityFrameworkCore" Version="9.23.60" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.0" />
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="9.0.0" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
```

---

## Conclusión

Este sistema implementa una solución robusta y escalable para cambiar dinámicamente entre proveedores de base de datos sin reiniciar la aplicación. La combinación de Factory Pattern, Middleware Pipeline y Scoped Services permite que cada request HTTP use un proveedor diferente de forma transparente y segura.

**Ventajas**:
- ✅ Zero downtime
- ✅ Cambio instantáneo
- ✅ Interfaz amigable (Swagger)
- ✅ Trazabilidad completa
- ✅ Health checks integrados
- ✅ Multi-entorno

**Próximos Pasos**:
1. Configurar instancias de SQL Server y PostgreSQL
2. Ejecutar migraciones para los tres proveedores
3. Implementar tests de integración multi-provider
4. Agregar métricas de performance por proveedor

---

## Autor

**Sistema desarrollado para**: ElectroHuila - Sistema de Agendamiento de Citas PQR  
**Fecha**: Octubre 2025  
**Branch**: HU-05-dev  
**Versión API**: v1.0
