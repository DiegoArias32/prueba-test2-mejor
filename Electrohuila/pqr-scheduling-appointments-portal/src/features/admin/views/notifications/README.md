# Notification Templates Feature

## Descripción

Feature completo para la gestión de plantillas de notificación (Email, SMS, Push) con editor avanzado, vista previa en tiempo real y validación de placeholders.

## Ubicación en el Proyecto

```
src/features/admin/views/notifications/
├── NotificationTemplatesView.tsx    # Vista principal
├── index.ts                         # Exports
└── README.md                        # Este archivo
```

## Componentes Relacionados

```
src/features/admin/components/
├── PlaceholderPicker.tsx           # Selector de placeholders
├── TemplatePreview.tsx             # Vista previa
└── NotificationTemplateModal.tsx   # Modal CRUD
```

## Servicios

```
src/services/notifications/
├── notification-template.service.ts   # API service
└── notification-template.types.ts     # Types & constants
```

## Integración

Esta vista se integra automáticamente en el AdminLayout:

```typescript
import { NotificationTemplatesView } from './notifications/NotificationTemplatesView';

// En AdminLayout.tsx
{activeSection === 'plantillas' && (
  <NotificationTemplatesView
    hasPermission={hasPermissionWrapper}
    onExportSuccess={(msg) => toastSuccess('Exportado', msg)}
    onExportWarning={(msg) => toastWarning('Sin Datos', msg)}
  />
)}
```

## Props

### NotificationTemplatesView

```typescript
interface NotificationTemplatesViewProps {
  hasPermission: (formCode: string, action: string) => boolean;
  onExportSuccess?: (message: string) => void;
  onExportWarning?: (message: string) => void;
}
```

## Permisos Requeridos

- `notification-templates:read` - Ver plantillas
- `notification-templates:create` - Crear nuevas plantillas
- `notification-templates:update` - Editar/Activar/Desactivar
- `notification-templates:delete` - Eliminar plantillas

## Características

### ✅ CRUD Completo
- Crear plantillas
- Editar plantillas
- Eliminar (lógico)
- Activar/Desactivar
- Vista previa

### ✅ Filtros y Búsqueda
- Búsqueda por nombre o código
- Filtro por tipo (Email, SMS, Push)
- Ordenamiento por columnas

### ✅ Validaciones
- Código único y formato válido
- Subject obligatorio para Emails
- Límite de 5000 caracteres
- Validación de placeholders

### ✅ Editor Avanzado
- Textarea con contador de caracteres
- Inserción rápida de placeholders
- Vista previa en tiempo real
- Highlighting de placeholders
- Advertencias de placeholders inválidos

### ✅ Vista Previa
- Email: Simulación de cliente de correo
- SMS: Simulación de teléfono móvil
- Push: Simulación de notificación push

### ✅ Exportación
- Exportar plantillas a JSON
- Incluye todas las plantillas filtradas

## Placeholders Disponibles

### Cliente
- `{{CLIENT_NAME}}` - Nombre del cliente

### Cita
- `{{APPOINTMENT_TYPE}}` - Tipo de cita
- `{{APPOINTMENT_DATE}}` - Fecha
- `{{APPOINTMENT_TIME}}` - Hora
- `{{APPOINTMENT_NUMBER}}` - Número de cita
- `{{CANCELLATION_REASON}}` - Razón de cancelación

### Sucursal
- `{{BRANCH_NAME}}` - Nombre de sucursal
- `{{BRANCH_ADDRESS}}` - Dirección
- `{{BRANCH_PHONE}}` - Teléfono

## Uso Básico

### Crear una plantilla

1. Click en "Nueva Plantilla"
2. Seleccionar tipo (Email, SMS, Push)
3. Completar formulario
4. Insertar placeholders desde el picker
5. Verificar vista previa
6. Guardar

### Editar una plantilla

1. Click en icono de editar (lápiz)
2. Modificar campos necesarios
3. Verificar vista previa
4. Guardar cambios

### Vista previa

1. Click en icono de ojo
2. Ver simulación realista del mensaje
3. Cerrar modal

## Ejemplos de Templates

Ver archivo: `TEMPLATE_EXAMPLES.json` en la raíz del proyecto.

## API Endpoints

Base URL: `https://8i6rrjp9sb.us-east-2.awsapprunner.com/api/v1`

- `GET /notificationtemplates` - Listar todas
- `GET /notificationtemplates/{id}` - Por ID
- `GET /notificationtemplates/by-code/{code}` - Por código
- `POST /notificationtemplates` - Crear
- `PUT /notificationtemplates/{id}` - Actualizar
- `DELETE /notificationtemplates/{id}` - Eliminar

## Estados de Plantilla

- **Activa** (Verde) - Plantilla habilitada y lista para usar
- **Inactiva** (Gris) - Plantilla deshabilitada temporalmente

## Flujo de Trabajo Recomendado

1. **Crear plantillas base** para cada tipo de notificación
2. **Probar** con vista previa
3. **Activar** solo las plantillas listas
4. **Mantener inactivas** las plantillas en desarrollo
5. **Exportar** periódicamente como backup

## Troubleshooting

### Problema: Placeholder no reconocido
**Solución:** Verificar que esté en `AVAILABLE_PLACEHOLDERS`

### Problema: No se puede guardar
**Solución:** Verificar validaciones (subject para Email, límite de caracteres)

### Problema: Vista previa no se actualiza
**Solución:** Toggle entre Editor y Vista Previa

### Problema: SMS muestra múltiples segmentos
**Solución:** Reducir contenido a ≤160 caracteres

## Mejores Prácticas

### Naming
- Usa prefijos: `APPT_`, `USER_`, `NOTIF_`
- Solo mayúsculas y guiones bajos
- Descriptivo y único

### Content
- SMS: Máximo 160 caracteres
- Emails: Claro y conciso
- Usa placeholders para personalización

### Organization
- Mantén solo plantillas activas en uso
- Documenta cambios importantes
- Exporta regularmente

## Testing

Antes de usar en producción:

- [ ] Compilación exitosa
- [ ] Permisos configurados
- [ ] Vista previa funciona
- [ ] Placeholders se reemplazan
- [ ] Validaciones funcionan
- [ ] Exportación funciona

## Documentación Adicional

- **Guía Completa:** `/NOTIFICATION_TEMPLATES_DOCUMENTATION.md`
- **Ejemplos de Uso:** `/USAGE_EXAMPLES.md`
- **Templates Ejemplo:** `/TEMPLATE_EXAMPLES.json`
- **Resumen Implementación:** `/IMPLEMENTATION_SUMMARY.md`

## Soporte

Para agregar nuevos placeholders:
1. Actualizar `AVAILABLE_PLACEHOLDERS`
2. Agregar a `PLACEHOLDER_INFO`
3. Actualizar `PREVIEW_DATA`
4. Documentar

---

**Versión:** 1.0.0
**Última actualización:** 2024-03-15
**Mantenido por:** Equipo de Desarrollo
