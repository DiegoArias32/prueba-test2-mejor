# Panel de Administración de Plantillas de Notificación

## Descripción General

Sistema completo para gestionar plantillas de notificación (Email, SMS, Push) con editor de templates, vista previa en tiempo real, y validación de placeholders.

## Arquitectura

### Estructura de Archivos

```
src/
├── services/
│   └── notifications/
│       ├── notification-template.service.ts    # Servicio API
│       └── notification-template.types.ts      # Tipos y constantes
│
├── features/admin/
│   ├── components/
│   │   ├── PlaceholderPicker.tsx              # Selector de placeholders
│   │   ├── TemplatePreview.tsx                # Vista previa de templates
│   │   └── NotificationTemplateModal.tsx      # Modal CRUD
│   │
│   └── views/
│       └── notifications/
│           ├── NotificationTemplatesView.tsx  # Vista principal
│           └── index.ts
│
└── features/admin/domain/entities/
    └── AdminTypes.ts                          # Tipos actualizados
```

## Características Principales

### 1. Gestión de Plantillas

- **Crear/Editar/Eliminar** plantillas de notificación
- **Activar/Desactivar** plantillas
- **Filtrar** por tipo (Email, SMS, Push)
- **Buscar** por nombre o código
- **Exportar** plantillas en formato JSON

### 2. Editor de Templates

#### Características:
- Editor de texto con sintaxis highlighting para placeholders
- Contador de caracteres en tiempo real
- Validación de placeholders
- Inserción rápida de placeholders con un clic
- Vista previa en vivo

#### Placeholders Disponibles:

##### Cliente
- `{{CLIENT_NAME}}` - Nombre completo del cliente
  - Ejemplo: "Juan Pérez García"

##### Cita
- `{{APPOINTMENT_TYPE}}` - Tipo de cita
  - Ejemplo: "Consulta General"
- `{{APPOINTMENT_DATE}}` - Fecha de la cita
  - Ejemplo: "15/03/2024"
- `{{APPOINTMENT_TIME}}` - Hora de la cita
  - Ejemplo: "10:30 AM"
- `{{APPOINTMENT_NUMBER}}` - Número de cita
  - Ejemplo: "APT-2024-001234"
- `{{CANCELLATION_REASON}}` - Razón de cancelación
  - Ejemplo: "Conflicto de horario"

##### Sucursal
- `{{BRANCH_NAME}}` - Nombre de la sucursal
  - Ejemplo: "Sede Principal"
- `{{BRANCH_ADDRESS}}` - Dirección de la sucursal
  - Ejemplo: "Calle 123 #45-67, Bogotá"
- `{{BRANCH_PHONE}}` - Teléfono de la sucursal
  - Ejemplo: "+57 300 123 4567"

### 3. Vista Previa en Tiempo Real

#### Email Preview
- Simulación de cliente de correo electrónico
- Muestra: De, Para, Asunto, Cuerpo
- Footer automático

#### SMS Preview
- Simulación de pantalla de teléfono móvil
- Burbuja de mensaje con diseño realista
- Contador de caracteres (160 por mensaje)
- Indicador de múltiples segmentos SMS

#### Push Notification Preview
- Simulación de notificación push en dispositivo móvil
- Diseño realista con icono, título y mensaje
- Timestamp automático

### 4. Validaciones

#### Código de Plantilla
- ✅ Requerido
- ✅ Solo letras mayúsculas, números y guiones bajos
- ✅ Único (readonly en edición)

#### Nombre de Plantilla
- ✅ Requerido
- ✅ Entre 3 y 100 caracteres

#### Asunto (solo Email)
- ✅ Requerido para tipo Email
- ✅ Acepta placeholders

#### Cuerpo del Mensaje
- ✅ Requerido
- ✅ Máximo 5000 caracteres
- ✅ Validación de placeholders inválidos
- ✅ Advertencias visuales para placeholders no reconocidos

## Endpoints de la API

### Base URL
```
https://8i6rrjp9sb.us-east-2.awsapprunner.com/api/v1
```

### Endpoints

#### Listar todas las plantillas
```http
GET /api/v1/notificationtemplates
```

**Response:**
```typescript
NotificationTemplateDto[]
```

#### Obtener plantilla por ID
```http
GET /api/v1/notificationtemplates/{id}
```

**Response:**
```typescript
NotificationTemplateDto
```

