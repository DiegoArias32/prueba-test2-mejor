# Admin Panel CRUD Modals

This directory contains all CRUD (Create/Read/Update/Delete) modals for the admin panel with comprehensive validations.

## Available Modals

### 1. UserModal (Empleados)
Modal for creating and editing users/employees.

**Fields:**
- Username (required, min 3 chars)
- Email (required, validated)
- Full Name (required, validated with ValidationUtils)
- Identification Type (select)
- Identification Number (required, validated)
- Phone (required, validated)
- Address (required, validated)
- Password (required only on create, min 6 chars)
- Roles (multi-select checkboxes, at least one required)

### 2. RoleModal (Roles)
Modal for creating and editing user roles.

**Fields:**
- Code (required, uppercase, no spaces, 3-20 chars, read-only after creation)
- Name (required, min 3 chars)
- Description (optional, 10-500 chars if provided)

### 3. BranchModal (Sedes)
Modal for creating and editing company branches.

**Fields:**
- Code (required, 3-10 chars, uppercase alphanumeric)
- Name (required, min 3 chars)
- City (required, min 3 chars)
- Address (required, validated)
- Phone (required, validated)
- Is Main (boolean, shows warning if checked)

### 4. AppointmentTypeModal (Tipos de Cita)
Modal for creating and editing appointment types.

**Fields:**
- Name (required, min 3 chars)
- Description (optional, 10-500 chars if provided)
- Icon (required, select from predefined icons)
- Duration Minutes (required, number, min 15, max 480, with quick-select buttons)
- Requires Documentation (boolean, shows info box if checked)

### 5. AvailableTimeModal (Horas Disponibles)
Modal for creating and editing available time slots.

**Fields:**
- Time (required, HH:mm format, 6:00-18:00)
- Branch (required, select from available branches)
- Appointment Type (optional, null means "all types")

## Usage Example

```tsx
import { useState } from 'react';
import { UserModal, RoleModal, BranchModal, AppointmentTypeModal, AvailableTimeModal } from './modals';

export const AdminPanel = () => {
  const [isUserModalOpen, setIsUserModalOpen] = useState(false);
  const [modalMode, setModalMode] = useState<'create' | 'edit'>('create');
  const [selectedItem, setSelectedItem] = useState(null);

  // Example: Roles data (fetch from API)
  const [roles, setRoles] = useState([
    { id: '1', code: 'ADMIN', name: 'Administrador' },
    { id: '2', code: 'EMPLOYEE', name: 'Empleado' }
  ]);

  const handleCreateUser = () => {
    setModalMode('create');
    setSelectedItem(null);
    setIsUserModalOpen(true);
  };

  const handleEditUser = (user: any) => {
    setModalMode('edit');
    setSelectedItem(user);
    setIsUserModalOpen(true);
  };

  const handleSaveUser = async (data: any) => {
    try {
      if (modalMode === 'create') {
        // Call API to create user
        await fetch('/api/users', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(data)
        });
      } else {
        // Call API to update user
        await fetch(\`/api/users/\${selectedItem.id}\`, {
          method: 'PUT',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(data)
        });
      }

      // Refresh data
      // fetchUsers();

      // Show success message
      alert('Usuario guardado exitosamente');
    } catch (error) {
      alert('Error al guardar usuario');
      throw error; // Re-throw to keep modal open
    }
  };

  return (
    <div>
      <button onClick={handleCreateUser}>
        Crear Usuario
      </button>

      <UserModal
        isOpen={isUserModalOpen}
        onClose={() => setIsUserModalOpen(false)}
        onSave={handleSaveUser}
        item={selectedItem}
        mode={modalMode}
        roles={roles}
      />
    </div>
  );
};
```

## Features

### Validation
All modals use `ValidationUtils` from `@/shared/utils/validation.utils` for consistent validation:
- Email validation
- Name validation (only letters)
- Phone validation (only numbers, 7-15 digits)
- Identification number validation (only numbers, 6-15 digits)
- Address validation (5-200 chars)
- Required field validation

### User Experience
- **Animations**: All modals use `animate-fade-in` and `animate-scale-in` animations
- **Error Messages**: Field-level error messages displayed below each input
- **Loading States**: Submit button disabled during save operation
- **Accessibility**: Proper labels, aria-labels, and keyboard navigation
- **Icons**: React Icons (Feather Icons) for visual enhancement
- **Responsive**: Works on all screen sizes
- **Smart Defaults**: Pre-filled with sensible default values

### Form Management
- Auto-reset form when modal opens/closes
- Separate data for create vs edit modes
- Real-time validation feedback
- Disabled submit when validation fails
- Form data typed with TypeScript interfaces

### Styling
- Consistent with Electrohuila brand colors (`#1797D5` primary, `#203461` dark)
- Modern, clean design with shadows and borders
- Hover states and transitions
- Info boxes for contextual help
- Preview boxes where applicable (AvailableTimeModal)

## Notes

### UserModal
- Password field only shown on create mode
- Multi-select checkboxes for roles with visual feedback
- Identification type dropdown with common Colombian ID types

### RoleModal
- Code field automatically converts to uppercase
- Code is read-only in edit mode (cannot be changed after creation)
- Character counter for description field

### BranchModal
- Code field automatically converts to uppercase
- Warning shown when marking as main branch
- City and branch name required

### AppointmentTypeModal
- Icon selector with predefined options and descriptions
- Duration quick-select buttons (15, 30, 45, 60, 90, 120 minutes)
- Character counter for description
- Info box when "Requires Documentation" is checked

### AvailableTimeModal
- Time dropdown with 30-minute intervals (6:00 AM - 6:00 PM)
- Shows time in both 24-hour and 12-hour format
- Optional appointment type (null = all types)
- Preview box showing selected configuration
- Disabled if no branches available

## Validation Rules Summary

| Field | Validation |
|-------|-----------|
| Username | Required, min 3 chars |
| Email | Required, valid email format |
| Full Name | Required, only letters, 2-100 chars |
| ID Number | Required, only numbers, 6-15 digits |
| Phone | Required/Optional, only numbers, 7-15 digits |
| Address | Required, 5-200 chars |
| Password | Required on create, min 6 chars |
| Role Code | Required, uppercase, no spaces, 3-20 chars |
| Branch Code | Required, alphanumeric + hyphens, 3-10 chars |
| Time | Required, HH:mm format, 6:00-18:00 |
| Duration | Required, number, 15-480 minutes |

## Error Handling

All modals follow the same error handling pattern:

1. **Validation Errors**: Shown inline below each field
2. **Save Errors**: Handled by parent component (can show toast/alert)
3. **Network Errors**: Re-thrown to keep modal open for retry
4. **Success**: Modal closes automatically after successful save

## Accessibility

- All form fields have proper `<label>` elements with `htmlFor` attribute
- Icons are decorative only, don't affect screen readers
- Modal can be closed with backdrop click
- Focus management handled automatically
- Color contrast meets WCAG AA standards

## Future Improvements

- Add loading skeleton for select dropdowns
- Add search/filter for large role/branch lists
- Add keyboard shortcuts (Esc to close, Ctrl+Enter to submit)
- Add unsaved changes warning
- Add field-level async validation (check unique codes)
- Add bulk operations support
