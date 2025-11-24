/**
 * @file PermissionsView.tsx
 * @description Permissions management view wrapper
 */

import React from 'react';
import { RolDto, FormPermissionDto, RolPermissionSummaryDto } from '@/services/api';
import { PermissionsManager, type PermissionUpdate } from '../components';

interface PermissionsViewProps {
  roles: RolDto[];
  permissions: FormPermissionDto[];
  rolPermissions: RolPermissionSummaryDto[];
  loading: boolean;
  onUpdatePermission: (rolId: number, formCode: string, permissionData: PermissionUpdate) => Promise<void>;
}

export const PermissionsView: React.FC<PermissionsViewProps> = ({
  roles,
  permissions,
  rolPermissions,
  loading,
  onUpdatePermission
}) => {
  return (
    <div className="bg-white rounded-2xl shadow-lg border border-gray-100 overflow-hidden">
      <div className="p-6">
        <PermissionsManager
          roles={roles}
          permissions={permissions}
          rolPermissions={rolPermissions}
          onUpdatePermission={onUpdatePermission}
          loading={loading}
        />
      </div>
    </div>
  );
};
