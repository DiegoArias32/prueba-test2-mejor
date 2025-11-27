# Fix: Appointment Types Deactivation and Inactive Tab Issues

**Date**: 2025-11-26
**Status**: FIXED

## Issues Identified

### Issue 1: Error Shown Despite Successful Deactivation
**Symptom**: Clicking "Desactivar" shows error "No se pudo desactivar el tipo de cita", but the item is actually deactivated in the database.

**Root Cause**:
1. Backend endpoint `PATCH /api/v1/appointmenttypes/delete-logical/{id}` returns `Ok()` with empty body (HTTP 200, no JSON)
2. Frontend `base-http.service.ts` line 71 tries to parse the empty response as JSON
3. JSON parsing fails, throwing an error
4. Error handler catches it and shows error message to user
5. **BUT the database update was successful before the response was sent**

### Issue 2: Inactivos Tab is Empty
**Symptom**: After deactivating an item, it disappears from "Activos" tab but doesn't show up in "Inactivos" tab.

**Root Cause**:
1. `useAppointmentTypes` hook's `fetchAppointmentTypes()` always fetched only active types
2. When tab switches to "Inactivos", the component tries to filter inactive items from an array that only contains active items
3. Result: empty list

## Solutions Implemented

### Fix 1: Handle Empty HTTP Responses

**File**: `pqr-scheduling-appointments-portal/src/services/base/base-http.service.ts`

**Changes**:
```typescript
// Added after response.ok check in both request() and publicRequest() methods:

// Handle empty responses (e.g., 200 OK with no body from logical delete operations)
const contentType = response.headers.get('content-type');
const contentLength = response.headers.get('content-length');

// If no content, return success object
if (contentLength === '0' || !contentType?.includes('application/json')) {
  return { success: true, message: 'Operation completed successfully' } as T;
}

return await response.json();
```

**Impact**:
- Now properly handles HTTP 200 responses with empty bodies
- Returns a success object instead of attempting to parse JSON
- Works for all logical delete operations across the system

### Fix 2: Always Fetch All Appointment Types

**File**: `pqr-scheduling-appointments-portal/src/features/admin/viewmodels/useAppointmentTypes.ts`

**Changes**:

1. **fetchAppointmentTypes**: Always fetch all types (active + inactive)
```typescript
const fetchAppointmentTypes = useCallback(async (activeOnly = true) => {
  setLoading(true);
  setError(null);
  try {
    // Always fetch all appointment types (including inactive) so we can display both tabs
    const data = await repository.getAllAppointmentTypesIncludingInactive();
    // Store all types - TypesView will filter them based on currentView
    setAppointmentTypes(data);
  } catch (err: unknown) {
    setError(err instanceof Error ? err.message : 'Error loading appointment types');
  } finally {
    setLoading(false);
  }
}, [repository]);
```

2. **deleteAppointmentType**: Update to mark as inactive instead of removing
```typescript
const deleteAppointmentType = useCallback(async (id: number, logical = true) => {
  setLoading(true);
  setError(null);
  try {
    if (logical) {
      await repository.deleteLogicalAppointmentType(id);
      // Update local state to mark as inactive instead of removing
      setAppointmentTypes(prev => prev.map(t =>
        t.id === id ? { ...t, isActive: false } : t
      ));
    } else {
      await repository.deleteAppointmentType(id);
      // Physical delete - remove from list
      setAppointmentTypes(prev => prev.filter(t => t.id !== id));
    }
  } catch (err: unknown) {
    setError(err instanceof Error ? err.message : 'Error deleting appointment type');
    throw err;
  } finally {
    setLoading(false);
  }
}, [repository]);
```

3. **activateAppointmentType**: Update to mark as active instead of removing
```typescript
const activateAppointmentType = useCallback(async (id: number) => {
  setLoading(true);
  setError(null);
  try {
    await repository.activateAppointmentType(id);
    // Update local state to mark as active instead of removing
    setAppointmentTypes(prev => prev.map(t =>
      t.id === id ? { ...t, isActive: true } : t
    ));
  } catch (err: unknown) {
    setError(err instanceof Error ? err.message : 'Error activating appointment type');
    throw err;
  } finally {
    setLoading(false);
  }
}, [repository]);
```

**Impact**:
- TypesView receives all appointment types (both active and inactive)
- TypesView filters them based on current tab (lines 45-46)
- Deactivated items now appear in "Inactivos" tab
- Activated items move back to "Activos" tab
- No page reload needed - instant UI update

## Testing Checklist

### Test Case 1: Deactivate Appointment Type
- [ ] Navigate to Admin Panel → Tipos de Cita
- [ ] Click "Desactivar" on an active type
- [ ] Verify: Success message (no error)
- [ ] Verify: Item disappears from "Activos" tab
- [ ] Switch to "Inactivos" tab
- [ ] Verify: Item appears in "Inactivos" tab with correct data

### Test Case 2: Activate Appointment Type
- [ ] Navigate to "Inactivos" tab
- [ ] Click "Activar" on an inactive type
- [ ] Verify: Success message
- [ ] Verify: Item disappears from "Inactivos" tab
- [ ] Switch to "Activos" tab
- [ ] Verify: Item appears in "Activos" tab

### Test Case 3: Data Persistence
- [ ] Deactivate a type
- [ ] Refresh the page
- [ ] Verify: Type still shows in "Inactivos" tab
- [ ] Verify: Type does not show in "Activos" tab

## Technical Details

### Backend Endpoints Used
- `GET /api/v1/appointmenttypes/all-including-inactive` - Returns all types
- `PATCH /api/v1/appointmenttypes/delete-logical/{id}` - Deactivates a type
- `PATCH /api/v1/appointmenttypes/{id}` (with isActive: true) - Activates a type

### Backend Response Format
- Success (logical delete): HTTP 200 OK with empty body
- Success (get all): HTTP 200 OK with JSON array
- Error: HTTP 400 BadRequest with `{ error: "message" }`

### Frontend Component Flow
1. **AdminLayout** loads appointment types on mount
2. **useAppointmentTypes** hook fetches all types (active + inactive)
3. **TypesView** receives all types and filters based on current tab
4. User clicks "Desactivar" → calls `onDeactivateType(id)`
5. Hook updates backend, then updates local state (isActive = false)
6. TypesView re-filters and shows updated data
7. Tab switch works because all data is already loaded

## Files Modified

1. `pqr-scheduling-appointments-portal/src/services/base/base-http.service.ts`
   - Added empty response handling

2. `pqr-scheduling-appointments-portal/src/features/admin/viewmodels/useAppointmentTypes.ts`
   - Modified `fetchAppointmentTypes` to always fetch all types
   - Modified `deleteAppointmentType` to update isActive flag
   - Modified `activateAppointmentType` to update isActive flag

## Related Issues
- This pattern should be applied to other similar features:
  - Branches (Sedes)
  - Users (Empleados)
  - Roles
  - Available Times (Horas Disponibles)

All these features have the same Activos/Inactivos tab pattern and should follow the same approach.
