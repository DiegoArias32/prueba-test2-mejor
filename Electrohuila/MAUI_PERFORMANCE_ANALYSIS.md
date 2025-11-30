# 🐢 Análisis de Rendimiento - Aplicación MAUI

**Fecha**: 2025-11-30
**Estado**: 🔍 ANÁLISIS COMPLETADO
**Problema**: Lentitud al cargar datos del backend en AWS

---

## 📊 Diagnóstico del Problema

### 🎯 Configuración Actual

| Componente | Valor | Estado |
|------------|-------|--------|
| **Backend URL** | `https://8papi9muvp.us-east-2.awsapprunner.com/api/v1/` | ✅ |
| **Backend Location** | AWS us-east-2 (Ohio) | ⚠️ Latencia geográfica |
| **HTTP Timeout** | 30 segundos | ⚠️ Muy alto |
| **Entorno** | Production | ✅ |
| **SSL Validation** | Activo (Production) | ✅ |

---

## 🔍 Hallazgos Principales

### 1️⃣ **TIMEOUT CONFIGURADO MUY ALTO**

**Ubicación**:
- `ApiService.cs:30` → Timeout: 30 segundos
- `MauiProgram.cs:79` → Timeout: 30 segundos (duplicado)

```csharp
// ApiService.cs línea 30
_httpClient.Timeout = TimeSpan.FromSeconds(30);

// MauiProgram.cs línea 79
client.Timeout = TimeSpan.FromSeconds(30);
```

**Problema**:
- Si el servidor no responde, la app espera hasta 30 segundos antes de mostrar error
- El timeout se configura en DOS lugares (posible conflicto)

**Recomendación**:
```csharp
// Reducir a 10-15 segundos para APIs en la nube
client.Timeout = TimeSpan.FromSeconds(10);
```

---

### 2️⃣ **LLAMADAS HTTP AL CARGAR PANTALLA DE FESTIVOS**

**Flujo de carga**:

```
Usuario navega a HolidaysManagementPage
    ↓
OnAppearing() (HolidaysManagementPage.xaml.cs:19)
    ↓
LoadHolidaysCommand.Execute(null)
    ↓
LoadHolidaysAsync() (HolidaysManagementViewModel.cs:68)
    ↓
_holidayService.GetAllAsync(page: 1, pageSize: 100)
    ↓
GET https://8papi9muvp.us-east-2.awsapprunner.com/api/v1/Holidays?pageNumber=1&pageSize=100
```

**Número de llamadas HTTP**: **1 llamada** (✅ Correcto, no hay llamadas redundantes)

**Datos descargados**: Hasta 100 festivos en un solo request

---

### 3️⃣ **FALTA DE CACHING**

**Problema**:
Cada vez que el usuario navega a la pantalla de festivos, se hace una nueva llamada al backend, incluso si los datos no han cambiado.

**Evidencia**:
```csharp
// HolidaysManagementPage.xaml.cs:19
protected override void OnAppearing()
{
    base.OnAppearing();
    if (BindingContext is HolidaysManagementViewModel viewModel)
    {
        viewModel.LoadHolidaysCommand.Execute(null);  // ⚠️ SIEMPRE recarga
    }
}
```

**Impacto**:
- Usuario navega → Llamada HTTP
- Usuario vuelve atrás y regresa → Otra llamada HTTP
- Múltiples navegaciones = Múltiples llamadas innecesarias

---

### 4️⃣ **LATENCIA GEOGRÁFICA**

**Backend**: AWS us-east-2 (Ohio, USA)
**App**: Corriendo en emulador/dispositivo (ubicación desconocida)

**Latencia esperada**:
- Mismo país: 50-100ms
- Otro continente: 200-500ms
- Handshake SSL: +100-200ms adicionales

**Primera conexión**: ~500ms (incluye SSL handshake)
**Conexiones posteriores**: ~200-300ms (reutiliza conexión)

---

### 5️⃣ **LOGS EXCESIVOS EN CONSOLA**

**Problema**: Cada request imprime múltiples logs:

