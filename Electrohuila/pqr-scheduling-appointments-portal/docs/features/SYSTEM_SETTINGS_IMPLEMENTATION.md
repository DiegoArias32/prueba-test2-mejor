# Panel de Configuración del Sistema - Implementación Completa

## Resumen

Se ha implementado un panel completo de administración para gestionar la Configuración del Sistema en el portal frontend.

## Archivos Creados

### 1. Servicios (Services Layer)

#### `src/services/system/system-setting.types.ts`
- Definiciones de tipos TypeScript para System Settings
- Interfaces: `SystemSettingDto`, `CreateSystemSettingDto`, `UpdateSystemSettingDto`, `UpdateSystemSettingValueDto`
- Tipos auxiliares: `SettingCategory`, `ParsedSettingValue`, `ValidationResult`

#### `src/services/system/system-setting.service.ts`
- Servicio HTTP que extiende `BaseHttpService`
- Métodos principales:
  - `getSystemSettings()`: Obtener todas las configuraciones
  - `getSystemSettingByKey(key)`: Obtener por clave
  - `createSystemSetting(dto)`: Crear nueva configuración
  - `updateSystemSetting(dto)`: Actualizar configuración completa
  - `updateSystemSettingValue(key, value)`: Actualizar solo valor (PATCH)
  - `parseSettingValue(setting)`: Parser de valores según tipo
  - `stringifySettingValue(value, type)`: Convertir valor a string

### 2. Utilidades (Utils)

#### `src/features/admin/utils/settingValidators.ts`
- Validadores específicos por tipo:
  - `validateNumber(value, min?, max?)`: Validación de números con rangos
  - `validateBoolean(value)`: Validación de valores booleanos
  - `validateTime(value)`: Validación de formato HH:mm
  - `validateJson(value)`: Validación de JSON
  - `validateString(value, minLength?, maxLength?)`: Validación de texto
  - `validateSettingKey(key)`: Validación de claves (UPPERCASE_SNAKE_CASE)
  - `getSettingConstraints(key)`: Obtener restricciones específicas
  - `validateSettingValue(key, value, type)`: Validador principal

Restricciones predefinidas:
- `MAX_APPOINTMENTS_PER_DAY`: 1-500
- `APPOINTMENT_CANCELLATION_HOURS`: 0-72
- `APPOINTMENT_REMINDER_HOURS`: 0-168

### 3. Componentes (Components)

#### `src/features/admin/components/SettingCard.tsx`
Tarjeta reutilizable para configuraciones individuales.

**Características:**
- Auto-save con debounce de 1 segundo (configurable)
- Estados: `unchanged`, `modified`, `saving`, `saved`, `error`
- Validación en tiempo real
- Inputs especializados según tipo:
  - **Boolean**: Toggle switch animado
  - **Number**: Input numérico con validación
  - **Time**: Time picker (HH:mm)
  - **Json**: Textarea con fuente monospace
  - **String**: Input de texto
- Indicadores visuales de estado
- Timestamp de última actualización

**Props:**
```typescript
interface SettingCardProps {
  setting: SystemSettingDto;
  onSave: (key: string, value: string) => Promise<void>;
  autoSave?: boolean;
  autoSaveDelay?: number;
}
```

#### `src/features/admin/views/components/modals/SystemSettingModal.tsx`
Modal para crear nuevas configuraciones (admin avanzado).

**Características:**
- Validación de formulario completa
- Campos:
  - Clave (KEY_FORMAT validado)
  - Tipo (String, Number, Boolean, Time, Json)
  - Valor (input adaptativo según tipo)
  - Descripción (textarea)
  - Encriptado (checkbox para datos sensibles)
- Preview de valor según tipo seleccionado
- Manejo de errores inline

### 4. Vistas (Views)

#### `src/features/admin/views/settings/SystemSettingsView.tsx`
Vista principal del panel de configuraciones.

**Características:**

