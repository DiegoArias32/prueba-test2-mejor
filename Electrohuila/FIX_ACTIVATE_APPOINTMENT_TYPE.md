# Fix: Activate AppointmentType 500 Error

## Problem Summary

Users could successfully view deactivated items in the "Inactivos" tab, but attempting to ACTIVATE an item resulted in:
- **CORS Error:** No 'Access-Control-Allow-Origin' header
- **HTTP Error:** PATCH http://localhost:5000/api/v1/appointmenttypes/11 → 500 (Internal Server Error)

## Root Cause

The frontend was calling the **update** endpoint (`PATCH /appointmenttypes/{id}`) with `{ id: 11, isActive: true }` to activate items, but:

1. **UpdateAppointmentTypeDto** did NOT have an `isActive` property
2. The **UpdateAppointmentTypeCommandHandler** did NOT update the `IsActive` field
3. When the frontend sent `{ id: 11, isActive: true }`, the backend:
   - Ignored the `isActive` property (not in DTO)
   - Tried to update with empty/default values for required fields (Name, Description, etc.)
   - Resulted in validation errors or exceptions → 500 error

## Solution

Created a dedicated **Activate** endpoint following the same pattern as **DeleteLogical** (deactivate):

### 1. Backend Changes

#### Created Command and Handler
- **File:** `C:\Users\User\Desktop\Proyecto Electrohuila\Electrohuila\pqr-scheduling-appointments-api\src\1. Core\ElectroHuila.Application\Features\AppointmentTypes\Commands\ActivateAppointmentType\ActivateAppointmentTypeCommand.cs`
- **File:** `C:\Users\User\Desktop\Proyecto Electrohuila\Electrohuila\pqr-scheduling-appointments-api\src\1. Core\ElectroHuila.Application\Features\AppointmentTypes\Commands\ActivateAppointmentType\ActivateAppointmentTypeCommandHandler.cs`

```csharp
public record ActivateAppointmentTypeCommand(int Id) : IRequest<Result>;

public class ActivateAppointmentTypeCommandHandler : IRequestHandler<ActivateAppointmentTypeCommand, Result>
{
    private readonly IAppointmentTypeRepository _appointmentTypeRepository;

    public async Task<Result> Handle(ActivateAppointmentTypeCommand request, CancellationToken cancellationToken)
    {
        var appointmentType = await _appointmentTypeRepository.GetByIdAsync(request.Id);
        if (appointmentType == null)
            return Result.Failure($"Tipo de cita con ID {request.Id} no encontrado");

        if (appointmentType.IsActive)
            return Result.Failure($"El tipo de cita con ID {request.Id} ya está activo");

        // Activación
        appointmentType.IsActive = true;
        appointmentType.UpdatedAt = DateTime.UtcNow;
        await _appointmentTypeRepository.UpdateAsync(appointmentType);

        return Result.Success();
    }
}
```

#### Added Controller Endpoint
- **File:** `C:\Users\User\Desktop\Proyecto Electrohuila\Electrohuila\pqr-scheduling-appointments-api\src\3. Presentation\ElectroHuila.WebApi\Controllers\V1\AppointmentTypesController.cs`

```csharp
/// <summary>
/// Activa un tipo de cita que fue previamente desactivado.
/// </summary>
[HttpPatch("activate/{id:int}")]
public async Task<IActionResult> Activate(int id)
{
    var command = new ActivateAppointmentTypeCommand(id);
    var result = await Mediator.Send(command);
    return HandleResult(result);
}
```

### 2. Frontend Changes

#### Added Service Method
- **File:** `C:\Users\User\Desktop\Proyecto Electrohuila\Electrohuila\pqr-scheduling-appointments-portal\src\services\catalogs\catalog.service.ts`

```typescript
/**
 * Activate appointment type (reactivate a deactivated appointment type)
 * FIXED: Endpoint exists in backend at PATCH /appointmenttypes/activate/{id}
 */
async activateAppointmentType(id: number): Promise<{ success: boolean; message: string }> {
  return this.patch<{ success: boolean; message: string }>(`/appointmenttypes/activate/${id}`, {});
}
```

#### Updated Repository
- **File:** `C:\Users\User\Desktop\Proyecto Electrohuila\Electrohuila\pqr-scheduling-appointments-portal\src\features\admin\repositories\admin.repository.ts`

