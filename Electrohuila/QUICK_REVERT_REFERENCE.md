# Quick Revert Reference - Polling Fixes

## Status: COMPLETE

---

## What Changed?

### 1. AdminLayout.tsx (Lines 777-793)
**Before:** Smart polling - only when WebSocket disconnected
**After:** Continuous polling - every 60 seconds always

```diff
- const pollInterval = wsConnected ? 60000 : 30000;
- if (!wsConnected) {
+ const interval = setInterval(() => {
+   loadMyAppointments();
+   admin.dashboardStats.fetchStats();
+ }, 60000); // Always 60 seconds

- }, [currentUser?.id, wsConnected]); // Was dependent on wsConnected
+ }, [currentUser?.id]); // Only depends on userId
```

### 2. useDashboardStats.ts
**Before:** Caching enabled (60-second cache)
**After:** No caching, auto-fetch on mount

```diff
- const STATS_CACHE_TIME_MS = 60000;
- const lastFetchTimeRef = useRef<number>(0);

+ useEffect(() => {
+   fetchStats();
+ }, [fetchStats]);

- lastFetchTime: lastFetchTimeRef.current
+ // Removed from return
```

### 3. websocket.service.ts
**Status:** No changes

---

## Key Differences

| Feature | Reverted | Original |
|---------|----------|----------|
| Polling Interval | 60 sec (always) | 5 min (offline) / 1 min (online) |
| Caching | None | 60 seconds |
| Auto-fetch | Yes | No |
| WebSocket Dependent | No | Yes |
| API Calls/Minute | 1 | 0.2 - 1 |

---

## Files Modified

1. ✓ `pqr-scheduling-appointments-portal/src/features/admin/views/AdminLayout.tsx`
2. ✓ `pqr-scheduling-appointments-portal/src/features/admin/viewmodels/useDashboardStats.ts`
3. ✓ `pqr-scheduling-appointments-portal/src/services/websocket.service.ts` (no changes)

---

## Testing

1. Open admin dashboard
2. Open browser DevTools (F12) → Network tab
3. Look for `/api/v1/admin/dashboard/stats` calls
4. Should see one every 60 seconds
5. Disconnect WebSocket → polling should continue
6. Refresh page → auto-fetch should trigger immediately

---

## API Call Pattern After Revert

```
Time 0:00 - Initial load + auto-fetch
Time 1:00 - Poll
Time 2:00 - Poll
Time 3:00 - Poll
...continues every 60 seconds
```

Even if WebSocket disconnects → polling continues unchanged.

---

## Performance Impact

- API calls increase (more frequent)
- Server load increases
- Bandwidth usage increases
- Client caching removed (always fresh data)

---

## To Re-apply Fixes Later

1. Add cache validation to `useDashboardStats.ts`
2. Add `wsConnected` condition to polling in `AdminLayout.tsx`
3. Restore 5-minute / 1-minute dual interval
4. Disable auto-fetch in hook
5. Re-add `lastFetchTimeRef` for cache tracking

---

**Revert Completed:** 2025-11-25
**Status:** Ready for Testing
