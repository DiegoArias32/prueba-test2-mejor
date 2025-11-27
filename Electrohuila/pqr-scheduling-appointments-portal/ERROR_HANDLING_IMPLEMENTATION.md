# Frontend Error Handling Implementation

## Overview

This document describes the implementation of improved error handling for appointment scheduling, specifically for holiday, Sunday, and past date validations.

## Changes Made

### 1. Error Parser Utility
**File:** `src/features/appointments/utils/errorParser.ts`

A comprehensive utility module for parsing and displaying backend error messages.

**Key Functions:**

- `parseErrorMessage(errorMessage)`: Parses error messages in format `CODE|User message`
  - Returns: `{ code: string; message: string }`
  - Handles null, undefined, and malformed inputs gracefully

- `getErrorHint(errorCode)`: Returns contextual hints based on error code
  - `SUNDAY_NOT_AVAILABLE`: "Por favor seleccione un día entre lunes y sábado"
  - `HOLIDAY_NOT_AVAILABLE`: "Por favor seleccione otra fecha"
  - `PAST_DATE_NOT_AVAILABLE`: "Seleccione una fecha desde hoy en adelante"

- `getErrorIcon(errorCode)`: Returns emoji icons for visual identification
  - `SUNDAY_NOT_AVAILABLE`: 📅
  - `HOLIDAY_NOT_AVAILABLE`: 🎉
  - `PAST_DATE_NOT_AVAILABLE`: ⏰
  - Default: ⚠️

- `getErrorColorClass(errorCode)`: Returns Tailwind color classes
  - Sunday: `text-amber-600`
  - Holiday: `text-red-600`
  - Past Date: `text-gray-600`

- `getErrorBgClass(errorCode)`: Returns Tailwind background classes
  - Sunday: `bg-amber-50`
  - Holiday: `bg-red-50`
  - Past Date: `bg-gray-50`

- `getErrorBorderClass(errorCode)`: Returns Tailwind border classes
  - Sunday: `border-amber-200`
  - Holiday: `border-red-200`
  - Past Date: `border-gray-200`

### 2. Updated Hook: useAppointmentScheduling
**File:** `src/features/appointments/viewmodels/useAppointmentScheduling.ts`

**Changes:**
- Added `parsedError` state to store parsed error information
- Updated `loadAvailableHours()` function to:
  - Clear parsed error on successful load
  - Parse error messages from backend response
  - Store both error code and user message
- Updated `scheduleAppointment()` function to:
  - Parse backend error messages
  - Store parsed error information
  - Return user-friendly message instead of raw HTTP errors
- Updated `resetFlow()` to clear parsed error state
- Exposed `parsedError` in hook return object

**Error Handling Flow:**
```typescript
try {
  // ... fetch or post operation
} catch (err: any) {
  const errorMsg = err.response?.data?.error || err.message;
  const parsed = parseErrorMessage(errorMsg);
  setParsedError(parsed);
  setError(parsed.message);
}
```

### 3. Updated Component: AppointmentFormStep
**File:** `src/features/appointments/views/steps/AppointmentFormStep.tsx`

**Changes:**
- Added `parsedError` prop to component interface
- Updated time selection display to:
  - Show error icon (emoji) based on error code
  - Display user-friendly error message
  - Apply appropriate colors/styling based on error type
  - Show contextual hint for each error type
  - Maintain original styling when no errors occur

**Time Slot Display Logic:**
```tsx
{availableHours.length === 0 ? (
  <motion.div className={`${getErrorBgClass(parsedError?.code)} ${getErrorBorderClass(parsedError?.code)}`}>
    <div className="text-2xl">{getErrorIcon(parsedError?.code)}</div>
    <p className={getErrorColorClass(parsedError?.code)}>
      {parsedError?.message}
    </p>
    {parsedError && (
      <p>{getErrorHint(parsedError.code)}</p>
    )}
  </motion.div>
) : (
  // Show time buttons
)}
```

### 4. Updated View: AppointmentSchedulingView
**File:** `src/features/appointments/views/AppointmentSchedulingView.tsx`

**Changes:**
- Extracted `parsedError` from hook
- Passed `parsedError` prop to `AppointmentFormStep` component

## User Experience Flow

### Scenario 1: User Selects Sunday
1. Backend returns: `SUNDAY_NOT_AVAILABLE|Los domingos no se atienden citas`
2. Error is parsed:
   - Code: `SUNDAY_NOT_AVAILABLE`
   - Message: `Los domingos no se atienden citas`
3. UI displays:
   - Icon: 📅 (calendar emoji)
   - Color: Amber (warning)
   - Message: "Los domingos no se atienden citas"
   - Hint: "Por favor seleccione un día entre lunes y sábado"

### Scenario 2: User Selects Holiday (e.g., Christmas)
1. Backend returns: `HOLIDAY_NOT_AVAILABLE|No se puede agendar porque es Navidad`
2. Error is parsed:
   - Code: `HOLIDAY_NOT_AVAILABLE`
   - Message: `No se puede agendar porque es Navidad`
