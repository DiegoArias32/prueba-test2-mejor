# Detailed Code Changes

## File 1: Error Parser Utility (NEW)
**Location:** `src/features/appointments/utils/errorParser.ts`

**Status:** CREATED - 98 lines

### Purpose
Provides utility functions for parsing backend error messages and generating appropriate UI elements (colors, icons, hints).

### Key Exports
```typescript
export interface ParsedError {
  code: string;
  message: string;
}

export function parseErrorMessage(errorMessage: string | null | undefined): ParsedError
export function getErrorHint(errorCode: string): string | null
export function getErrorIcon(errorCode: string): string
export function getErrorColorClass(errorCode: string): string
export function getErrorBgClass(errorCode: string): string
export function getErrorBorderClass(errorCode: string): string
```

### Implementation Details
- Safely handles null/undefined/empty inputs
- Parses error format: `CODE|Message`
- Provides fallback values for unknown error codes
- All color classes are Tailwind CSS compatible
- Emoji icons for visual identification

### Error Code Support
| Code | Message | Icon | Color | Background |
|------|---------|------|-------|-----------|
| SUNDAY_NOT_AVAILABLE | Los domingos no se atienden citas | 📅 | text-amber-600 | bg-amber-50 |
| HOLIDAY_NOT_AVAILABLE | No se puede agendar porque es [Holiday] | 🎉 | text-red-600 | bg-red-50 |
| PAST_DATE_NOT_AVAILABLE | No se pueden agendar citas en fechas pasadas | ⏰ | text-gray-600 | bg-gray-50 |
| Unknown | Custom message | ⚠️ | text-gray-600 | bg-gray-50 |

---

## File 2: Error Parser Tests (NEW)
**Location:** `src/features/appointments/utils/errorParser.test.ts`

**Status:** CREATED - 181 lines

### Test Coverage
- 38 test cases covering all functions
- Edge case handling (null, undefined, empty string)
- Multiple pipe separator handling
- All error codes and their properties

### Test Examples
```typescript
it('should parse error message with pipe separator', () => {
  const result = parseErrorMessage('SUNDAY_NOT_AVAILABLE|Los domingos no se atienden citas');
  expect(result.code).toBe('SUNDAY_NOT_AVAILABLE');
  expect(result.message).toBe('Los domingos no se atienden citas');
});

it('should handle null or undefined input', () => {
  const resultNull = parseErrorMessage(null);
  expect(resultNull.code).toBe('UNKNOWN_ERROR');
  expect(resultNull.message).toBe('Ocurrió un error inesperado');
});

it('should return Sunday hint', () => {
  const hint = getErrorHint('SUNDAY_NOT_AVAILABLE');
  expect(hint).toBe('Por favor seleccione un día entre lunes y sábado');
});
```

---

## File 3: Hook Update - useAppointmentScheduling
**Location:** `src/features/appointments/viewmodels/useAppointmentScheduling.ts`

**Status:** MODIFIED - 410 lines

### Changes Summary

#### 1. Import Addition (Lines 8-9)
**Before:**
```typescript
import { appointmentRepository } from '../repositories/appointment.repository';
```

**After:**
```typescript
import { appointmentRepository } from '../repositories/appointment.repository';
import { parseErrorMessage, type ParsedError } from '../utils/errorParser';
```

#### 2. New State Declaration (Line 24)
**Added:**
```typescript
const [parsedError, setParsedError] = useState<ParsedError | null>(null);
```

#### 3. Updated loadAvailableHours() Function (Lines 163-202)

**Before:**
```typescript
const loadAvailableHours = useCallback(async () => {
  if (!formData.appointmentDate || !formData.branch) return;

  setLoadingHours(true);
  try {
    const selectedBranch = branches.find((s) => s.name === formData.branch);
    if (!selectedBranch) {
      setAvailableHours([]);
      return;
    }

    const horas = await appointmentRepository.getPublicAvailableTimes(
      formData.appointmentDate,
      selectedBranch.id
    );

    setAvailableHours(horas || []);

    if (formData.appointmentTime && !horas.includes(formData.appointmentTime)) {
      setFormData((prev) => ({
        ...prev,
        appointmentTime: '',
      }));
    }
  } catch (err) {
    console.error('Error al cargar horas:', err);
    setAvailableHours([]);
  } finally {
    setLoadingHours(false);
  }
}, [formData.appointmentDate, formData.branch, formData.appointmentTime, branches]);
```

