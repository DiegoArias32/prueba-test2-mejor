# Quick Start Guide - Notification Templates

## 🚀 Inicio Rápido (5 minutos)

### 1. Acceder al Panel

```
1. Abrir http://localhost:3000/admin
2. Login con credenciales de administrador
3. Click en "Plantillas" en el sidebar
```

### 2. Crear Tu Primera Plantilla

**Ejemplo: Confirmación de Cita por Email**

1. **Click en "Nueva Plantilla"**

2. **Seleccionar plantilla predefinida:**
   - Dropdown: "APPT_CONFIRMATION - Confirmación de Cita"

3. **Configurar tipo:**
   - Seleccionar: 📧 Email

4. **Completar datos (ya pre-llenados):**
   ```
   Código: APPT_CONFIRMATION
   Nombre: Confirmación de Cita
   Tipo: Email
   Asunto: ✅ Confirmación de tu cita - {{APPOINTMENT_DATE}}
   ```

5. **Editar el cuerpo del mensaje:**
   ```
   Estimado/a {{CLIENT_NAME}},

   Tu cita ha sido confirmada exitosamente.

   📋 DETALLES:
   • Tipo: {{APPOINTMENT_TYPE}}
   • Fecha: {{APPOINTMENT_DATE}}
   • Hora: {{APPOINTMENT_TIME}}
   • Número: {{APPOINTMENT_NUMBER}}

   📍 UBICACIÓN:
   {{BRANCH_NAME}}
   {{BRANCH_ADDRESS}}
   📞 {{BRANCH_PHONE}}

   ¡Te esperamos!
   ```

6. **Insertar placeholders:**
   - Click en los botones de placeholders para insertarlos
   - O escribirlos manualmente: `{{PLACEHOLDER_NAME}}`

7. **Ver vista previa:**
   - Click en "Mostrar Vista Previa"
   - Verás cómo se ve el email real

8. **Guardar:**
   - Click en "Guardar Plantilla"
   - ✅ ¡Listo!

### 3. Crear Plantilla SMS

1. **Click en "Nueva Plantilla"**

2. **Seleccionar:**
   - Dropdown: "APPT_REMINDER_SMS - Recordatorio SMS"

3. **Tipo:** 💬 SMS

4. **Mensaje corto (≤160 caracteres):**
   ```
   🔔 Recordatorio: Cita mañana {{APPOINTMENT_DATE}} a las {{APPOINTMENT_TIME}} en {{BRANCH_NAME}}. #{{APPOINTMENT_NUMBER}}
   ```

5. **Verificar contador:** Debe mostrar menos de 160 caracteres

6. **Vista previa:** Ver simulación de teléfono

7. **Guardar**

### 4. Usar la Plantilla en tu Código

```typescript
import { notificationTemplateService } from '@/services';

// 1. Obtener plantilla
const template = await notificationTemplateService
  .getNotificationTemplateByCode('APPT_CONFIRMATION');

// 2. Reemplazar placeholders
const message = notificationTemplateService.replacePlaceholders(
  template.bodyTemplate,
  {
    CLIENT_NAME: 'Juan Pérez',
    APPOINTMENT_TYPE: 'Consulta General',
    APPOINTMENT_DATE: '20/03/2024',
    APPOINTMENT_TIME: '10:30 AM',
    APPOINTMENT_NUMBER: 'APT-001',
    BRANCH_NAME: 'Sede Principal',
    BRANCH_ADDRESS: 'Calle 123',
    BRANCH_PHONE: '+57 300 123 4567'
  }
);

// 3. Enviar
await sendEmail(clientEmail, message);
```

## 📝 Placeholders Más Usados

| Placeholder | Descripción | Ejemplo |
|-------------|-------------|---------|
| `{{CLIENT_NAME}}` | Nombre del cliente | "Juan Pérez" |
| `{{APPOINTMENT_DATE}}` | Fecha de cita | "20/03/2024" |
| `{{APPOINTMENT_TIME}}` | Hora de cita | "10:30 AM" |
| `{{APPOINTMENT_NUMBER}}` | Número de cita | "APT-001" |
| `{{BRANCH_NAME}}` | Nombre de sucursal | "Sede Principal" |

## 🎯 Tips Rápidos

### ✅ DO's
- ✅ Usa plantillas predefinidas
- ✅ Mantén SMS bajo 160 caracteres
- ✅ Siempre usa vista previa antes de guardar
- ✅ Activa solo plantillas listas para producción
- ✅ Exporta plantillas como backup

### ❌ DON'Ts
- ❌ No uses placeholders que no existen
- ❌ No hagas SMS muy largos (costos)
- ❌ No olvides el subject en Emails
- ❌ No uses caracteres especiales en códigos
- ❌ No elimines plantillas en uso

## 🔥 Casos de Uso Comunes

### 1. Recordatorio 24h antes

**Tipo:** Email
**Código:** `APPT_REMINDER`
**Cuándo:** Enviar automáticamente 24h antes de la cita

