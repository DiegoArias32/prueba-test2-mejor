# Quick Reference Card - Admin CRUD Modals

## Import

```typescript
import {
  UserModal,
  RoleModal,
  BranchModal,
  AppointmentTypeModal,
  AvailableTimeModal
} from '@/features/admin/views/components/modals';
```

---

## UserModal

### Props
```typescript
{
  isOpen: boolean;
  onClose: () => void;
  onSave: (data: UserFormData) => Promise<void>;
  item?: any;
  mode: 'create' | 'edit';
  roles: Role[];  // Required!
}
```

### Data Structure
```typescript
interface UserFormData {
  username: string;
  email: string;
  fullName: string;
  identificationType: 'CC' | 'CE' | 'TI' | 'PP' | 'NIT';
  identificationNumber: string;
  phone: string;
  address: string;
  password?: string;  // Only on create
  roleIds: string[];
}
```

### Usage
```tsx
<UserModal
  isOpen={isOpen}
  onClose={() => setIsOpen(false)}
  onSave={handleSave}
  item={selectedUser}
  mode="edit"
  roles={rolesData}
/>
```

---

## RoleModal

### Props
```typescript
{
  isOpen: boolean;
  onClose: () => void;
  onSave: (data: RoleFormData) => Promise<void>;
  item?: any;
  mode: 'create' | 'edit';
}
```

### Data Structure
```typescript
interface RoleFormData {
  code: string;        // UPPERCASE, no spaces, 3-20 chars
  name: string;        // Display name
  description: string; // Optional
}
```

### Usage
```tsx
<RoleModal
  isOpen={isOpen}
  onClose={() => setIsOpen(false)}
  onSave={handleSave}
  item={selectedRole}
  mode="create"
/>
```

---

## BranchModal

### Props
```typescript
{
  isOpen: boolean;
  onClose: () => void;
  onSave: (data: BranchFormData) => Promise<void>;
  item?: any;
  mode: 'create' | 'edit';
}
```

### Data Structure
```typescript
interface BranchFormData {
  code: string;      // 3-10 chars, alphanumeric + hyphens
  name: string;      // Branch name
  address: string;   // Full address
  city: string;      // City name
  phone: string;     // Phone number
  isMain: boolean;   // Is main branch?
}
```

### Usage
```tsx
<BranchModal
  isOpen={isOpen}
  onClose={() => setIsOpen(false)}
  onSave={handleSave}
  item={selectedBranch}
  mode="edit"
/>
```

---

## AppointmentTypeModal

### Props
```typescript
{
  isOpen: boolean;
  onClose: () => void;
  onSave: (data: AppointmentTypeFormData) => Promise<void>;
  item?: any;
  mode: 'create' | 'edit';
}
```

### Data Structure
```typescript
interface AppointmentTypeFormData {
  name: string;
  description: string;           // Optional
  icon: string;                  // Icon name (e.g., 'FiCalendar')
  durationMinutes: number;       // 15-480
  requiresDocumentation: boolean;
}
```

### Available Icons
```
'FiCalendar', 'FiFileText', 'FiUser', 'FiClipboard',
'FiTool', 'FiZap', 'FiInfo', 'FiPhone'
```

### Usage
```tsx
<AppointmentTypeModal
  isOpen={isOpen}
  onClose={() => setIsOpen(false)}
  onSave={handleSave}
  item={selectedType}
  mode="create"
/>
```

---

## AvailableTimeModal

### Props
```typescript
{
  isOpen: boolean;
  onClose: () => void;
  onSave: (data: AvailableTimeFormData) => Promise<void>;
  item?: any;
  mode: 'create' | 'edit';
  branches: Branch[];           // Required!
  appointmentTypes: AppointmentType[];  // Required!
}
```

### Data Structure
```typescript
interface AvailableTimeFormData {
  time: string;              // HH:mm format (e.g., "08:00")
  branchId: string;          // Branch ID
  appointmentTypeId: string | null;  // null = all types
}
```

### Usage
```tsx
<AvailableTimeModal
  isOpen={isOpen}
  onClose={() => setIsOpen(false)}
  onSave={handleSave}
  item={selectedTime}
  mode="edit"
  branches={branchesData}
  appointmentTypes={appointmentTypesData}
/>
```

---

## Common Pattern

