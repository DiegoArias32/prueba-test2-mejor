# Deployment Checklist - Notification Templates Feature

## ✅ Pre-Deployment Checklist

### 1. Archivos Creados (10 archivos nuevos)

- [x] `src/services/notifications/notification-template.service.ts`
- [x] `src/services/notifications/notification-template.types.ts`
- [x] `src/features/admin/components/PlaceholderPicker.tsx`
- [x] `src/features/admin/components/TemplatePreview.tsx`
- [x] `src/features/admin/components/NotificationTemplateModal.tsx`
- [x] `src/features/admin/components/index.ts`
- [x] `src/features/admin/views/notifications/NotificationTemplatesView.tsx`
- [x] `src/features/admin/views/notifications/index.ts`
- [x] `src/features/admin/views/notifications/README.md`
- [x] `NOTIFICATION_TEMPLATES_DOCUMENTATION.md`
- [x] `USAGE_EXAMPLES.md`
- [x] `TEMPLATE_EXAMPLES.json`
- [x] `IMPLEMENTATION_SUMMARY.md`
- [x] `DEPLOYMENT_CHECKLIST.md` (este archivo)

### 2. Archivos Modificados (3 archivos)

- [x] `src/services/index.ts`
- [x] `src/features/admin/domain/entities/AdminTypes.ts`
- [x] `src/features/admin/views/AdminLayout.tsx`

## 🔧 Pasos de Deployment

### Paso 1: Instalación de Dependencias

```bash
cd C:\Users\User\Desktop\ad\pqr-scheduling-appointments-portal
npm install
```

**Verificar que no haya errores de dependencias.**

### Paso 2: Compilación

```bash
npm run build
```

**Verificar:**
- [ ] Compilación exitosa sin errores
- [ ] Sin warnings críticos de TypeScript
- [ ] Todos los imports se resuelven correctamente

### Paso 3: Verificación de TypeScript

```bash
npm run type-check
# o
npx tsc --noEmit
```

**Verificar:**
- [ ] No hay errores de tipos
- [ ] Todas las interfaces coinciden
- [ ] Imports correctos

### Paso 4: Configuración del Backend

#### 4.1 Crear Tabla en Base de Datos

Asegúrate de que exista la tabla `NotificationTemplates` con la estructura:

```sql
CREATE TABLE NotificationTemplates (
    Id INT PRIMARY KEY IDENTITY(1,1),
    TemplateCode NVARCHAR(100) NOT NULL UNIQUE,
    TemplateName NVARCHAR(100) NOT NULL,
    Subject NVARCHAR(200),
    BodyTemplate NVARCHAR(MAX) NOT NULL,
    TemplateType NVARCHAR(50) NOT NULL CHECK (TemplateType IN ('Email', 'SMS', 'Push')),
    Placeholders NVARCHAR(MAX),
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
```

#### 4.2 Verificar Endpoints API

**Base URL:** `https://8i6rrjp9sb.us-east-2.awsapprunner.com/api/v1`

Probar cada endpoint:

```bash
# Listar todas las plantillas
curl -X GET "https://8i6rrjp9sb.us-east-2.awsapprunner.com/api/v1/notificationtemplates" \
  -H "Authorization: Bearer YOUR_TOKEN"

# Crear plantilla de prueba
curl -X POST "https://8i6rrjp9sb.us-east-2.awsapprunner.com/api/v1/notificationtemplates" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "templateCode": "TEST_TEMPLATE",
    "templateName": "Test Template",
    "subject": "Test Subject",
    "bodyTemplate": "Hello {{CLIENT_NAME}}",
    "templateType": "Email"
  }'
```

**Verificar:**
- [ ] GET `/notificationtemplates` retorna array
- [ ] POST crea plantilla correctamente
- [ ] PUT actualiza plantilla
- [ ] DELETE elimina (lógico) plantilla
- [ ] Manejo correcto de errores (400, 404, 500)

#### 4.3 Configurar Permisos

Crear en la tabla de permisos:

```sql
-- Crear formulario
INSERT INTO Forms (FormCode, FormName, ModuleId)
VALUES ('notification-templates', 'Plantillas de Notificación', [MODULE_ID]);

-- Crear permisos
INSERT INTO Permissions (PermissionCode, PermissionName)
VALUES
  ('notification-templates:read', 'Ver Plantillas de Notificación'),
  ('notification-templates:create', 'Crear Plantillas de Notificación'),
  ('notification-templates:update', 'Actualizar Plantillas de Notificación'),
  ('notification-templates:delete', 'Eliminar Plantillas de Notificación');

-- Asignar permisos a roles (ejemplo para Admin)
INSERT INTO RolePermissions (RoleId, FormId, PermissionId)
SELECT
  r.Id,
  f.Id,
  p.Id
FROM Roles r
CROSS JOIN Forms f
CROSS JOIN Permissions p
WHERE
  r.Code = 'ADMIN'
  AND f.FormCode = 'notification-templates'
  AND p.PermissionCode LIKE 'notification-templates:%';
```

