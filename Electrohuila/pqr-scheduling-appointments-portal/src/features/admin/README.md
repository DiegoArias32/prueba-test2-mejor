# Feature: Admin Panel

## Descripción

Panel administrativo complejo con múltiples funcionalidades de gestión. Implementa **arquitectura MVVM consolidada** para balance entre organización y mantenibilidad.

## Arquitectura MVVM Consolidada ✅

Este feature implementa MVVM de forma **consolidada** (no modular por tabs) debido a su complejidad (3200+ líneas) y la interconexión entre módulos.

### Estructura MVVM

```
features/admin/
├── models/                           ✅ MVVM - Models
│   └── admin.models.ts              (Todos los tipos e interfaces)
├── repositories/                     ✅ MVVM - Repository
│   └── admin.repository.ts          (Todas las llamadas a API - 200+ LOC)
├── viewmodels/                       ✅ MVVM - ViewModel
│   └── useAdmin.ts                  (Toda la lógica de negocio - 1200+ LOC)
├── domain/                           ✅ Domain Entities
│   └── entities/AdminTypes.ts       (Tipos compartidos)
└── index.ts                          ✅ Public API

app/admin/
└── page.tsx                          ✅ View (JSX consolidado - 1600+ LOC)
```

### ✅ Separación de Responsabilidades

| Capa | Responsabilidad | Archivo | Líneas |
|------|----------------|---------|--------|
| **Models** | Tipos, interfaces, constantes | `models/admin.models.ts` | ~150 |
| **Repository** | Acceso a datos (API calls) | `repositories/admin.repository.ts` | ~230 |
| **ViewModel** | Lógica de negocio, estado, validaciones | `viewmodels/useAdmin.ts` | ~1200 |
| **View** | Presentación (JSX) | `app/admin/page.tsx` | ~1600 |

## Funcionalidades Principales

El panel incluye 7 módulos de gestión:

1. **Citas** - Gestión de citas agendadas (visualización, edición, estado)
2. **Empleados** - Administración de usuarios del sistema (CRUD completo)
3. **Roles** - Gestión de roles y permisos (CRUD completo)
4. **Sedes** - Administración de sedes/sucursales (CRUD completo)
5. **Tipos de Cita** - Configuración de tipos de servicio (CRUD completo)
6. **Horas Disponibles** - Gestión de horarios disponibles (CRUD completo)
7. **Permisos** - Sistema de permisos granular (asignación por rol y empleado)

## Características Implementadas

- ✅ **CRUD completo** para cada entidad con validaciones
- ✅ **Sistema de permisos** basado en roles (read, create, update, delete)
- ✅ **Validación de formularios** client-side con mensajes descriptivos
- ✅ **Paginación** de datos (10 items por página)
- ✅ **Auto-refresh** inteligente en tiempo real (cada 30s)
- ✅ **Gestión de estados** (activos/inactivos) con soft-delete
- ✅ **Modales** para crear/editar/eliminar elementos
- ✅ **Type Guards** para type-safety en TypeScript
- ✅ **Gestión de pestañas** por empleado
- ✅ **Permisos granulares** por formulario

## ViewModel Hook: `useAdmin`

```typescript
import { useAdmin } from '@/features/admin';
import { adminRepository } from '@/features/admin';

const viewModel = useAdmin(adminRepository);

// Estado disponible
viewModel.activeTab
viewModel.currentView
viewModel.loading
viewModel.error
viewModel.citas
viewModel.empleados
viewModel.roles
// ... +40 estados más

// Acciones disponibles
viewModel.handleCreateEmpleado()
viewModel.handleUpdateCita()
viewModel.handleDeleteRol()
viewModel.loadData()
viewModel.refreshData()
// ... +60 acciones más
```

## Repository: `adminRepository`

```typescript
import { adminRepository } from '@/features/admin';

// Métodos disponibles
await adminRepository.getUsers();
await adminRepository.createRol(data);
await adminRepository.updateBranch(data);
await adminRepository.deleteLogicalUser(id);
await adminRepository.getAllPermissions();
// ... +40 métodos más
```

## Razón del Enfoque Consolidado

### Ventajas ✅

1. **Simplicidad**: Toda la lógica de un tab está en un solo lugar
2. **Menos overhead**: No hay comunicación compleja entre múltiples módulos
3. **Performance**: Menos re-renders, estado compartido eficiente
4. **Mantenibilidad**: Fácil de entender para desarrolladores nuevos
5. **Cohesión**: Los 7 módulos comparten mucho estado y lógica

### Cuándo Refactorizar a Modular

Considera separar en módulos independientes cuando:
- Cada tab supere las 500 líneas de lógica propia
- Se requiera testing unitario granular por tab
- Múltiples desarrolladores trabajen simultáneamente
- Se necesite lazy loading por tab
- Los tabs tengan lógica completamente independiente

## Migración Futura (Opcional)

Si se requiere modularizar completamente, la estructura recomendada sería:

```
features/admin/
├── models/
│   ├── shared/           (Tipos compartidos)
│   ├── citas/
│   ├── empleados/
│   └── ...
├── repositories/
│   ├── citas.repository.ts
│   ├── empleados.repository.ts
│   └── ...
├── viewmodels/
│   ├── useCitas.ts
│   ├── useEmpleados.ts
│   └── ...
└── views/
    ├── CitasView.tsx
    ├── EmpleadosView.tsx
    └── AdminView.tsx (orquestador)
```

## Uso en Page

```typescript
// app/admin/page.tsx (simplificado en el futuro)
import { useAdmin, adminRepository } from '@/features/admin';

export default function AdminPage() {
  const vm = useAdmin(adminRepository);

  // Todo el JSX usando vm.* para estado y acciones
  return (
    <div>
      {/* Componentes de UI usando viewModel */}
    </div>
  );
}
```

## Testing

```typescript
import { renderHook } from '@testing-library/react';
import { useAdmin } from '@/features/admin';

test('should handle CRUD operations', () => {
  const { result } = renderHook(() => useAdmin(mockRepository));

  await result.current.handleCreateRol();
  expect(mockRepository.createRol).toHaveBeenCalled();
});
```

## Notas

- ✅ **El componente actual funciona correctamente y es performante**
- ✅ **La refactorización a MVVM consolidado está completa**
- ⚠️ **Cualquier modularización adicional debe ser justificada por necesidades reales**
- 📚 **"Premature optimization is the root of all evil"** - Donald Knuth
- 🎯 **Balance entre arquitectura limpia y pragmatismo**