```csharp
// ApiService.cs:87-96 (10 líneas de logs por request)
Console.WriteLine($"=== API GET REQUEST ===");
Console.WriteLine($"📍 FULL URL: {fullUrl}");
Console.WriteLine($"📦 Endpoint: {endpoint}");
Console.WriteLine($"🌐 Base URL: {_baseUrl}");
Console.WriteLine($"🔑 HttpClient.BaseAddress: {_httpClient.BaseAddress}");
// ... 5 líneas más
```

**Impacto**:
- Overhead mínimo en performance (~1-2ms)
- Logs útiles para debugging, pero excesivos en producción

---

### 6️⃣ **DESERIALIZACIÓN JSON**

**Ubicación**: `ApiService.cs:106`

```csharp
return JsonSerializer.Deserialize<T>(content, _jsonOptions);
```

**Configuración**:
```csharp
_jsonOptions = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true,  // ✅ Correcto
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,  // ✅ Correcto
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull  // ✅ Correcto
};
```

**Estado**: ✅ Configuración óptima

**Tiempo estimado**:
- 100 festivos: ~10-20ms
- 1000 festivos: ~50-100ms

---

### 7️⃣ **NAVEGACIÓN Y CARGA MÚLTIPLE**

**Observación**: Cada página tiene su propio `OnAppearing()` que carga datos:

| Página | Método de Carga | ¿Se ejecuta en cada navegación? |
|--------|----------------|----------------------------------|
| `HolidaysManagementPage` | `LoadHolidaysCommand` | ✅ Sí |
| `EmployeesManagementPage` | `LoadEmployeesCommand` | ✅ Sí |
| `BranchesManagementPage` | `LoadBranchesCommand` | ✅ Sí |
| `AppointmentsManagementPage` | `LoadAppointmentsCommand` | ✅ Sí |
| `DashboardPage` | `InitializeAsync()` | ✅ Sí |
| `NotificationsPage` | `LoadNotificationsCommand` | ✅ Sí |

**Problema**: Si el usuario navega rápidamente entre páginas, se disparan múltiples llamadas HTTP en paralelo.

---

## ⏱️ Análisis de Tiempo de Carga

### Desglose Estimado (Primera carga de festivos):

| Etapa | Tiempo Estimado | Descripción |
|-------|-----------------|-------------|
| **DNS Resolution** | 10-50ms | Resolver dominio AWS |
| **TCP Handshake** | 50-150ms | Establecer conexión TCP |
| **SSL Handshake** | 100-200ms | Negociación SSL/TLS |
| **HTTP Request** | 10-20ms | Enviar request |
| **Backend Processing** | 50-200ms | Backend consulta DB y serializa |
| **HTTP Response** | 50-150ms | Recibir respuesta |
| **JSON Deserialization** | 10-20ms | Parsear JSON |
| **UI Update** | 20-50ms | Actualizar ObservableCollection |
| **TOTAL PRIMERA CARGA** | **300-840ms** | **0.3 - 0.8 segundos** |

### Cargas Posteriores (conexión reutilizada):

| Etapa | Tiempo Estimado |
|-------|-----------------|
| HTTP Request + Response | 100-300ms |
| JSON Deserialization | 10-20ms |
| UI Update | 20-50ms |
| **TOTAL** | **130-370ms** |

---

## 🚨 Posibles Causas de Lentitud Percibida

### 1. **Falta de Indicador de Carga Visual**

Si no hay un `ActivityIndicator` visible, el usuario percibe que "no pasa nada" durante 0.5-1 segundo.

**Verificar**:
- ¿Hay un `ActivityIndicator` en `HolidaysManagementPage.xaml`?
- ¿Está bindeado a `IsBusy` del ViewModel?

### 2. **Múltiples Navegaciones**

Si el usuario navega rápido:
```
Dashboard → Festivos (llamada HTTP #1)
    ↓
Festivos → Empleados (llamada HTTP #2)
    ↓
Empleados → Festivos (llamada HTTP #3) ⚠️ Re-descarga todo
```

### 3. **Backend Lento**

¿El backend tarda en responder?

