# Referencia de Código - Paginación en Festivos

## 1. Propiedades del ViewModel

### Estructura de Datos:
```csharp
// Colecciones principales:
Holidays               // Todos los festivos cargados del backend (~1000)
FilteredHolidays      // Todos los festivos después de aplicar filtros y búsqueda
PagedHolidays         // Solo los festivos de la página actual (max 15)

// Configuración de paginación:
CurrentPage = 1       // Página donde estamos
PageSize = 15         // Cuántos items por página
TotalPages = 5        // Calculado: Math.Ceiling(FilteredHolidays.Count / PageSize)

// Estados:
HasNextPage = true    // Si Current < Total
HasPreviousPage = false // Si Current > 1
PageInfo = "Página 1 de 5"
```

---

## 2. Algoritmo de ApplyFilter()

```
ENTRADA: Holidays collection + SearchText + SelectedFilter

PASO 1: Filtrar por tipo
├─ Si SelectedFilter == "Nacional"
│  └─ Mantener solo festivos donde HolidayType = "NATIONAL" o "NACIONAL"
├─ Si SelectedFilter == "Local"
│  └─ Mantener solo festivos donde HolidayType = "LOCAL"
├─ Si SelectedFilter == "Company"
│  └─ Mantener solo festivos donde HolidayType = "COMPANY" o "EMPRESA"
└─ Si SelectedFilter == "All"
   └─ Mantener todos los festivos

PASO 2: Filtrar por búsqueda
├─ Si SearchText no está vacío
│  └─ Mantener solo festivos donde:
│     - HolidayName contiene SearchText O
│     - BranchName contiene SearchText O
│     - FormattedDate contiene SearchText
└─ (case-insensitive)

PASO 3: Calcular paginación
├─ filteredList.Count = total de items después de filtros
├─ TotalPages = Math.Ceiling(filteredList.Count / PageSize)
├─ Si CurrentPage > TotalPages → CurrentPage = TotalPages
└─ Si CurrentPage < 1 → CurrentPage = 1

PASO 4: Actualizar estados
├─ HasNextPage = (CurrentPage < TotalPages)
├─ HasPreviousPage = (CurrentPage > 1)
└─ PageInfo = $"Página {CurrentPage} de {TotalPages}"

PASO 5: Extraer items de página actual
├─ startIndex = (CurrentPage - 1) * PageSize
├─ pageItems = filteredList.Skip(startIndex).Take(PageSize)
└─ Ejemplo: Página 2, PageSize 15
           startIndex = (2-1) * 15 = 15
           Items 15-29 (índices 15 a 29)

PASO 6: Actualizar UI
├─ FilteredHolidays.Clear()
├─ FilteredHolidays.AddAll(filteredList) // Todos los filtrados
├─ PagedHolidays.Clear()
├─ PagedHolidays.AddAll(pageItems) // Solo página actual
└─ HasHolidays = PagedHolidays.Any()

SALIDA: PagedHolidays con máximo PageSize items
```

---

## 3. Flujo Temporal de Eventos

### Caso 1: Carga Inicial
```
OnAppearing()
  ↓
LoadHolidaysCommand.Execute()
  ↓
LoadHolidaysAsync()
  ├─ Verifica cache (5 min)
  ├─ Si no hay: GetAllAsync(page: 1, pageSize: 1000)
  ├─ Holidays.Clear() y llena con todos
  ├─ UpdateCounts() → TotalHolidays, NationalCount, LocalCount, CompanyCount
  └─ ApplyFilter()
       ├─ CurrentPage = 1 (default)
       ├─ TotalPages = ceil(1000/15) = 67
       ├─ HasNextPage = true, HasPreviousPage = false
       ├─ PageInfo = "Página 1 de 67"
       └─ PagedHolidays = Holidays[0:15]

UI Mostrará:
- Stats: 1000 Total, xxx Nacional, xxx Local, xxx Empresa
- Cards: 15 items
- PageInfo: "Página 1 de 67"
- Botón Anterior: DESHABILITADO (gris)
- Botón Siguiente: HABILITADO (azul)
```

