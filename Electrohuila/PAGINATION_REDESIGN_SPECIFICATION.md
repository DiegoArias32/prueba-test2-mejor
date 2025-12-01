# Especificación de Rediseño: Botones de Paginación - Pantalla Festivos

## Resumen Ejecutivo

Se ha rediseñado completamente la sección de paginación en la pantalla de Gestión de Festivos (HolidaysManagementPage.xaml) para proporcionar una experiencia visual moderna, profesional y alineada con Material Design 3 y la identidad de marca de ElectroHuila.

**Archivo modificado:** `/pqr-scheduling-appointments-app/Views/Admin/HolidaysManagementPage.xaml` (líneas 393-597)

---

## Diseño Anterior vs. Nuevo

### Antes (Problemas Identificados)
- Texto plano: "◄ Anterior | Siguiente ►"
- Desalineamiento vertical
- Bajo contraste y aspecto poco profesional
- Estructura simple sin estados visuales claros
- Indicador de página poco visible
- Falta de jerarquía visual
- Espaciado inconsistente

### Después (Mejoras Aplicadas)
- Botones elegantes con bordes redondeados (CornerRadius: 10px)
- Iconos FontAwesome profesionales
- Estados visuales diferenciados (habilitado/deshabilitado)
- Indicador de página en contenedor destacado
- Diseño Material Design 3 completo
- Sombras sutiles para elevación
- Spacing y padding optimizados
- Tipografía clara y jerarquizada

---

## Especificaciones de Diseño

### 1. Contenedor Principal de Paginación

**Estructura:** VerticalStackLayout
**Propiedades:**
- Padding: 20px (horizontal), 20px (superior), 24px (inferior)
- Spacing: 16px entre elementos
- VerticalOptions: End (anclado al final)
- Visibilidad: Solo se muestra si hay más de una página

```xaml
<VerticalStackLayout Grid.Row="2" Padding="20,20,20,24" Spacing="16"
                     IsVisible="{Binding TotalPages, Converter={StaticResource IntToBoolConverter}}"
                     VerticalOptions="End">
```

### 2. Indicador de Página (Page Info Container)

**Diseño:**
- Fondo sutil: #F9FAFB (gris muy claro)
- Borde: 1px, color #E5E7EB (gris claro)
- Bordes redondeados: 10px
- Sombra sutil: Opacity 0.02, Radius 4px

