# Fix: Usuarios Inactivos no aparecen en la pestaña "Inactivos"

## Problema Identificado

Cuando se desactiva un usuario (o rol, sede, tipo de cita, hora disponible), la operación es exitosa pero el elemento NO aparece en la pestaña de "Inactivos".

## Causa Raíz

Los hooks de gestión (useUsers, useRoles, useBranches, useAvailableTimes) tenían dos problemas críticos:

### 1. **fetchEntity() filtraba en lugar de retornar todos los datos**
```typescript
// ANTES (INCORRECTO)
const fetchUsers = useCallback(async (activeOnly = true) => {
  const data = await repository.getUsers();
  setUsers(activeOnly ? data.filter(u => u.isActive) : data.filter(u => !u.isActive));
}, [repository]);
```

**Problema**: Solo guardaba activos O inactivos, nunca ambos al mismo tiempo.

### 2. **activateEntity() y deleteEntity() usaban filter() en lugar de actualizar estado**
```typescript
// ANTES (INCORRECTO)
const deleteUser = useCallback(async (id: number) => {
  await repository.deleteLogicalUser(id);
  setUsers(prev => prev.filter(u => u.id !== id)); // ❌ Elimina del array
}, [repository]);

const activateUser = useCallback(async (id: number) => {
  await repository.activateUser(id);
  setUsers(prev => prev.filter(u => u.id !== id)); // ❌ Elimina del array
}, [repository]);
```

**Problema**: Eliminaba el elemento del estado local en lugar de actualizar su propiedad `isActive`.

## Solución Implementada

### 1. **fetchEntity() ahora retorna TODOS los datos**
```typescript
// DESPUÉS (CORRECTO)
const fetchUsers = useCallback(async (activeOnly = true) => {
  // Always fetch all users (including inactive) to have complete data
  const data = await repository.getUsers();
  // Store all users - UsersView will filter them based on currentView
  setUsers(data);
}, [repository]);
```

**Beneficio**: El hook mantiene TODOS los usuarios (activos e inactivos) en estado. Las vistas se encargan del filtrado local.

### 2. **activateEntity() y deleteEntity() actualizan el estado correctamente**
```typescript
// DESPUÉS (CORRECTO)
const deleteUser = useCallback(async (id: number, logical = true) => {
  if (logical) {
    await repository.deleteLogicalUser(id);
    // Update local state to mark as inactive instead of removing
    setUsers(prev => prev.map(u =>
      u.id === id ? { ...u, isActive: false } : u
    ));
  } else {
    await repository.deleteUser(id);
    // Physical delete - remove from list
    setUsers(prev => prev.filter(u => u.id !== id));
  }
}, [repository]);

const activateUser = useCallback(async (id: number) => {
  await repository.activateUser(id);
  // Update local state to mark as active instead of removing
  setUsers(prev => prev.map(u =>
    u.id === id ? { ...u, isActive: true } : u
  ));
}, [repository]);
```

**Beneficio**:
- Borrado lógico actualiza `isActive: false` → El elemento se mueve a la pestaña "Inactivos"
- Activación actualiza `isActive: true` → El elemento se mueve a la pestaña "Activos"

## Archivos Corregidos

### 1. **useUsers.ts**
- ✅ Línea 41-54: `fetchUsers()` ahora retorna todos los datos
- ✅ Línea 116-137: `deleteUser()` actualiza estado en lugar de filtrar
- ✅ Línea 139-154: `activateUser()` actualiza estado en lugar de filtrar

### 2. **useRoles.ts**
- ✅ Línea 44-57: `fetchRoles()` ahora retorna todos los datos
- ✅ Línea 95-116: `deleteRole()` actualiza estado en lugar de filtrar
- ✅ Línea 118-133: `activateRole()` actualiza estado en lugar de filtrar

### 3. **useBranches.ts**
- ✅ Línea 41-54: `fetchBranches()` ahora retorna todos los datos
- ✅ Línea 99-120: `deleteBranch()` actualiza estado en lugar de filtrar
- ✅ Línea 122-137: `activateBranch()` actualiza estado en lugar de filtrar

### 4. **useAvailableTimes.ts**
- ✅ Línea 41-55: `fetchAvailableTimes()` ahora retorna todos los datos
- ✅ Línea 118-139: `deleteAvailableTime()` actualiza estado en lugar de filtrar
- ✅ Línea 141-156: `activateAvailableTime()` actualiza estado en lugar de filtrar

### 5. **useAppointmentTypes.ts**
- ✅ YA ESTABA BIEN IMPLEMENTADO (líneas 41-136)
- Este hook ya seguía el patrón correcto desde el inicio