### Paso 5: Testing Frontend

#### 5.1 Acceso al Panel

1. **Abrir aplicación:**
   ```
   http://localhost:3000/admin
   ```

2. **Login con credenciales de admin**

3. **Verificar que aparezca tab "Plantillas"**

**Checklist:**
- [ ] Tab "Plantillas" visible en sidebar
- [ ] Click en tab navega a la vista
- [ ] Título correcto: "Plantillas de Notificación"
- [ ] Breadcrumb correcto

#### 5.2 Funcionalidad de Vista Principal

**Verificar:**
- [ ] Cards de estadísticas se muestran correctamente
- [ ] Tabla de plantillas carga
- [ ] Búsqueda funciona
- [ ] Filtro por tipo funciona
- [ ] Botón "Nueva Plantilla" visible (si tiene permisos)
- [ ] Botón "Exportar" funciona
- [ ] Estados activo/inactivo se muestran correctamente
- [ ] Badges de tipo tienen colores correctos

#### 5.3 Modal de Creación

**Abrir modal "Nueva Plantilla" y verificar:**

- [ ] Modal se abre correctamente
- [ ] Select de plantillas predefinidas funciona
- [ ] Selector de tipo (Email, SMS, Push) funciona
- [ ] Campo Subject aparece/desaparece según tipo
- [ ] Textarea de cuerpo funciona
- [ ] Contador de caracteres se actualiza
- [ ] PlaceholderPicker se muestra
- [ ] Click en placeholder inserta en textarea
- [ ] Cursor se posiciona correctamente después de insertar
- [ ] Toggle vista previa funciona
- [ ] Vista previa se actualiza en tiempo real
- [ ] Checkbox "Activa" funciona
- [ ] Botón "Guardar" funciona
- [ ] Modal se cierra después de guardar
- [ ] Toast de éxito aparece

#### 5.4 Validaciones

**Probar validaciones dejando campos vacíos:**

- [ ] Error si código está vacío
- [ ] Error si código tiene caracteres inválidos
- [ ] Error si nombre está vacío
- [ ] Error si subject vacío (solo Email)
- [ ] Error si cuerpo vacío
- [ ] Error si cuerpo excede 5000 caracteres
- [ ] Advertencia si hay placeholders inválidos
- [ ] Mensajes de error se muestran correctamente
- [ ] Errores desaparecen al corregir

#### 5.5 Vista Previa

**Para cada tipo de plantilla:**

**Email:**
- [ ] Muestra campos De, Para, Asunto
- [ ] Muestra cuerpo del mensaje
- [ ] Footer automático aparece
- [ ] Placeholders están resueltos
- [ ] Diseño realista

**SMS:**
- [ ] Muestra simulación de teléfono
- [ ] Burbuja de mensaje visible
- [ ] Contador de caracteres correcto
- [ ] Advertencia de múltiples segmentos si >160 chars
- [ ] Placeholders están resueltos

**Push:**
- [ ] Muestra simulación de notificación móvil
- [ ] Icono y título visibles
- [ ] Mensaje visible
- [ ] Timestamp aparece
- [ ] Placeholders están resueltos

#### 5.6 Edición

**Editar una plantilla existente:**

- [ ] Modal se abre con datos pre-cargados
- [ ] Código está readonly (no editable)
- [ ] Otros campos son editables
- [ ] Vista previa funciona
- [ ] Guardar actualiza correctamente
- [ ] Tabla se refresca después de guardar

#### 5.7 Otras Acciones

**Vista Previa Rápida:**
- [ ] Click en ícono de ojo abre modal
- [ ] Vista previa se muestra correctamente
- [ ] Modal se cierra

**Activar/Desactivar:**
- [ ] Click en toggle cambia estado
- [ ] Badge en tabla se actualiza
- [ ] Confirmación funciona

**Eliminar:**
- [ ] Click en ícono de trash
- [ ] Confirmación aparece
- [ ] Plantilla se elimina (soft delete)
- [ ] Tabla se actualiza

**Exportar:**
- [ ] Click en "Exportar"
- [ ] Archivo JSON se descarga
- [ ] JSON contiene plantillas correctas
- [ ] Toast de éxito aparece

### Paso 6: Testing de Integración

#### 6.1 Crear Plantillas de Ejemplo

**Crear estas plantillas:**

1. **APPT_CONFIRMATION** (Email)
2. **APPT_REMINDER_SMS** (SMS)
3. **APPT_REMINDER_PUSH** (Push)