3. UI displays:
   - Icon: 🎉 (celebration emoji)
   - Color: Red (error)
   - Message: "No se puede agendar porque es Navidad"
   - Hint: "Por favor seleccione otra fecha"

### Scenario 3: User Selects Past Date
1. Backend returns: `PAST_DATE_NOT_AVAILABLE|No se pueden agendar citas en fechas pasadas`
2. Error is parsed:
   - Code: `PAST_DATE_NOT_AVAILABLE`
   - Message: `No se pueden agendar citas en fechas pasadas`
3. UI displays:
   - Icon: ⏰ (clock emoji)
   - Color: Gray (neutral)
   - Message: "No se pueden agendar citas en fechas pasadas"
   - Hint: "Seleccione una fecha desde hoy en adelante"

### Scenario 4: Appointment Submission Error
1. User tries to schedule appointment on restricted date
2. Backend returns error code with message
3. Toast notification displays user-friendly message
4. NOT the raw HTTP error, but the parsed message

## Testing

Unit tests are provided in `src/features/appointments/utils/errorParser.test.ts`

**Test Coverage:**
- Error message parsing with various formats
- Null/undefined handling
- Icon, hint, and color class generation for each error type
- Fallback behaviors

**Run tests:**
```bash
npm test -- errorParser.test.ts
```

## Backend Requirements

The backend must return errors in the format:
```
CODE|User message
```

**Supported Error Codes:**
- `SUNDAY_NOT_AVAILABLE`
- `HOLIDAY_NOT_AVAILABLE`
- `PAST_DATE_NOT_AVAILABLE`
- Custom error codes (will display message with default styling)

**Example Backend Response:**
```json
{
  "error": "SUNDAY_NOT_AVAILABLE|Los domingos no se atienden citas",
  "statusCode": 400
}
```

## Styling Details

All styling uses Tailwind CSS classes for:
- Text colors (text-amber-600, text-red-600, text-gray-600)
- Background colors (bg-amber-50, bg-red-50, bg-gray-50)
- Border colors (border-amber-200, border-red-200, border-gray-200)
- Responsive design maintained
- Dark mode compatible (when Tailwind dark mode is configured)

## Performance Considerations

- Error parsing is synchronous and minimal overhead
- No additional API calls for error handling
- Error state is managed at hook level, avoiding prop drilling
- Memoization of error parser functions if needed for future optimization

## Accessibility

- Error messages are clearly visible with appropriate contrast ratios
- Icons are supplementary (text also explains the issue)
- Color alone is not used to convey meaning (icon + text + color)
- Animations respect `prefers-reduced-motion`

## Future Enhancements

1. **Localization**: Translate error messages and hints to multiple languages
2. **Analytics**: Track which error types occur most frequently
3. **Retry Logic**: Add automatic retry for transient errors
4. **Toast Notifications**: Integrate with toast library for submission errors
5. **Error Recovery**: Suggest alternate dates for Saturday selection
6. **Error Logging**: Send errors to monitoring service

## Files Modified Summary

| File | Changes |
|------|---------|
| `src/features/appointments/utils/errorParser.ts` | Created - Error parsing utility |
| `src/features/appointments/utils/errorParser.test.ts` | Created - Unit tests |
| `src/features/appointments/viewmodels/useAppointmentScheduling.ts` | Updated - Added error parsing logic |
| `src/features/appointments/views/steps/AppointmentFormStep.tsx` | Updated - Display custom error messages |
| `src/features/appointments/views/AppointmentSchedulingView.tsx` | Updated - Pass parsedError prop |

## Migration Guide

If you have existing error handling code:

1. Import error parser utility:
```typescript
import { parseErrorMessage, getErrorIcon, getErrorHint } from '../utils/errorParser';
```

2. Parse errors:
```typescript
const parsed = parseErrorMessage(errorMsg);
setError(parsed.message);
setParsedError(parsed);
```

3. Display error:
```tsx
{parsedError && (
  <div className={getErrorBgClass(parsedError.code)}>
    <span>{getErrorIcon(parsedError.code)}</span>
    <p className={getErrorColorClass(parsedError.code)}>
      {parsedError.message}
    </p>
    {getErrorHint(parsedError.code) && (
      <p>{getErrorHint(parsedError.code)}</p>
    )}
  </div>
)}
```

## Troubleshooting

**Error message not parsing correctly:**
- Verify backend sends format: `CODE|Message`
- Check for extra whitespace
- Ensure error is in response.data.error or error.message

**Icons not displaying:**
- Ensure terminal/browser supports emoji
- Check if emoji support is available in your environment
- Use fallback text if needed

**Colors not applying:**
- Verify Tailwind CSS is properly configured
- Check if dark mode is interfering
- Ensure custom Tailwind config includes all color variants

## Support

For issues or questions about error handling implementation:
1. Check unit tests for usage examples
2. Review component props in AppointmentFormStep
3. Verify backend error format matches specification
4. Check browser console for detailed error information
