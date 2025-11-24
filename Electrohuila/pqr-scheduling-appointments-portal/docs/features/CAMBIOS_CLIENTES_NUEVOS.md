# Implementación de Flujo para Clientes Nuevos en Agendamiento de Citas

## Fecha de Implementación
2025-11-16

## Resumen
Se ha implementado exitosamente el flujo de agendamiento de citas para usuarios que no tienen número de cliente, permitiendo que puedan registrarse y agendar una cita en el mismo proceso.

---

## Archivos Modificados

### 1. **Componentes Nuevos Creados**

#### `src/features/appointments/views/components/NewClientForm.tsx`
**Descripción:** Componente de formulario para capturar datos de clientes nuevos.

**Funcionalidades:**
- Captura de información básica del cliente:
  - Tipo de documento (CC, TI, CE, RC, NIT)
  - Número de documento (validación: solo números, máx 15 dígitos)
  - Nombre completo (3-200 caracteres)
  - Email (validación de formato)
  - Celular (validación: 10 dígitos)
  - Teléfono fijo (opcional)
  - Dirección (opcional)
- Validaciones en tiempo real
- Feedback visual de errores
- Notificación al componente padre sobre validez del formulario

---

### 2. **Componentes Modificados**

#### `src/features/appointments/views/steps/ClientValidationStep.tsx`
**Cambios:**
- Añadidas nuevas props:
  - `isNewClient: boolean`
  - `onNewClientClick: () => void`
  - `onBackToClientNumber: () => void`
  - `onNewClientDataChange: (data: NewClientData, isValid: boolean) => void`
  - `onContinueAsNewClient: () => void`
  - `isNewClientFormValid: boolean`

- UI modificada:
  - Mensaje de error mejorado cuando el cliente no existe
  - Botón "¿No tienes número de cliente? Regístrate aquí" visible al fallar validación
  - Renderizado condicional del formulario de cliente nuevo
  - Botón para continuar con agendamiento cuando formulario es válido
  - Botón para volver a ingresar número de cliente

**Flujo:**
```
Usuario ingresa número → Validación falla →
Mensaje: "No se encontró número de cliente" →
Botón: "¿No tienes número de cliente? Regístrate aquí" →
Muestra NewClientForm →
Usuario completa datos →
Botón: "Continuar con agendamiento" (habilitado solo si formulario válido)
```

---

#### `src/features/appointments/views/AppointmentSchedulingView.tsx`
**Cambios:**
- Integración de nuevos estados del viewmodel:
  - `isNewClient`
  - `isNewClientFormValid`
  - `handleNewClientClick`
  - `handleBackToClientNumber`
  - `handleNewClientDataChange`
  - `handleContinueAsNewClient`

- Paso 'form' modificado:
  - Ahora permite continuar sin `clientData` si es cliente nuevo
  - Crea objeto temporal `displayClientData` para mostrar "Cliente Nuevo"

---

#### `src/features/appointments/viewmodels/useAppointmentScheduling.ts`
**Cambios principales:**

1. **Nuevos estados:**
```typescript
const [isNewClient, setIsNewClient] = useState(false);
const [newClientData, setNewClientData] = useState<NewClientData | null>(null);
const [isNewClientFormValid, setIsNewClientFormValid] = useState(false);
```

2. **Nuevos métodos:**

- `handleNewClientClick()`: Activa modo de registro de cliente nuevo
- `handleBackToClientNumber()`: Vuelve al formulario de número de cliente
- `handleNewClientDataChange()`: Actualiza datos del nuevo cliente y validez
- `handleContinueAsNewClient()`: Avanza al formulario de cita si datos son válidos

3. **Método `scheduleAppointment()` modificado:**

**Flujo para cliente nuevo:**
```typescript
if (isNewClient && newClientData) {
  // Validar formulario completo
  if (!isNewClientFormValid) {
    throw new Error('Campos incompletos');
  }

  // Llamar endpoint de registro simple
  const response = await appointmentRepository.scheduleAppointmentForNewUser({
    userData: newClientData,
    branchId,
    appointmentTypeId,
    appointmentDate,
    appointmentTime,
    observations
  });

  // Crear ClientDto temporal para confirmación
  const tempClientData: ClientDto = {
    id: 0,
    clientNumber: response.requestNumber, // Número de solicitud generado
    documentType: newClientData.documentType,
    documentNumber: newClientData.documentNumber,
    fullName: newClientData.fullName,
    email: newClientData.email,
    phone: newClientData.phone,
    mobile: newClientData.mobile,
    address: newClientData.address,
    createdAt: new Date().toISOString(),
    isActive: true,
    isEnabled: true,
  };

  // Generar QR usando documento como identificador
  const qrData = `CITA:${response.appointmentNumber}|CLIENTE:${newClientData.documentNumber}`;
}
```