#### Obtener plantilla por código
```http
GET /api/v1/notificationtemplates/by-code/{code}
```

**Response:**
```typescript
NotificationTemplateDto
```

#### Crear plantilla
```http
POST /api/v1/notificationtemplates
```

**Request Body:**
```typescript
{
  templateCode: string;
  templateName: string;
  subject: string;
  bodyTemplate: string;
  templateType: 'Email' | 'SMS' | 'Push';
  placeholders?: string;
}
```

#### Actualizar plantilla
```http
PUT /api/v1/notificationtemplates/{id}
```

**Request Body:**
```typescript
{
  id: number;
  templateCode: string;
  templateName: string;
  subject: string;
  bodyTemplate: string;
  templateType: 'Email' | 'SMS' | 'Push';
  placeholders?: string;
}
```

#### Eliminar plantilla (lógico)
```http
DELETE /api/v1/notificationtemplates/{id}
```

## Tipos de Datos

### NotificationTemplateDto
```typescript
interface NotificationTemplateDto {
  id: number;
  templateCode: string;
  templateName: string;
  subject: string;
  bodyTemplate: string;
  templateType: 'Email' | 'SMS' | 'Push';
  placeholders: string;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}
```

## Plantillas Predefinidas

El sistema incluye códigos predefinidos para facilitar la creación:

1. **APPT_CONFIRMATION** - Confirmación de Cita
2. **APPT_REMINDER** - Recordatorio de Cita (24h)
3. **APPT_REMINDER_SMS** - Recordatorio SMS
4. **APPT_CANCELLATION** - Cancelación de Cita
5. **APPT_RESCHEDULED** - Cita Reagendada
6. **APPT_COMPLETED** - Cita Completada
7. **APPT_NO_SHOW** - Inasistencia a Cita

## Ejemplo de Uso

### 1. Crear una Plantilla de Confirmación de Cita (Email)

```typescript
const template = {
  templateCode: 'APPT_CONFIRMATION',
  templateName: 'Confirmación de Cita',
  templateType: 'Email',
  subject: 'Confirmación de tu cita - {{APPOINTMENT_DATE}}',
  bodyTemplate: `
Hola {{CLIENT_NAME}},

Tu cita ha sido confirmada exitosamente.

Detalles de la cita:
- Tipo: {{APPOINTMENT_TYPE}}
- Fecha: {{APPOINTMENT_DATE}}
- Hora: {{APPOINTMENT_TIME}}
- Número de cita: {{APPOINTMENT_NUMBER}}

Ubicación:
{{BRANCH_NAME}}
{{BRANCH_ADDRESS}}
Teléfono: {{BRANCH_PHONE}}

Si necesitas cancelar o reagendar, por favor contáctanos con anticipación.

¡Te esperamos!
  `
};
```

### 2. Crear una Plantilla de Recordatorio (SMS)

```typescript
const template = {
  templateCode: 'APPT_REMINDER_SMS',
  templateName: 'Recordatorio SMS',
  templateType: 'SMS',
  subject: '',
  bodyTemplate: `Recordatorio: Tienes cita mañana {{APPOINTMENT_DATE}} a las {{APPOINTMENT_TIME}} en {{BRANCH_NAME}}. Cita #{{APPOINTMENT_NUMBER}}`
};
```

### 3. Crear una Plantilla de Cancelación (Push)

```typescript
const template = {
  templateCode: 'APPT_CANCELLATION',
  templateName: 'Cancelación de Cita',
  templateType: 'Push',
  subject: 'Cita Cancelada',
  bodyTemplate: `Tu cita del {{APPOINTMENT_DATE}} a las {{APPOINTMENT_TIME}} ha sido cancelada. Razón: {{CANCELLATION_REASON}}`
};
```

## Permisos

El sistema utiliza el sistema de permisos del panel admin:

- **notification-templates:read** - Ver plantillas
- **notification-templates:create** - Crear plantillas
- **notification-templates:update** - Editar/Activar/Desactivar plantillas
- **notification-templates:delete** - Eliminar plantillas

## Integración en AdminLayout

La vista se integra automáticamente en el panel de administración:

1. **Tab**: "Plantillas" aparece en el sidebar
2. **Ruta**: `/admin#plantillas`
3. **Título**: "Plantillas de Notificación"
4. **Permisos**: Gestionados por el sistema de permisos existente

## Características Avanzadas

