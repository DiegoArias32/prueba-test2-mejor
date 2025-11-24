/**
 * @file AdminView.tsx
 * @description Main Admin View - Simplified wrapper around AdminLayout
 * @module features/admin/views
 *
 * REFACTORED ARCHITECTURE:
 * This file has been refactored from a 2,118-line monolith into a modular structure:
 *
 * - AdminLayout.tsx: Main layout container with routing logic
 * - users/UsersView.tsx: Users management
 * - roles/RolesView.tsx: Roles management
 * - appointments/AppointmentsView.tsx: Appointments management
 * - branches/BranchesView.tsx: Branches management
 * - types/TypesView.tsx: Appointment types management
 * - times/TimesView.tsx: Available times management
 * - permissions/PermissionsView.tsx: Permissions management
 * - components/shared/: Reusable table components (pagination, search, sorting)
 * - utils/tableUtils.ts: Shared utility functions
 *
 * Each view is self-contained, handles its own state, and is max 200-300 lines.
 */

'use client';

import React from 'react';
import { AdminLayout } from './AdminLayout';

/**
 * Main AdminView Component - Entry point for admin panel
 */
export const AdminView: React.FC = () => {
  return <AdminLayout />;
};
