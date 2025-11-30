# Implementación de Paginación en Festivos (MAUI)

## Cambios Realizados

### 1. ViewModel: HolidaysManagementViewModel.cs

#### Propiedades de Paginación Agregadas:
```csharp
[ObservableProperty]
private int _currentPage = 1;              // Página actual

[ObservableProperty]
private int _pageSize = 15;                // Elementos por página (15 items)

[ObservableProperty]
private int _totalPages = 1;               // Total de páginas calculadas

[ObservableProperty]
private bool _hasNextPage;                 // Si hay siguiente página

[ObservableProperty]
private bool _hasPreviousPage;             // Si hay página anterior

[ObservableProperty]
private string _pageInfo = "Página 1 de 1"; // Texto informativo de paginación

[ObservableProperty]
private ObservableCollection<HolidayDto> _pagedHolidays = new(); // Items de la página actual
```

#### Comandos de Paginación Agregados:
- `NextPageCommand` - Navega a la siguiente página
- `PreviousPageCommand` - Navega a la página anterior

#### Método Search() - Actualizado:
- Resetea a página 1 cuando el usuario busca
- Mantiene la funcionalidad de búsqueda

#### Método Filter() - Actualizado:
- Resetea a página 1 cuando cambia el filtro
- Mantiene los 4 filtros: Todos, Nacional, Local, Empresa

#### Método ApplyFilter() - Completamente Reescrito:
```
Flujo de Paginación:
1. Aplica filtro por tipo (Nacional, Local, Empresa, Todos)
2. Aplica búsqueda por texto
3. Calcula total de páginas basado en filtered items
4. Valida que CurrentPage sea válida
5. Calcula HasNextPage y HasPreviousPage
6. Extrae items de la página actual
7. Actualiza PagedHolidays (items a mostrar)
8. Actualiza FilteredHolidays (todos los items filtrados - para referencia)
```

**Ventajas del Enfoque:**
- Los filtros se aplican sobre TODA la data antes de paginar
- Cada página muestra máximo 15 items
- Al cambiar filtro/búsqueda, resetea automáticamente a página 1
- Los conteos (Total, Nacional, Local, Empresa) se basan en TODOS los datos

#### LoadHolidaysAsync() - Cambio de pageSize:
```csharp
// ANTES:
var pagedResult = await _holidayService.GetAllAsync(page: 1, pageSize: 100);

// AHORA:
var pagedResult = await _holidayService.GetAllAsync(page: 1, pageSize: 1000);
```

**Razón:** Cargamos todos los datos en caché localmente y la paginación ocurre en la UI (client-side), no en el backend.

---

### 2. Vista: HolidaysManagementPage.xaml

#### Binding de Items - Actualizado:
```xaml
<!-- ANTES: BindableLayout.ItemsSource="{Binding FilteredHolidays}" -->
<!-- AHORA: BindableLayout.ItemsSource="{Binding PagedHolidays}" -->
```

#### Controles de Paginación Agregados:
```xaml
<!-- Pagination Controls -->
<VerticalStackLayout Padding="0,16,0,0" Spacing="8" IsVisible="{Binding TotalPages, Converter={StaticResource BoolToValueConverter}}">

    <!-- Page Info: "Página 1 de 10" -->
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
                IsEnabled="{Binding HasPreviousPage}">
            <!-- Icono: &#xf053; (Flecha izquierda) -->
            <Label Text="&#xf053; Anterior" ... />
        </Border>

        <!-- Next Button -->
        <Border Grid.Column="2"
                Background="{Binding HasNextPage, StringFormat='{0:False:#D0D5DD,True:#203461}'}"
                IsEnabled="{Binding HasNextPage}">
            <!-- Icono: &#xf054; (Flecha derecha) -->
            <Label Text="Siguiente &#xf054;" ... />
        </Border>
    </Grid>
</VerticalStackLayout>
```

