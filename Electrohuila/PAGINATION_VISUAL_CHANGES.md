# Cambios Visuales - Paginación en Festivos

## 1. ViewModel - Propiedades Agregadas

### ANTES:
```csharp
[ObservableProperty]
private ObservableCollection<HolidayDto> _holidays = new();

[ObservableProperty]
private ObservableCollection<HolidayDto> _filteredHolidays = new();

[ObservableProperty]
private string _searchText = string.Empty;

[ObservableProperty]
private string _selectedFilter = "All";

[ObservableProperty]
private bool _hasHolidays;

[ObservableProperty]
private int _totalHolidays;

[ObservableProperty]
private int _nationalCount;

[ObservableProperty]
private int _localCount;

[ObservableProperty]
private int _companyCount;
```

### DESPUES:
```csharp
[ObservableProperty]
private ObservableCollection<HolidayDto> _holidays = new();

[ObservableProperty]
private ObservableCollection<HolidayDto> _filteredHolidays = new();

[ObservableProperty]
private ObservableCollection<HolidayDto> _pagedHolidays = new();    // ← NUEVO

[ObservableProperty]
private string _searchText = string.Empty;

[ObservableProperty]
private string _selectedFilter = "All";

[ObservableProperty]
private bool _hasHolidays;

[ObservableProperty]
private int _totalHolidays;

[ObservableProperty]
private int _nationalCount;

[ObservableProperty]
private int _localCount;

[ObservableProperty]
private int _companyCount;

// Pagination properties                           // ← NUEVO
[ObservableProperty]
private int _currentPage = 1;

[ObservableProperty]
private int _pageSize = 15;

[ObservableProperty]
private int _totalPages = 1;

[ObservableProperty]
private bool _hasNextPage;

[ObservableProperty]
private bool _hasPreviousPage;

[ObservableProperty]
private string _pageInfo = "Página 1 de 1";
```

---

## 2. ViewModel - Comandos Nuevos

### ANTES:
```csharp
[RelayCommand]
private void Search() { ... }

[RelayCommand]
private void Filter(string filter) { ... }
```

### DESPUES:
```csharp
[RelayCommand]
private void Search() { ... }

[RelayCommand]
private void Filter(string filter) { ... }

[RelayCommand]                                    // ← NUEVO
private void NextPage()
{
    if (HasNextPage)
    {
        CurrentPage++;
        ApplyFilter();
    }
}

[RelayCommand]                                    // ← NUEVO
private void PreviousPage()
{
    if (HasPreviousPage)
    {
        CurrentPage--;
        ApplyFilter();
    }
}
```

---

## 3. ViewModel - Filter() y Search()

### ANTES:
```csharp
[RelayCommand]
private void Search()
{
    ApplyFilter();
}

[RelayCommand]
private void Filter(string filter)
{
    SelectedFilter = filter;
    ApplyFilter();
}
```

### DESPUES:
```csharp
[RelayCommand]
private void Search()
{
    CurrentPage = 1;  // ← NUEVO: Resetea a página 1
    ApplyFilter();
}

[RelayCommand]
private void Filter(string filter)
{
    SelectedFilter = filter;
    CurrentPage = 1;  // ← NUEVO: Resetea a página 1
    ApplyFilter();
}
```

---

## 4. ViewModel - ApplyFilter() (GRAN CAMBIO)

### ANTES (42 líneas):
```csharp
private void ApplyFilter()
{
    var filtered = Holidays.AsEnumerable();

    // Apply type filter
    if (SelectedFilter != "All")
    {
        var filterUpper = SelectedFilter.ToUpperInvariant();
        filtered = filtered.Where(h => /* ... */);
    }

    // Apply search filter
    if (!string.IsNullOrWhiteSpace(SearchText))
    {
        var searchLower = SearchText.ToLowerInvariant();
        filtered = filtered.Where(h => /* ... */);
    }

    // OPTIMIZE: Single enumeration + batch update
    var filteredList = filtered.ToList();

    FilteredHolidays.Clear();
    foreach (var holiday in filteredList)
    {
        FilteredHolidays.Add(holiday);
    }

    HasHolidays = FilteredHolidays.Any();

    #if DEBUG
    if (Holidays.Any())
    {
        Console.WriteLine($"🔍 {SelectedFilter} filter applied: {FilteredHolidays.Count}/{Holidays.Count}");
    }
    #endif
}
```