### Caso 2: Usuario Busca "Navidad"
```
SearchText = "Navidad"
  ↓
OnSearchTextChanged() [automatic]
  ↓
ApplyFilter()
  ├─ Paso 1: Filtro tipo = Sin cambios (SelectedFilter = "All")
  ├─ Paso 2: Filtro búsqueda = Contiene "Navidad"
  │         Result: 5 items encontrados
  ├─ Paso 3: TotalPages = ceil(5/15) = 1
  ├─ CurrentPage = 1 (se resetea automáticamente)
  ├─ HasNextPage = false, HasPreviousPage = false
  ├─ PageInfo = "Página 1 de 1"
  └─ PagedHolidays = 5 items

UI Mostrará:
- PageInfo: "Página 1 de 1"
- Cards: 5 items (solo Navidad)
- Botón Anterior: DESHABILITADO
- Botón Siguiente: DESHABILITADO
```

### Caso 3: Usuario Filtra por "Nacional" + Va a Página 2
```
FilterCommand("National")
  ↓
Filter("National")
  ├─ SelectedFilter = "National"
  ├─ CurrentPage = 1 (resetea)
  └─ ApplyFilter()
       ├─ Paso 1: Filtro tipo = Solo NATIONAL
       │         Result: 300 items
       ├─ Paso 2: Sin búsqueda
       ├─ Paso 3: TotalPages = ceil(300/15) = 20
       ├─ HasNextPage = true, HasPreviousPage = false
       ├─ PageInfo = "Página 1 de 20"
       └─ PagedHolidays = 15 items (0-14)

UI Mostrará:
- Stats: 1000 Total, 300 Nacional, xxx Local, xxx Empresa
- Cards: 15 items Nacionales
- PageInfo: "Página 1 de 20"
- Botón Anterior: DESHABILITADO
- Botón Siguiente: HABILITADO

// Usuario da click en Siguiente
NextPageCommand.Execute()
  ↓
NextPage()
  ├─ HasNextPage == true ✓
  ├─ CurrentPage++ (1 → 2)
  └─ ApplyFilter()
       ├─ SelectedFilter aún es "National"
       ├─ Paso 1-2: Filtro = 300 Nacionales
       ├─ Paso 3: TotalPages = 20
       ├─ Paso 4: HasNextPage = true, HasPreviousPage = true
       ├─ PageInfo = "Página 2 de 20"
       ├─ startIndex = (2-1)*15 = 15
       └─ PagedHolidays = items[15:30]

UI Mostrará:
- PageInfo: "Página 2 de 20"
- Cards: items 16-30 Nacionales
- Botón Anterior: HABILITADO
- Botón Siguiente: HABILITADO
```

---

## 4. Comparación: Antes vs Después

### Antes (Sin Paginación)
```csharp
// ViewModel
[ObservableProperty]
private ObservableCollection<HolidayDto> _filteredHolidays = new();

// XAML
<VerticalStackLayout BindableLayout.ItemsSource="{Binding FilteredHolidays}">

// Resultado:
// - Si hay 100 items filtrados → Renderiza 100 items en UI
// - Si hay 300 items filtrados → Renderiza 300 items
// - Lag noticeable al filtrar
// - Scroll lento
```

### Después (Con Paginación)
```csharp
// ViewModel
[ObservableProperty]
private ObservableCollection<HolidayDto> _pagedHolidays = new(); // Máximo 15

[ObservableProperty]
private int _pageSize = 15;

[ObservableProperty]
private int _totalPages;

[ObservableProperty]
private bool _hasNextPage;

[ObservableProperty]
private bool _hasPreviousPage;

[RelayCommand]
private void NextPage() { /* ... */ }

[RelayCommand]
private void PreviousPage() { /* ... */ }

// XAML
<VerticalStackLayout BindableLayout.ItemsSource="{Binding PagedHolidays}">
  <!-- Siempre máximo 15 items -->

  <!-- Botones Next/Previous -->
</VerticalStackLayout>

// Resultado:
// - Siempre renderiza máximo 15 items
// - Filtrado muy rápido
// - Scroll suave
// - UX mejorada con paginación clara
```

---

## 5. Lógica de Habilitación de Botones

### Button Previous (Anterior):
```xaml
IsEnabled="{Binding HasPreviousPage}"
Background="{Binding HasPreviousPage, StringFormat='{0:False:#D0D5DD,True:#203461}'}"
TextColor="{Binding HasPreviousPage, StringFormat='{0:False:#9CA3AF,True:White}'}"
```

Tabla de estados:
```
Página  | HasPreviousPage | Habilitado | Color Fondo | Color Texto
--------|-----------------|------------|------------|------------
1       | false          | NO         | Gris       | Gris
2       | true           | SI         | Azul       | Blanco
3       | true           | SI         | Azul       | Blanco
Última  | true           | SI         | Azul       | Blanco
```

