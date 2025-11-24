# Import Optimization Opportunities

**Project:** PQR Scheduling Appointments Portal
**Date:** 2025-11-14
**Priority:** LOW (All current imports are functional)

---

## Overview

This document identifies optional optimizations for import patterns. All current imports are working correctly, but these changes would improve consistency with established patterns.

**Note:** These are optional improvements, not required fixes.

---

## Component Imports

### 1. StatusBadge Import in AppointmentsView

**File:** `src/features/admin/views/appointments/AppointmentsView.tsx`

**Current:**
```typescript
import { StatusBadge } from '@/shared/components/StatusBadge';
```

**Recommended:**
```typescript
import { StatusBadge } from '@/shared/components';
```

**Impact:** Very low - Both patterns work correctly
**Effort:** Minimal - 1 line change
**Benefit:** Consistency with recommended pattern

---

## Service Imports

### Migration to Domain-Specific Services

Multiple files currently use the backward-compatible `apiService`. While this is perfectly acceptable, migrating to domain-specific services provides better type safety and IntelliSense.

### Example Files Using apiService

**Files (20+ files):**
- `src/app/gestion-citas/page.tsx`
- `src/features/admin/repositories/admin.repository.ts`
- `src/features/admin/viewmodels/useAdminUI.ts`
- `src/features/admin/views/AdminLayout.tsx`
- `src/features/appointment-consultation/viewmodels/useAppointmentConsultation.ts`
- `src/features/cuentas-nuevas/repositories/cuentas-nuevas.repository.ts`
- `src/features/proyecto-nuevo/repositories/proyecto-nuevo.repository.ts`
- `src/features/verificar-cita/repositories/verificar-cita.repository.ts`
- And 12+ more...

**Current Pattern:**
```typescript
import { apiService, AppointmentDto } from '@/services/api';

// Usage
const appointments = await apiService.getAppointments();
```

**Recommended Pattern:**
```typescript
import { appointmentService } from '@/services';
import type { AppointmentDto } from '@/services';

// Usage
const appointments = await appointmentService.getAppointments();
```

### Benefits of Migration

1. **Better IntelliSense:** Service methods are more discoverable
2. **Type Safety:** Clearer method signatures
3. **Code Splitting:** Only import services you need
4. **Smaller Bundles:** Better tree-shaking potential
5. **Clearer Intent:** Domain-specific services show what the code does

### Migration Strategy

**Recommended Approach:** Gradual migration as files are modified

```typescript
// When you modify a file that uses apiService, update it to:

// OLD:
import { apiService } from '@/services/api';
const appointments = await apiService.getAppointments();
const user = await apiService.getCurrentUser();
const branches = await apiService.getBranches();

// NEW:
import { appointmentService, authService, catalogService } from '@/services';
const appointments = await appointmentService.getAppointments();
const user = await authService.getCurrentUser();
const branches = await catalogService.getBranches();
```

---

## Detailed Migration Examples

### Example 1: Admin Repository

**File:** `src/features/admin/repositories/admin.repository.ts`

**Before:**
```typescript
import { apiService } from '@/services/api';

export class AdminRepository {
  async getAppointments() {
    return await apiService.getAppointments();
  }

  async getUsers() {
    return await apiService.getUsers();
  }

  async getBranches() {
    return await apiService.getBranches();
  }
}
```

**After:**
```typescript
import { appointmentService, userService, catalogService } from '@/services';

export class AdminRepository {
  async getAppointments() {
    return await appointmentService.getAppointments();
  }

  async getUsers() {
    return await userService.getUsers();
  }

  async getBranches() {
    return await catalogService.getBranches();
  }
}
```

**Benefits:**
- Clearer which service each method uses
- Better code organization
- Easier to test (mock specific services)

---

### Example 2: Appointment Consultation ViewModel

**File:** `src/features/appointment-consultation/viewmodels/useAppointmentConsultation.ts`

**Before:**
```typescript
import { AppointmentDto, ClientDto } from '@/services/api';

const queryAppointment = async (clientNumber: string) => {
  const result = await apiService.queryPublicAppointment(clientNumber);
  return result;
};
```

**After:**
```typescript
import { appointmentService } from '@/services';
import type { AppointmentDto, ClientDto } from '@/services';

const queryAppointment = async (clientNumber: string) => {
  const result = await appointmentService.queryPublicAppointment(clientNumber);
  return result;
};
```

**Benefits:**
- Clear separation of types and logic
- Better IntelliSense for appointment methods
- Consistent with new patterns

---

### Example 3: Cuentas Nuevas Repository

**File:** `src/features/cuentas-nuevas/repositories/cuentas-nuevas.repository.ts`

**Before:**
```typescript
import { apiService, BranchDto, AppointmentTypeDto } from '@/services/api';

export class CuentasNuevasRepository {
  async getBranches(): Promise<BranchDto[]> {
    return await apiService.getPublicBranches();
  }

  async getAppointmentTypes(): Promise<AppointmentTypeDto[]> {
    return await apiService.getPublicAppointmentTypes();
  }

  async createRequest(data: any) {
    return await apiService.requestNewAccountPublic(data);
  }
}
```