**After:**
```typescript
const loadAvailableHours = useCallback(async () => {
  if (!formData.appointmentDate || !formData.branch) return;

  setLoadingHours(true);
  setParsedError(null);  // Clear error on start
  try {
    const selectedBranch = branches.find((s) => s.name === formData.branch);
    if (!selectedBranch) {
      setAvailableHours([]);
      setParsedError(null);
      return;
    }

    const horas = await appointmentRepository.getPublicAvailableTimes(
      formData.appointmentDate,
      selectedBranch.id
    );

    setAvailableHours(horas || []);
    setParsedError(null);  // Clear error on success

    if (formData.appointmentTime && !horas.includes(formData.appointmentTime)) {
      setFormData((prev) => ({
        ...prev,
        appointmentTime: '',
      }));
    }
  } catch (err: any) {  // Type annotation added
    console.error('Error al cargar horas:', err);

    // NEW: Parse error message from backend
    const errorMsg = err.response?.data?.error || err.message || 'Error al cargar horarios disponibles';
    const parsed = parseErrorMessage(errorMsg);
    setParsedError(parsed);
    setAvailableHours([]);
  } finally {
    setLoadingHours(false);
  }
}, [formData.appointmentDate, formData.branch, formData.appointmentTime, branches]);
```

**Key Improvements:**
- Clears error state on start (prevents stale errors)
- Parses backend error messages
- Stores both code and message
- Clear separation of concerns

#### 4. Updated scheduleAppointment() Function (Lines 207-321)

**Before:**
```typescript
const scheduleAppointment = useCallback(async () => {
  setLoadingState('loading');
  setError('');

  try {
    // ... appointment creation logic ...
    setStep('confirmation');
    setLoadingState('success');
  } catch (err) {
    setError(err instanceof Error ? err.message : 'Error al agendar cita');
    setLoadingState('error');
  }
}, [clientData, isNewClient, newClientData, isNewClientFormValid, branches, appointmentTypes, formData]);
```

**After:**
```typescript
const scheduleAppointment = useCallback(async () => {
  setLoadingState('loading');
  setError('');
  setParsedError(null);  // Clear error on start

  try {
    // ... appointment creation logic (same as before) ...
    setStep('confirmation');
    setLoadingState('success');
  } catch (err: any) {  // Type annotation added
    // NEW: Parse error message from backend
    const errorMsg = err.response?.data?.error || (err instanceof Error ? err.message : 'Error al agendar cita');
    const parsed = parseErrorMessage(errorMsg);

    setError(parsed.message);
    setParsedError(parsed);
    setLoadingState('error');
  }
}, [clientData, isNewClient, newClientData, isNewClientFormValid, branches, appointmentTypes, formData]);
```

**Key Improvements:**
- Clears error state before attempting
- Parses backend error messages
- Uses parsed message instead of raw error
- Stores both code and message for UI decisions

#### 5. Updated resetFlow() Function (Lines 326-346)

**Before:**
```typescript
const resetFlow = useCallback(() => {
  setStep('client');
  setClientNumber('');
  setClientData(null);
  setIsNewClient(false);
  setNewClientData(null);
  setIsNewClientFormValid(false);
  setAppointmentConfirmation(null);
  setQrCodeDataURL('');
  setError('');
  setValidationErrors({});
  // ... form data reset
}, []);
```

**After:**
```typescript
const resetFlow = useCallback(() => {
  setStep('client');
  setClientNumber('');
  setClientData(null);
  setIsNewClient(false);
  setNewClientData(null);
  setIsNewClientFormValid(false);
  setAppointmentConfirmation(null);
  setQrCodeDataURL('');
  setError('');
  setParsedError(null);  // NEW: Clear parsed error
  setValidationErrors({});
  // ... form data reset
}, []);
```

#### 6. Updated Return Object (Lines 370-409)

**Before:**
```typescript
return {
  step,
  loadingState,
  isLoading: loadingState === 'loading',
  loadingHours,
  error,
  validationErrors,
  // ... other properties
};
```

**After:**
```typescript
return {
  step,
  loadingState,
  isLoading: loadingState === 'loading',
  loadingHours,
  error,
  parsedError,  // NEW: Expose parsed error
  validationErrors,
  // ... other properties
};
```

### Summary of Changes
- 3 new `setParsedError()` calls for error state management
- 1 error message parsing operation
- Type annotation added to catch blocks
- Exposed `parsedError` in return object
- Maintains backward compatibility with existing error handling

---

## File 4: Component Update - AppointmentFormStep
**Location:** `src/features/appointments/views/steps/AppointmentFormStep.tsx`

**Status:** MODIFIED - 557 lines

