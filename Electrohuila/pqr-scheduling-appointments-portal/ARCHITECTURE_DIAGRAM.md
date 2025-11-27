# Frontend Error Handling Architecture

## Component Flow Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                     AppointmentSchedulingView                     │
│                   (Main Container Component)                      │
└────────────────────────────┬────────────────────────────────────┘
                             │
                    Uses Hook: useAppointmentScheduling()
                             │
         ┌───────────────────┴────────────────────┐
         │                                        │
         ▼                                        ▼
┌──────────────────────────┐         ┌──────────────────────────┐
│  useAppointmentScheduling │         │  Appointment Repository   │
│  (State Management)      │         │  (API calls)             │
├──────────────────────────┤         ├──────────────────────────┤
│ States:                  │         │ Methods:                 │
│ - step                   │         │ - getPublicAvailableTimes│
│ - error                  │         │ - schedulePublicAppointm│
│ - parsedError (NEW)      │         │ - scheduleAppointmentFor│
│ - availableHours         │         │   NewUser               │
│ - formData               │         └──────────────────────────┘
│ - etc...                 │                      │
│                          │          Calls API which returns:
│ Error Handling (NEW):     │         "CODE|User message"
│ ─────────────────        │                      │
│ try {                    │                      │
│   const result = await   │────────────────────>│
│   appointmentRepository  │                      │
│     .getPublicAvailable  │<─────────────────────│
│     Times(...)           │
│ } catch (err: any) {     │
│   const errorMsg =       │
│     err.response?.data   │
│     ?.error || ...       │
│   const parsed =         │
│     parseErrorMessage()  │
│   setParsedError(parsed) │
│   setError(parsed.msg)   │
│ }                        │
│                          │
│ Return:                  │
│ - parsedError (NEW)      │
│ - error                  │
│ - availableHours         │
│ - etc...                 │
└──────────────────────────┘
         │              │
         │              └─────────────────────────────────────┐
         │                                                    │
         └────────────────────────────────┐                  │
                                          │                  │
         ┌────────────────────────────────┴──────────┐        │
         │                                            │        │
         ▼                                            ▼        │
┌──────────────────────────────────┐    ┌────────────────────────┐
│  AppointmentFormStep             │    │ Error Parser Utility   │
│  (Time Selection Component)      │    │ (errorParser.ts)       │
├──────────────────────────────────┤    ├────────────────────────┤
│ Props (NEW):                     │    │ Functions:             │
│ - parsedError                    │    │ - parseErrorMessage()  │
│ - availableHours                 │    │ - getErrorHint()       │
│ - loadingHours                   │    │ - getErrorIcon()       │
│ - error                          │    │ - getErrorColorClass() │
│ - etc...                         │    │ - getErrorBgClass()    │
│                                  │    │ - getErrorBorderClass()│
│ Display Logic (UPDATED):         │    └────────────────────────┘
│ ─────────────────────────────    │              │
│ if availableHours.length === 0   │              │
│   if parsedError:                │<─────────────│
│     - Show error icon            │   Provides styling
│     - Show error message         │   and utilities
│     - Show error hint            │
│     - Apply error colors         │
│     - Apply error bg color       │
│   else:                          │
│     - Show generic message       │
│ else:                            │
│   - Show time slot buttons       │
└──────────────────────────────────┘
```

## Data Flow Sequence Diagram

```
User Interaction
       │
       ▼
