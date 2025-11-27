# Error Handling - Code Examples

## Overview
This document shows practical examples of the improved error handling implementation.

---

## Example 1: Holiday Error (HOLIDAY_NOT_AVAILABLE)

### User Action
User selects December 25, 2025 (Christmas)

### API Response
```json
{
  "status": 400,
  "error": "HOLIDAY_NOT_AVAILABLE|No se puede agendar porque es Navidad"
}
```

### Code Flow

#### Step 1: Error is thrown
```typescript
// In appointment.repository.ts
const response = await api.get('/available-times?date=2025-12-25&branchId=1');
// Throws error with response.data.error = "HOLIDAY_NOT_AVAILABLE|No se puede agendar porque es Navidad"
```

#### Step 2: Error is caught and parsed
```typescript
// In useAppointmentScheduling.ts - loadAvailableHours()
catch (err: any) {
  const errorMsg = err.response?.data?.error;
  // = "HOLIDAY_NOT_AVAILABLE|No se puede agendar porque es Navidad"

  const parsed = parseErrorMessage(errorMsg);
  // Returns:
  // {
  //   code: 'HOLIDAY_NOT_AVAILABLE',
  //   message: 'No se puede agendar porque es Navidad',
  //   isExpectedValidation: true  ← Automatically set!
  // }
```

#### Step 3: Smart logging decision
```typescript
  // Check if we should log
  if (process.env.NODE_ENV === 'development' && !parsed.isExpectedValidation) {
    // This block is SKIPPED because:
    // - parsed.isExpectedValidation = true
    // - No console output
  }

  // Set the error for UI display
  setParsedError(parsed);  // UI will show the message
  setAvailableHours([]);   // Clear available hours
}
```

#### Step 4: UI displays friendly message
```typescript
// In AppointmentForm component
{parsedError && (
  <div className="bg-red-50 border-red-200 text-red-600">
    🎉 {parsedError.message}
    <div>Por favor seleccione otra fecha</div>
  </div>
)}
```

### Result
- **Console**: ✅ Silent (no error logged)
- **UI**: 🎉 No se puede agendar porque es Navidad
- **UX**: User sees clear message, no developer clutter

---

## Example 2: Network Error (Unexpected)

### User Action
User has poor internet connection

### API Response
```
Network Error: timeout of 5000ms exceeded
```

### Code Flow

#### Step 1: Error is thrown
```typescript
const response = await api.get('/available-times?date=2025-12-26&branchId=1');
// Throws network error
```

#### Step 2: Error is caught and parsed
```typescript
catch (err: any) {
  const errorMsg = err.message;
  // = "timeout of 5000ms exceeded"

  const parsed = parseErrorMessage(errorMsg);
  // Returns:
  // {
  //   code: 'UNKNOWN_ERROR',
  //   message: 'timeout of 5000ms exceeded',
  //   isExpectedValidation: false  ← Not an expected validation!
  // }
```

#### Step 3: Smart logging decision
```typescript
  // Check if we should log
  if (process.env.NODE_ENV === 'development' && !parsed.isExpectedValidation) {
    // This block EXECUTES because:
    // - We're in development mode
    // - parsed.isExpectedValidation = false

    console.warn('[AvailableTimes] Error inesperado:', {
      code: 'UNKNOWN_ERROR',
      message: 'timeout of 5000ms exceeded',
      details: {...}
    });
    // ⚠️ Console warning appears for developer
  }

  setParsedError(parsed);
  setAvailableHours([]);
}
```

### Result
- **Console (Dev)**: ⚠️ Warning with structured error details
- **Console (Prod)**: ✅ Silent (no logging in production)
- **UI**: Error message shown to user
- **UX**: Developer can debug, user sees friendly error

---

## Example 3: Multiple Error Types

### Scenario: User tries multiple dates

```typescript
// User selects Sunday
API: "SUNDAY_NOT_AVAILABLE|No se puede agendar los domingos"
Console: Silent ✅
UI: 📅 No se puede agendar los domingos

// User selects yesterday
API: "PAST_DATE_NOT_AVAILABLE|La fecha no puede ser anterior a hoy"
Console: Silent ✅
UI: ⏰ La fecha no puede ser anterior a hoy

// User selects Christmas
API: "HOLIDAY_NOT_AVAILABLE|No se puede agendar porque es Navidad"
Console: Silent ✅
UI: 🎉 No se puede agendar porque es Navidad

// API server crashes
API: Network Error
Console: ⚠️ [AvailableTimes] Error inesperado: {...} (dev only)
UI: Ocurrió un error inesperado
```

---

## Example 4: Adding a New Validation Error

### Requirement
Add validation for "appointment type not available on selected date"

### Step 1: Backend sends error
```csharp
// In AvailableTimesService.cs
if (!appointmentType.IsAvailableOnDate(date))
{
    throw new ValidationException(
        "APPOINTMENT_TYPE_NOT_AVAILABLE|Este tipo de cita no está disponible en la fecha seleccionada"
    );
}
```

