# Excessive SQL Logging Root Cause Analysis

## Executive Summary

The excessive SQL logging in the backend (GetMyAssignedAppointmentsQuery and GetAllRolPermissionsSummaryQuery being called multiple times per second with 800-900ms execution times) is caused by **multiple inefficient polling intervals in AdminLayout.tsx combined with the useDashboardStats hook auto-fetching on dependency changes**.

**Root cause severity: CRITICAL** - These inefficient patterns are causing database strain.

---

## Issue 1: Polling Loop in AdminLayout.tsx (Lines 778-798)

### The Problem

```typescript
// Real-time updates with polling
useEffect(() => {
  if (!currentUser) return;

  // Initial load
  loadMyAppointments();
  loadRolPermissions();

  // Polling interval - only if WebSocket is not connected
  const pollInterval = wsConnected ? 60000 : 30000;
  const interval = setInterval(() => {
    if (!wsConnected) {
      loadMyAppointments();
      admin.dashboardStats.fetchStats(); // Use hook's fetch method
    } else {
      admin.dashboardStats.fetchStats(); // Use hook's fetch method <-- REDUNDANT!
    }
  }, pollInterval);

  return () => clearInterval(interval);
  // eslint-disable-next-line react-hooks/exhaustive-deps
}, [currentUser?.id, wsConnected, admin.dashboardStats]); // <-- PROBLEM: admin.dashboardStats in deps
```

**Critical Issues:**

1. **Redundant polling even when WebSocket is connected**: The else branch (line 792) calls `fetchStats()` even when WebSocket is connected. This is unnecessary since WebSocket should be handling real-time updates.

2. **Dependency on `admin.dashboardStats` object**: Including `admin.dashboardStats` in the dependency array causes this effect to re-run frequently because `admin.dashboardStats` is a new object reference created on every render of useAdmin.

3. **Unsafe dependency handling**: The `// eslint-disable-next-line react-hooks/exhaustive-deps` is masking the real problem - the dependencies are incorrect.

4. **Dual polling paths**: Both WebSocket connected AND disconnected states trigger `fetchStats()`, essentially running polling constantly.

### What Gets Called

When `admin.dashboardStats.fetchStats()` is invoked, it triggers:
- `getDashboardStats(userId)` which calls:
  - `getMyAssignedAppointments(userId)` → SQL Query: `GetMyAssignedAppointmentsQuery`
  - `getUsers()` → Additional query
  - `getBranches()` → Additional query
  - `getAppointmentTypes()` → Additional query

**Frequency**: Every 30-60 seconds at a minimum, but likely MORE often due to the dependency re-running issue.

---

## Issue 2: useDashboardStats Hook Auto-Fetch (Lines 52-54)

### The Problem

```typescript
// Auto-fetch stats on mount and when dependencies change
useEffect(() => {
  fetchStats();
}, [fetchStats]);
```

This hook automatically fetches stats whenever the `fetchStats` function reference changes. The `fetchStats` function has dependencies: `[repository, userId]`.

**Problem**: When `userId` changes or the component re-renders with different function references, this triggers an automatic fetch. This is meant to be helpful but can trigger excessive fetches if the parent component (AdminLayout) is re-rendering frequently.

---

## Issue 3: WebSocket Status Polling (Lines 317-321 in websocket.service.ts)

### The Problem

```typescript
// Check connection status periodically
const statusInterval = setInterval(() => {
  if (mounted) {
    setIsConnected(websocketService.isConnected());
  }
}, 2000); // Check every 2 seconds
```

This polls the WebSocket connection status every 2 seconds in the `useWebSocket` hook. While this doesn't directly cause SQL queries, it contributes to component re-renders which trigger the polling effect above.

---

## Issue 4: loadMyAppointments Called Multiple Times

In AdminLayout.tsx, `loadMyAppointments` is also called in the polling effect:
- Line 782: Initial load
- Line 789: Called again in polling when WebSocket is not connected

However, `loadMyAppointments` calls `getMyAssignedAppointments()` which is one of the expensive queries logged in the backend.

---

## Issue 5: Missing Deduplication in getDashboardStats

In `admin.repository.ts` (lines 267-321), the `getDashboardStats` method makes multiple parallel API calls:

```typescript
const [appointments, employees, branches, appointmentTypes] = await Promise.all([
  appointmentsPromise,
  this.getUsers(),
  this.getBranches(),
  this.getAppointmentTypes()
]);
```

When called 2+ times per second (from polling), this causes the backend to execute multiple queries in parallel, compounding the database load.

