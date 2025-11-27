# Polling Revert - Detailed Code Changes

## Change Summary

The polling fixes have been reverted in 2 files:

1. **AdminLayout.tsx** - Polling logic reverted
2. **useDashboardStats.ts** - Caching removed, auto-fetch restored
3. **websocket.service.ts** - No changes

---

## File 1: AdminLayout.tsx

### Location: Lines 777-793

### BEFORE REVERT (With Fixes):
```typescript
  // Real-time updates with polling - ONLY when WebSocket is disconnected
  // This is a fallback mechanism. WebSocket should handle real-time updates.
  useEffect(() => {
    if (!currentUser?.id) return;

    // Initial load
    loadMyAppointments();
    admin.dashboardStats.fetchStats(); // Initial dashboard stats

    // Only poll if WebSocket is not connected (fallback mechanism)
    if (!wsConnected) {
      const interval = setInterval(() => {
        loadMyAppointments();
        admin.dashboardStats.fetchStats();
      }, 300000); // 5 minute polling when WebSocket is offline

      return () => clearInterval(interval);
    }

    // When WebSocket is connected, rely on real-time updates
    return () => {};
  }, [currentUser?.id, wsConnected]);
```

### AFTER REVERT (Original):
```typescript
  // Real-time updates with polling
  // This polls continuously regardless of WebSocket connection status
  useEffect(() => {
    if (!currentUser?.id) return;

    // Initial load
    loadMyAppointments();
    admin.dashboardStats.fetchStats();

    // Always poll for updates
    const interval = setInterval(() => {
      loadMyAppointments();
      admin.dashboardStats.fetchStats();
    }, 60000); // Poll every 1 minute

    return () => clearInterval(interval);
  }, [currentUser?.id]);
```

### Key Differences:

| Item | Before Revert | After Revert |
|------|--------------|--------------|
| Comment | "ONLY when WebSocket disconnected" | "Polls continuously" |
| Condition | `if (!wsConnected) { ... }` | Always executes |
| Interval | 300000 ms (5 minutes) | 60000 ms (1 minute) |
| Return statement | Conditional empty return | Always returns clearInterval |
| Dependencies | `[currentUser?.id, wsConnected]` | `[currentUser?.id]` |

---

## File 2: useDashboardStats.ts

### Location: Full file

### BEFORE REVERT (With Caching):
```typescript
/**
 * Hook - Dashboard Statistics
 * Handles fetching and managing dashboard statistics
 *
 * PERFORMANCE NOTE: This hook now includes client-side caching to prevent excessive API calls.
 * Stats are cached for 60 seconds by default. Repeated calls within the cache window are ignored.
 */

'use client';

import { useState, useCallback, useRef } from 'react';
import { AdminRepository } from '../repositories/admin.repository';
import { DashboardStatsDto } from '../models/admin.models';

type DashboardStats = DashboardStatsDto;

interface UseDashboardStatsReturn {
  stats: DashboardStats;
  loading: boolean;
  error: string | null;
  fetchStats: () => Promise<void>;
  lastFetchTime?: number;
}

const DEFAULT_STATS: DashboardStats = {
  totalAppointments: 0,
  completedAppointments: 0,
  pendingAppointments: 0,
  totalEmployees: 0,
  totalBranches: 0,
  totalAppointmentTypes: 0,
  myAppointmentsToday: 0,
  myPendingAppointments: 0
};

// Cache configuration: 60 seconds
const STATS_CACHE_TIME_MS = 60000;

export const useDashboardStats = (repository: AdminRepository, userId?: number): UseDashboardStatsReturn => {
  const [stats, setStats] = useState<DashboardStats>(DEFAULT_STATS);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const lastFetchTimeRef = useRef<number>(0);

  const fetchStats = useCallback(async () => {
    const now = Date.now();

    // Prevent excessive API calls: Only fetch if cache is expired
    if (now - lastFetchTimeRef.current < STATS_CACHE_TIME_MS) {
      return;
    }

    setLoading(true);
    setError(null);
    try {
      const data = await repository.getDashboardStats(userId);
      setStats(data);
      lastFetchTimeRef.current = now;
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Error loading dashboard stats');
      setStats(DEFAULT_STATS);
    } finally {
      setLoading(false);
    }
  }, [repository, userId]);

  // NOTE: Auto-fetch on mount is intentionally removed to prevent excessive API calls
  // Call fetchStats() explicitly when needed (e.g., from AdminLayout or on user interaction)
  // See AdminLayout.tsx for polling logic

  return {
    stats,
    loading,
    error,
    fetchStats,
    lastFetchTime: lastFetchTimeRef.current
  };
};
```