### Button Next (Siguiente):
```xaml
IsEnabled="{Binding HasNextPage}"
Background="{Binding HasNextPage, StringFormat='{0:False:#D0D5DD,True:#203461}'}"
TextColor="{Binding HasNextPage, StringFormat='{0:False:#9CA3AF,True:White}'}"
```

Tabla de estados:
```
Página  | CurrentPage < TotalPages | HasNextPage | Habilitado | Color Fondo
--------|--------------------------|-------------|------------|------------
1       | true                     | true        | SI         | Azul
Penúlt  | true                     | true        | SI         | Azul
Última  | false                    | false       | NO         | Gris
Única   | false                    | false       | NO         | Gris
```

---

## 6. Debugging: Output en Consola

Cuando ejecutas en DEBUG, verás logs como:

```
✅ Loaded 1000 holidays from backend
📈 Stats - Total: 1000, National: 300, Local: 400, Company: 300
🔍 All filter applied: 1000/1000 | Page 1/67 | Showing 15 items

// Usuario filtra
🔍 National filter applied: 300/1000 | Page 1/20 | Showing 15 items

// Usuario busca
🔍 National filter applied: 5/1000 | Page 1/1 | Showing 5 items

// Usuario va a página 2
🔍 National filter applied: 300/1000 | Page 2/20 | Showing 15 items
```

---

## 7. Cambios en el OnSearchTextChanged

### Antes:
```csharp
partial void OnSearchTextChanged(string value)
{
    ApplyFilter();
}
```

### Después (Igual, pero con reseteo automático):
```csharp
partial void OnSearchTextChanged(string value)
{
    ApplyFilter();
    // El reseteo a página 1 ocurre implícitamente en Search()
    // que es llamado automáticamente
}
```

Nota: El reseteo a página 1 se puede hacer de dos formas:
1. En `OnSearchTextChanged()` directamente
2. En `Search()` command que es ejecutado por el usuario

Actualmente está en `Search()` que se ejecuta cuando el usuario hace click en botón Search.
Para hacerlo automático en cada keystroke, cambiar a:

```csharp
partial void OnSearchTextChanged(string value)
{
    CurrentPage = 1; // Resetea inmediatamente
    ApplyFilter();
}
```

---

## 8. Calculadora de Paginación

Ejemplos rápidos:

```
Ejemplo 1: 150 items totales, PageSize = 15
├─ TotalPages = Math.Ceiling(150 / 15) = 10
├─ Página 1: items 0-14
├─ Página 5: items 60-74
├─ Página 10: items 135-149

Ejemplo 2: 247 items totales, PageSize = 15
├─ TotalPages = Math.Ceiling(247 / 15) = 17
├─ Página 17: items 240-246 (solo 7 items)

Ejemplo 3: 5 items totales, PageSize = 15
├─ TotalPages = Math.Ceiling(5 / 15) = 1
├─ Página 1: items 0-4
├─ Sin siguiente página

Ejemplo 4: 0 items totales, PageSize = 15
├─ TotalPages = 1 (por defecto, nunca es 0)
├─ PagedHolidays es vacío
├─ Muestra "No hay festivos"
```

---

## 9. Modificar PageSize

Si necesitas cambiar el tamaño de página (ej: 20 items en lugar de 15):

**Archivo:** HolidaysManagementViewModel.cs
**Línea:** ~58

```csharp
// Cambiar de:
private int _pageSize = 15;

// A:
private int _pageSize = 20;
```

El cambio se aplica automáticamente a toda la paginación.

---

## 10. Futuros Mejoras Potenciales

```csharp
// 1. Permitir al usuario elegir PageSize:
[ObservableProperty]
private int _pageSize = 15;

[RelayCommand]
private void ChangePageSize(int newSize)
{
    PageSize = newSize;
    CurrentPage = 1;
    ApplyFilter();
}

// 2. Ir a página específica:
[RelayCommand]
private void GoToPage(int pageNumber)
{
    if (pageNumber >= 1 && pageNumber <= TotalPages)
    {
        CurrentPage = pageNumber;
        ApplyFilter();
    }
}

// 3. Infinite scroll en lugar de botones:
// Cargar siguientes 15 items cuando user scrollea al final

// 4. Recordar página actual:
// Guardar CurrentPage en Preferences
// Restaurar al volver a la pantalla
```