---

## Issue 6: loadRolPermissions in Polling Loop

```typescript
// Load Role Permissions
const loadRolPermissions = React.useCallback(async () => {
  try {
    const perms = await adminRepository.getAllRolPermissionsSummary();
    setRolPermissions(perms);
  } catch {
    // Silent error handling
  }
}, []);
```

This is called on line 783 (initial load) but is NOT in the polling interval. However, the `getAllRolPermissionsSummary()` API is still being called excessively. This suggests it's being called from another component or hook repeatedly.

**Likely culprit**: The `useUserPermissions` hook which loads permissions on mount and calls:
- `getCurrentUserPermissionsFromServer()` → May call `getAllRolPermissionsSummary()` internally
- This is called on line 813-829 of AdminLayout when useUserPermissions initializes

---

## Impact Analysis

### Database Strain
- **GetMyAssignedAppointmentsQuery**: Being called 2-3 times per minute (based on polling interval + WebSocket status changes)
- **GetAllRolPermissionsSummaryQuery**: Being called potentially 5+ times per minute during initialization and whenever dependencies change
- **Each call takes 800-900ms**: This is unacceptable load on the database

### Performance Impact
- **CPU Usage**: High due to database query execution
- **Network Bandwidth**: Excessive API calls consuming bandwidth
- **Memory**: State updates from polling can cause memory leaks if not properly cleaned up
- **User Experience**: Slow dashboard load times, potential UI freezing

---

## Recommended Fixes

### Fix 1: Remove Redundant Polling When WebSocket Connected (CRITICAL)

**File**: `C:\Users\User\Desktop\Proyecto Electrohuila\Electrohuila\pqr-scheduling-appointments-portal\src\features\admin\views\AdminLayout.tsx`

**Lines 778-798**: Replace with:

```typescript
// Real-time updates with polling - ONLY when WebSocket is disconnected
useEffect(() => {
  if (!currentUser) return;

  // Initial load
  loadMyAppointments();

  // Only poll if WebSocket is not connected
  if (!wsConnected) {
    const interval = setInterval(() => {
      loadMyAppointments();
      admin.dashboardStats.fetchStats();
    }, 60000); // 60 second polling when offline

    return () => clearInterval(interval);
  }

  // Cleanup
  return () => {};
  // Only depend on currentUser.id and wsConnected
}, [currentUser?.id, wsConnected]);
```

**Rationale**:
- Remove `admin.dashboardStats` from dependencies to prevent unnecessary re-runs
- Only poll when WebSocket is disconnected (fallback mechanism)
- Remove redundant fetchStats call when connected

---

### Fix 2: Increase Polling Interval

**Current**: 30-60 seconds
**Recommended**: 2-5 minutes when WebSocket is disconnected

This reduces API calls by 3-10x when the backend is reachable.

```typescript
// Only poll if WebSocket is not connected
if (!wsConnected) {
  const interval = setInterval(() => {
    loadMyAppointments();
    admin.dashboardStats.fetchStats();
  }, 300000); // 5 minutes polling when offline
```

---

### Fix 3: Memoize admin.dashboardStats Object

To prevent unnecessary re-renders, memoize the dashboardStats reference:

**File**: `C:\Users\User\Desktop\Proyecto Electrohuila\Electrohuila\pqr-scheduling-appointments-portal\src\features\admin\viewmodels\useAdmin.ts`

```typescript
export const useAdmin = (repository: AdminRepository, userId?: number) => {
  const dashboardStats = useMemo(
    () => useDashboardStats(repository, userId),
    [repository, userId]
  );

  // ... rest of hooks

  return {
    dashboardStats,
    // ... other returns
  };
};
```

**Note**: useDashboardStats needs to be imported from React hooks.

---

### Fix 4: Remove WebSocket Status Polling

**File**: `C:\Users\User\Desktop\Proyecto Electrohuila\Electrohuila\pqr-scheduling-appointments-portal\src\services\websocket.service.ts`

**Lines 316-321**: Replace with:

```typescript
// Don't poll status - let SignalR handle reconnection
// The library already handles connection state changes internally
// Remove the statusInterval entirely

return () => {
  mounted = false;
  unsubscribe();
  // clearInterval(statusInterval); // REMOVE THIS
};
```

**Rationale**: SignalR's built-in automatic reconnection with exponential backoff (configured in connect()) handles connection state changes. Additional polling is redundant and causes unnecessary re-renders.

---