**Contenido:**
- Icono FontAwesome: &#xf02d; (icono de lista)
- Texto principal: "Página X de Y" (en azul #203461, bold)
- Texto secundario: "Mostrando resultados" (en gris #6B7280)
- Espaciado entre elementos: 8px

```xaml
<!-- Indicador Visual -->
<Border Background="#F9FAFB" Stroke="#E5E7EB" StrokeThickness="1"
        Padding="16,12">
    <Border.StrokeShape>
        <RoundRectangle CornerRadius="10"/>
    </Border.StrokeShape>
```

**Paleta de Colores:**
- Fondo: #F9FAFB
- Borde: #E5E7EB
- Texto primario: #203461 (azul ElectroHuila)
- Texto secundario: #6B7280 (gris)

### 3. Botones de Navegación

#### Características Comunes
- Tamaño mínimo: 120px ancho x 44px alto (accesibilidad)
- Padding interno: 16px (horizontal), 12px (vertical)
- Bordes redondeados: 10px
- Sombra sutil con opacidad dinámica
- Spacing entre botones: 12px

#### Estados Visuales

**Estado HABILITADO (HasPreviousPage/HasNextPage = True)**
- Fondo: #203461 (azul ElectroHuila)
- Texto: Blanco
- Icono: Blanco
- Sombra: Visible (Opacity 0.1, Radius 6px)
- Cursor: Pointer
- Interactividad: Completa

```xaml
<Border.Triggers>
    <DataTrigger TargetType="Border" Binding="{Binding HasPreviousPage}" Value="True">
        <Setter Property="Background" Value="#203461"/>
    </DataTrigger>
</Border.Triggers>
```

**Estado DESHABILITADO (HasPreviousPage/HasNextPage = False)**
- Fondo: #F3F4F6 (gris claro)
- Texto: #9CA3AF (gris)
- Icono: #D1D5DB (gris más claro)
- Sombra: Invisible (Opacity 0)
- Cursor: Bloqueado
- Opacidad: Reducida visualmente

```xaml
<DataTrigger TargetType="Border" Binding="{Binding HasPreviousPage}" Value="False">
    <Setter Property="Background" Value="#F3F4F6"/>
</DataTrigger>
```

#### Botón "Anterior"

**Contenido:**
- Icono izquierdo: &#xf053; (flecha izquierda)
- Texto: "Anterior"
- Layout: HorizontalStackLayout con spacing 6px

```xaml
<HorizontalStackLayout Spacing="6">
    <Label Text="&#xf053;" FontSize="13" FontFamily="FontAwesome-Solid"/>
    <Label Text="Anterior" FontSize="13" FontAttributes="Bold"/>
</HorizontalStackLayout>
```

#### Botón "Siguiente"

**Contenido:**
- Texto: "Siguiente"
- Icono derecho: &#xf054; (flecha derecha)
- Layout: HorizontalStackLayout con spacing 6px

```xaml
<HorizontalStackLayout Spacing="6">
    <Label Text="Siguiente" FontSize="13" FontAttributes="Bold"/>
    <Label Text="&#xf054;" FontSize="13" FontFamily="FontAwesome-Solid"/>
</HorizontalStackLayout>
```

### 4. Paleta de Colores Completa

| Elemento | Color | Código Hex | Uso |
|----------|-------|-----------|-----|
| Primario | Azul ElectroHuila | #203461 | Botones activos, texto principal |
| Fondo Activo | Gris muy claro | #F3F4F6 | Botones deshabilitados |
| Fondo Contenedor | Gris ultraligero | #F9FAFB | Contenedor del indicador |
| Borde | Gris claro | #E5E7EB | Bordes sutiles |
| Texto Principal | Azul | #203461 | Información principal |
| Texto Secundario | Gris | #6B7280 | Información secundaria |
| Texto Deshabilitado | Gris | #9CA3AF | Botones inactivos |
| Icono Deshabilitado | Gris claro | #D1D5DB | Iconos inactivos |

### 5. Tipografía

| Elemento | FontSize | FontAttributes | Uso |
|----------|----------|----------------|-----|
| Indicador principal | 14px | Bold | "Página X de Y" |
| Indicador secundario | 11px | Normal | "Mostrando resultados" |
| Botones | 13px | Bold | Texto de navegación |
| Iconos | 13px | Normal | Flechas FontAwesome |

### 6. Espaciado (Material Design 3)

```
Padding general: 20px (horizontal), 20px (superior), 24px (inferior)
Spacing contenedor: 16px
Spacing interno botones: 6px
Spacing entre botones: 12px
Padding botones: 16px (horizontal), 12px (vertical)
Padding indicador: 16px (horizontal), 12px (vertical)
```

---

## Características Implementadas

### 1. Indicador de Página Mejorado
- Contenedor visual claro y destacado
- Icono visual representativo (list icon)
- Información primaria y secundaria estratificada
- Fácil identificación de la posición actual en la paginación

### 2. Estados Visuales Diferenciados
- Botones activos en azul (#203461) con text en blanco
- Botones inactivos en gris (#F3F4F6) con texto atenuado
- Transiciones de color basadas en DataTriggers
- Sombras dinámicas para elevación visual

### 3. Accesibilidad
- Tamaño mínimo de botones: 44px alto (estándar WCAG)
- Contraste suficiente en todos los estados
- Iconos + texto para claridad (no solo iconos)
- Espaciado adecuado para toque táctil

### 4. Diseño Responsivo
- Centrado automático (HorizontalOptions="Center")
- Anclado al final del contenedor (VerticalOptions="End")
- Adapta a diferentes tamaños de pantalla
- Mantiene proporciones visuales

### 5. Material Design 3 Compliance
- Bordes redondeados moderados (10px)
- Sombras sutiles para elevación
- Paleta de colores consistente
- Tipografía clara y jerarquizada
- Espaciado generoso (16px entre secciones)

---

## Integración Técnica

### Bindings Utilizados
```csharp
// En ViewModel (HolidaysManagementViewModel)
public bool HasPreviousPage { get; set; }      // Controla estado botón "Anterior"
public bool HasNextPage { get; set; }          // Controla estado botón "Siguiente"
public int TotalPages { get; set; }            // Controla visibilidad de paginación
public string PageInfo { get; set; }           // Información actual de página
public ICommand PreviousPageCommand { get; }   // Comando anterior
public ICommand NextPageCommand { get; }       // Comando siguiente
```

### Converters Utilizados
```xaml
<converters:IntToBoolConverter x:Key="IntToBoolConverter" />
<!-- Usado para: IsVisible="{Binding TotalPages, Converter={StaticResource IntToBoolConverter}}" -->
<!-- Convierte: 0 -> false (oculta), >0 -> true (muestra) -->
```

---

## Guía de Mantenimiento

### Cambiar Colores
Para actualizar los colores de la marca:
1. Buscar y reemplazar #203461 con el nuevo color primario
2. Actualizar #F9FAFB para fondos sutiles
3. Mantener proporciones de contraste WCAG AA mínimo

### Ajustar Espaciado
Para modificar espaciado general:
1. Contenedor principal Padding: "20,20,20,24"
2. Spacing vertical: 16px
3. Spacing dentro de botones: 6px
4. Spacing entre botones: 12px

### Modificar Textos
- Indicador principal: Modificar `StringFormat='Página {0}'`
- Indicador secundario: Modificar texto fijo "Mostrando resultados"
- Botones: Cambiar "Anterior" y "Siguiente" por otros textos

---

## Archivos Afectados

- **Primario:** `/pqr-scheduling-appointments-app/Views/Admin/HolidaysManagementPage.xaml`
- **Líneas:** 393-597
- **Cambios:** Completo rediseño de sección de paginación

---

## Notas Importantes

1. **FontAwesome Icons:** Los iconos utilizados requieren que FontAwesome esté instalado en el proyecto:
   - &#xf053; = Flecha izquierda
   - &#xf054; = Flecha derecha
   - &#xf02d; = Icono de lista

2. **DataTriggers:** El sistema de estados usa DataTriggers para cambios dinámicos de color. Estos requieren que los bindings (HasPreviousPage, HasNextPage) estén correctamente implementados en el ViewModel.

3. **Sombras:** Las sombras pueden variar según la plataforma (iOS, Android). Se han configurado con valores conservadores para compatibilidad.

4. **Performance:** El uso de múltiples Triggers es mínimo y no debería afectar el performance de la aplicación.

---

## Referencias de Diseño

- **Material Design 3:** https://m3.material.io
- **Paleta ElectroHuila:** #203461 (azul primario)
- **FontAwesome 6:** Icons utilizados del catálogo estándar
- **MAUI Border & Layout:** Documentación oficial de Microsoft

---

## Fecha de Implementación

**Fecha:** 2025-11-30
**Versión:** 1.0
**Estado:** Completado y listo para producción