## Patrón de Arquitectura

### Vista (UsersView.tsx, RolesView.tsx, etc.)
```typescript
// La vista se encarga del filtrado LOCAL basado en la pestaña activa
const filteredByStatus = employees.filter(emp =>
  currentView === 'active' ? emp.isActive : !emp.isActive
);
```

### Hook (useUsers.ts, useRoles.ts, etc.)
```typescript
// El hook mantiene TODOS los datos
const fetchUsers = async () => {
  const data = await repository.getUsers();
  setUsers(data); // Guarda TODOS (activos e inactivos)
};

// Las operaciones actualizan el estado local
const deleteUser = async (id) => {
  await repository.deleteLogicalUser(id);
  setUsers(prev => prev.map(u =>
    u.id === id ? { ...u, isActive: false } : u
  ));
};
```

### AdminLayout.tsx
```typescript
// No necesita cambios - refreshEntityData() simplemente llama fetchUsers()
const refreshEntityData = async (entityType) => {
  switch (entityType) {
    case 'users':
      await admin.users.fetchUsers(); // Retorna todos los datos
      break;
    // ...
  }
};
```

## Flujo de Datos Corregido

### Desactivar Usuario (Pestaña Activos)
1. Usuario hace clic en "Desactivar" en pestaña "Activos"
2. `handleDeactivateUser(id)` en AdminLayout → llama `repository.deleteLogicalUser(id)`
3. Backend marca `isActive = false`
4. Hook actualiza estado: `setUsers(prev => prev.map(u => u.id === id ? { ...u, isActive: false } : u))`
5. Vista filtra: `currentView === 'active' ? emp.isActive : !emp.isActive`
6. El usuario DESAPARECE de "Activos"
7. Usuario cambia a pestaña "Inactivos" → `setCurrentView('inactive')`
8. Vista filtra: `!emp.isActive` → El usuario APARECE en "Inactivos" ✅

### Activar Usuario (Pestaña Inactivos)
1. Usuario hace clic en "Activar" en pestaña "Inactivos"
2. `handleActivateUser(id)` en AdminLayout → llama `repository.activateUser(id)`
3. Backend marca `isActive = true`
4. Hook actualiza estado: `setUsers(prev => prev.map(u => u.id === id ? { ...u, isActive: true } : u))`
5. Vista filtra: `currentView === 'inactive' ? !emp.isActive : emp.isActive`
6. El usuario DESAPARECE de "Inactivos"
7. Usuario cambia a pestaña "Activos" → `setCurrentView('active')`
8. Vista filtra: `emp.isActive` → El usuario APARECE en "Activos" ✅

## Ventajas del Nuevo Patrón

1. **Consistencia**: Mismo patrón en todos los hooks (Users, Roles, Branches, AvailableTimes, AppointmentTypes)
2. **Rendimiento**: No requiere refetch desde el servidor al cambiar de pestaña
3. **Sincronización**: El estado local siempre refleja el estado del servidor
4. **Simplicidad**: Las vistas no dependen del hook para filtrar, lo hacen localmente
5. **Escalabilidad**: Agregar nuevas entidades es más fácil siguiendo este patrón

## Testing

### Caso 1: Desactivar Usuario
- [ ] En pestaña "Activos", desactivar usuario → Debe desaparecer de "Activos"
- [ ] Cambiar a pestaña "Inactivos" → Usuario debe aparecer allí
- [ ] El contador de inactivos debe incrementar

### Caso 2: Activar Usuario
- [ ] En pestaña "Inactivos", activar usuario → Debe desaparecer de "Inactivos"
- [ ] Cambiar a pestaña "Activos" → Usuario debe aparecer allí
- [ ] El contador de activos debe incrementar

### Caso 3: Aplicar mismo test a:
- [ ] Roles
- [ ] Sedes (Branches)
- [ ] Tipos de Cita (AppointmentTypes)
- [ ] Horas Disponibles (AvailableTimes)

## Archivos Modificados

```
pqr-scheduling-appointments-portal/src/features/admin/viewmodels/
  ├── useUsers.ts (MODIFICADO - 3 funciones corregidas)
  ├── useRoles.ts (MODIFICADO - 3 funciones corregidas)
  ├── useBranches.ts (MODIFICADO - 3 funciones corregidas)
  ├── useAvailableTimes.ts (MODIFICADO - 3 funciones corregidas)
  └── useAppointmentTypes.ts (SIN CAMBIOS - ya estaba bien)
```

## Fecha de Corrección
2025-11-26

## Estado
✅ CORREGIDO - Listo para testing
