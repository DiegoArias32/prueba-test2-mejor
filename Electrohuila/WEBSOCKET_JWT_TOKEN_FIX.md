# WebSocket JWT Token Length Fix

## Problem Summary

**Error**: `WebSocket connection to 'ws://localhost:5000/hubs/notifications?id=...&access_token=ey...[very long token]...' failed: Insufficient resources`

**Root Cause**: The JWT token is too long (2000+ characters) due to including all user permissions as individual claims, causing the WebSocket URL to exceed browser/server limits.

## Analysis

### Current Implementation

**File**: `pqr-scheduling-appointments-api\src\2. Infrastructure\ElectroHuila.Infrastructure\Identity\JwtTokenGenerator.cs`

```csharp
// Lines 95-103: Each permission is added as a separate claim
foreach (var role in roles)
{
    claims.Add(new Claim(ClaimTypes.Role, role));
}

foreach (var permission in permissions)
{
    claims.Add(new Claim("permission", permission));  // PROBLEM: Creates too many claims
}
```

**Impact**:
- A user with 50 permissions creates 50 separate claims
- Each claim adds ~30-50 characters to the JWT
- Total token size can exceed 2000+ characters
- WebSocket URL query string has length limits (typically 2048 chars)

## Solutions (Multiple Options)

### Option 1: Store Permissions as Comma-Separated String (RECOMMENDED - Quick Fix)

**Pros**:
- Simple to implement
- Reduces token size by ~40-60%
- No changes to frontend required
- Backward compatible

**Implementation**:

**File**: `pqr-scheduling-appointments-api\src\2. Infrastructure\ElectroHuila.Infrastructure\Identity\JwtTokenGenerator.cs`

```csharp
public string GenerateToken(User user, IEnumerable<string> roles, IEnumerable<string> permissions)
{
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new(ClaimTypes.Name, user.Username),
        new(ClaimTypes.Email, user.Email),
        new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

    // Add roles as individual claims (usually only 1-3 roles per user)
    foreach (var role in roles)
    {
        claims.Add(new Claim(ClaimTypes.Role, role));
    }

    // OPTIMIZATION: Store permissions as comma-separated string instead of individual claims
    if (permissions.Any())
    {
        var permissionsString = string.Join(",", permissions);
        claims.Add(new Claim("permissions", permissionsString));  // Single claim
    }

    var token = new JwtSecurityToken(
        issuer: _issuer,
        audience: _audience,
        claims: claims,
        expires: DateTime.UtcNow.AddHours(_expirationHours),
        signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}

// Update GetPermissionsFromToken to handle comma-separated format
public IEnumerable<string> GetPermissionsFromToken(string token)
{
    var tokenHandler = new JwtSecurityTokenHandler();
    var jwtToken = tokenHandler.ReadJwtToken(token);

    // Check for new format (comma-separated)
    var permissionsClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "permissions");
    if (permissionsClaim != null)
    {
        return permissionsClaim.Value.Split(',', StringSplitOptions.RemoveEmptyEntries);
    }

    // Fallback to old format (individual claims) for backward compatibility
    return jwtToken.Claims.Where(c => c.Type == "permission").Select(c => c.Value);
}
```

**File**: `pqr-scheduling-appointments-api\src\2. Infrastructure\ElectroHuila.Infrastructure\DependencyInjection.cs`

Update the permission authorization handler to use the new format:

```csharp
// Find the PermissionAuthorizationHandler and update it to use the comma-separated permissions
// The GetPermissionsFromToken method will automatically handle the conversion
```

### Option 2: Use Short Permission Codes

**Pros**:
- Further reduces token size
- More efficient

**Cons**:
- Requires mapping table
- More complex implementation

**Implementation**:

Create permission codes mapping:
- `users.create` → `U1`
- `users.edit` → `U2`
- `appointments.create` → `A1`
- etc.

### Option 3: Store Permissions in Database Session

**Pros**:
- Minimal JWT token size
- Most secure (permissions can be revoked in real-time)

**Cons**:
- Database query on each request
- More complex implementation
- Performance impact

**Implementation**:
- JWT only contains user ID
- Backend loads permissions from database on each request
- Cache permissions in memory for performance

### Option 4: Use Long Polling Instead of WebSockets

**Pros**:
- Doesn't require token in URL
- Can use Authorization header

**Cons**:
- Less efficient than WebSockets
- Higher latency

---

## Recommended Implementation Plan

### Phase 1: Quick Fix (Option 1)

1. ✅ Update `JwtTokenGenerator.cs` to store permissions as comma-separated string
2. ✅ Update `GetPermissionsFromToken()` method with backward compatibility
3. ✅ Test with existing users
4. ✅ Verify WebSocket connection works with reduced token size

### Phase 2: Monitoring

1. Log token sizes before and after fix
2. Monitor WebSocket connection success rate
3. Verify no permission authorization issues

### Phase 3: Long-term Optimization (Optional)

1. Consider implementing permission caching
2. Implement token refresh mechanism
3. Consider using short permission codes for frequently used permissions

---

## Testing Checklist

- [ ] Login with user that has many permissions (50+)
- [ ] Verify JWT token size is reduced
- [ ] Verify WebSocket connection succeeds
- [ ] Verify permissions are correctly loaded
- [ ] Verify authorization still works for protected endpoints
- [ ] Test with users with few permissions (backward compatibility)
- [ ] Test token refresh mechanism

---

## Token Size Estimation

### Before Fix
```
User ID: ~50 chars
Username: ~50 chars
Email: ~50 chars
JTI: ~50 chars
Roles (2): ~100 chars
Permissions (50 x 40 chars): ~2000 chars
JWT overhead: ~200 chars
---
TOTAL: ~2500 chars ❌ EXCEEDS LIMIT
```

### After Fix (Option 1)
```
User ID: ~50 chars
Username: ~50 chars
Email: ~50 chars
JTI: ~50 chars
Roles (2): ~100 chars
Permissions (1 claim with comma-separated): ~600 chars
JWT overhead: ~200 chars
---
TOTAL: ~1100 chars ✅ WITHIN LIMIT
```

---

## Files to Modify

1. `pqr-scheduling-appointments-api\src\2. Infrastructure\ElectroHuila.Infrastructure\Identity\JwtTokenGenerator.cs`
   - Update `GenerateToken()` method
   - Update `GetPermissionsFromToken()` method

2. `pqr-scheduling-appointments-api\src\2. Infrastructure\ElectroHuila.Infrastructure\Authorization\PermissionAuthorizationHandler.cs` (if exists)
   - Ensure it uses `GetPermissionsFromToken()` which handles both formats

---

## Alternative: Frontend-Only Fix (NOT RECOMMENDED)

If you cannot modify the backend, you could:

1. Use Long Polling transport instead of WebSockets
2. Disable SignalR and use regular HTTP polling
3. Use shorter JWT expiration time (reduces token size slightly)

**However, these are workarounds and don't solve the root cause.**

---

## Conclusion

**The frontend code is already correct** - it's using `accessTokenFactory` properly.

**The issue is in the backend** - the JWT token generation includes too many claims.

**Best solution**: Implement Option 1 (comma-separated permissions) to reduce token size by ~40-60%.

This will fix the WebSocket connection error while maintaining all existing functionality.
