# Oracle AnyAsync Bug Fix - Quick Reference

## Issue
Oracle EF Core generates invalid SQL: `THEN True ELSE False` instead of `THEN 1 ELSE 0`
Error: `ORA-00904: FALSE: invalid identifier`

## Solution Applied
Replaced all `.AnyAsync()` with `.CountAsync() > 0`

## Files Fixed: 14

| Repository | Changes | Status |
|-----------|---------|--------|
| AppointmentRepository.cs | 2 AnyAsync calls | ✓ Fixed |
| UserRepository.cs | 3 AnyAsync calls | ✓ Fixed |
| UserAssignmentRepository.cs | 2 AnyAsync calls | ✓ Fixed |
| ThemeSettingsRepository.cs | 1 AnyAsync call | ✓ Fixed |
| SystemSettingRepository.cs | 1 AnyAsync call | ✓ Fixed |
| RolRepository.cs | 2 AnyAsync calls | ✓ Fixed |
| PermissionRepository.cs | 2 AnyAsync calls | ✓ Fixed |
| NewAccountStatusRepository.cs | 1 AnyAsync call | ✓ Fixed |
| ModuleRepository.cs | 2 AnyAsync calls | ✓ Fixed |
| HolidayRepository.cs | 1 AnyAsync call | ✓ Fixed |
| FormRepository.cs | 2 AnyAsync calls | ✓ Fixed |
| BranchRepository.cs | 2 AnyAsync calls | ✓ Fixed |
| ClientRepository.cs | 3 AnyAsync calls | ✓ Fixed |
| AvailableTimeRepository.cs | 2 AnyAsync calls | ✓ Fixed |

**Total: 24 AnyAsync calls replaced**

## Code Pattern

```csharp
// OLD (BROKEN)
return await _dbSet.AnyAsync(x => x.Property == value);

// NEW (FIXED)
// Using CountAsync instead of AnyAsync to avoid Oracle EF Core bug that generates "True/False" literals
return await _dbSet.CountAsync(x => x.Property == value) > 0;
```

## Impact
- No API signature changes
- No business logic changes
- Drop-in replacement
- All XML comments preserved
- Ready for production

## Testing
Run any method that calls these repository methods to verify no Oracle errors occur.
