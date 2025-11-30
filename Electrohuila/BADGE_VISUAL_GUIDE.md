# Guía Visual del Badge de Notificaciones

## Aspecto Visual del Badge

### Estado Sin Notificaciones No Leídas
```
┌─────────────────────────────────────────┐
│  MENÚ LATERAL                           │
├─────────────────────────────────────────┤
│  🏠  Panel Principal                    │
│  📅  Mis Citas                          │
│  🎉  Festivos                           │
│  🔔  Notificaciones                     │  ← Sin badge (count = 0)
└─────────────────────────────────────────┘
```

### Estado Con 3 Notificaciones No Leídas
```
┌─────────────────────────────────────────┐
│  MENÚ LATERAL                           │
├─────────────────────────────────────────┤
│  🏠  Panel Principal                    │
│  📅  Mis Citas                          │
│  🎉  Festivos                           │
│  🔔  Notificaciones              ⬤ 3   │  ← Badge rojo con número
└─────────────────────────────────────────┘
         └─ Badge: Circular, rojo, con sombra
```

### Estado Con Más de 99 Notificaciones
```
┌─────────────────────────────────────────┐
│  🔔  Notificaciones            ⬤ 99+  │  ← Badge muestra "99+"
└─────────────────────────────────────────┘
```

---

## Especificaciones de Diseño

### Badge Container (Border)
```
┌──────────────────────────────────┐
│  Propiedad       │  Valor         │
├──────────────────┼────────────────┤
│  Background      │  #DC2626 (Rojo)│
│  Width           │  24 px         │
│  Height          │  24 px         │
│  Border Radius   │  12 px         │
│  Stroke          │  0             │
│  Shadow Opacity  │  0.5           │
│  Shadow Radius   │  4 px          │
│  Shadow Offset   │  0, 2          │
└──────────────────┴────────────────┘
```

### Badge Text (Label)
```
┌──────────────────────────────────┐
│  Propiedad       │  Valor         │
├──────────────────┼────────────────┤
│  Color           │  White         │
│  Font Size       │  10            │
│  Font Attributes │  Bold          │
│  Alignment       │  Center        │
│  Vertical Align  │  Center        │
└──────────────────┴────────────────┘
```

---

## Estados Visuales del Badge

### 1. Oculto (No Visible)
**Condiciones:**
- UnreadCount = 0
- HasUnreadNotifications = false

**Apariencia:**
```
│  🔔  Notificaciones                     │
```

### 2. Badge con Número Individual (1-9)
**Condiciones:**
- UnreadCount = 1-9
- HasUnreadNotifications = true

**Apariencia:**
```
│  🔔  Notificaciones              ⬤ 5   │
                                   └─ Número centrado
```

### 3. Badge con Número Doble Dígito (10-99)
**Condiciones:**
- UnreadCount = 10-99
- HasUnreadNotifications = true

**Apariencia:**
```
│  🔔  Notificaciones             ⬤ 42   │
                                   └─ Dos dígitos ajustados
```

### 4. Badge con Overflow (100+)
**Condiciones:**
- UnreadCount > 99
- HasUnreadNotifications = true

**Apariencia:**
```
│  🔔  Notificaciones            ⬤ 99+  │
                                   └─ Texto "99+"
```

---

## Transiciones de Estado

### Escenario 1: Nueva Notificación Llega
```
Antes:
│  🔔  Notificaciones              ⬤ 5   │

[Nueva notificación recibida vía SignalR]

Después (inmediato):
│  🔔  Notificaciones              ⬤ 6   │
```

### Escenario 2: Usuario Marca Como Leída
```
Antes:
│  🔔  Notificaciones              ⬤ 3   │

[Usuario marca 1 notificación como leída]

Después (inmediato):
│  🔔  Notificaciones              ⬤ 2   │
```

### Escenario 3: Usuario Marca Todas Como Leídas
```
Antes:
│  🔔  Notificaciones              ⬤ 5   │

[Usuario pulsa "Marcar todas como leídas"]

Después (inmediato):
│  🔔  Notificaciones                     │
                                   └─ Badge desaparece
```

### Escenario 4: Actualización Periódica (Timer)
```
Tiempo: 10:00 AM
│  🔔  Notificaciones              ⬤ 3   │

[5 minutos después - Timer actualiza]

Tiempo: 10:05 AM
│  🔔  Notificaciones              ⬤ 5   │
                                   └─ Nuevas notificaciones del servidor
```

---

## Layout del Menu Item

```
┌──────────────────────────────────────────────────────────┐
│  Grid (Padding: 16,12 | Columns: Auto,*,Auto)           │
│                                                           │
│  ┌────────┐  ┌──────────────────────┐  ┌──────────┐    │
│  │ Column │  │      Column 1        │  │ Column 2 │    │
│  │   0    │  │                      │  │          │    │
│  │        │  │                      │  │          │    │
│  │  🔔    │  │   Notificaciones     │  │   ⬤ 3   │    │
│  │  Icon  │  │      Title           │  │  Badge   │    │
│  │        │  │                      │  │          │    │
│  │ 24x24  │  │   (Fill space)       │  │  24x24   │    │
│  └────────┘  └──────────────────────┘  └──────────┘    │
│                                                           │
│  ColumnSpacing: 16px                                     │
└──────────────────────────────────────────────────────────┘
```

---

## Colores del Badge

### Color Primario
```
Hex: #DC2626
RGB: rgb(220, 38, 38)
Nombre: Red-600 (Tailwind)
```

### Color de Texto
```
Hex: #FFFFFF
RGB: rgb(255, 255, 255)
Nombre: White
```

