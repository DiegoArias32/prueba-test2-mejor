# Error Handling Quick Reference

## Problem Fixed
Holiday validation errors (like Christmas - Dec 25) were showing as console errors, cluttering the developer console with expected business logic validations.

## Solution
Implemented smart error classification that distinguishes between:
- **Expected validation errors** (holidays, Sundays, past dates) - No console spam
- **Unexpected errors** (API failures, network issues) - Logged for debugging

## What Changed

### Before
```
Console:
❌ Error al cargar horas: Error: HOLIDAY_NOT_AVAILABLE|No se puede agendar porque es Navidad
   at loadAvailableHours (useAppointmentScheduling.ts:192)
   ... [full stack trace]
```

### After
```
Console:
✅ (silent - no error logged)

UI:
🎉 No se puede agendar porque es Navidad
   Por favor seleccione otra fecha
```

## Modified Files

### 1. errorParser.ts
Added classification of expected validation errors:
```typescript
export const EXPECTED_VALIDATION_CODES = [
  'HOLIDAY_NOT_AVAILABLE',
  'SUNDAY_NOT_AVAILABLE',
  'PAST_DATE_NOT_AVAILABLE',
  'NO_HOURS_AVAILABLE',
  'DUPLICATE_APPOINTMENT',
  'CLIENT_NOT_FOUND',
  'OUTSIDE_BUSINESS_HOURS'
];
```

### 2. useAppointmentScheduling.ts
Smart logging that only shows unexpected errors in development:
```typescript
if (process.env.NODE_ENV === 'development' && !parsed.isExpectedValidation) {
  console.warn('[AvailableTimes] Error inesperado:', {
    code: parsed.code,
    message: parsed.message,
    details: err.response?.data || err.message
  });
}
```

## Developer Benefits

1. **Clean Console**: No spam from business validations
2. **Dev Mode Only**: Logs only appear in development
3. **Structured Errors**: Clear format with code, message, and details
4. **Context Tags**: `[AvailableTimes]` or `[ScheduleAppointment]` prefixes
5. **Production Clean**: Zero console output in production builds

## Adding New Validation Errors

To add a new expected validation error code:

1. Open `errorParser.ts`
2. Add the code to `EXPECTED_VALIDATION_CODES`:
```typescript
export const EXPECTED_VALIDATION_CODES = [
  'HOLIDAY_NOT_AVAILABLE',
  'SUNDAY_NOT_AVAILABLE',
  // ... existing codes
  'YOUR_NEW_CODE_HERE',  // ← Add here
] as const;
```

That's it! The error will automatically:
- Not log to console
- Display user-friendly message in UI
- Be treated as expected validation

## Testing

Test these scenarios to verify it works:

| Scenario | Expected Console | Expected UI |
|----------|-----------------|-------------|
| Select Dec 25 | Silent | 🎉 Holiday message |
| Select Sunday | Silent | 📅 Sunday message |
| Select yesterday | Silent | ⏰ Past date message |
| Network error (dev) | ⚠️ Warning with details | Error message |
| Network error (prod) | Silent | Error message |

## File Paths

- **Error Parser**: `pqr-scheduling-appointments-portal/src/features/appointments/utils/errorParser.ts`
- **ViewModel**: `pqr-scheduling-appointments-portal/src/features/appointments/viewmodels/useAppointmentScheduling.ts`
- **Documentation**: `IMPROVED_ERROR_HANDLING_SUMMARY.md`

## Key Takeaway

**Expected business validations are not errors** - they're normal user flow. The console should only show unexpected technical issues.