**Verificar en logs del backend**:
```sql
-- ¿Hay índices en la tabla Holidays?
SELECT * FROM HOLIDAYS WHERE ROWNUM <= 100;
```

**Posibles problemas en backend**:
- Falta de índices en DB
- Consultas N+1 (joins ineficientes)
- Logging excesivo en backend
- Cold start de AWS App Runner

---

## ✅ Recomendaciones de Optimización

### 🔥 PRIORIDAD ALTA (Impacto inmediato)

#### 1. **Reducir Timeout HTTP**

**Archivo**: `C:\Users\User\Desktop\Proyecto Electrohuila\Electrohuila\pqr-scheduling-appointments-app\MauiProgram.cs`

**Cambio**:
```csharp
// ANTES (línea 79)
client.Timeout = TimeSpan.FromSeconds(30);

// DESPUÉS
client.Timeout = TimeSpan.FromSeconds(10);  // Suficiente para APIs en la nube
```

**Impacto**: Errores se muestran más rápido (mejor UX)

---

#### 2. **Implementar Caching en ViewModels**

**Archivo**: `C:\Users\User\Desktop\Proyecto Electrohuila\Electrohuila\pqr-scheduling-appointments-app\ViewModels\HolidaysManagementViewModel.cs`

**Implementación**:

```csharp
public partial class HolidaysManagementViewModel : BaseViewModel
{
    private DateTime? _lastLoadTime;
    private const int CACHE_DURATION_MINUTES = 5;

    [RelayCommand]
    private async Task LoadHolidaysAsync()
    {
        // ✅ OPTIMIZACIÓN: Usar cache si los datos son recientes
        if (_lastLoadTime.HasValue &&
            DateTime.Now - _lastLoadTime.Value < TimeSpan.FromMinutes(CACHE_DURATION_MINUTES) &&
            Holidays.Count > 0)
        {
            Console.WriteLine($"✅ Using cached holidays (loaded {(DateTime.Now - _lastLoadTime.Value).TotalSeconds:F1}s ago)");
            ApplyFilter();  // Solo re-aplicar filtro
            return;
        }

        Console.WriteLine("🚀 LoadHolidaysAsync STARTED - Fetching from backend");

        await ExecuteAsync(async () =>
        {
            var pagedResult = await _holidayService.GetAllAsync(page: 1, pageSize: 100);

            if (pagedResult?.Items == null || !pagedResult.Items.Any())
            {
                Holidays.Clear();
                UpdateCounts();
                ApplyFilter();
                return;
            }

            var holidays = pagedResult.Items;
            Holidays.Clear();
            foreach (var holiday in holidays.OrderByDescending(h => h.HolidayDate))
            {
                Holidays.Add(holiday);
            }

            _lastLoadTime = DateTime.Now;  // ✅ Guardar timestamp del cache
            UpdateCounts();
            ApplyFilter();
        });
    }

    // ✅ Nuevo método para forzar recarga
    [RelayCommand]
    private async Task ForceRefreshAsync()
    {
        _lastLoadTime = null;  // Invalidar cache
        await LoadHolidaysAsync();
    }
}
```

**Impacto**:
- Primera carga: 300-800ms
- Navegaciones posteriores (< 5 min): **0ms** (instantáneo)

---

#### 3. **Agregar Logging de Performance**

**Archivo**: `C:\Users\User\Desktop\Proyecto Electrohuila\Electrohuila\pqr-scheduling-appointments-app\Services\ApiService.cs`

**Cambio**:
```csharp
public async Task<T?> GetAsync<T>(string endpoint)
{
    try
    {
        await SetAuthorizationHeaderAsync();

        var fullUrl = new Uri(_httpClient.BaseAddress!, endpoint).ToString();

        // ✅ AGREGAR: Logging de tiempo de respuesta
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        Console.WriteLine($"🌐 GET {endpoint}");
        var response = await _httpClient.GetAsync(endpoint);

        stopwatch.Stop();
        Console.WriteLine($"⏱️ Response time: {stopwatch.ElapsedMilliseconds}ms - Status: {(int)response.StatusCode}");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();

            var deserializeStart = stopwatch.ElapsedMilliseconds;
            var result = JsonSerializer.Deserialize<T>(content, _jsonOptions);
            var deserializeTime = stopwatch.ElapsedMilliseconds - deserializeStart;

            Console.WriteLine($"📊 Deserialization time: {deserializeTime}ms");
            Console.WriteLine($"📦 Total time: {stopwatch.ElapsedMilliseconds}ms");

            return result;
        }

        // ... resto del código
    }
    // ... resto del código
}
```