### DESPUES (65 líneas):
```csharp
private void ApplyFilter()
{
    var filtered = Holidays.AsEnumerable();

    // Apply type filter
    if (SelectedFilter != "All")
    {
        var filterUpper = SelectedFilter.ToUpperInvariant();
        filtered = filtered.Where(h => /* ... */);
    }

    // Apply search filter
    if (!string.IsNullOrWhiteSpace(SearchText))
    {
        var searchLower = SearchText.ToLowerInvariant();
        filtered = filtered.Where(h => /* ... */);
    }

    // ← NUEVO: Calcular paginación
    var filteredList = filtered.ToList();
    var totalFilteredCount = filteredList.Count;

    // Calculate pagination
    TotalPages = totalFilteredCount > 0
        ? (int)Math.Ceiling((double)totalFilteredCount / PageSize)
        : 1;

    // Ensure CurrentPage is valid
    if (CurrentPage > TotalPages)
        CurrentPage = TotalPages;
    if (CurrentPage < 1)
        CurrentPage = 1;

    // Update pagination state
    HasNextPage = CurrentPage < TotalPages;
    HasPreviousPage = CurrentPage > 1;
    PageInfo = $"Página {CurrentPage} de {TotalPages}";

    // Get the page items
    var startIndex = (CurrentPage - 1) * PageSize;
    var pageItems = filteredList
        .Skip(startIndex)
        .Take(PageSize)
        .ToList();

    // Update FilteredHolidays with all filtered items
    FilteredHolidays.Clear();
    foreach (var holiday in filteredList)
    {
        FilteredHolidays.Add(holiday);
    }

    // Update PagedHolidays with current page items only
    PagedHolidays.Clear();
    foreach (var holiday in pageItems)
    {
        PagedHolidays.Add(holiday);
    }

    HasHolidays = PagedHolidays.Any();

    #if DEBUG
    if (Holidays.Any())
    {
        Console.WriteLine($"🔍 {SelectedFilter} filter applied: {filteredList.Count}/{Holidays.Count} | Page {CurrentPage}/{TotalPages} | Showing {pageItems.Count} items");
    }
    #endif
}
```

---

## 5. ViewModel - LoadHolidaysAsync()

### ANTES:
```csharp
// Get all holidays from backend
var pagedResult = await _holidayService.GetAllAsync(page: 1, pageSize: 100);
```

### DESPUES:
```csharp
// Get all holidays from backend (requesting all with large pageSize to cache complete data)
// Actual pagination happens on the UI side with PagedHolidays collection
var pagedResult = await _holidayService.GetAllAsync(page: 1, pageSize: 1000);
```

---

## 6. XAML - Binding de Items

### ANTES (Línea 292):
```xaml
<VerticalStackLayout Padding="20,0,20,20"
                     Spacing="12"
                     BindableLayout.ItemsSource="{Binding FilteredHolidays}">
```

### DESPUES (Línea 290-292):
```xaml
<VerticalStackLayout Padding="20,0,20,20"
                     Spacing="12"
                     BindableLayout.ItemsSource="{Binding PagedHolidays}">
```

**Cambio importante:** De `FilteredHolidays` a `PagedHolidays`
- FilteredHolidays = TODOS los items que cumplen el filtro
- PagedHolidays = Solo los items de la página actual (max 15)

---

## 7. XAML - Verificación de Empty State

### ANTES (Línea 295):
```xaml
<VerticalStackLayout IsVisible="{Binding FilteredHolidays.Count, Converter={StaticResource InvertedBoolConverter}}"
```

### DESPUES (Línea 295):
```xaml
<VerticalStackLayout IsVisible="{Binding PagedHolidays.Count, Converter={StaticResource InvertedBoolConverter}}"
```

---

## 8. XAML - Controles de Paginación (NUEVO)

### ANTES:
```xaml
<!-- Final del VerticalStackLayout -->
</VerticalStackLayout>
```