### Changes Summary

#### 1. Import Addition (Line 27)
**Before:**
```typescript
import { BranchDto, AppointmentTypeDto } from '@/core/types/appointment.types';
```

**After:**
```typescript
import { BranchDto, AppointmentTypeDto } from '@/core/types/appointment.types';
import { getErrorHint, getErrorIcon, getErrorColorClass, getErrorBgClass, getErrorBorderClass, type ParsedError } from '../../utils/errorParser';
```

#### 2. Interface Update (Lines 43-57)

**Before:**
```typescript
interface AppointmentFormStepProps {
  clientData: ClientData;
  formData: FormData;
  branches: BranchDto[];
  appointmentTypes: AppointmentTypeDto[];
  availableHours: string[];
  loadingHours: boolean;
  isLoading: boolean;
  error: string | null;
  validationErrors: Record<string, string>;
  onFormChange: (field: string, value: string) => void;
  onSubmit: (e: React.FormEvent) => Promise<void>;
  onBack: () => void;
}
```

**After:**
```typescript
interface AppointmentFormStepProps {
  clientData: ClientData;
  formData: FormData;
  branches: BranchDto[];
  appointmentTypes: AppointmentTypeDto[];
  availableHours: string[];
  loadingHours: boolean;
  isLoading: boolean;
  error: string | null;
  parsedError: ParsedError | null;  // NEW
  validationErrors: Record<string, string>;
  onFormChange: (field: string, value: string) => void;
  onSubmit: (e: React.FormEvent) => Promise<void>;
  onBack: () => void;
}
```

#### 3. Destructuring Update (Lines 59-73)

**Before:**
```typescript
export const AppointmentFormStep: FC<AppointmentFormStepProps> = ({
  clientData,
  formData,
  branches,
  appointmentTypes,
  availableHours,
  loadingHours,
  isLoading,
  error,
  validationErrors,
  onFormChange,
  onSubmit,
  onBack
}) => {
```

**After:**
```typescript
export const AppointmentFormStep: FC<AppointmentFormStepProps> = ({
  clientData,
  formData,
  branches,
  appointmentTypes,
  availableHours,
  loadingHours,
  isLoading,
  error,
  parsedError,  // NEW
  validationErrors,
  onFormChange,
  onSubmit,
  onBack
}) => {
```

#### 4. Time Selection Display (Lines 434-521)

**Before:**
```tsx
{loadingHours ? (
  <div className="text-center py-8">
    <LoadingSpinner size="lg" text="Cargando horarios disponibles..." />
  </div>
) : availableHours.length === 0 ? (
  <motion.div
    initial={{ opacity: 0, scale: 0.9 }}
    animate={{ opacity: 1, scale: 1 }}
    className="text-center py-8 bg-gray-50 rounded-lg border-2 border-gray-200"
  >
    <div className="w-16 h-16 mx-auto bg-gray-100 rounded-2xl flex items-center justify-center mb-4">
      <FiClock className="w-8 h-8 text-gray-400" />
    </div>
    <p className="text-gray-600 font-medium">No hay horarios disponibles</p>
    <p className="text-gray-500 text-sm mt-2">
      Para la fecha y sede seleccionadas
    </p>
  </motion.div>
) : (
  // Time buttons
)}
```

**After:**
```tsx
{loadingHours ? (
  <div className="text-center py-8">
    <LoadingSpinner size="lg" text="Cargando horarios disponibles..." />
  </div>
) : availableHours.length === 0 ? (
  <motion.div
    initial={{ opacity: 0, scale: 0.9 }}
    animate={{ opacity: 1, scale: 1 }}
    className={`text-center py-8 rounded-lg border-2 ${
      parsedError
        ? `${getErrorBgClass(parsedError.code)} ${getErrorBorderClass(parsedError.code)}`
        : 'bg-gray-50 border-gray-200'
    }`}
  >
    <div className={`w-16 h-16 mx-auto rounded-2xl flex items-center justify-center mb-4 text-2xl ${
      parsedError ? 'bg-white' : 'bg-gray-100'
    }`}>
      {parsedError ? getErrorIcon(parsedError.code) : <FiClock className="w-8 h-8 text-gray-400" />}
    </div>
    <p className={`font-medium ${
      parsedError
        ? getErrorColorClass(parsedError.code)
        : 'text-gray-600'
    }`}>
      {parsedError?.message || 'No hay horarios disponibles'}
    </p>
    {parsedError ? (
      <motion.p
        initial={{ opacity: 0, y: -5 }}
        animate={{ opacity: 1, y: 0 }}
        className={`text-sm mt-3 ${getErrorColorClass(parsedError.code)} opacity-75`}
      >
        {getErrorHint(parsedError.code)}
      </motion.p>
    ) : (
      <p className="text-gray-500 text-sm mt-2">
        Para la fecha y sede seleccionadas
      </p>
    )}
  </motion.div>
) : (
  // Time buttons (unchanged)
)}
```