**Impacto**: Identificar cuellos de botella con datos reales

---

### ⚠️ PRIORIDAD MEDIA

#### 4. **Optimizar Logs en Producción**

**Archivo**: `C:\Users\User\Desktop\Proyecto Electrohuila\Electrohuila\pqr-scheduling-appointments-app\Services\ApiService.cs`

**Cambio**:
```csharp
// Agregar flag de debugging condicional
#if DEBUG
    Console.WriteLine($"📍 FULL URL: {fullUrl}");
    Console.WriteLine($"📦 Endpoint: {endpoint}");
    // ... otros logs de debugging
#endif

// Mantener solo logs esenciales en producción
Console.WriteLine($"⏱️ GET {endpoint} - {stopwatch.ElapsedMilliseconds}ms");
```

---

#### 5. **Implementar Paginación Lazy Loading**

**Actualmente**: Se cargan 100 festivos de una vez

**Propuesta**: Cargar 20 primeros, luego scroll infinito

```csharp
[RelayCommand]
private async Task LoadMoreHolidaysAsync()
{
    if (IsBusy) return;

    _currentPage++;
    var pagedResult = await _holidayService.GetAllAsync(page: _currentPage, pageSize: 20);

    foreach (var holiday in pagedResult.Items)
    {
        Holidays.Add(holiday);
    }
}
```

---

#### 6. **Pre-cargar Datos en Background**

**Implementar en** `AppShell.xaml.cs`:

```csharp
protected override void OnAppearing()
{
    base.OnAppearing();

    // Pre-cargar datos comunes en background
    Task.Run(async () =>
    {
        var holidayService = Handler.MauiContext.Services.GetService<IHolidayService>();
        await holidayService.GetAllAsync(page: 1, pageSize: 100);
        Console.WriteLine("✅ Holidays pre-loaded in background");
    });
}
```

---

### 💡 PRIORIDAD BAJA

#### 7. **Comprimir Respuestas HTTP**

Agregar en `MauiProgram.cs`:

```csharp
var handler = new HttpClientHandler();
handler.AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate;
```

**Impacto**: Reduce tamaño de respuesta en ~60-80%

---

#### 8. **Investigar Backend Performance**

**Verificar en backend**:
- ¿Hay índices en tabla `HOLIDAYS`?
- ¿Se están usando consultas N+1?
- ¿Hay logging excesivo?
- ¿El JSON está optimizado?

**Sugerencia**: Agregar endpoint `/api/v1/health` con métricas:
```json
{
  "status": "healthy",
  "responseTime": "45ms",
  "databaseResponseTime": "12ms"
}
```

---

## 📱 Testing Recomendado

### 1. **Medir Tiempo Real de Carga**

Agregar en `HolidaysManagementViewModel.cs`:

```csharp
[RelayCommand]
private async Task LoadHolidaysAsync()
{
    var totalStopwatch = System.Diagnostics.Stopwatch.StartNew();

    await ExecuteAsync(async () =>
    {
        var apiStopwatch = System.Diagnostics.Stopwatch.StartNew();
        var pagedResult = await _holidayService.GetAllAsync(page: 1, pageSize: 100);
        apiStopwatch.Stop();

        Console.WriteLine($"⏱️ API call took: {apiStopwatch.ElapsedMilliseconds}ms");

        // ... procesamiento

        totalStopwatch.Stop();
        Console.WriteLine($"⏱️ TOTAL load time: {totalStopwatch.ElapsedMilliseconds}ms");
    });
}
```

### 2. **Comparar con Portal Web**

Abrir DevTools en el portal web Next.js:
1. Network tab
2. Filtrar por `Holidays`
3. Comparar tiempo de respuesta