┌─────────────────────────────┐
│ User selects date (Sunday)  │
└────────────────┬────────────┘
                 │
                 ▼
        ┌─────────────────┐
        │ loadAvailableTimes()
        │ hook method     │
        └────────┬────────┘
                 │
                 ▼
        ┌──────────────────────┐
        │ appointmentRepository │
        │ .getPublicAvailableTimes
        │ (date, branchId)    │
        └────────┬─────────────┘
                 │
                 ▼
        ┌──────────────────────┐
        │ Backend API          │
        │ /available-times     │
        └────────┬─────────────┘
                 │
                 ▼
        ┌──────────────────────────────┐
        │ Response (400 error):        │
        │ {                            │
        │   error: "SUNDAY_NOT_        │
        │   AVAILABLE|Los domingos..." │
        │ }                            │
        └────────┬─────────────────────┘
                 │
                 ▼
        ┌──────────────────────┐
        │ catch (err: any) {    │
        │   const errorMsg =    │
        │   err.response?.data  │
        │   ?.error             │
        └────────┬──────────────┘
                 │
                 ▼
        ┌──────────────────────────────┐
        │ parseErrorMessage(errorMsg)  │
        │ Returns:                     │
        │ {                            │
        │   code: "SUNDAY_NOT_         │
        │   AVAILABLE",                │
        │   message: "Los domingos..." │
        │ }                            │
        └────────┬─────────────────────┘
                 │
                 ▼
        ┌──────────────────────┐
        │ setParsedError(      │
        │   parsed             │
        │ )                    │
        └────────┬─────────────┘
                 │
                 ▼
        ┌──────────────────────┐
        │ Component re-renders │
        │ with parsedError     │
        └────────┬─────────────┘
                 │
                 ▼
        ┌──────────────────────────────┐
        │ AppointmentFormStep renders  │
        │ with availableHours.length   │
        │ === 0                        │
        └────────┬─────────────────────┘
                 │
                 ▼
        ┌──────────────────────────────┐
        │ getErrorIcon(parsedError     │
        │ .code) → "📅"                │
        │                              │
        │ getErrorColorClass(parsed    │
        │ Error.code) → "text-amber-   │
        │ 600"                         │
        │                              │
        │ getErrorBgClass(parsedError  │
        │ .code) → "bg-amber-50"       │
        │                              │
        │ getErrorHint(parsedError     │
        │ .code) → "Por favor..."      │
        └────────┬─────────────────────┘
                 │
                 ▼
        ┌──────────────────────────────┐
        │ User sees:                   │
        │ ┌────────────────────────┐   │
        │ │ 📅                     │   │
        │ │ Los domingos no se     │   │
        │ │ atienden citas         │   │
        │ │                        │   │
        │ │ Por favor seleccione   │   │
        │ │ un día entre lunes y   │   │
        │ │ sábado                 │   │
        │ └────────────────────────┘   │
        │ (styled with amber colors)   │
        └──────────────────────────────┘
```

## State Management Structure

```
useAppointmentScheduling Hook
│
├─ Error Handling States
│  ├─ error: string (raw error message from hook)
│  └─ parsedError: ParsedError | null (NEW)
│      ├─ code: string (e.g., "SUNDAY_NOT_AVAILABLE")
│      └─ message: string (e.g., "Los domingos no se atienden citas")
│
├─ Form States
│  ├─ formData
│  ├─ availableHours: string[]
│  └─ validationErrors
│
├─ Loading States
│  ├─ loadingState: LoadingState
│  ├─ loadingHours: boolean
│  └─ isLoading: boolean
│
├─ Navigation States
│  ├─ step: AppointmentStep
│  └─ clientData
│
└─ Methods
   ├─ loadAvailableHours() (UPDATED)
   │  ├─ Clears parsedError on start
   │  ├─ Sets parsedError on error
   │  └─ Clears parsedError on success
   │
   ├─ scheduleAppointment() (UPDATED)
   │  ├─ Clears parsedError on start
   │  ├─ Parses error and sets parsedError
   │  └─ Sets error message from parsed data
   │
   └─ resetFlow() (UPDATED)
      └─ Clears parsedError
```

## Error Code Handling Matrix

```
┌──────────────────────┬────────┬────────┬───────────────┬───────────────────────────┐
│ Error Code           │ Icon   │ Color  │ Background    │ Hint Text                 │
├──────────────────────┼────────┼────────┼───────────────┼───────────────────────────┤
│ SUNDAY_NOT_          │ 📅     │ amber- │ bg-amber-50   │ Por favor seleccione un   │
│ AVAILABLE            │        │ 600    │ border-amber- │ día entre lunes y sábado  │
│                      │        │        │ 200           │                           │
├──────────────────────┼────────┼────────┼───────────────┼───────────────────────────┤
│ HOLIDAY_NOT_         │ 🎉     │ red-   │ bg-red-50     │ Por favor seleccione      │
│ AVAILABLE            │        │ 600    │ border-red-   │ otra fecha                │
│                      │        │        │ 200           │                           │
├──────────────────────┼────────┼────────┼───────────────┼───────────────────────────┤
│ PAST_DATE_NOT_       │ ⏰     │ gray-  │ bg-gray-50    │ Seleccione una fecha      │
│ AVAILABLE            │        │ 600    │ border-gray-  │ desde hoy en adelante      │
│                      │        │        │ 200           │                           │
├──────────────────────┼────────┼────────┼───────────────┼───────────────────────────┤
│ Unknown / Other      │ ⚠️     │ gray-  │ bg-gray-50    │ (null - no hint)          │
│                      │        │ 600    │ border-gray-  │                           │
│                      │        │        │ 200           │                           │
└──────────────────────┴────────┴────────┴───────────────┴───────────────────────────┘
```

## File Dependencies

```
AppointmentSchedulingView
    │
    ├─> useAppointmentScheduling (hook)
    │    ├─> appointmentRepository
    │    └─> errorParser (parseErrorMessage)
    │
    └─> AppointmentFormStep (component)
         ├─> errorParser utilities
         │    ├─ getErrorIcon
         │    ├─ getErrorColorClass
         │    ├─ getErrorBgClass
         │    ├─ getErrorBorderClass
         │    └─ getErrorHint
         │
         └─> Props:
              └─ parsedError (from useAppointmentScheduling)
```

## Error Parsing Pipeline

```
┌──────────────────────────────────────────┐
│ Backend Error Response                   │
│ { error: "CODE|User friendly message" }  │
└────────────────┬─────────────────────────┘
                 │
                 ▼
        ┌─────────────────────┐
        │ Extract error text  │
        │ from response       │
        └────────┬────────────┘
                 │
                 ▼
        ┌──────────────────────────────┐
        │ parseErrorMessage(errorText) │
        │ - Split by '|'               │
        │ - Trim whitespace            │
        │ - Handle null/undefined      │
        └────────┬─────────────────────┘
                 │
                 ▼
        ┌────────────────────────┐
        │ ParsedError Object     │
        │ {                      │
        │   code: string,        │
        │   message: string      │
        │ }                      │
        └────────┬───────────────┘
                 │
       ┌─────────┼──────────────────────┐
       │         │                      │
       ▼         ▼                      ▼
   getError  getError-              getError-
   Icon()    ColorClass()            BgClass()
       │         │                      │
       ▼         ▼                      ▼
   "📅"    "text-amber-600"        "bg-amber-50"
       │         │                      │
       └─────────┼──────────────────────┘
                 │
                 ▼
        ┌──────────────────────┐
        │ Component Display:   │
        │ - Icon               │
        │ - Colored message    │
        │ - Colored background │
        │ - Helpful hint       │
        └──────────────────────┘
```

## Performance Considerations

```
┌─────────────────────────────────────┐
│ Performance Profile                 │
├─────────────────────────────────────┤
│                                     │
│ Error Parsing:                      │
│ - Time Complexity: O(n)             │
│   where n = message length          │
│ - Space Complexity: O(1)            │
│ - No additional API calls           │
│ - Pure synchronous operations       │
│                                     │
│ Component Rendering:                │
│ - No extra re-renders               │
│ - Uses existing hook state          │
│ - Minimal DOM changes               │
│                                     │
│ Memory Usage:                       │
│ - Single ParsedError object per     │
│   error state                       │
│ - All utilities are functions       │
│   (no stored data)                  │
│                                     │
│ Overall Impact:                     │
│ - Negligible performance overhead   │
│ - No breaking changes               │
│ - Fully backwards compatible        │
│                                     │
└─────────────────────────────────────┘
```

## Integration Points

```
┌─────────────────────────────────────────────────────────────┐
│                    External Systems                         │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  Backend API                                               │
│  ├─ Returns error format: "CODE|Message"                   │
│  └─ Status codes: 400, 422, 500, etc.                      │
│                                                             │
│  React Framework                                           │
│  ├─ useState for state management                          │
│  ├─ useCallback for memoization                           │
│  └─ useEffect for side effects                            │
│                                                             │
│  Tailwind CSS                                              │
│  ├─ Color classes (text-*, bg-*, border-*)                │
│  ├─ Responsive utilities                                   │
│  └─ Animation support                                      │
│                                                             │
│  Framer Motion                                             │
│  ├─ motion.div for animations                             │
│  ├─ AnimatePresence for transitions                       │
│  └─ Custom animations                                      │
│                                                             │
│  React Icons (FiClock)                                     │
│  └─ Fallback icon when no error                           │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

## Testing Coverage

```
errorParser.test.ts
│
├─ parseErrorMessage()
│  ├─ Parse valid format "CODE|Message"
│  ├─ Handle without pipe separator
│  ├─ Handle null/undefined input
│  ├─ Handle empty string
│  └─ Handle multiple pipes
│
├─ getErrorHint()
│  ├─ SUNDAY_NOT_AVAILABLE
│  ├─ HOLIDAY_NOT_AVAILABLE
│  ├─ PAST_DATE_NOT_AVAILABLE
│  └─ Unknown code (null)
│
├─ getErrorIcon()
│  ├─ SUNDAY_NOT_AVAILABLE (📅)
│  ├─ HOLIDAY_NOT_AVAILABLE (🎉)
│  ├─ PAST_DATE_NOT_AVAILABLE (⏰)
│  └─ Default (⚠️)
│
├─ getErrorColorClass()
│  ├─ SUNDAY_NOT_AVAILABLE
│  ├─ HOLIDAY_NOT_AVAILABLE
│  ├─ PAST_DATE_NOT_AVAILABLE
│  └─ Default
│
├─ getErrorBgClass()
│  ├─ SUNDAY_NOT_AVAILABLE
│  ├─ HOLIDAY_NOT_AVAILABLE
│  ├─ PAST_DATE_NOT_AVAILABLE
│  └─ Default
│
└─ getErrorBorderClass()
   ├─ SUNDAY_NOT_AVAILABLE
   ├─ HOLIDAY_NOT_AVAILABLE
   ├─ PAST_DATE_NOT_AVAILABLE
   └─ Default
```

## Summary

This architecture provides:

1. **Separation of Concerns**: Error parsing logic isolated in utility module
2. **Type Safety**: Full TypeScript interfaces and types
3. **Scalability**: Easy to add new error codes
4. **Maintainability**: Clear data flow and component responsibilities
5. **Testability**: Comprehensive unit test coverage
6. **Performance**: Minimal overhead and efficient state management
7. **Accessibility**: Clear visual indicators with meaningful text
8. **User Experience**: Contextual hints and helpful guidance
