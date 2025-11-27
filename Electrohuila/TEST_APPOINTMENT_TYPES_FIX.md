# Test Plan: Appointment Types Deactivation Fix

## Pre-Test Setup
1. Ensure backend is running
2. Ensure frontend is running
3. Login as admin user
4. Navigate to Admin Panel → Tipos de Cita

## Test 1: Deactivate an Appointment Type

### Steps:
1. Note the current count of items in "Activos" tab
2. Select any active appointment type (remember its name)
3. Click "Desactivar" button
4. Confirm in the dialog

### Expected Results:
- ✅ Success message appears (NOT error message)
- ✅ Item immediately disappears from "Activos" tab
- ✅ Count in "Activos" tab decreases by 1

### Steps (continued):
5. Click on "Inactivos" tab

### Expected Results:
- ✅ The deactivated item appears in the "Inactivos" tab
- ✅ Item shows all correct data (name, icon, time, etc.)
- ✅ "Activar" button is visible for the item

## Test 2: Activate an Inactive Appointment Type

### Steps:
1. In "Inactivos" tab, note the count
2. Click "Activar" on any inactive type
3. Confirm in the dialog

### Expected Results:
- ✅ Success message appears
- ✅ Item immediately disappears from "Inactivos" tab
- ✅ Count in "Inactivos" tab decreases by 1

### Steps (continued):
4. Click on "Activos" tab

### Expected Results:
- ✅ The activated item appears in the "Activos" tab
- ✅ Item shows all correct data
- ✅ "Desactivar" button is visible for the item

## Test 3: Data Persistence

### Steps:
1. Deactivate an appointment type
2. Wait for success message
3. Press F5 to hard refresh the page
4. Navigate back to Admin Panel → Tipos de Cita

### Expected Results:
- ✅ "Activos" tab loads correctly
- ✅ Deactivated item is NOT in "Activos" tab
- ✅ Switch to "Inactivos" tab
- ✅ Deactivated item IS in "Inactivos" tab

## Test 4: Multiple Operations

### Steps:
1. Deactivate 3 different appointment types (without page refresh)
2. Switch to "Inactivos" tab
3. Activate 1 of them back
4. Switch to "Activos" tab

### Expected Results:
- ✅ All 3 deactivated items appear in "Inactivos" tab
- ✅ After activating 1, only 2 remain in "Inactivos" tab
- ✅ The activated item appears in "Activos" tab
- ✅ No errors shown during any operation

## Test 5: Network/Console Inspection

### Steps:
1. Open browser DevTools (F12)
2. Go to Network tab
3. Deactivate an appointment type
4. Check the network request

### Expected Results:
- ✅ Request: `PATCH /api/v1/appointmenttypes/delete-logical/{id}`
- ✅ Response: HTTP 200 OK
- ✅ Response body: empty or minimal JSON
- ✅ No console errors

### Steps (continued):
5. Go to Console tab
6. Deactivate another type

### Expected Results:
- ✅ No JavaScript errors in console
- ✅ No warnings about JSON parsing

## Test 6: Edge Cases

### Test 6a: Deactivate Last Active Item
1. Deactivate all appointment types except one
2. Deactivate the last one
3. Check "Activos" tab

**Expected**: "Activos" tab shows "No hay datos disponibles" or similar empty state

### Test 6b: Activate Last Inactive Item
1. Have only one inactive item
2. Activate it
3. Check "Inactivos" tab

**Expected**: "Inactivos" tab shows empty state

## Common Issues to Watch For

### ❌ Issue 1: Error Message on Success
**Symptom**: "No se pudo desactivar el tipo de cita" appears even though operation succeeds
**Status**: SHOULD BE FIXED

### ❌ Issue 2: Empty Inactivos Tab
**Symptom**: Deactivated items don't appear in "Inactivos" tab
**Status**: SHOULD BE FIXED

### ❌ Issue 3: Item Removed Instead of Moved
**Symptom**: Item disappears completely instead of moving to other tab
**Status**: SHOULD BE FIXED

## Regression Tests

### Verify Other Features Still Work:
- ✅ Create new appointment type
- ✅ Edit existing appointment type
- ✅ Search/filter appointment types
- ✅ Sort appointment types
- ✅ Export to CSV
- ✅ Pagination

## Sign-Off

| Test | Status | Notes |
|------|--------|-------|
| Test 1 - Deactivate | ⬜ | |
| Test 2 - Activate | ⬜ | |
| Test 3 - Persistence | ⬜ | |
| Test 4 - Multiple Ops | ⬜ | |
| Test 5 - Network | ⬜ | |
| Test 6 - Edge Cases | ⬜ | |
| Regression Tests | ⬜ | |

**Tester**: ________________
**Date**: ________________
**Overall Status**: ⬜ PASS / ⬜ FAIL
**Notes**: ________________________________________________________________