**Usar plantillas predefinidas para facilitar.**

#### 6.2 Probar Flujo Completo

1. **Crear plantilla Email:**
   - Código: TEST_EMAIL
   - Tipo: Email
   - Subject con placeholders
   - Cuerpo con múltiples placeholders
   - Verificar vista previa
   - Guardar

2. **Crear plantilla SMS:**
   - Código: TEST_SMS
   - Tipo: SMS
   - Cuerpo ≤160 caracteres
   - Verificar contador
   - Guardar

3. **Crear plantilla Push:**
   - Código: TEST_PUSH
   - Tipo: Push
   - Subject corto
   - Cuerpo conciso
   - Guardar

4. **Verificar todas en tabla**

5. **Exportar a JSON**

6. **Editar una plantilla**

7. **Desactivar una plantilla**

8. **Activar nuevamente**

9. **Eliminar una plantilla de prueba**

### Paso 7: Performance Testing

**Verificar performance con muchas plantillas:**

- [ ] Crear 20+ plantillas
- [ ] Tabla carga rápidamente
- [ ] Búsqueda es instantánea
- [ ] Filtros funcionan rápido
- [ ] Modal abre sin lag
- [ ] Vista previa se actualiza sin lag

### Paso 8: Browser Testing

**Probar en diferentes navegadores:**

- [ ] Chrome/Edge (Chromium)
- [ ] Firefox
- [ ] Safari (si disponible)

**Verificar:**
- [ ] Layout correcto
- [ ] Funcionalidad completa
- [ ] Sin errores en consola
- [ ] Responsive design

### Paso 9: Mobile Testing

**Probar en vista móvil (DevTools):**

- [ ] Tabla es scrollable horizontalmente
- [ ] Modal es responsivo
- [ ] Botones son clickeables
- [ ] Vista previa se ve bien

### Paso 10: Error Handling

**Probar escenarios de error:**

1. **Sin conexión a API:**
   - [ ] Mensaje de error aparece
   - [ ] No crash de aplicación

2. **Token expirado:**
   - [ ] Redirect a login
   - [ ] Mensaje apropiado

3. **Servidor retorna 500:**
   - [ ] Error se captura
   - [ ] Toast de error aparece

4. **Datos inválidos:**
   - [ ] Validación frontend funciona
   - [ ] Error backend se muestra

## 📊 Métricas de Éxito

Después del deployment, verificar:

- [ ] 0 errores en consola
- [ ] 0 errores de compilación
- [ ] Tiempo de carga <2 segundos
- [ ] Todas las funciones CRUD operativas
- [ ] Vista previa funciona para todos los tipos
- [ ] Validaciones funcionan correctamente
- [ ] Permisos se respetan

## 🚀 Post-Deployment

### Monitoreo (Primera Semana)

- [ ] Revisar logs de errores diariamente
- [ ] Verificar uso de la funcionalidad
- [ ] Recopilar feedback de usuarios
- [ ] Monitorear performance

### Tareas Adicionales

1. **Crear plantillas iniciales en producción**
   - Usar TEMPLATE_EXAMPLES.json como referencia
   - Importar plantillas más comunes

2. **Documentar para usuarios finales**
   - Crear guía de usuario
   - Video tutorial (opcional)

3. **Capacitación**
   - Entrenar a administradores
   - Documentar casos de uso comunes

## 🐛 Rollback Plan

Si hay problemas críticos:

### Opción 1: Desactivar Tab

```typescript
// En AdminLayout.tsx, comentar temporalmente:
// case 'plantillas': return 'Plantillas de Notificación';

// Y comentar sección de renderizado:
// {activeSection === 'plantillas' && (
//   <NotificationTemplatesView ... />
// )}
```

### Opción 2: Rollback Completo

```bash
git revert [commit-hash]
git push
npm run build
```

## 📝 Sign-Off

**Desarrollador:**
- Nombre: _________________
- Fecha: _________________
- Firma: _________________

**QA Tester:**
- Nombre: _________________
- Fecha: _________________
- Firma: _________________

**DevOps:**
- Nombre: _________________
- Fecha: _________________
- Firma: _________________

**Product Owner:**
- Nombre: _________________
- Fecha: _________________
- Firma: _________________

---

## 📞 Contactos de Soporte

**Desarrollo:**
- Email: dev@empresa.com
- Slack: #dev-support

**Infraestructura:**
- Email: devops@empresa.com
- Slack: #devops

**Emergencias:**
- Phone: +XX XXX XXX XXXX

---

**Versión del Checklist:** 1.0.0
**Última Actualización:** 2024-03-15
**Feature:** Notification Templates Management