```typescript
async activateAppointmentType(id: number): Promise<{ success: boolean; message: string }> {
  return await apiService.activateAppointmentType(id);  // ✅ Changed from updateAppointmentType
}
```

## API Endpoints

| Endpoint | Method | Purpose |
|----------|--------|---------|
| `/appointmenttypes/delete-logical/{id}` | PATCH | Deactivate (set isActive = false) |
| `/appointmenttypes/activate/{id}` | PATCH | Activate (set isActive = true) |
| `/appointmenttypes/{id}` | PATCH/PUT | Update properties (name, description, etc.) |

## Testing

### Build Status
Backend compiles successfully:
```
Compilación correcta.
8 Advertencia(s)
0 Errores
```

### Test Steps

1. **Start Backend:**
   ```bash
   cd "C:\Users\User\Desktop\Proyecto Electrohuila\Electrohuila\pqr-scheduling-appointments-api"
   dotnet run
   ```

2. **Start Frontend:**
   ```bash
   cd "C:\Users\User\Desktop\Proyecto Electrohuila\Electrohuila\pqr-scheduling-appointments-portal"
   npm run dev
   ```

3. **Test Activate Flow:**
   - Navigate to Admin Panel → Tipos de Citas
   - Click "Inactivos" tab
   - Find a deactivated appointment type
   - Click "Activar" button
   - Should succeed with 200 OK response
   - Item should move from "Inactivos" to "Activos" tab

### Expected Behavior

**Before Fix:**
```
PATCH /api/v1/appointmenttypes/11
Body: { "id": 11, "isActive": true }
Response: 500 Internal Server Error
```

**After Fix:**
```
PATCH /api/v1/appointmenttypes/activate/11
Body: {}
Response: 200 OK
{
  "success": true,
  "errors": null,
  "data": null
}
```

## Pattern Consistency

This fix establishes a consistent pattern for activate/deactivate across all entities:

| Entity | Deactivate Endpoint | Activate Endpoint |
|--------|-------------------|-------------------|
| AppointmentTypes | `PATCH /appointmenttypes/delete-logical/{id}` | `PATCH /appointmenttypes/activate/{id}` |
| Branches | `PATCH /branches/delete-logical/{id}` | `PATCH /branches/activate/{id}` (TODO) |
| Clients | `PATCH /clients/delete-logical/{id}` | `PATCH /clients/activate/{id}` (TODO) |
| Users | `PATCH /users/delete-logical/{id}` | `PATCH /users/activate/{id}` (TODO) |

## Files Modified

### Backend
1. ✅ `ActivateAppointmentTypeCommand.cs` (NEW)
2. ✅ `ActivateAppointmentTypeCommandHandler.cs` (NEW)
3. ✅ `AppointmentTypesController.cs` (MODIFIED - added Activate endpoint)

### Frontend
4. ✅ `catalog.service.ts` (MODIFIED - added activateAppointmentType method)
5. ✅ `admin.repository.ts` (MODIFIED - updated to use new service method)

## Prevention Recommendations

1. **Use dedicated endpoints** for activate/deactivate operations
2. **Do NOT overload update endpoints** with isActive changes
3. **Follow the established pattern:**
   - Deactivate: `PATCH /{entity}/delete-logical/{id}`
   - Activate: `PATCH /{entity}/activate/{id}`
   - Update: `PATCH /{entity}/{id}` (for other properties)

4. **DTO Design:** Update DTOs should NOT include isActive field
5. **Validation:** Each operation should have proper validation (e.g., "already active", "already inactive")

## Next Steps

Apply the same fix pattern to other entities:
- [ ] Branches
- [ ] Clients
- [ ] Users
- [ ] Available Times
- [ ] Any other entity with logical delete

## Related Files

- Backend Controller: `C:\Users\User\Desktop\Proyecto Electrohuila\Electrohuila\pqr-scheduling-appointments-api\src\3. Presentation\ElectroHuila.WebApi\Controllers\V1\AppointmentTypesController.cs`
- Frontend Service: `C:\Users\User\Desktop\Proyecto Electrohuila\Electrohuila\pqr-scheduling-appointments-portal\src\services\catalogs\catalog.service.ts`
- Admin Repository: `C:\Users\User\Desktop\Proyecto Electrohuila\Electrohuila\pqr-scheduling-appointments-portal\src\features\admin\repositories\admin.repository.ts`