**Organización por categorías:**
```typescript
const SETTING_CATEGORIES = [
  {
    id: 'appointments',
    label: 'Citas',
    icon: 'calendar',
    settings: [
      'MAX_APPOINTMENTS_PER_DAY',
      'APPOINTMENT_CANCELLATION_HOURS'
    ]
  },
  {
    id: 'notifications',
    label: 'Notificaciones',
    icon: 'bell',
    settings: [
      'EMAIL_NOTIFICATIONS_ENABLED',
      'SMS_NOTIFICATIONS_ENABLED',
      'APPOINTMENT_REMINDER_HOURS'
    ]
  },
  {
    id: 'businessHours',
    label: 'Horarios de Atención',
    icon: 'clock',
    settings: [
      'BUSINESS_HOURS_START',
      'BUSINESS_HOURS_END'
    ]
  },
  {
    id: 'general',
    label: 'General',
    icon: 'settings',
    settings: [] // Configuraciones no categorizadas
  }
];
```

**Funcionalidades:**
- Grid responsive (1 col móvil, 2 cols tablet, 3 cols desktop)
- Búsqueda en tiempo real (por clave y descripción)
- Botón "Recargar" para refrescar desde servidor
- Botón "Nueva Configuración" (abre modal)
- Timestamp de última actualización
- Estados de carga y error con UI feedback
- Empty states informativos

### 5. Integración con Admin Panel

#### Actualizaciones en `src/features/admin/views/AdminLayout.tsx`
- Importación de `SystemSettingsView`
- Caso `'settings'` en `getPageTitle()`: "Configuración del Sistema"
- Renderizado condicional cuando `activeSection === 'settings'`
- Actualización de tabNames en `getTabsForSidebar()`

#### Actualizaciones en `src/features/admin/views/components/Sidebar.tsx`
- Importación de icono `FiSliders`
- Nuevo item de menú:
  ```typescript
  {
    id: 'settings',
    name: 'Configuración',
    icon: FiSliders,
    color: 'text-cyan-500',
    bgColor: 'bg-cyan-50',
    hoverColor: 'hover:bg-cyan-100'
  }
  ```
- Actualización del filtro `visibleItems` para incluir `'settings'` como tab siempre visible (junto con dashboard y my-appointments)

#### Actualizaciones en `src/features/admin/models/admin.models.ts`
- `TabType` extendido: `'settings' | 'plantillas'` agregados

### 6. Índice de Servicios

#### Actualizaciones en `src/services/index.ts`
- Importación de `SystemSettingService`
- Instancia exportada: `systemSettingService`
- Métodos agregados al `apiService` unificado:
  ```typescript
  getSystemSettings
  getSystemSettingByKey
  createSystemSetting
  updateSystemSetting
  updateSystemSettingValue
  getActiveSystemSettings
  getSystemSettingsByType
  parseSettingValue
  stringifySettingValue
  ```
- Export de tipos: `export * from './system/system-setting.types'`

## Endpoints de la API Utilizados

```typescript
GET    /api/v1/systemsettings          // Listar todas
GET    /api/v1/systemsettings/{key}    // Obtener por clave
POST   /api/v1/systemsettings          // Crear nueva
PUT    /api/v1/systemsettings/{id}     // Actualizar completa
PATCH  /api/v1/systemsettings/value    // Actualizar solo valor
```

## Configuraciones Pre-cargadas en la API (7)

1. **MAX_APPOINTMENTS_PER_DAY** (Number): 50
2. **APPOINTMENT_CANCELLATION_HOURS** (Number): 24
3. **EMAIL_NOTIFICATIONS_ENABLED** (Boolean): true
4. **SMS_NOTIFICATIONS_ENABLED** (Boolean): false
5. **APPOINTMENT_REMINDER_HOURS** (Number): 24
6. **BUSINESS_HOURS_START** (Time): 08:00
7. **BUSINESS_HOURS_END** (Time): 17:00

## Estructura de Archivos Creados

