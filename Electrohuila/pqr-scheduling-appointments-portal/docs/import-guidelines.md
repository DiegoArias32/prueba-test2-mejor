# Import Guidelines

This document outlines the standard import patterns for the PQR Scheduling Appointments Portal codebase.

## Directory Structure Overview

```
src/
├── app/                    # Next.js app router pages
├── core/                   # Core functionality (types, config, HTTP client)
│   ├── api/               # HTTP client
│   ├── config/            # Configuration files
│   └── types/             # Core type definitions
├── features/              # Feature modules (domain-driven)
│   ├── admin/
│   ├── appointment-consultation/
│   ├── appointment-management/
│   ├── cuentas-nuevas/
│   ├── proyecto-nuevo/
│   └── verificar-cita/
├── services/              # API services (domain-separated)
│   ├── appointments/      # Appointment-related services
│   ├── auth/              # Authentication services
│   ├── catalogs/          # Catalog services
│   ├── permissions/       # Permission services
│   ├── users/             # User services
│   └── index.ts           # Unified export point
└── shared/                # Shared components, hooks, and utilities
    ├── components/        # Reusable UI components
    ├── hooks/             # Custom React hooks
    ├── layouts/           # Layout components
    ├── utils/             # Utility functions
    └── constants/         # Shared constants
```

## Import Patterns

### 1. Shared Components

**Recommended: Use centralized exports**

```typescript
// Good - Use index exports
import {
  StatusBadge,
  AppointmentCard,
  CancelAppointmentModal,
  StatsCard
} from '@/shared/components';

// Also Good - For specific component groups
import { FormField, FormActions, Input, Select } from '@/shared/components/Form';
import { Spinner, Skeleton, TableSkeleton } from '@/shared/components/Loading';
import { Table, TableHeader, TableRow, TablePagination } from '@/shared/components/Table';
```

**Avoid: Direct file imports**

```typescript
// Bad - Bypasses centralized exports
import { StatusBadge } from '@/shared/components/StatusBadge';
import { AppointmentCard } from '@/shared/components/AppointmentCard';
```

**Exception: When you need specific exports from a component file**

```typescript
// OK - When you need non-component exports
import { ToastContainer, useToast } from '@/shared/components/Toast';
```

---

### 2. Shared Utilities

**Recommended: Use centralized exports or named module exports**

```typescript
// Good - Use index exports for validators
import { validateEmail, validatePhone, validateRequired } from '@/shared/utils';

// Good - Named imports from specific utility modules
import { ValidationUtils, FormErrors } from '@/shared/utils/validation.utils';
import { formatDate, formatTime, getCurrentDateTime } from '@/shared/utils/formatters';
import {
  getAppointmentStatusColor,
  getAppointmentStatusIcon,
  getAppointmentStatusName
} from '@/shared/utils/catalogHelpers';
```

**Avoid: Generic index imports**

```typescript
// Bad - Too generic, unclear what's being imported
import * as utils from '@/shared/utils';
```

---

### 3. Shared Hooks

**Recommended: Use centralized exports**

```typescript
// Good - Use index exports
import { useFormValidation, useCatalogs } from '@/shared/hooks';

// Also Good - Direct import for specific hooks
import { useFormValidation } from '@/shared/hooks/useFormValidation';
import { useCatalogs } from '@/shared/hooks/useCatalogs';
```

---

### 4. Services

**Recommended: Use service instances from centralized export**

```typescript
// Good - Import service instances
import {
  authService,
  appointmentService,
  userService,
  catalogService,
  permissionService
} from '@/services';

// Example usage
const appointments = await appointmentService.getAppointments();
const user = await authService.getCurrentUser();
```

**Legacy Support: apiService for backward compatibility**

```typescript
// Acceptable - For backward compatibility
import { apiService } from '@/services';

// This still works but is deprecated
const appointments = await apiService.getAppointments();
```

**Type Imports**

```typescript
// Good - Import types from centralized services export
import type {
  AppointmentDto,
  ClientDto,
  BranchDto,
  AppointmentTypeDto,
  UserDto,
  RolDto
} from '@/services';

// Also Good - Import from specific service
import type { AppointmentDto } from '@/services/appointments/appointment.types';
```

**Avoid: Direct service file imports**

```typescript
// Bad - Bypasses centralized exports
import { AuthService } from '@/services/auth/auth.service';
import { AppointmentService } from '@/services/appointments/appointment.service';
```

---

### 5. Core Types

**Recommended: Import from core/types**

```typescript
// Good - Import from centralized core types
import type { AppointmentStatus, AuthUser } from '@/core/types';

// Also Good - Import from specific type files
import type { AppointmentStatus } from '@/core/types/appointment.types';
import type { AuthUser } from '@/core/types/auth.types';
```

---

### 6. Feature Modules

Feature modules are self-contained and should expose a clean public API through their index files.

**Recommended: Import from feature index**

```typescript
// Good - Use feature module exports
import { useAdmin, useAdminAppointments } from '@/features/admin';

// Good - Import views from feature
import { AdminView } from '@/features/admin/views/AdminView';
```

**Internal Feature Imports**

Within a feature module, use relative imports:

```typescript
// Good - Within the same feature
import { AdminRepository } from '../repositories/admin.repository';
import { useAdminUI } from '../viewmodels/useAdminUI';
```

---

## Migration Guide from Legacy Imports

If you're working with legacy code, here's how to update old import patterns:

### Old Pattern → New Pattern

