# TODO: Apply Same Fix to Other Features with Activos/Inactivos Tabs

## Overview
The AppointmentTypes feature had two bugs:
1. Error shown on successful deactivation (due to empty HTTP response)
2. Inactivos tab was empty (due to incorrect data fetching/filtering)

These same bugs exist in other features that have Activos/Inactivos tabs.

## Fixed: AppointmentTypes ✅
**Status**: COMPLETE
**Files Modified**:
- `base-http.service.ts` - Handle empty responses
- `useAppointmentTypes.ts` - Fetch all, update state correctly

## Pending Fixes

### 1. Available Times (Horas Disponibles)
**File**: `pqr-scheduling-appointments-portal/src/features/admin/viewmodels/useAvailableTimes.ts`

**Current Issues**:
```typescript
// Line 127: Removes from list instead of marking inactive
setAvailableTimes(prev => prev.filter(t => t.id !== id));

// Line 141: Removes from list instead of marking active
setAvailableTimes(prev => prev.filter(t => t.id !== id));
```

**Required Changes**:
1. Modify `fetchAvailableTimes()` to always fetch all times (active + inactive)
2. Update `deleteAvailableTime()` to mark as inactive: `isActive: false`
3. Update `activateAvailableTime()` to mark as active: `isActive: true`

**Pattern to Follow**: Same as AppointmentTypes fix

---

### 2. Branches (Sedes)
**File**: `pqr-scheduling-appointments-portal/src/features/admin/viewmodels/useBranches.ts`

**Current Issues**:
```typescript
// Line 108: Removes from list instead of marking inactive
setBranches(prev => prev.filter(b => b.id !== id));

// Line 122: Removes from list instead of marking active
setBranches(prev => prev.filter(b => b.id !== id));
```

**Required Changes**:
1. Modify `fetchBranches()` to always fetch all branches (active + inactive)
2. Update `deleteBranch()` to mark as inactive: `isActive: false`
3. Update `activateBranch()` to mark as active: `isActive: true`

**Pattern to Follow**: Same as AppointmentTypes fix

---

### 3. Users/Employees (Empleados)
**File**: `pqr-scheduling-appointments-portal/src/features/admin/viewmodels/useUsers.ts`

**Current Issues**:
```typescript
// Line 123: Removes from list instead of marking inactive
setUsers(prev => prev.filter(u => u.id !== id));

// Line 137: Removes from list instead of marking active
setUsers(prev => prev.filter(u => u.id !== id));
```

**Required Changes**:
1. Modify `fetchUsers()` to always fetch all users (active + inactive)
2. Update `deleteUser()` to mark as inactive: `isActive: false`
3. Update `activateUser()` to mark as active: `isActive: true`

**Pattern to Follow**: Same as AppointmentTypes fix

---

### 4. Roles (Roles)
**File**: `pqr-scheduling-appointments-portal/src/features/admin/viewmodels/useRoles.ts`

**Current Issues**:
```typescript
// Line 102: Removes from list instead of marking inactive
setRoles(prev => prev.filter(r => r.id !== id));

// Line 116: Removes from list instead of marking active
setRoles(prev => prev.filter(r => r.id !== id));
```

**Required Changes**:
1. Modify `fetchRoles()` to always fetch all roles (active + inactive)
2. Update `deleteRol()` to mark as inactive: `isActive: false`
3. Update `activateRol()` to mark as active: `isActive: true`

**Pattern to Follow**: Same as AppointmentTypes fix

---

## Common Fix Pattern

For each feature, apply this pattern:

### Step 1: Modify fetch function
```typescript
// BEFORE
const fetchXXX = useCallback(async (activeOnly = true) => {
  const data = activeOnly
    ? await repository.getXXX()
    : await repository.getAllXXXIncludingInactive();
  setXXX(activeOnly ? data.filter(t => t.isActive) : data.filter(t => !t.isActive));
}, [repository]);

// AFTER
const fetchXXX = useCallback(async (activeOnly = true) => {
  // Always fetch all (including inactive) so we can display both tabs
  const data = await repository.getAllXXXIncludingInactive();
  // Store all - View will filter based on currentView
  setXXX(data);
}, [repository]);
```

### Step 2: Modify delete function
```typescript
// BEFORE
const deleteXXX = useCallback(async (id: number, logical = true) => {
  if (logical) {
    await repository.deleteLogicalXXX(id);
  } else {
    await repository.deleteXXX(id);
  }
  setXXX(prev => prev.filter(t => t.id !== id));  // ❌ WRONG
}, [repository]);

// AFTER
const deleteXXX = useCallback(async (id: number, logical = true) => {
  if (logical) {
    await repository.deleteLogicalXXX(id);
    // Update local state to mark as inactive
    setXXX(prev => prev.map(t =>
      t.id === id ? { ...t, isActive: false } : t
    ));
  } else {
    await repository.deleteXXX(id);
    // Physical delete - remove from list
    setXXX(prev => prev.filter(t => t.id !== id));
  }
}, [repository]);
```

### Step 3: Modify activate function
```typescript
// BEFORE
const activateXXX = useCallback(async (id: number) => {
  await repository.activateXXX(id);
  setXXX(prev => prev.filter(t => t.id !== id));  // ❌ WRONG
}, [repository]);

// AFTER
const activateXXX = useCallback(async (id: number) => {
  await repository.activateXXX(id);
  // Update local state to mark as active
  setXXX(prev => prev.map(t =>
    t.id === id ? { ...t, isActive: true } : t
  ));
}, [repository]);
```

## Testing Checklist

For each feature after applying the fix:
- [ ] Deactivate item → appears in "Inactivos" tab
- [ ] Activate item → appears in "Activos" tab
- [ ] No error messages on successful operations
- [ ] Page refresh maintains correct state
- [ ] Multiple operations work without refresh

## Backend Requirements

Ensure each feature has these endpoints:
- `GET /api/v1/XXX/all-including-inactive` - Returns all items
- `PATCH /api/v1/XXX/delete-logical/{id}` - Deactivates item
- `PATCH /api/v1/XXX/{id}` (with isActive: true) - Activates item

## Priority

1. **High Priority** (User-facing, frequently used):
   - Branches (Sedes)
   - Users (Empleados)

2. **Medium Priority**:
   - Available Times (Horas Disponibles)
   - Roles

## Notes

The base-http.service.ts fix (handling empty responses) is already done and will apply to ALL features automatically. The individual viewmodel fixes need to be applied per feature.

## Estimated Effort

- Per feature: 15-30 minutes
- Total for all 4 remaining: 1-2 hours
- Testing all features: 1 hour
- **Total estimated time**: 2-3 hours