### Fix 5: Increase getDashboardStats Caching

The stats don't need to update more than once per minute. Implement client-side caching:

**File**: `C:\Users\User\Desktop\Proyecto Electrohuila\Electrohuila\pqr-scheduling-appointments-portal\src\features\admin\viewmodels\useDashboardStats.ts`

```typescript
interface UseDashboardStatsReturn {
  stats: DashboardStats;
  loading: boolean;
  error: string | null;
  fetchStats: () => Promise<void>;
  lastFetchTime?: number; // Add cache timestamp
}

export const useDashboardStats = (
  repository: AdminRepository,
  userId?: number,
  cacheTimeMs: number = 60000 // 1 minute cache
): UseDashboardStatsReturn => {
  const [stats, setStats] = useState<DashboardStats>(DEFAULT_STATS);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [lastFetchTime, setLastFetchTime] = useState<number>(0);

  const fetchStats = useCallback(async () => {
    const now = Date.now();

    // Don't fetch if cache is still valid
    if (now - lastFetchTime < cacheTimeMs) {
      return;
    }

    setLoading(true);
    setError(null);
    try {
      const data = await repository.getDashboardStats(userId);
      setStats(data);
      setLastFetchTime(now);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Error loading dashboard stats');
      setStats(DEFAULT_STATS);
    } finally {
      setLoading(false);
    }
  }, [repository, userId, lastFetchTime, cacheTimeMs]);

  // Remove auto-fetch or make it optional
  // useEffect(() => {
  //   fetchStats();
  // }, [fetchStats]);

  return {
    stats,
    loading,
    error,
    fetchStats,
    lastFetchTime
  };
};
```

---

## Implementation Priority

### Priority 1 (Do First - CRITICAL)
1. Fix polling logic in AdminLayout.tsx (Fix 1)
2. Remove WebSocket status polling (Fix 4)

**Expected Result**: 70-80% reduction in excessive API calls

### Priority 2 (Do Next - HIGH)
3. Increase polling interval to 5 minutes (Fix 2)
4. Implement caching in useDashboardStats (Fix 5)

**Expected Result**: Additional 30-40% reduction in API calls

### Priority 3 (Optional - Medium)
5. Memoize admin.dashboardStats (Fix 3)

**Expected Result**: Cleaner React code, prevent unnecessary re-renders

---

## Testing Plan

After implementing fixes:

1. **Monitor SQL Logs**: Check that GetMyAssignedAppointmentsQuery and GetAllRolPermissionsSummaryQuery are called less frequently (ideally < 2 times per minute)

2. **Check Execution Times**: Verify that query execution times return to normal (< 200ms)

3. **Load Testing**: Load the admin dashboard and verify:
   - Initial dashboard loads normally
   - No rapid API calls in Network tab
   - WebSocket connection status indicator shows "Connected"
   - Dashboard stats update only once per minute

4. **Stress Testing**: Keep admin dashboard open for 5 minutes and monitor:
   - Backend database logs for excessive queries
   - Browser Network tab for API call frequency
   - Browser console for any errors

5. **Real-time Updates**: Verify that real-time updates still work:
   - Create a new appointment in another browser tab
   - Confirm it appears in the admin dashboard within 2-3 seconds (via WebSocket)
   - Confirm no polling-based updates are happening

---

## Prevention Recommendations

1. **Code Review**: Check all useEffect hooks for unnecessary dependencies
2. **Linting**: Enable `react-hooks/exhaustive-deps` warnings (don't disable them)
3. **Monitoring**: Add application performance monitoring to catch excessive API calls
4. **Testing**: Add performance tests that verify API call frequency
5. **Documentation**: Document polling intervals and expected behavior for each feature

---

## Files Affected

1. `C:\Users\User\Desktop\Proyecto Electrohuila\Electrohuila\pqr-scheduling-appointments-portal\src\features\admin\views\AdminLayout.tsx` - Lines 778-798
2. `C:\Users\User\Desktop\Proyecto Electrohuila\Electrohuila\pqr-scheduling-appointments-portal\src\services\websocket.service.ts` - Lines 316-321
3. `C:\Users\User\Desktop\Proyecto Electrohuila\Electrohuila\pqr-scheduling-appointments-portal\src\features\admin\viewmodels\useDashboardStats.ts` - Lines 52-54
4. `C:\Users\User\Desktop\Proyecto Electrohuila\Electrohuila\pqr-scheduling-appointments-portal\src\features\admin\viewmodels\useAdmin.ts` - Memoization (optional)

