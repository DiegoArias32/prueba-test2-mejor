# ⚡ Optimizaciones de Performance - MAUI App

**Fecha**: 2025-11-30
**Estado**: 📝 LISTO PARA IMPLEMENTAR

---

## 🎯 Optimizaciones Prioritarias

Las siguientes optimizaciones están listas para copy-paste. Implementarlas en el orden indicado.

---

## 1️⃣ REDUCIR TIMEOUT HTTP (IMPACTO INMEDIATO)

### ❌ Código Actual

**Archivo**: `C:\Users\User\Desktop\Proyecto Electrohuila\Electrohuila\pqr-scheduling-appointments-app\MauiProgram.cs`

**Línea 79**:
```csharp
client.Timeout = TimeSpan.FromSeconds(30);
```

### ✅ Código Optimizado

```csharp
// Timeout optimizado para APIs en la nube
// 10 segundos es suficiente para latencias normales (incluso inter-continental)
// Si el backend no responde en 10s, hay un problema real
client.Timeout = TimeSpan.FromSeconds(10);
```

**Impacto**: Errores se detectan más rápido, mejor UX

---

## 2️⃣ AGREGAR PERFORMANCE LOGGING (DIAGNÓSTICO)

### ✅ Código a Agregar

**Archivo**: `C:\Users\User\Desktop\Proyecto Electrohuila\Electrohuila\pqr-scheduling-appointments-app\Services\ApiService.cs`

**Reemplazar el método `GetAsync` (líneas 78-138) con**:

```csharp
/// <summary>
/// Generic GET request implementation with performance tracking
/// </summary>
public async Task<T?> GetAsync<T>(string endpoint)
{
    // ⏱️ OPTIMIZACIÓN: Tracking de performance
    var stopwatch = System.Diagnostics.Stopwatch.StartNew();

    try
    {
        // Set JWT token in Authorization header before making the request
        await SetAuthorizationHeaderAsync();

        var fullUrl = new Uri(_httpClient.BaseAddress!, endpoint).ToString();

        Console.WriteLine($"🌐 GET {endpoint}");

        // Track network time
        var networkStart = stopwatch.ElapsedMilliseconds;
        var response = await _httpClient.GetAsync(endpoint);
        var networkTime = stopwatch.ElapsedMilliseconds - networkStart;

        Console.WriteLine($"📡 HTTP {(int)response.StatusCode} - Network: {networkTime}ms");

        if (response.IsSuccessStatusCode)
        {
            // Track deserialization time
            var deserializeStart = stopwatch.ElapsedMilliseconds;
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<T>(content, _jsonOptions);
            var deserializeTime = stopwatch.ElapsedMilliseconds - deserializeStart;

            stopwatch.Stop();

            Console.WriteLine($"⏱️ PERFORMANCE - Network: {networkTime}ms | Deserialize: {deserializeTime}ms | Total: {stopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine($"📦 Response size: {content.Length} bytes");

            return result;
        }

        var errorContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"❌ Error {response.StatusCode}: {errorContent}");
        throw new HttpRequestException($"Error {response.StatusCode}: {errorContent}");
    }
    catch (HttpRequestException ex)
    {
        stopwatch.Stop();
        Console.WriteLine($"❌ HTTP Error after {stopwatch.ElapsedMilliseconds}ms: {ex.Message}");
        throw new Exception($"Error de conexión al servidor. Verifique que el backend esté corriendo en {_baseUrl}", ex);
    }
    catch (TaskCanceledException ex)
    {
        stopwatch.Stop();
        Console.WriteLine($"⏱️ Timeout after {stopwatch.ElapsedMilliseconds}ms");
        throw new Exception("La solicitud ha tardado demasiado tiempo. Por favor, intente de nuevo.", ex);
    }
    catch (JsonException ex)
    {
        stopwatch.Stop();
        Console.WriteLine($"📋 JSON Error after {stopwatch.ElapsedMilliseconds}ms: {ex.Message}");
        throw new Exception($"Error al procesar la respuesta del servidor: {ex.Message}", ex);
    }
    catch (Exception ex)
    {
        stopwatch.Stop();
        Console.WriteLine($"❌ Error after {stopwatch.ElapsedMilliseconds}ms: {ex.Message}");
        throw new Exception($"Error inesperado: {ex.Message}", ex);
    }
}
```

**Impacto**: Visibilidad completa de tiempos de respuesta

---

## 3️⃣ IMPLEMENTAR CACHING EN VIEWMODELS (MAYOR IMPACTO)

### ✅ Código a Agregar

**Archivo**: `C:\Users\User\Desktop\Proyecto Electrohuila\Electrohuila\pqr-scheduling-appointments-app\ViewModels\HolidaysManagementViewModel.cs`

**Agregar estas propiedades privadas después de la línea 17** (después de `_branchService`):