### Basic Setup
```typescript
// State
const [isModalOpen, setIsModalOpen] = useState(false);
const [modalMode, setModalMode] = useState<'create' | 'edit'>('create');
const [selectedItem, setSelectedItem] = useState<any>(null);

// Handlers
const handleCreate = () => {
  setModalMode('create');
  setSelectedItem(null);
  setIsModalOpen(true);
};

const handleEdit = (item: any) => {
  setModalMode('edit');
  setSelectedItem(item);
  setIsModalOpen(true);
};

const handleSave = async (data: any) => {
  try {
    if (modalMode === 'create') {
      await api.create(data);
    } else {
      await api.update(selectedItem.id, data);
    }
    await refreshData();
    setIsModalOpen(false);
  } catch (error) {
    console.error('Save error:', error);
    // Show error toast/message
    throw error; // Keep modal open
  }
};

const handleClose = () => {
  setIsModalOpen(false);
};
```

---

## Validation Reference

| Field | Validation Method | Rules |
|-------|------------------|-------|
| Username | Custom | Required, min 3 chars |
| Email | `validateEmail()` | Valid email format |
| Full Name | `validateName()` | Letters only, 2-100 chars |
| ID Number | `validateIdentificationNumber()` | Numbers only, 6-15 digits |
| Phone | `validatePhone()` | Numbers only, 7-15 digits |
| Address | `validateAddress()` | 5-200 chars |
| Code | Custom | Various formats (see modal) |
| Time | Custom | HH:mm, 6:00-18:00 |
| Duration | Custom | Number, 15-480 |

---

## Error Handling

### Save Success
```typescript
const handleSave = async (data: any) => {
  await api.save(data);
  // Modal closes automatically
};
```

### Save Error
```typescript
const handleSave = async (data: any) => {
  try {
    await api.save(data);
  } catch (error) {
    toast.error('Error saving data');
    throw error; // Keep modal open
  }
};
```

---

## Styling Customization

### Colors
```css
Primary: #1797D5 (Electrohuila blue)
Dark: #203461 (Electrohuila navy)
Error: red-500/red-600
Success: green-500
```

### Animations
```css
.animate-fade-in { /* 200ms fade */ }
.animate-scale-in { /* 200ms scale */ }
```

---

## TypeScript Types

### Common Interfaces
```typescript
interface FormErrors {
  [key: string]: string;
}

interface ValidationResult {
  isValid: boolean;
  message: string;
}
```

---

## Keyboard Shortcuts

| Key | Action |
|-----|--------|
| Esc | Close modal |
| Tab | Navigate fields |
| Enter | Submit form |
| Click backdrop | Close modal |

---

## Accessibility

- ✓ All fields have labels
- ✓ Error messages linked to fields
- ✓ Focus management
- ✓ Screen reader support
- ✓ Keyboard navigation
- ✓ ARIA attributes

---

## Best Practices

1. **Always provide dependencies**
   - UserModal needs `roles`
   - AvailableTimeModal needs `branches` and `appointmentTypes`

2. **Handle errors in parent**
   ```typescript
   const handleSave = async (data) => {
     try {
       await save(data);
     } catch (error) {
       // Show error message
       throw error; // Keep modal open
     }
   };
   ```

3. **Refresh data after save**
   ```typescript
   await save(data);
   await fetchData(); // Refresh list
   ```

4. **Use proper TypeScript types**
   ```typescript
   const [item, setItem] = useState<User | null>(null);
   ```

5. **Clear selection on create**
   ```typescript
   setMode('create');
   setItem(null); // Important!
   ```

---

## Common Issues

### Modal doesn't open
- Check `isOpen` state
- Check conditional rendering

### Validation not working
- Ensure ValidationUtils is imported
- Check field names match

### Data not loading in edit mode
- Ensure `item` prop is passed
- Check `mode` is set to 'edit'
- Verify useEffect dependencies

### Dependencies not showing
- For UserModal: Pass `roles` prop
- For AvailableTimeModal: Pass `branches` and `appointmentTypes`

### TypeScript errors
- Ensure all required props are passed
- Check data structure matches interface

---

## Testing Checklist

- [ ] Modal opens correctly
- [ ] Modal closes correctly
- [ ] Create mode works
- [ ] Edit mode loads data
- [ ] Validation works
- [ ] Save works
- [ ] Error handling works
- [ ] Loading state works
- [ ] Dependencies load
- [ ] Responsive on mobile

---

## File Locations

```
Portal:
src/features/admin/views/components/modals/
├── UserModal.tsx
├── RoleModal.tsx
├── BranchModal.tsx
├── AppointmentTypeModal.tsx
├── AvailableTimeModal.tsx
└── index.ts

Utilities:
src/shared/utils/validation.utils.ts
```

---

## Support

See full documentation:
- README.md - Usage guide
- SUMMARY.md - Feature summary
- STRUCTURE.md - Component hierarchy

---

**Created**: October 30, 2025
**Version**: 1.0.0
**Total Modals**: 5
**Total Lines**: 1,815 lines of code