### Color de Sombra
```
Hex: #DC2626 (mismo que fondo)
Opacity: 0.5
Blur: 4px
Offset: 0, 2
```

---

## Comparación con Otros Items del Menú

### Item Normal (Sin Badge)
```
┌──────────────────────────────────────────┐
│  🏠  Panel Principal                     │
│  ↑    ↑                                  │
│ Icon  Title                              │
└──────────────────────────────────────────┘
```

### Item con Badge (Notificaciones)
```
┌──────────────────────────────────────────┐
│  🔔  Notificaciones              ⬤ 5    │
│  ↑    ↑                           ↑      │
│ Icon  Title                      Badge   │
└──────────────────────────────────────────┘
```

---

## Responsividad

### En Pantallas Pequeñas
- Badge mantiene tamaño fijo (24x24)
- Texto se ajusta automáticamente
- Espaciado se mantiene

### En Pantallas Grandes
- Badge mantiene tamaño fijo (24x24)
- Más espacio entre elementos
- Todo proporcionalmente distribuido

---

## Accessibility (Accesibilidad)

### Consideraciones:
1. **Contraste:** Rojo (#DC2626) sobre blanco tiene ratio > 4.5:1 ✅
2. **Tamaño:** 24x24px es suficientemente grande para tocar ✅
3. **Semántica:** Badge es decorativo, información principal en título ✅

### Mejoras Futuras:
- [ ] Agregar AutomationProperties.Name para lectores de pantalla
- [ ] Agregar descripción verbal del contador
- [ ] Soporte para alto contraste

---

## Ejemplos de Código XAML del Badge

### Badge Completo (Como está implementado)
```xml
<Border Grid.Column="2"
        Background="#DC2626"
        WidthRequest="24"
        HeightRequest="24"
        StrokeThickness="0"
        HorizontalOptions="End"
        VerticalOptions="Center">
    <Border.IsVisible>
        <MultiBinding Converter="{StaticResource NotificationBadgeVisibilityConverter}">
            <Binding Path="Title" />
            <Binding Source="{RelativeSource AncestorType={x:Type Shell}}"
                     Path="BindingContext.HasUnreadNotifications" />
        </MultiBinding>
    </Border.IsVisible>
    <Border.StrokeShape>
        <RoundRectangle CornerRadius="12"/>
    </Border.StrokeShape>
    <Border.Shadow>
        <Shadow Brush="#DC2626" Opacity="0.5" Radius="4" Offset="0,2" />
    </Border.Shadow>
    <Label Text="{Binding Source={RelativeSource AncestorType={x:Type Shell}},
                          Path=BindingContext.NotificationBadgeText}"
           FontSize="10"
           FontAttributes="Bold"
           TextColor="White"
           HorizontalOptions="Center"
           VerticalOptions="Center"
           Margin="0"/>
</Border>
```

---

## Variaciones de Diseño (Personalizaciones Posibles)

### Variación 1: Badge Cuadrado con Esquinas Redondeadas
```xml
<Border.StrokeShape>
    <RoundRectangle CornerRadius="6"/> <!-- Cambiar de 12 a 6 -->
</Border.StrokeShape>
```

### Variación 2: Badge Más Grande
```xml
<Border WidthRequest="32"   <!-- De 24 a 32 -->
        HeightRequest="32">
    <Label FontSize="12"/>  <!-- De 10 a 12 -->
</Border>
```

### Variación 3: Badge Azul en lugar de Rojo
```xml
<Border Background="#3B82F6"> <!-- Azul -->
    <Border.Shadow>
        <Shadow Brush="#3B82F6" /> <!-- Sombra azul -->
    </Border.Shadow>
</Border>
```

### Variación 4: Sin Sombra (Flat Design)
```xml
<!-- Simplemente remover Border.Shadow -->
```

---

## Testing Visual

### Checklist de Verificación Visual:
- [ ] Badge aparece solo en item "Notificaciones"
- [ ] Badge es circular (no ovalado)
- [ ] Color es rojo (#DC2626)
- [ ] Texto es blanco y centrado
- [ ] Tamaño es consistente (24x24)
- [ ] Sombra es visible pero sutil
- [ ] Badge desaparece cuando count = 0
- [ ] Número se actualiza en tiempo real
- [ ] "99+" aparece para números > 99
- [ ] Badge no afecta layout de otros items

### Screenshots Recomendados:
1. Menu con badge mostrando "1"
2. Menu con badge mostrando "42"
3. Menu con badge mostrando "99+"
4. Menu sin badge (count = 0)
5. Comparación lado a lado: antes/después de marcar como leída

---

## Consistencia con Admin Portal Web

El badge de la app MAUI está diseñado para coincidir visualmente con el badge del portal web admin:

### Similitudes:
✅ Color rojo (#DC2626)
✅ Forma circular
✅ Texto blanco centrado
✅ Formato "99+" para números grandes
✅ Desaparece cuando count = 0
✅ Se actualiza en tiempo real

### Diferencias (por plataforma):
- Web: Tamaño puede escalar con viewport
- MAUI: Tamaño fijo (24x24) para touch targets
- Web: Puede usar hover effects
- MAUI: Optimizado para touch (sin hover)

---

## Conclusión

El badge implementado tiene:
- ✅ Diseño profesional y moderno
- ✅ Alta visibilidad sin ser intrusivo
- ✅ Consistencia visual con el portal web
- ✅ Fácil de personalizar
- ✅ Accesible y responsive
- ✅ Actualizaciones en tiempo real

**El badge está listo para producción y proporciona una excelente experiencia de usuario.**

---

**Nota:** Para ver el badge en acción, ejecuta la aplicación y navega al menú lateral. El badge aparecerá automáticamente si hay notificaciones no leídas.