```typescript
// OLD: Monolithic utils imports
import { validateEmail } from '@/utils/validation';
// NEW:
import { validateEmail } from '@/shared/utils';

// OLD: Direct component imports
import { StatusBadge } from '@/components/StatusBadge';
// NEW:
import { StatusBadge } from '@/shared/components';

// OLD: Old hooks location
import { useCatalogs } from '@/hooks/useCatalogs';
// NEW:
import { useCatalogs } from '@/shared/hooks';

// OLD: Direct API service import
import { ApiService } from '@/services/ApiService';
// NEW:
import { apiService } from '@/services';
// OR (preferred):
import { appointmentService, authService } from '@/services';
```

---

## Import Order Convention

For consistency, organize imports in this order:

```typescript
// 1. External dependencies
import React, { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';

// 2. Services
import { appointmentService, authService } from '@/services';
import type { AppointmentDto, UserDto } from '@/services';

// 3. Shared components
import { StatusBadge, AppointmentCard } from '@/shared/components';

// 4. Shared hooks
import { useFormValidation, useCatalogs } from '@/shared/hooks';

// 5. Shared utilities
import { formatDate, formatTime } from '@/shared/utils/formatters';
import { ValidationUtils } from '@/shared/utils/validation.utils';

// 6. Core types
import type { AppointmentStatus } from '@/core/types';

// 7. Feature-specific imports
import { useAdmin } from '@/features/admin';

// 8. Relative imports (within same feature)
import { AdminLayout } from './AdminLayout';
import styles from './styles.module.css';
```

---

## Best Practices

### DO:

1. **Use centralized index exports** whenever available
2. **Import types using `type` keyword** for better tree-shaking
3. **Use named imports** instead of default exports when possible
4. **Group related imports** together
5. **Use absolute paths** with `@/` alias for src imports
6. **Keep feature modules self-contained** with their own viewmodels, repositories, and views

### DON'T:

1. **Don't bypass index exports** for direct file imports (except when necessary)
2. **Don't use wildcards** (`import * as`) unless truly needed
3. **Don't mix legacy and new import patterns** in the same file
4. **Don't import implementation details** from other features (use public APIs)
5. **Don't create circular dependencies** between modules

---

## Examples of Correct Usage

### Example 1: Admin View Component

```typescript
import React, { useState } from 'react';
import { appointmentService, userService } from '@/services';
import type { AppointmentDto, UserDto } from '@/services';
import { StatusBadge, Table, Spinner } from '@/shared/components';
import { useFormValidation } from '@/shared/hooks';
import { formatDate } from '@/shared/utils/formatters';
import { ValidationUtils } from '@/shared/utils/validation.utils';

export const AdminView: React.FC = () => {
  // Component implementation
};
```

### Example 2: Appointment Service Usage

```typescript
import { appointmentService } from '@/services';
import type { AppointmentDto, CreateAppointmentRequest } from '@/services';

export const useAppointments = () => {
  const fetchAppointments = async () => {
    const appointments = await appointmentService.getAppointments();
    return appointments;
  };

  const createAppointment = async (data: CreateAppointmentRequest) => {
    return await appointmentService.createAppointment(data);
  };

  return { fetchAppointments, createAppointment };
};
```

### Example 3: Form Component with Validation

```typescript
import { useState } from 'react';
import { FormField, FormActions, Input, Select } from '@/shared/components/Form';
import { useFormValidation } from '@/shared/hooks';
import { ValidationUtils, FormErrors } from '@/shared/utils/validation.utils';

export const AppointmentForm: React.FC = () => {
  const [errors, setErrors] = useState<FormErrors>({});

  const validateForm = (data: any) => {
    return ValidationUtils.validateAppointmentForm(data);
  };

  // Component implementation
};
```

---

## Troubleshooting

### Import not found error

If you get "Module not found" errors:

1. Check if the export exists in the index file
2. Verify the path alias is correct (`@/` maps to `src/`)
3. Restart your TypeScript server / IDE
4. Check if you need to import from a specific submodule

### Circular dependency warning

If you encounter circular dependencies:

1. Review your import structure
2. Consider moving shared types to `@/core/types`
3. Use dependency injection or lazy imports
4. Restructure modules to remove circular references

### Type errors with service imports

If types aren't resolving:

1. Use `import type { ... }` for type-only imports
2. Check that types are properly exported in service index files
3. Verify TypeScript configuration in `tsconfig.json`

---

## Service-Specific Import Patterns

### Authentication Service

```typescript
import { authService } from '@/services';
import type { LoginRequest, AuthUser } from '@/services';

// Usage
const user = await authService.login(credentials);
const isAuth = authService.isAuthenticated();
```

### Appointment Service

```typescript
import { appointmentService } from '@/services';
import type {
  AppointmentDto,
  CreateAppointmentRequest,
  AvailableTimeDto
} from '@/services';

// Usage
const appointments = await appointmentService.getAppointments();
const times = await appointmentService.getAvailableHours(branchId, date);
```

### Catalog Service

```typescript
import { catalogService } from '@/services';
import type { BranchDto, AppointmentTypeDto, ClientDto } from '@/services';

// Usage
const branches = await catalogService.getBranches();
const types = await catalogService.getAppointmentTypes();
```

### User Service

```typescript
import { userService } from '@/services';
import type { UserDto, RolDto } from '@/services';

// Usage
const users = await userService.getUsers();
const roles = await userService.getRoles();
```

### Permission Service

```typescript
import { permissionService } from '@/services';
import type { FormPermissionDto, RolPermissionSummaryDto } from '@/services';

// Usage
const permissions = await permissionService.getAllPermissions();
const rolPerms = await permissionService.getRolPermissionsSummary(rolId);
```

---

## Related Documentation

- [TypeScript Configuration](../tsconfig.json)
- [Service Architecture](./service-architecture.md) *(to be created)*
- [Component Guidelines](./component-guidelines.md) *(to be created)*
- [Feature Module Structure](./feature-module-structure.md) *(to be created)*

---

**Last Updated:** 2025-11-14
**Maintained By:** Development Team