### AFTER REVERT (Original):
```typescript
/**
 * Hook - Dashboard Statistics
 * Handles fetching and managing dashboard statistics
 */

'use client';

import { useState, useCallback, useEffect } from 'react';
import { AdminRepository } from '../repositories/admin.repository';
import { DashboardStatsDto } from '../models/admin.models';

type DashboardStats = DashboardStatsDto;

interface UseDashboardStatsReturn {
  stats: DashboardStats;
  loading: boolean;
  error: string | null;
  fetchStats: () => Promise<void>;
}

const DEFAULT_STATS: DashboardStats = {
  totalAppointments: 0,
  completedAppointments: 0,
  pendingAppointments: 0,
  totalEmployees: 0,
  totalBranches: 0,
  totalAppointmentTypes: 0,
  myAppointmentsToday: 0,
  myPendingAppointments: 0
};

export const useDashboardStats = (repository: AdminRepository, userId?: number): UseDashboardStatsReturn => {
  const [stats, setStats] = useState<DashboardStats>(DEFAULT_STATS);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchStats = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await repository.getDashboardStats(userId);
      setStats(data);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Error loading dashboard stats');
      setStats(DEFAULT_STATS);
    } finally {
      setLoading(false);
    }
  }, [repository, userId]);

  // Auto-fetch on mount
  useEffect(() => {
    fetchStats();
  }, [fetchStats]);

  return {
    stats,
    loading,
    error,
    fetchStats
  };
};
```

### Key Differences:

| Item | Before Revert | After Revert |
|------|--------------|--------------|
| Imports | `useCallback, useRef` | `useCallback, useEffect` |
| Cache constant | `STATS_CACHE_TIME_MS = 60000` | Removed |
| Cache ref | `lastFetchTimeRef` | Removed |
| Return type | `lastFetchTime?: number` | Removed |
| Cache validation | `if (now - lastFetchTimeRef.current < STATS_CACHE_TIME_MS) return;` | Removed |
| Auto-fetch | None (disabled) | `useEffect` hook with fetchStats() |
| Performance note | Extensive comment about caching | Simple auto-fetch comment |

---

## File 3: websocket.service.ts

**Status:** No changes required
**File remains:** Unchanged from original state
**Location:** `pqr-scheduling-appointments-portal/src/services/websocket.service.ts`

---

## Migration Path

### If you want to restore the fixes later:

#### Step 1: AdminLayout.tsx (Lines 777-793)
Add back the WebSocket connection condition:
```typescript
useEffect(() => {
  if (!currentUser?.id) return;

  loadMyAppointments();
  admin.dashboardStats.fetchStats();

  // Restore conditional polling
  if (!wsConnected) {
    const interval = setInterval(() => {
      loadMyAppointments();
      admin.dashboardStats.fetchStats();
    }, 300000); // 5 minutes

    return () => clearInterval(interval);
  }

  return () => {};
}, [currentUser?.id, wsConnected]); // Re-add wsConnected
```

#### Step 2: useDashboardStats.ts
Re-add caching logic:
```typescript
import { useRef } from 'react'; // Add useRef back

const STATS_CACHE_TIME_MS = 60000;

const lastFetchTimeRef = useRef<number>(0);

// In fetchStats callback:
const now = Date.now();
if (now - lastFetchTimeRef.current < STATS_CACHE_TIME_MS) {
  return;
}
lastFetchTimeRef.current = now;

// Remove auto-fetch useEffect
// Add lastFetchTime back to return
```

---

## Verification

To verify the revert was successful, check these files:

### AdminLayout.tsx - Lines 777-793
Should contain:
- `// Poll every 1 minute` comment
- `60000` interval value
- `[currentUser?.id]` dependency (no wsConnected)
- No conditional polling logic

### useDashboardStats.ts - Full file
Should contain:
- `useEffect` import from React
- `useEffect(() => { fetchStats(); }, [fetchStats]);`
- No `STATS_CACHE_TIME_MS` constant
- No `lastFetchTimeRef` variable
- No `lastFetchTime` in return type

---

**Date Completed:** 2025-11-25
**Status:** REVERTED SUCCESSFULLY