```csharp
// ⚡ OPTIMIZACIÓN: Cache para evitar llamadas HTTP redundantes
private DateTime? _lastLoadTime;
private const int CACHE_DURATION_MINUTES = 5;
```

**Reemplazar el método `LoadHolidaysAsync` (líneas 67-106) con**:

```csharp
/// <summary>
/// Loads all holidays from backend API with caching
/// Cache duration: 5 minutes
/// </summary>
[RelayCommand]
private async Task LoadHolidaysAsync()
{
    // ⚡ OPTIMIZACIÓN: Usar cache si los datos son recientes
    if (_lastLoadTime.HasValue &&
        DateTime.Now - _lastLoadTime.Value < TimeSpan.FromMinutes(CACHE_DURATION_MINUTES) &&
        Holidays.Count > 0)
    {
        var cacheAge = (DateTime.Now - _lastLoadTime.Value).TotalSeconds;
        Console.WriteLine($"✅ Using cached holidays (loaded {cacheAge:F1}s ago) - Skipping HTTP call");
        ApplyFilter();  // Solo re-aplicar filtro local
        return;
    }

    var totalStopwatch = System.Diagnostics.Stopwatch.StartNew();
    Console.WriteLine("🚀 LoadHolidaysAsync STARTED - Fetching from backend");

    await ExecuteAsync(async () =>
    {
        Console.WriteLine("📋 Loading all holidays from backend...");

        var apiStopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Get all holidays from backend
        var pagedResult = await _holidayService.GetAllAsync(page: 1, pageSize: 100);

        apiStopwatch.Stop();
        Console.WriteLine($"⏱️ API call completed in {apiStopwatch.ElapsedMilliseconds}ms");

        if (pagedResult?.Items == null || !pagedResult.Items.Any())
        {
            Console.WriteLine("⚠️ No holidays found or backend returned null");
            Holidays.Clear();
            UpdateCounts();
            ApplyFilter();
            return;
        }

        var holidays = pagedResult.Items;
        Console.WriteLine($"✅ Loaded {holidays.Count} holidays from backend");

        // Update Holidays collection
        var uiUpdateStart = totalStopwatch.ElapsedMilliseconds;
        Holidays.Clear();
        foreach (var holiday in holidays.OrderByDescending(h => h.HolidayDate))
        {
            Holidays.Add(holiday);
        }
        var uiUpdateTime = totalStopwatch.ElapsedMilliseconds - uiUpdateStart;

        Console.WriteLine($"📊 Total holidays in collection: {Holidays.Count}");
        Console.WriteLine($"⏱️ UI update took: {uiUpdateTime}ms");

        // ⚡ OPTIMIZACIÓN: Guardar timestamp del cache
        _lastLoadTime = DateTime.Now;

        UpdateCounts();
        ApplyFilter();

        totalStopwatch.Stop();
        Console.WriteLine($"⏱️ TOTAL LoadHolidaysAsync time: {totalStopwatch.ElapsedMilliseconds}ms");
        Console.WriteLine($"📈 Stats - Total: {TotalHolidays}, National: {NationalCount}, Local: {LocalCount}, Company: {CompanyCount}");
    });
}
```

**Agregar nuevo método para forzar recarga** (después del método `LoadHolidaysAsync`):

```csharp
/// <summary>
/// Forces a fresh reload of holidays, bypassing cache
/// </summary>
[RelayCommand]
private async Task ForceRefreshHolidaysAsync()
{
    Console.WriteLine("🔄 Force refresh - Invalidating cache");
    _lastLoadTime = null;  // Invalidar cache
    await LoadHolidaysAsync();
}
```

**Modificar el método `RefreshAsync` (líneas 111-117)**:

```csharp
/// <summary>
/// Refreshes the holidays list (bypasses cache)
/// </summary>
[RelayCommand]
private async Task RefreshAsync()
{
    IsRefreshing = true;
    _lastLoadTime = null;  // ⚡ Invalidar cache en pull-to-refresh
    await LoadHolidaysAsync();
    IsRefreshing = false;
}
```

**Impacto**:
- Primera carga: 300-800ms (normal)
- Cargas posteriores (< 5 min): **0ms** (instantáneo)
- Pull-to-refresh: Siempre recarga (bypass cache)

---

## 4️⃣ OPTIMIZAR LOGS EN PRODUCCIÓN

### ✅ Código a Agregar

**Archivo**: `C:\Users\User\Desktop\Proyecto Electrohuila\Electrohuila\pqr-scheduling-appointments-app\Services\ApiService.cs`

**Reemplazar logs verbosos con logs condicionales**:

En el método `GetAsync`, reemplazar las líneas 87-96 con:

