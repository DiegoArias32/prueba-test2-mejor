/**
 * Hook - Dashboard Statistics
 * Handles fetching and managing dashboard statistics
 */

'use client';

import { useState, useCallback } from 'react';
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

  return {
    stats,
    loading,
    error,
    fetchStats
  };
};
