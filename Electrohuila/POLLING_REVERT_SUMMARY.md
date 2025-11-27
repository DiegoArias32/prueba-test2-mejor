# Polling Issue - Revert Summary

Date: 2025-11-25
Status: REVERTED - Code restored to previous polling behavior

## Files Reverted

### 1. AdminLayout.tsx
**Location:** `pqr-scheduling-appointments-portal/src/features/admin/views/AdminLayout.tsx`

**Changes Reverted (Lines 777-793):**
- **BEFORE (Fixed version):**
  - Conditional polling: Only polls when WebSocket is disconnected
  - 5-minute polling interval when offline
  - Dependency: `[currentUser?.id, wsConnected]`
  - Smart fallback mechanism

- **AFTER (Reverted version):**
  - Continuous polling regardless of WebSocket connection status
  - 1-minute polling interval
  - Dependency: `[currentUser?.id]` (removed wsConnected dependency)
  - Always polls with `loadMyAppointments()` and `admin.dashboardStats.fetchStats()`

### 2. useDashboardStats.ts
**Location:** `pqr-scheduling-appointments-portal/src/features/admin/viewmodels/useDashboardStats.ts`

**Changes Reverted:**
- **BEFORE (Cached version):**
  - Client-side caching with 60-second cache time (STATS_CACHE_TIME_MS)
  - Manual check: `if (now - lastFetchTimeRef.current < STATS_CACHE_TIME_MS) return;`
  - lastFetchTimeRef tracking for cache expiration
  - Auto-fetch on mount intentionally removed
  - Return type included `lastFetchTime?: number`
  - Comments noting explicit fetch calls only

- **AFTER (Reverted version):**
  - Removed caching logic entirely
  - Removed lastFetchTimeRef and cache time checking
  - Restored auto-fetch on component mount via useEffect hook
  - Simplified return type (removed lastFetchTime)
  - Direct API calls without cache validation
  - Imports restored: `useEffect` added back to imports

### 3. websocket.service.ts
**Location:** `pqr-scheduling-appointments-portal/src/services/websocket.service.ts`

**Status:** No changes required - file was not modified

## What This Reverts

The original polling fixes implemented:
1. Smart conditional polling based on WebSocket connection status
2. Intelligent caching to prevent excessive API calls
3. Graceful fallback mechanism

This revert returns to:
1. Continuous polling every 60 seconds (1 minute)
2. No client-side caching
3. Auto-fetch behavior that may result in redundant API calls

## Behavior Changes

| Aspect | Fixed Version | Reverted Version |
|--------|---------------|------------------|
| Polling Interval | 5 minutes (offline only) | 1 minute (always) |
| Condition | Only when WebSocket disconnected | Always active |
| Caching | 60-second client-side cache | No caching |
| Auto-fetch | Disabled | Enabled |
| API Calls | Minimized | More frequent |
| Dependencies | wsConnected included | wsConnected removed |

## Files Successfully Restored

- ✓ AdminLayout.tsx (Lines 777-793)
- ✓ useDashboardStats.ts (Full hook)
- ✓ websocket.service.ts (No changes needed)

## Testing Recommendations

After this revert, verify:
1. Dashboard stats update every 1 minute
2. Appointments list refreshes continuously
3. API call frequency increases to previous levels
4. No WebSocket dependency for polling logic
5. Auto-fetch occurs on component mount

## Notes

- Only one commit in git history (6e2ea04 prueba)
- No previous commit history available for reference
- Revert based on fixing logic reversal
- Original code restored to basic polling behavior