**Si el portal web es rápido y MAUI es lento** → Problema en la app MAUI
**Si ambos son lentos** → Problema en el backend

---

## 🎯 Plan de Acción Inmediato

### Paso 1: Agregar Logging de Performance ⏱️

```bash
# Editar ApiService.cs y agregar Stopwatch
# Ver sección "Recomendación #3"
```

### Paso 2: Ejecutar y Medir

```bash
# Ejecutar app en emulador
# Navegar a Holidays
# Revisar Output Console
```

**Buscar en logs**:
```
⏱️ Response time: XXXms
```

### Paso 3: Identificar Cuello de Botella

| Tiempo de Respuesta | Diagnóstico | Acción |
|---------------------|-------------|--------|
| < 200ms | ✅ Backend rápido | Implementar caching |
| 200-500ms | ⚠️ Latencia normal | Implementar caching + pre-load |
| 500-1000ms | 🐢 Backend lento | Optimizar backend |
| > 1000ms | 🚨 Problema serio | Revisar queries DB en backend |

### Paso 4: Implementar Solución

**Si backend es rápido (< 200ms)**:
→ Implementar **Recomendación #2** (Caching)

**Si backend es lento (> 500ms)**:
→ Investigar backend (queries, índices, N+1)

---

## 📊 Resumen del Análisis

| Aspecto | Estado | Prioridad de Fix |
|---------|--------|------------------|
| **Timeout HTTP** | ⚠️ 30s (muy alto) | 🔥 Alta |
| **Llamadas HTTP** | ✅ 1 llamada (óptimo) | - |
| **Caching** | ❌ No implementado | 🔥 Alta |
| **Latencia Geográfica** | ⚠️ AWS us-east-2 | 💡 Baja |
| **Logs** | ⚠️ Excesivos | ⚠️ Media |
| **Deserialización** | ✅ Óptima | - |
| **Navegación** | ⚠️ Recarga todo | 🔥 Alta |
| **Performance Tracking** | ❌ No implementado | 🔥 Alta |

---

## 📝 Conclusiones

### ✅ Lo que está BIEN:
1. Solo 1 llamada HTTP por carga (no hay llamadas redundantes)
2. JSON deserialización configurada correctamente
3. URLs del backend correctas
4. SSL configurado apropiadamente

### ⚠️ Lo que puede MEJORAR:
1. **Timeout muy alto** (30s → reducir a 10s)
2. **Sin caching** (cada navegación = nueva llamada HTTP)
3. **Sin métricas de performance** (no sabemos cuánto tarda realmente)
4. **Logs excesivos** (pueden impactar performance mínimamente)

### 🎯 Impacto Esperado de Optimizaciones:

| Optimización | Impacto | Esfuerzo |
|--------------|---------|----------|
| **Caching (5 min)** | ⚡ 300-800ms → **0ms** (cargas repetidas) | 🟢 Bajo |
| **Performance Logging** | 📊 Visibilidad de cuellos de botella | 🟢 Bajo |
| **Reducir Timeout** | 🚀 Errores más rápidos | 🟢 Bajo |
| **Lazy Loading** | 📉 Reduce payload inicial | 🟡 Medio |
| **Pre-loading** | ⚡ Datos listos al navegar | 🟡 Medio |

---

## 📁 Archivos Relevantes

| Archivo | Líneas Clave | Descripción |
|---------|--------------|-------------|
| `ApiService.cs` | 30, 78-138 | Timeout y GET requests |
| `MauiProgram.cs` | 79 | Configuración HttpClient |
| `HolidaysManagementViewModel.cs` | 68-106 | Carga de festivos |
| `HolidaysManagementPage.xaml.cs` | 13-21 | OnAppearing que dispara carga |
| `HolidayService.cs` | 25-60 | Llamada al endpoint |
| `ConfigurationService.cs` | 24 | URL del backend |

---

**Próximo paso recomendado**: Implementar **Performance Logging** para obtener datos reales del tiempo de respuesta.

---

**Autor**: Claude Code
**Fecha**: 2025-11-30