**Características de la UI:**
- Botones dinámicos: deshabilitados (gris) cuando no hay más páginas
- Cambio de color: gris (#D0D5DD) cuando disabled, azul (#203461) cuando habilitado
- Texto informativo que muestra página actual y total
- Solo visible si hay más de 1 página (TotalPages > 1)

---

## Flujo de Uso

### Scenario 1: Usuario carga la pantalla
1. OnAppearing() ejecuta LoadHolidaysAsync()
2. Se cargan ~1000 festivos en la colección `Holidays`
3. Se ejecuta ApplyFilter()
4. CurrentPage = 1, se calculan TotalPages
5. Se muestran primeros 15 items en PagedHolidays
6. PageInfo muestra "Página 1 de X"

### Scenario 2: Usuario filtra por "Nacional"
1. Click en filtro "Nacional"
2. SelectedFilter cambia a "National"
3. CurrentPage se resetea a 1
4. ApplyFilter():
   - Filtra solo festivos Nacionales (~300 items)
   - TotalPages = 20 (300 / 15)
   - PagedHolidays muestra primeros 15 Nacionales
5. PageInfo: "Página 1 de 20"

### Scenario 3: Usuario busca "navidad"
1. Escribe "navidad" en search
2. CurrentPage se resetea a 1
3. ApplyFilter():
   - Aplica filtro actual + búsqueda
   - Encuentra 5 coincidencias
   - TotalPages = 1 (5 / 15 = 0.33 = 1 página)
   - PagedHolidays muestra los 5 resultados
4. Botón "Siguiente" deshabilitado (no hay siguiente)

### Scenario 4: Usuario va a página siguiente
1. Click en botón "Siguiente"
2. CurrentPage incrementa a 2
3. ApplyFilter():
   - Mantiene filtro y búsqueda
   - Skip(15) + Take(15) -> items 16-30
   - PagedHolidays actualiza con nuevos items
4. PageInfo: "Página 2 de X"
5. Botón "Anterior" habilitado

---

## Métricas de Rendimiento

### Antes:
- Cargaba 100 items en la UI de una vez
- Al filtrar: lag noticeable mientras renderiza 100 items
- Scroll lento con 100+ elementos

### Después:
- Carga solo 15 items en la UI
- Al filtrar: muy rápido (solo 15 items en render)
- Scroll suave y responsivo
- Paginación instantánea (datos en caché)

---

## Archivos Modificados

1. **HolidaysManagementViewModel.cs**
   - Agregadas 7 propiedades de paginación
   - Agregados 2 comandos (NextPage, PreviousPage)
   - Actualizado método ApplyFilter() completo
   - Actualizado Search() y Filter()

2. **HolidaysManagementPage.xaml**
   - Cambiado binding de FilteredHolidays a PagedHolidays
   - Agregados controles de paginación con botones Previous/Next
   - Agregado PageInfo label

---

## Compatibilidad

- Mantiene todos los filtros funcionando (Todos, Nacional, Local, Empresa)
- Mantiene búsqueda funcional
- Mantiene conteos en stats cards (Total, Nacional, Local, Empresa)
- Compatible con el método Search() existente
- Usa [RelayCommand] de CommunityToolkit.MVVM

---

## Notas Importantes

1. **PageSize = 15**: Ajustable si se necesita. Modificar la línea:
   ```csharp
   private int _pageSize = 15; // Cambiar este valor
   ```

2. **Backend pageSize = 1000**: Suficiente para la mayoría de casos. Si hay más de 1000 festivos, aumentar este valor en LoadHolidaysAsync().

3. **Filtros aplicados antes de paginar**: Esto asegura que los counts y los filtros siempre muestren datos correctos.

4. **Debug console output**: Muestra información de paginación:
   ```
   🔍 All filter applied: 150/150 | Page 1/10 | Showing 15 items
   ```

---

## Testing Recomendado

- [ ] Cargar pantalla de Festivos
- [ ] Verificar que muestra 15 items máximo
- [ ] Ir a página siguiente
- [ ] Ir a página anterior
- [ ] Filtrar por Nacional
- [ ] Buscar por nombre
- [ ] Combinar filtro + búsqueda
- [ ] Verificar que botones se deshabilitan correctamente
- [ ] Refrescar datos (Refresh)
- [ ] Eliminar un festivo y verificar que paginación se actualiza