**Flujo para cliente existente:**
```typescript
else if (clientData) {
  // Flujo normal existente
  const response = await appointmentRepository.schedulePublicAppointment({
    clientNumber: clientData.clientNumber,
    branchId,
    appointmentTypeId,
    appointmentDate,
    appointmentTime,
    observations
  });
}
```

4. **Método `resetFlow()` actualizado:**
- Reinicia estados de cliente nuevo

---

#### `src/features/appointments/repositories/appointment.repository.ts`
**Nuevo método:**

```typescript
async scheduleAppointmentForNewUser(appointmentData: {
  userData: {
    documentType: string;
    documentNumber: string;
    fullName: string;
    phone: string;
    mobile: string;
    email: string;
    address: string;
  };
  branchId: number;
  appointmentTypeId: number;
  appointmentDate: string;
  appointmentTime: string;
  observations?: string;
}): Promise<{
  requestNumber: string;
  appointmentDate: string;
  appointmentTime: string;
  status: string;
  message: string;
}>
```

**Endpoint llamado:** `POST /api/v1/public/schedule-simple-appointment`

**Formato de request body:**
```json
{
  "DocumentType": "CC",
  "DocumentNumber": "1234567890",
  "FullName": "Juan Pérez García",
  "Phone": "6012345678",
  "Mobile": "3001234567",
  "Email": "juan.perez@example.com",
  "Address": "Calle 123 #45-67",
  "BranchId": 1,
  "AppointmentTypeId": 2,
  "AppointmentDate": "2025-11-20",
  "AppointmentTime": "10:00",
  "Observations": "Observaciones opcionales"
}
```

---

### 3. **Estilos CSS**

#### `src/app/globals.css`
**Añadida animación:**
```css
@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(-10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.animate-fadeIn {
  animation: fadeIn 0.3s ease-out forwards;
}
```

---

### 4. **Correcciones Adicionales**

#### `src/app/gestion-citas/page.tsx`
**Corrección:** Método `completeAppointment()` actualizado para usar solo 2 parámetros (id, notes) en lugar de 3.

```typescript
// Antes:
await apiService.completeAppointment(
  selectedCita.id,
  completionData.assignedTechnician,
  completionData.technicianObservations
);

// Después:
const notes = `Técnico: ${completionData.assignedTechnician}\nObservaciones: ${completionData.technicianObservations}`;
await apiService.completeAppointment(
  selectedCita.id,
  notes
);
```

---

## Flujo Completo de Usuario

### Escenario 1: Cliente Existente
1. Usuario ingresa número de cliente
2. Sistema valida y encuentra el cliente
3. Continúa a formulario de cita (flujo normal)
4. Agenda usando endpoint: `POST /api/v1/public/schedule-appointment`

### Escenario 2: Cliente Nuevo
1. Usuario ingresa número de cliente
2. Sistema valida y NO encuentra el cliente
3. Muestra mensaje: "No se encontró número de cliente"
4. Usuario hace click en "¿No tienes número de cliente? Regístrate aquí"
5. Muestra formulario `NewClientForm`
6. Usuario completa:
   - Tipo de documento ✓
   - Número de documento ✓ (requerido)
   - Nombre completo ✓ (requerido)
   - Email ✓ (requerido)
   - Celular ✓ (requerido)
   - Teléfono fijo (opcional)
   - Dirección (opcional)
7. Botón "Continuar con agendamiento" se habilita cuando todos los campos requeridos son válidos
8. Usuario hace click en "Continuar con agendamiento"
9. Avanza a formulario de cita con datos prellenados
10. Usuario selecciona:
    - Motivo de cita
    - Sede
    - Fecha
    - Hora
    - Observaciones (opcional)
11. Agenda usando endpoint: `POST /api/v1/public/schedule-simple-appointment`
12. Muestra confirmación con:
    - Número de solicitud (generado por backend)
    - Datos del solicitante
    - Detalles de la cita
    - Código QR (usando documento como identificador)

---

## Validaciones Implementadas

### Formulario de Cliente Nuevo
- **Tipo de documento:** Requerido (select)
- **Número de documento:**
  - Requerido
  - Solo números
  - Máximo 15 dígitos
- **Nombre completo:**
  - Requerido
  - Mínimo 3 caracteres
  - Máximo 200 caracteres
- **Email:**
  - Requerido
  - Formato válido (regex)
- **Celular:**
  - Requerido
  - Exactamente 10 dígitos
  - Solo números
- **Teléfono fijo:** Opcional, máx 20 caracteres
- **Dirección:** Opcional, máx 500 caracteres

---

## Endpoints del Backend

### Endpoint Existente (Cliente registrado)
```
POST /api/v1/public/schedule-appointment
```

**Request:**
```json
{
  "clientNumber": "1234567890",
  "branchId": 1,
  "appointmentTypeId": 2,
  "appointmentDate": "2025-11-20",
  "appointmentTime": "10:00",
  "observations": "Observaciones"
}
```