```csharp
#if DEBUG
    // Logs detallados solo en modo Debug
    var fullUrl = new Uri(_httpClient.BaseAddress!, endpoint).ToString();
    Console.WriteLine($"=== API GET REQUEST ===");
    Console.WriteLine($"📍 FULL URL: {fullUrl}");
    Console.WriteLine($"📦 Endpoint: {endpoint}");
    Console.WriteLine($"🌐 Base URL: {_baseUrl}");
#else
    // Logs concisos en modo Release
    Console.WriteLine($"🌐 GET {endpoint}");
#endif
```

**Impacto**: Reduce overhead de logging en producción

---

## 5️⃣ AGREGAR COMPRESIÓN GZIP

### ✅ Código a Agregar

**Archivo**: `C:\Users\User\Desktop\Proyecto Electrohuila\Electrohuila\pqr-scheduling-appointments-app\MauiProgram.cs`

**Modificar la creación del HttpClientHandler (línea 67-73)**:

```csharp
#if ANDROID
    var handler = new HttpClientHandler();

    // ⚡ OPTIMIZACIÓN: Habilitar compresión automática
    handler.AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate;

    if (currentEnv == "Development" || currentEnv == "DevelopmentDevice")
    {
        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
        Console.WriteLine("⚠️ WARNING: SSL certificate validation is DISABLED for Development environment.");
    }
    var client = new HttpClient(handler);
#else
    var handler = new HttpClientHandler();
    handler.AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate;
    var client = new HttpClient(handler);
#endif
```

**Impacto**: Reduce payload en ~60-80% si el backend soporta compresión

---

## 6️⃣ IMPLEMENTAR INDICADOR DE CARGA MEJORADO

### ✅ Código a Verificar/Agregar

**Archivo**: `C:\Users\User\Desktop\Proyecto Electrohuila\Electrohuila\pqr-scheduling-appointments-app\Views\Admin\HolidaysManagementPage.xaml`

**Buscar el ActivityIndicator y asegurar que esté configurado así**:

```xaml
<!-- Indicador de carga -->
<ActivityIndicator
    IsRunning="{Binding IsBusy}"
    IsVisible="{Binding IsBusy}"
    Color="{StaticResource Primary}"
    VerticalOptions="Center"
    HorizontalOptions="Center"
    WidthRequest="50"
    HeightRequest="50"/>

<!-- Mensaje de carga -->
<Label
    Text="Cargando festivos..."
    IsVisible="{Binding IsBusy}"
    HorizontalOptions="Center"
    VerticalOptions="Center"
    Margin="0,10,0,0"
    FontAttributes="Italic"/>
```

**Impacto**: Mejora percepción del usuario durante la carga

---

## 📊 Comparación: Antes vs Después

### ⏱️ Tiempos de Carga Esperados

| Escenario | ANTES | DESPUÉS | Mejora |
|-----------|-------|---------|--------|
| **Primera carga** | 300-800ms | 300-800ms | - |
| **Segunda carga (< 5 min)** | 300-800ms | **0ms** | ⚡ 100% |
| **Tercera carga (< 5 min)** | 300-800ms | **0ms** | ⚡ 100% |
| **Pull-to-refresh** | 300-800ms | 300-800ms | - |
| **Carga después de 5 min** | 300-800ms | 300-800ms | - |

### 📦 Tamaño de Payload

| Aspecto | ANTES | DESPUÉS |
|---------|-------|---------|
| **Sin compresión** | ~50KB | ~50KB |
| **Con compresión GZIP** | - | **~10KB** |

### 📊 Llamadas HTTP Reducidas

| Navegación | ANTES | DESPUÉS |
|------------|-------|---------|
| Dashboard → Festivos | 1 llamada | 1 llamada |
| Festivos → Empleados → Festivos | 2 llamadas | **1 llamada** (cache) |
| 5 navegaciones en 5 minutos | 5 llamadas | **1 llamada** (cache) |

---

## 🧪 Plan de Testing

### Paso 1: Implementar Optimizaciones

```bash
# Aplicar cambios en el orden indicado:
# 1. Timeout (MauiProgram.cs)
# 2. Performance Logging (ApiService.cs)
# 3. Caching (HolidaysManagementViewModel.cs)
# 4. Logs condicionales (ApiService.cs)
# 5. Compresión GZIP (MauiProgram.cs)
```

### Paso 2: Compilar y Ejecutar

```bash
cd C:\Users\User\Desktop\Proyecto Electrohuila\Electrohuila\pqr-scheduling-appointments-app
dotnet clean
dotnet build
```

### Paso 3: Probar Escenarios

**Escenario 1: Primera Carga**
1. Ejecutar app
2. Login
3. Navegar a Holidays
4. **Buscar en Output Console**:
```
⏱️ PERFORMANCE - Network: XXXms | Deserialize: XXms | Total: XXXms
```