### 1. Validación de Placeholders
```typescript
// El servicio valida automáticamente los placeholders
const invalidPlaceholders = templateService.validatePlaceholders(
  template.bodyTemplate,
  AVAILABLE_PLACEHOLDERS
);

// Retorna array de placeholders inválidos
// Ejemplo: ['{{INVALID_PLACEHOLDER}}', '{{UNKNOWN}}']
```

### 2. Reemplazo de Placeholders
```typescript
// Reemplaza placeholders con valores reales
const message = templateService.replacePlaceholders(
  template.bodyTemplate,
  {
    CLIENT_NAME: 'Juan Pérez',
    APPOINTMENT_DATE: '15/03/2024',
    APPOINTMENT_TIME: '10:30 AM',
    // ... más valores
  }
);
```

### 3. Extracción de Placeholders
```typescript
// Extrae todos los placeholders únicos del template
const placeholders = templateService.extractPlaceholders(
  template.bodyTemplate
);
// Retorna: ['{{CLIENT_NAME}}', '{{APPOINTMENT_DATE}}', ...]
```

## Estadísticas del Panel

El dashboard muestra:

- **Total** de plantillas
- **Activas** - Plantillas habilitadas
- **Email** - Cantidad de plantillas de email
- **SMS** - Cantidad de plantillas SMS
- **Push** - Cantidad de plantillas push

## Exportación de Datos

Las plantillas se pueden exportar en formato JSON:

```json
[
  {
    "id": 1,
    "templateCode": "APPT_CONFIRMATION",
    "templateName": "Confirmación de Cita",
    "subject": "Confirmación de tu cita - {{APPOINTMENT_DATE}}",
    "bodyTemplate": "Hola {{CLIENT_NAME}}...",
    "templateType": "Email",
    "placeholders": "[\"{{CLIENT_NAME}}\", ...]",
    "isActive": true,
    "createdAt": "2024-03-15T10:00:00Z",
    "updatedAt": "2024-03-15T10:00:00Z"
  }
]
```

## Troubleshooting

### Placeholder no reconocido
**Problema:** El sistema marca un placeholder como inválido.
**Solución:** Verifica que el placeholder esté en la lista de `AVAILABLE_PLACEHOLDERS` en `notification-template.types.ts`.

### Vista previa no se actualiza
**Problema:** Los cambios en el template no se reflejan en la vista previa.
**Solución:** Verifica que el estado showPreview esté activado. El componente usa `useMemo` para optimización.

### Error al guardar plantilla
**Problema:** Error 400 o 500 al guardar.
**Solución:**
1. Verifica que todos los campos requeridos estén completos
2. Para Email, el subject es obligatorio
3. Verifica que el templateCode sea único y válido

### SMS muestra múltiples segmentos
**Problema:** El SMS indica que usará múltiples mensajes.
**Solución:** Reduce el contenido a 160 caracteres o menos. Cada segmento adicional tiene costo.

## Mejores Prácticas

### 1. Naming de Códigos
- Usa prefijos descriptivos: `APPT_`, `USER_`, `NOTIF_`
- Solo mayúsculas y guiones bajos
- Descriptivo y único

### 2. Contenido de Templates
- Mantén los SMS concisos (≤160 caracteres)
- Usa placeholders para personalización
- Incluye toda la información relevante
- Mantén un tono profesional

### 3. Subjects de Email
- Claros y descriptivos
- Incluye placeholders relevantes
- No muy largos (≤50 caracteres ideal)

### 4. Organización
- Crea templates específicos para cada caso de uso
- Usa nombres descriptivos
- Mantén activas solo las plantillas en uso
- Documenta cambios importantes

## Soporte y Mantenimiento

Para agregar nuevos placeholders:

1. Actualiza `AVAILABLE_PLACEHOLDERS` en `notification-template.types.ts`
2. Agrega la información en `PLACEHOLDER_INFO`
3. Actualiza `PREVIEW_DATA` con datos de ejemplo
4. Documenta el nuevo placeholder

## Próximas Mejoras Sugeridas

- [ ] Historial de versiones de templates
- [ ] Duplicar templates
- [ ] Importar templates desde JSON
- [ ] Tests A/B de templates
- [ ] Estadísticas de uso por template
- [ ] Editor WYSIWYG para emails HTML
- [ ] Variables condicionales en templates
- [ ] Soporte multiidioma

---

**Última actualización:** 2024-03-15
**Versión:** 1.0.0
**Autor:** Sistema de Gestión de Citas