### Step 2: Add to expected codes (Frontend)
```typescript
// In errorParser.ts
export const EXPECTED_VALIDATION_CODES = [
  'HOLIDAY_NOT_AVAILABLE',
  'SUNDAY_NOT_AVAILABLE',
  'PAST_DATE_NOT_AVAILABLE',
  'NO_HOURS_AVAILABLE',
  'DUPLICATE_APPOINTMENT',
  'CLIENT_NOT_FOUND',
  'OUTSIDE_BUSINESS_HOURS',
  'APPOINTMENT_TYPE_NOT_AVAILABLE',  // ← Add here
] as const;
```

### Step 3: Optionally add UI styling
```typescript
// In errorParser.ts - getErrorIcon()
export function getErrorIcon(errorCode: string): string {
  const icons: Record<string, string> = {
    'SUNDAY_NOT_AVAILABLE': '📅',
    'HOLIDAY_NOT_AVAILABLE': '🎉',
    'APPOINTMENT_TYPE_NOT_AVAILABLE': '📋',  // ← Add icon
  };
  return icons[errorCode] || '⚠️';
}
```

### Result
```typescript
// User selects invalid appointment type for date
API: "APPOINTMENT_TYPE_NOT_AVAILABLE|Este tipo de cita no está disponible en la fecha seleccionada"
Console: Silent ✅ (automatically, no code changes needed!)
UI: 📋 Este tipo de cita no está disponible en la fecha seleccionada
```

---

## Example 5: Development vs Production

### Development Mode
```typescript
// User triggers network error
if (process.env.NODE_ENV === 'development' && !parsed.isExpectedValidation) {
  console.warn('[AvailableTimes] Error inesperado:', {
    code: 'UNKNOWN_ERROR',
    message: 'Network timeout',
    details: {...}
  });
}

// Console output:
⚠️ [AvailableTimes] Error inesperado: {
  code: 'UNKNOWN_ERROR',
  message: 'Network timeout',
  details: {
    config: {...},
    request: {...},
    response: undefined
  }
}
```

### Production Mode
```typescript
// Same error in production
if (process.env.NODE_ENV === 'development' && !parsed.isExpectedValidation) {
  // This block doesn't execute because:
  // - process.env.NODE_ENV === 'production'
  // - No console output!
}

// Console output:
(empty)
```

---

## Example 6: parseErrorMessage() Function

### Test Cases

```typescript
// Case 1: Proper format with code
parseErrorMessage("HOLIDAY_NOT_AVAILABLE|No se puede agendar porque es Navidad")
// Returns: {
//   code: 'HOLIDAY_NOT_AVAILABLE',
//   message: 'No se puede agendar porque es Navidad',
//   isExpectedValidation: true
// }

// Case 2: Message with pipe character
parseErrorMessage("CUSTOM_ERROR|Error: Message|with|pipes")
// Returns: {
//   code: 'CUSTOM_ERROR',
//   message: 'Error: Message|with|pipes',  // Joins back the parts
//   isExpectedValidation: false
// }

// Case 3: No code (plain message)
parseErrorMessage("Something went wrong")
// Returns: {
//   code: 'UNKNOWN_ERROR',
//   message: 'Something went wrong',
//   isExpectedValidation: false
// }

// Case 4: Empty/null
parseErrorMessage(null)
// Returns: {
//   code: 'UNKNOWN_ERROR',
//   message: 'Ocurrió un error inesperado',
//   isExpectedValidation: false
// }
```

---

## Example 7: Integration with UI Components

### AppointmentForm Component
```typescript
import { getErrorIcon, getErrorHint, getErrorBgClass, getErrorBorderClass, getErrorColorClass } from '../utils/errorParser';

export const AppointmentForm = () => {
  const { parsedError, availableHours, loadingHours } = useAppointmentScheduling();

  return (
    <div>
      {/* Date selection */}
      <input type="date" {...} />

      {/* Error display */}
      {parsedError && (
        <div className={`p-4 rounded-lg border ${getErrorBgClass(parsedError.code)} ${getErrorBorderClass(parsedError.code)}`}>
          <div className={`flex items-center gap-2 ${getErrorColorClass(parsedError.code)}`}>
            <span className="text-2xl">{getErrorIcon(parsedError.code)}</span>
            <div>
              <p className="font-medium">{parsedError.message}</p>
              {getErrorHint(parsedError.code) && (
                <p className="text-sm mt-1">{getErrorHint(parsedError.code)}</p>
              )}
            </div>
          </div>
        </div>
      )}

      {/* Available hours */}
      {loadingHours ? (
        <div>Cargando horarios...</div>
      ) : availableHours.length > 0 ? (
        <div>
          {availableHours.map(hour => (
            <button key={hour}>{hour}</button>
          ))}
        </div>
      ) : !parsedError && (
        <div>No hay horarios disponibles</div>
      )}
    </div>
  );
};
```

---

## Key Points

1. **Automatic Classification**: Just add error code to `EXPECTED_VALIDATION_CODES` array
2. **Smart Logging**: Only unexpected errors in development mode
3. **User-Friendly**: Always show UI message, never leave user confused
4. **Type-Safe**: TypeScript ensures error codes are valid
5. **Zero Config**: Works in dev and production automatically
6. **Clean Console**: No spam from business logic validations
7. **Easy Debugging**: Structured error format with context tags

---

## File Locations

- **Error Parser**: `src/features/appointments/utils/errorParser.ts`
- **View Model**: `src/features/appointments/viewmodels/useAppointmentScheduling.ts`
- **UI Component**: `src/features/appointments/views/AppointmentForm.tsx`