**Response:**
```json
{
  "appointmentNumber": "CITA-2025-001",
  "appointmentDate": "2025-11-20",
  "appointmentTime": "10:00",
  "status": "Pendiente",
  "message": "Cita agendada exitosamente"
}
```

---

### Endpoint Nuevo (Cliente nuevo)
```
POST /api/v1/public/schedule-simple-appointment
```

**Request:**
```json
{
  "DocumentType": "CC",
  "DocumentNumber": "1234567890",
  "FullName": "Juan Pérez García",
  "Phone": "6012345678",
  "Mobile": "3001234567",
  "Email": "juan.perez@example.com",
  "Address": "Calle 123 #45-67",
  "BranchId": 1,
  "AppointmentTypeId": 2,
  "AppointmentDate": "2025-11-20",
  "AppointmentTime": "10:00",
  "Observations": "Observaciones"
}
```

**Response:**
```json
{
  "requestNumber": "REQ-2025-001",
  "appointmentDate": "2025-11-20",
  "appointmentTime": "10:00",
  "status": "Pendiente",
  "message": "Solicitud de cita creada exitosamente. Su número de solicitud es REQ-2025-001"
}
```

---

## Testing Recomendado

### Casos de Prueba

1. **Cliente existente - Flujo normal**
   - Ingresar número de cliente válido
   - Verificar que avanza a formulario de cita
   - Completar y agendar
   - Verificar confirmación correcta

2. **Cliente no existe - Opción de registro**
   - Ingresar número de cliente inválido
   - Verificar mensaje de error
   - Verificar botón "Regístrate aquí" visible

3. **Formulario cliente nuevo - Validaciones**
   - Intentar continuar con campos vacíos (botón debe estar deshabilitado)
   - Ingresar email inválido
   - Ingresar celular con menos/más de 10 dígitos
   - Verificar mensajes de error en tiempo real

4. **Cliente nuevo - Flujo completo**
   - Completar formulario de cliente nuevo
   - Continuar a formulario de cita
   - Agendar cita
   - Verificar que se usa endpoint `/schedule-simple-appointment`
   - Verificar confirmación con número de solicitud

5. **Navegación**
   - Desde formulario cliente nuevo, volver a número de cliente
   - Verificar que se limpian datos

6. **Responsive**
   - Probar en móvil, tablet y desktop
   - Verificar que formularios sean usables

---

## Mejoras Futuras Sugeridas

1. **Validación de documento duplicado:**
   - Antes de enviar, verificar si el documento ya existe en el sistema

2. **Auto-completado:**
   - Si el usuario tiene cuenta pero no recuerda el número de cliente, buscar por documento

3. **Notificaciones:**
   - Enviar email de confirmación con número de solicitud
   - SMS al celular registrado

4. **Tracking:**
   - Permitir consultar estado de solicitud con número de documento

5. **Campos adicionales:**
   - Municipio/Ciudad
   - Departamento
   - Barrio/Sector

---

## Archivos Afectados - Resumen

### Creados (1)
- `src/features/appointments/views/components/NewClientForm.tsx`

### Modificados (7)
- `src/features/appointments/views/steps/ClientValidationStep.tsx`
- `src/features/appointments/views/AppointmentSchedulingView.tsx`
- `src/features/appointments/viewmodels/useAppointmentScheduling.ts`
- `src/features/appointments/repositories/appointment.repository.ts`
- `src/app/globals.css`
- `src/app/gestion-citas/page.tsx`
- `next.config.ts`

---

## Checklist de Verificación

- [x] Componente `NewClientForm` creado con validaciones
- [x] `ClientValidationStep` actualizado con flujo de cliente nuevo
- [x] `AppointmentSchedulingView` integrado con nuevos estados
- [x] `useAppointmentScheduling` viewmodel actualizado
- [x] `appointment.repository` con método `scheduleAppointmentForNewUser`
- [x] Animaciones CSS añadidas
- [x] Código compila correctamente
- [x] Tipos TypeScript definidos
- [x] Props pasadas correctamente entre componentes
- [ ] Testing end-to-end realizado
- [ ] Probado con backend real

---

## Notas Importantes

1. **Endpoint `/api/v1/public/schedule-simple-appointment` debe existir en el backend**
   - Si no existe, debe ser creado según el contrato especificado

2. **El número de solicitud (`requestNumber`) es generado por el backend**
   - Frontend no genera este número

3. **El QR para clientes nuevos usa el número de documento como identificador**
   - Formato: `CITA:{appointmentNumber}|CLIENTE:{documentNumber}`

4. **Los datos del cliente nuevo se envían en PascalCase al backend**
   - `DocumentType`, `DocumentNumber`, `FullName`, etc.

---

## Autor
- Claude Code Assistant
- Fecha: 2025-11-16