```
Asunto: 🔔 Recordatorio: Tu cita es mañana - {{APPOINTMENT_DATE}}

Hola {{CLIENT_NAME}},

Recordatorio de tu cita mañana:
• {{APPOINTMENT_TYPE}}
• {{APPOINTMENT_DATE}} a las {{APPOINTMENT_TIME}}
• {{BRANCH_NAME}}

¡Nos vemos mañana!
```

### 2. Confirmación Inmediata

**Tipo:** Email
**Código:** `APPT_CONFIRMATION`
**Cuándo:** Inmediatamente después de agendar

```
Asunto: ✅ Confirmación - Cita {{APPOINTMENT_NUMBER}}

Estimado/a {{CLIENT_NAME}},

Tu cita está confirmada:
[Detalles completos...]
```

### 3. Recordatorio SMS

**Tipo:** SMS
**Código:** `APPT_REMINDER_SMS`
**Cuándo:** 2 horas antes de la cita

```
⏰ Recordatorio: Tu cita es HOY a las {{APPOINTMENT_TIME}} en {{BRANCH_NAME}}. #{{APPOINTMENT_NUMBER}}
```

### 4. Push Notification

**Tipo:** Push
**Código:** `APPT_REMINDER_PUSH`
**Cuándo:** 1 hora antes de la cita

```
Título: Recordatorio de Cita
Mensaje: Tu cita de {{APPOINTMENT_TYPE}} es en 1 hora. ¡No llegues tarde!
```

## 🛠️ Troubleshooting Rápido

### Problema: "Placeholder inválido"
**Solución:** Verifica que el placeholder esté en la lista de disponibles
```
Correctos: {{CLIENT_NAME}}, {{APPOINTMENT_DATE}}
Incorrectos: {{NOMBRE_CLIENTE}}, {{FECHA}}
```

### Problema: "Subject requerido"
**Solución:** Si es Email, el subject es obligatorio
```
✅ Email: Siempre incluir subject
✅ SMS: Subject no se usa (dejar vacío)
✅ Push: Subject opcional
```

### Problema: "SMS muy largo"
**Solución:** Reducir a 160 caracteres o menos
```
Antes (180 chars): "Recordatorio: Su cita de Consulta General está programada para mañana..."
Después (145 chars): "🔔 Cita mañana {{APPOINTMENT_DATE}} {{APPOINTMENT_TIME}} en {{BRANCH_NAME}}. #{{APPOINTMENT_NUMBER}}"
```

### Problema: "No puedo guardar"
**Solución:** Verificar validaciones
- Código no vacío y sin espacios
- Nombre entre 3-100 caracteres
- Cuerpo no excede 5000 caracteres
- Placeholders válidos

## 📱 Atajos de Teclado

- `Ctrl + S` - Guardar plantilla (en modal)
- `Esc` - Cerrar modal
- `Ctrl + F` - Enfocar búsqueda
- `Tab` - Navegar entre campos

## 🎨 Emojis Útiles para Templates

```
✅ ❌ ⚠️ ℹ️ 🔔 ⏰ 📅 📞 📍 📧 💬 👋
🏢 🕐 📋 ✨ 💡 🔥 🎯 📝 🚀 ⭐ 🙌 👍
```

**Copiar y pegar directamente en tus templates!**

## 📦 Plantillas Pre-hechas

Importar desde `TEMPLATE_EXAMPLES.json`:

1. Abrir archivo JSON
2. Copiar contenido de cada plantilla
3. Crear nueva plantilla en el panel
4. Pegar contenido
5. Guardar

**12 plantillas listas para usar:**
- ✅ Confirmación Email
- ✅ Recordatorio Email (24h)
- ✅ Recordatorio SMS
- ✅ Recordatorio Push
- ✅ Cancelación
- ✅ Reagendamiento
- ✅ Cita Completada
- ✅ No Show
- ✅ Bienvenida
- ✅ Confirmación SMS
- ✅ Cancelación SMS
- ✅ Recordatorio Hoy

## 🎓 Próximos Pasos

1. **Crear plantillas básicas** (Confirmación, Recordatorio)
2. **Probar vista previa** con datos reales
3. **Integrar en tu código** de notificaciones
4. **Monitorear uso** y ajustar según feedback
5. **Crear plantillas especializadas** según necesidades

## 📚 Documentación Completa

- **Guía Detallada:** `NOTIFICATION_TEMPLATES_DOCUMENTATION.md`
- **Ejemplos de Código:** `USAGE_EXAMPLES.md`
- **Templates Ejemplo:** `TEMPLATE_EXAMPLES.json`
- **Checklist Deploy:** `DEPLOYMENT_CHECKLIST.md`

## 💬 Soporte

**¿Necesitas ayuda?**
1. Revisar esta guía
2. Consultar documentación completa
3. Verificar ejemplos de código
4. Contactar soporte técnico

---

**¡Empieza ahora mismo!** En 5 minutos tendrás tu primera plantilla funcionando.

**Versión:** 1.0.0
**Actualizado:** 2024-03-15