**After:**
```typescript
import { catalogService } from '@/services';
import type { BranchDto, AppointmentTypeDto } from '@/services';

export class CuentasNuevasRepository {
  async getBranches(): Promise<BranchDto[]> {
    return await catalogService.getPublicBranches();
  }

  async getAppointmentTypes(): Promise<AppointmentTypeDto[]> {
    return await catalogService.getPublicAppointmentTypes();
  }

  async createRequest(data: any) {
    return await catalogService.requestNewAccountPublic(data);
  }
}
```

**Benefits:**
- All catalog-related operations use catalogService
- Clearer domain boundaries
- Easier to maintain and test

---

## Quick Reference: Service Method Mapping

### appointmentService
- `getAppointments()`, `getAppointmentById()`, `createAppointment()`
- `updateAppointment()`, `deleteAppointment()`, `cancelAppointment()`
- `getAvailableHours()`, `validateAvailability()`
- `schedulePublicAppointment()`, `queryPublicAppointment()`
- `verifyAppointmentByQR()`
- All available time methods

### authService
- `login()`, `logout()`, `refreshToken()`
- `getCurrentUser()`, `getCurrentUserRoles()`, `getCurrentUserPermissions()`
- `isAuthenticated()`, `hasFormPermission()`

### userService
- `getUsers()`, `getUserById()`, `createUser()`, `updateUser()`, `deleteUser()`
- `getRoles()`, `createRol()`, `updateRol()`, `deleteRol()`
- `getUserTabsPermissions()`, `updateUserTabs()`

### catalogService
- `getBranches()`, `getAppointmentTypes()`, `getClients()`
- `getPublicBranches()`, `getPublicAppointmentTypes()`
- `getDocumentTypes()`, `getProjectTypes()`, `getPropertyTypes()`
- `createClient()`, `validatePublicClient()`
- `requestNewAccountPublic()`

### permissionService
- `getAllPermissions()`
- `getRolPermissionsSummary()`, `getRolFormPermissions()`
- `assignPermissionToRol()`, `removePermissionFromRol()`

---

## Prioritized Migration List

### High Impact Files (Repositories)

These files would benefit most from migration:

1. `src/features/admin/repositories/admin.repository.ts`
   - Uses: appointmentService, userService, catalogService, permissionService
   - Estimated effort: 30 minutes

2. `src/features/appointment-consultation/viewmodels/useAppointmentConsultation.ts`
   - Uses: appointmentService
   - Estimated effort: 10 minutes

3. `src/features/cuentas-nuevas/repositories/cuentas-nuevas.repository.ts`
   - Uses: catalogService
   - Estimated effort: 10 minutes

4. `src/features/proyecto-nuevo/repositories/proyecto-nuevo.repository.ts`
   - Uses: catalogService
   - Estimated effort: 10 minutes

5. `src/features/verificar-cita/repositories/verificar-cita.repository.ts`
   - Uses: appointmentService
   - Estimated effort: 10 minutes

**Total Estimated Effort:** 1-2 hours

### Medium Impact Files (ViewModels)

These files could be updated when next modified:

- `src/features/admin/viewmodels/useAdminUI.ts`
- Other feature viewmodels

### Low Impact Files (Views)

These can be updated gradually:

- `src/app/gestion-citas/page.tsx`
- `src/features/admin/views/*.tsx`
- Other view components

---

## Testing After Migration

After migrating imports, verify:

1. **No TypeScript Errors:**
   ```bash
   npx tsc --noEmit
   ```

2. **Application Runs:**
   ```bash
   npm run dev
   ```

3. **Functionality Works:**
   - Test the features you modified
   - Verify API calls work correctly
   - Check error handling

---

## Automation Opportunities

### ESLint Rule (Future)

Could create a custom ESLint rule to encourage domain-specific services:

```javascript
// .eslintrc.js (example)
{
  rules: {
    'no-restricted-imports': [
      'warn',
      {
        paths: [
          {
            name: '@/services/api',
            message: 'Use domain-specific services from @/services instead of apiService'
          }
        ]
      }
    ]
  }
}
```

### Code Mod Script (Future)

Could create a codemod to automatically migrate imports:

```javascript
// migrate-imports.js (example)
// Script to automatically update apiService to domain-specific services
```

---

## Summary

### Current State
- All imports are functional and correct
- Backward compatibility maintained
- No broken imports

### Optimization Opportunities
- 1 component import could use index export
- 20+ files could use domain-specific services
- All optimizations are optional

### Recommended Approach
- **Don't rush:** Migrate gradually as you modify files
- **No breaking changes:** Both patterns work fine
- **Focus on new code:** Use new patterns for new features
- **Update when convenient:** Migrate existing code when you touch it

### Priority
**LOW** - These are nice-to-have improvements, not critical fixes

---

## Conclusion

The codebase is in good shape. The identified optimizations would improve consistency and provide better developer experience, but are not required for functionality. Recommend implementing these changes gradually as part of normal development workflow.

---

**Document Created:** 2025-11-14
**Type:** Optional Optimizations
**Priority:** LOW
**Status:** For Reference