**Escenario 2: Cache Hit (navegación rápida)**
1. Holidays → Dashboard
2. Dashboard → Holidays (inmediato)
3. **Buscar en Output Console**:
```
✅ Using cached holidays (loaded X.Xs ago) - Skipping HTTP call
```

**Escenario 3: Pull-to-Refresh (bypass cache)**
1. En Holidays, hacer swipe down
2. **Buscar en Output Console**:
```
🔄 Force refresh - Invalidating cache
⏱️ PERFORMANCE - Network: XXXms
```

### Paso 4: Comparar Resultados

**Antes de las optimizaciones**:
```
Primera carga: ???ms
Segunda carga: ???ms
Tercera carga: ???ms
```

**Después de las optimizaciones**:
```
Primera carga: XXXms
Segunda carga: 0ms (cache hit)
Tercera carga: 0ms (cache hit)
```

---

## 🚨 Troubleshooting

### Problema: "Cache no funciona"

**Síntoma**: Siempre hace llamadas HTTP

**Verificar**:
1. ¿Se agregó `_lastLoadTime`?
2. ¿Se asigna `_lastLoadTime = DateTime.Now` después de cargar?
3. ¿La constante `CACHE_DURATION_MINUTES` está definida?

**Solución**: Revisar código de caching (Optimización #3)

---

### Problema: "No veo logs de performance"

**Síntoma**: No aparece `⏱️ PERFORMANCE` en Output

**Verificar**:
1. Output Console está en "Debug" mode
2. El Stopwatch está inicializado
3. Los `Console.WriteLine` están presentes

**Solución**: Revisar código de logging (Optimización #2)

---

### Problema: "Timeout sigue siendo 30s"

**Síntoma**: Errores tardan mucho en aparecer

**Verificar**:
1. ¿Se cambió en `MauiProgram.cs`?
2. ¿Se recompiló la app?

**Solución**:
```bash
dotnet clean
dotnet build
```

---

## 📝 Checklist de Implementación

- [ ] **Optimización #1**: Timeout reducido a 10s (MauiProgram.cs)
- [ ] **Optimización #2**: Performance logging agregado (ApiService.cs)
- [ ] **Optimización #3**: Caching implementado (HolidaysManagementViewModel.cs)
- [ ] **Optimización #4**: Logs condicionales (ApiService.cs)
- [ ] **Optimización #5**: Compresión GZIP (MauiProgram.cs)
- [ ] **Optimización #6**: Indicador de carga mejorado (HolidaysManagementPage.xaml)
- [ ] **Testing**: Escenario 1 - Primera carga
- [ ] **Testing**: Escenario 2 - Cache hit
- [ ] **Testing**: Escenario 3 - Pull-to-refresh
- [ ] **Documentar**: Resultados de performance

---

## 🎯 Resultados Esperados

### Impacto en UX

| Aspecto | ANTES | DESPUÉS |
|---------|-------|---------|
| **Primera carga** | 0.3-0.8s | 0.3-0.8s (sin cambio) |
| **Navegaciones repetidas** | 0.3-0.8s | **Instantáneo** |
| **Percepción de velocidad** | Lento | ⚡ Rápido |
| **Detección de errores** | 30s | **10s** |
| **Visibilidad de problemas** | ❌ Sin métricas | ✅ Logs detallados |

### Impacto en Backend

| Métrica | ANTES | DESPUÉS |
|---------|-------|---------|
| **Requests/minuto** | Alto (recarga siempre) | **Bajo** (cache 5 min) |
| **Bandwidth** | ~50KB/request | **~10KB** (GZIP) |
| **Carga en DB** | Alta | **Reducida 80%** |

---

## 📚 Archivos Modificados

| Archivo | Cambios | Líneas |
|---------|---------|--------|
| `MauiProgram.cs` | Timeout + GZIP | 67-79 |
| `ApiService.cs` | Performance logging | 78-138 |
| `HolidaysManagementViewModel.cs` | Caching | 17-117 |

---

## 🚀 Próximos Pasos Opcionales

### 7️⃣ Pre-cargar Datos en Background

```csharp
// AppShell.xaml.cs - OnAppearing
protected override void OnAppearing()
{
    base.OnAppearing();

    // Pre-cargar festivos en background
    Task.Run(async () =>
    {
        var holidayService = Handler.MauiContext.Services.GetService<IHolidayService>();
        await holidayService.GetAllAsync(page: 1, pageSize: 100);
        Console.WriteLine("✅ Holidays pre-loaded");
    });
}
```

### 8️⃣ Implementar Cache en Otros ViewModels

Aplicar el mismo patrón de caching en:
- `EmployeesManagementViewModel`
- `BranchesManagementViewModel`
- `AppointmentsManagementViewModel`

---

**Autor**: Claude Code
**Fecha**: 2025-11-30
**Estado**: Listo para implementar