```
pqr-scheduling-appointments-portal/
├── src/
│   ├── services/
│   │   ├── system/
│   │   │   ├── system-setting.service.ts       ✅ NUEVO
│   │   │   └── system-setting.types.ts         ✅ NUEVO
│   │   └── index.ts                             ⚡ ACTUALIZADO
│   │
│   └── features/
│       └── admin/
│           ├── components/
│           │   └── SettingCard.tsx              ✅ NUEVO
│           │
│           ├── models/
│           │   └── admin.models.ts              ⚡ ACTUALIZADO
│           │
│           ├── utils/
│           │   └── settingValidators.ts         ✅ NUEVO
│           │
│           └── views/
│               ├── settings/
│               │   └── SystemSettingsView.tsx   ✅ NUEVO
│               │
│               ├── components/
│               │   ├── modals/
│               │   │   └── SystemSettingModal.tsx ✅ NUEVO
│               │   └── Sidebar.tsx              ⚡ ACTUALIZADO
│               │
│               └── AdminLayout.tsx              ⚡ ACTUALIZADO
```

## Características Principales

### 1. **Auto-Save Inteligente**
- Debounce de 1 segundo
- Indicador visual de estado (saving/saved/error)
- Rollback automático en caso de error
- Prevención de guardado si valor no cambió

### 2. **Validación Robusta**
- Validación en tiempo real
- Mensajes de error específicos
- Restricciones por tipo de dato
- Restricciones personalizadas por clave

### 3. **UX Optimizada**
- Inputs especializados por tipo
- Toggle switch animado para booleanos
- Time picker para horas
- Editor JSON con syntax highlighting
- Feedback visual inmediato
- Loading states y empty states

### 4. **Organización por Categorías**
- Cards agrupados por contexto funcional
- Iconos distintivos por categoría
- Grid responsive
- Búsqueda global

### 5. **Seguridad**
- Solo para usuarios admin
- Validación cliente y servidor
- Opción de encriptación para datos sensibles
- Timestamps de auditoría

## Flujo de Uso

### Usuario Final (Admin)
1. Acceder al panel admin → Click en "Configuración" (icono Sliders)
2. Ver configuraciones organizadas por categorías
3. Buscar configuración específica (opcional)
4. Modificar valor directamente en la tarjeta
5. Auto-save tras 1 segundo de inactividad
6. Ver confirmación visual (checkmark verde)

### Admin Avanzado
1. Click en "Nueva Configuración"
2. Completar formulario modal:
   - Clave (UPPERCASE_SNAKE_CASE)
   - Tipo (String/Number/Boolean/Time/Json)
   - Valor inicial
   - Descripción
   - Marcar si es encriptado
3. Crear → Aparece en categoría "General"
4. Editar inline como configuración normal

## Tecnologías Utilizadas

- **Framework**: Next.js 15 + React 19
- **TypeScript**: Strict mode
- **Arquitectura**: MVVM por features
- **HTTP Client**: BaseHttpService (fetch API)
- **Validación**: Custom validators con TypeScript
- **Iconos**: react-icons (Feather Icons)
- **Estilos**: Tailwind CSS

## Próximas Mejoras (Opcionales)

1. **Historial de Cambios**: Tracking de quién y cuándo modificó cada valor
2. **Confirmación para Cambios Críticos**: Modal de confirmación para valores sensibles
3. **Export/Import**: Exportar configuraciones como JSON
4. **Restaurar Valores por Defecto**: Reset individual por configuración
5. **Validación de Dependencias**: Validar relaciones entre configuraciones
6. **Permisos Granulares**: Restringir acceso a configuraciones específicas
7. **Preview de Impacto**: Mostrar qué funcionalidades afecta cada configuración

## Testing

### Comandos para Probar

```bash
# Desarrollo
npm run dev

# Build de producción
npm run build

# Linter
npm run lint
```

### Casos de Prueba Recomendados

1. ✅ Cargar configuraciones desde API
2. ✅ Modificar valor numérico (validar rangos)
3. ✅ Toggle boolean (switch)
4. ✅ Modificar hora (time picker)
5. ✅ Buscar configuraciones
6. ✅ Crear nueva configuración
7. ✅ Validar formato de clave
8. ✅ Probar auto-save (esperar 1 seg)
9. ✅ Simular error de API
10. ✅ Verificar responsive design

## Contacto y Soporte

Para preguntas o reportar problemas con esta implementación, contactar al equipo de desarrollo.

---

**Versión**: 1.0.0
**Fecha**: 2025-11-16
**Autor**: Claude AI Assistant
**Estado**: ✅ Implementación Completa