**Key Improvements:**
- Conditional styling based on error code
- Dynamic icon display (emoji or FiClock)
- User-friendly error message
- Contextual hints
- Maintains original styling for generic errors
- Smooth animations for hint text

### Summary of Changes
- 1 new import with 6 utility functions
- 1 new prop in interface
- 1 new destructured prop
- Enhanced time display with 30+ lines of conditional logic
- No breaking changes to other functionality

---

## File 5: View Update - AppointmentSchedulingView
**Location:** `src/features/appointments/views/AppointmentSchedulingView.tsx`

**Status:** MODIFIED - 160 lines

### Changes Summary

#### 1. Updated Hook Destructuring (Lines 16-43)

**Before:**
```typescript
const {
  step,
  isLoading,
  loadingHours,
  error,
  clientNumber,
  setClientNumber,
  clientData,
  appointmentConfirmation,
  qrCodeDataURL,
  branches,
  appointmentTypes,
  availableHours,
  formData,
  validationErrors,
  isNewClient,
  isNewClientFormValid,
  validateClient,
  scheduleAppointment,
  resetFlow,
  updateFormField,
  setStep,
  handleNewClientClick,
  handleBackToClientNumber,
  handleNewClientDataChange,
  handleContinueAsNewClient
} = useAppointmentScheduling();
```

**After:**
```typescript
const {
  step,
  isLoading,
  loadingHours,
  error,
  parsedError,  // NEW
  clientNumber,
  setClientNumber,
  clientData,
  appointmentConfirmation,
  qrCodeDataURL,
  branches,
  appointmentTypes,
  availableHours,
  formData,
  validationErrors,
  isNewClient,
  isNewClientFormValid,
  validateClient,
  scheduleAppointment,
  resetFlow,
  updateFormField,
  setStep,
  handleNewClientClick,
  handleBackToClientNumber,
  handleNewClientDataChange,
  handleContinueAsNewClient
} = useAppointmentScheduling();
```

#### 2. Component Prop Update (Lines 112-127)

**Before:**
```tsx
<AppointmentFormStep
  clientData={displayClientData}
  formData={formData}
  branches={branches}
  appointmentTypes={appointmentTypes}
  availableHours={availableHours}
  loadingHours={loadingHours}
  isLoading={isLoading}
  error={error}
  validationErrors={validationErrors}
  onFormChange={handleFormChange}
  onSubmit={handleFormSubmit}
  onBack={handleBackToClient}
/>
```

**After:**
```tsx
<AppointmentFormStep
  clientData={displayClientData}
  formData={formData}
  branches={branches}
  appointmentTypes={appointmentTypes}
  availableHours={availableHours}
  loadingHours={loadingHours}
  isLoading={isLoading}
  error={error}
  parsedError={parsedError}  // NEW
  validationErrors={validationErrors}
  onFormChange={handleFormChange}
  onSubmit={handleFormSubmit}
  onBack={handleBackToClient}
/>
```

### Summary of Changes
- 1 new destructured property from hook
- 1 new prop passed to component
- Minimal changes, purely propagating new data
- No changes to component logic or rendering
- Maintains clean prop passing pattern

---

## Summary Statistics

| File | Type | Status | Changes | Lines |
|------|------|--------|---------|-------|
| errorParser.ts | NEW | Created | - | 98 |
| errorParser.test.ts | NEW | Created | - | 181 |
| useAppointmentScheduling.ts | MODIFIED | Updated | 12 | 410 |
| AppointmentFormStep.tsx | MODIFIED | Updated | 3 | 557 |
| AppointmentSchedulingView.tsx | MODIFIED | Updated | 2 | 160 |
| **TOTAL** | - | - | **17** | **1,406** |

### Change Complexity
- **New Code:** 279 lines (errorParser + tests)
- **Modified Code:** 17 locations
- **Breaking Changes:** None
- **Backward Compatibility:** Full

### Key Metrics
- **Test Coverage:** 38 test cases
- **Error Codes Supported:** 3 + fallback
- **UI Elements Updated:** 1 component
- **State Management Changes:** 1 new state variable
- **Utility Functions:** 6 new exports