### DESPUES (Líneas 388-440):
```xaml
<!-- Pagination Controls -->
<VerticalStackLayout Padding="0,16,0,0" Spacing="8" IsVisible="{Binding TotalPages, Converter={StaticResource BoolToValueConverter}}">

    <!-- Page Info -->
    <Label Text="{Binding PageInfo}"
           FontSize="13"
           FontAttributes="Bold"
           TextColor="#6B7280"
           HorizontalOptions="Center"/>

    <!-- Navigation Buttons -->
    <Grid ColumnDefinitions="Auto,*,Auto" ColumnSpacing="8" HorizontalOptions="Center">

        <!-- Previous Button -->
        <Border Grid.Column="0"
                Background="{Binding HasPreviousPage, StringFormat='{0:False:#D0D5DD,True:#203461}'}"
                StrokeThickness="0"
                Padding="14,8"
                IsEnabled="{Binding HasPreviousPage}">
            <Border.StrokeShape>
                <RoundRectangle CornerRadius="8"/>
            </Border.StrokeShape>
            <Border.GestureRecognizers>
                <TapGestureRecognizer Command="{Binding PreviousPageCommand}" />
            </Border.GestureRecognizers>
            <Label Text="&#xf053; Anterior"
                   FontSize="12"
                   FontAttributes="Bold"
                   TextColor="{Binding HasPreviousPage, StringFormat='{0:False:#9CA3AF,True:White}'}"
                   HorizontalOptions="Center"/>
        </Border>

        <!-- Spacer -->
        <BoxView Grid.Column="1" WidthRequest="20"/>

        <!-- Next Button -->
        <Border Grid.Column="2"
                Background="{Binding HasNextPage, StringFormat='{0:False:#D0D5DD,True:#203461}'}"
                StrokeThickness="0"
                Padding="14,8"
                IsEnabled="{Binding HasNextPage}">
            <Border.StrokeShape>
                <RoundRectangle CornerRadius="8"/>
            </Border.StrokeShape>
            <Border.GestureRecognizers>
                <TapGestureRecognizer Command="{Binding NextPageCommand}" />
            </Border.GestureRecognizers>
            <Label Text="Siguiente &#xf054;"
                   FontSize="12"
                   FontAttributes="Bold"
                   TextColor="{Binding HasNextPage, StringFormat='{0:False:#9CA3AF,True:White}'}"
                   HorizontalOptions="Center"/>
        </Border>
    </Grid>
</VerticalStackLayout>
```

---

## 9. Resumen de Cambios

| Elemento | Antes | Después | Impacto |
|----------|-------|---------|---------|
| pageSize en backend | 100 | 1000 | Carga todo en memoria una sola vez |
| Items en UI | Variable (todos los filtrados) | Máximo 15 | Mejor rendimiento |
| Propiedades | 12 | 19 | +7 para paginación |
| Comandos | Search, Filter | Search, Filter, NextPage, PreviousPage | +2 comandos |
| ApplyFilter() | 42 líneas | 65 líneas | +23 líneas (lógica paginación) |
| XAML items | FilteredHolidays | PagedHolidays | Mostrar solo página actual |
| XAML UI | Sin controles | Botones Previous/Next | UI de paginación |

---

## 10. Estructura de Carpetas

```
HolidaysManagementViewModel.cs
  - Propiedades (12 → 19)
  - Métodos (6 → 8)

HolidaysManagementPage.xaml
  - Binding ItemsSource (1 línea cambio)
  - Grid de controles (52 líneas nuevas)

HolidaysManagementPage.xaml.cs
  - Sin cambios (el binding automático maneja todo)
```

---

## 11. Checklist de Verificación

- [x] Propiedades de paginación agregadas
- [x] Comandos NextPage y PreviousPage creados
- [x] Search y Filter resetean a página 1
- [x] ApplyFilter calcula páginas correctamente
- [x] PagedHolidays se llena con items de página actual
- [x] FilteredHolidays se llena con todos los items filtrados
- [x] XAML binding cambiado a PagedHolidays
- [x] Botones Previous/Next en XAML
- [x] Botones responden a HasPreviousPage/HasNextPage
- [x] PageInfo muestra "Página X de Y"
- [x] Backend pageSize cambiado a 1000
- [x] Debug console muestra información de paginación

---

## 12. Files Modified Summary

```
Modified Files:
  1. ViewModels/HolidaysManagementViewModel.cs
     - Lines 23-70: Added pagination properties
     - Lines 172-214: Updated Search, Filter, added NextPage, PreviousPage
     - Lines 221-305: Completely rewrote ApplyFilter with pagination logic
     - Line 112: Changed pageSize from 100 to 1000

  2. Views/Admin/HolidaysManagementPage.xaml
     - Line 292: Changed ItemsSource from FilteredHolidays to PagedHolidays
     - Line 295: Updated IsVisible binding to PagedHolidays.Count
     - Lines 388-440: Added pagination controls section

Unchanged Files:
  - Views/Admin/HolidaysManagementPage.xaml.cs (binding works automatically)
```

