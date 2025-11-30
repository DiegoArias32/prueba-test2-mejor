/**
 * Sidebar Component
 * Menú lateral colapsable con grupos para el panel administrativo
 */

'use client';

import React, { useState } from 'react';
import {
  FiHome,
  FiCalendar,
  FiUsers,
  FiShield,
  FiMapPin,
  FiClock,
  FiFileText,
  FiSettings,
  FiLogOut,
  FiBell,
  FiUser,
  FiSliders,
  FiChevronDown,
  FiChevronRight,
  FiMenu
} from 'react-icons/fi';
import { TabType } from '../../models/admin.models';

interface SidebarProps {
  activeTab: TabType | 'dashboard' | 'my-appointments';
  onTabChange: (tab: TabType | 'dashboard' | 'my-appointments') => void;
  availableTabs: Array<{ id: string; name: string }>;
  currentUser: { username: string; email: string } | null;
  onLogout: () => void;
  notificationCount?: number;
}

export const Sidebar: React.FC<SidebarProps> = ({
  activeTab,
  onTabChange,
  availableTabs,
  currentUser,
  onLogout,
  notificationCount = 0
}) => {
  const [isCollapsed, setIsCollapsed] = useState(false);
  const [expandedGroups, setExpandedGroups] = useState<string[]>(['appointments', 'users', 'config']);

  const toggleGroup = (groupId: string) => {
    setExpandedGroups(prev =>
      prev.includes(groupId)
        ? prev.filter(id => id !== groupId)
        : [...prev, groupId]
    );
  };

  // Definir grupos de menú
  const menuGroups = [
    {
      id: 'main',
      name: 'Principal',
      items: [
        {
          id: 'dashboard',
          name: 'Panel Principal',
          icon: FiHome,
          color: 'text-blue-500',
          bgColor: 'bg-blue-50',
          alwaysVisible: true
        },
        {
          id: 'my-appointments',
          name: 'Mis Citas',
          icon: FiBell,
          color: 'text-orange-500',
          bgColor: 'bg-orange-50',
          badge: notificationCount > 0 ? notificationCount : undefined,
          alwaysVisible: true
        }
      ]
    },
    {
      id: 'appointments',
      name: 'Gestión de Citas',
      icon: FiCalendar,
      items: [
        {
          id: 'citas',
          name: 'Citas',
          icon: FiCalendar,
          color: 'text-green-500',
          bgColor: 'bg-green-50'
        },
        {
          id: 'tipos-cita',
          name: 'Tipos de Cita',
          icon: FiFileText,
          color: 'text-yellow-500',
          bgColor: 'bg-yellow-50'
        },
        {
          id: 'horas-disponibles',
          name: 'Horas Disponibles',
          icon: FiClock,
          color: 'text-teal-500',
          bgColor: 'bg-teal-50'
        }
      ]
    },
    {
      id: 'users',
      name: 'Gestión de Usuarios',
      icon: FiUsers,
      items: [
        {
          id: 'empleados',
          name: 'Empleados',
          icon: FiUsers,
          color: 'text-purple-500',
          bgColor: 'bg-purple-50'
        },
        {
          id: 'roles',
          name: 'Roles',
          icon: FiShield,
          color: 'text-indigo-500',
          bgColor: 'bg-indigo-50'
        },
        {
          id: 'permisos',
          name: 'Permisos',
          icon: FiSettings,
          color: 'text-pink-500',
          bgColor: 'bg-pink-50'
        }
      ]
    },
    {
      id: 'config',
      name: 'Configuración',
      icon: FiSettings,
      items: [
        {
          id: 'sedes',
          name: 'Sedes',
          icon: FiMapPin,
          color: 'text-red-500',
          bgColor: 'bg-red-50'
        },
        {
          id: 'festivos',
          name: 'Festivos',
          icon: FiCalendar,
          color: 'text-amber-500',
          bgColor: 'bg-amber-50'
        },
        {
          id: 'settings',
          name: 'Sistema',
          icon: FiSliders,
          color: 'text-cyan-500',
          bgColor: 'bg-cyan-50'
        }
      ]
    }
  ];

  // Filtrar grupos y items según permisos
  const visibleGroups = menuGroups.map(group => ({
    ...group,
    items: group.items.filter(item => {
      if (item.alwaysVisible) return true;
      return availableTabs.some(tab => tab.id === item.id);
    })
  })).filter(group => group.items.length > 0);

  return (
    <aside className={`
      fixed left-0 top-0 h-screen bg-linear-to-b from-[#203461] to-[#1797D5] text-white shadow-2xl overflow-hidden
      transition-all duration-300
      ${isCollapsed ? 'w-20' : 'w-64'}
    `}>
      {/* Header */}
      <div className="p-4 border-b border-white/10">
        <div className="flex items-center justify-between">
          <div className={`flex items-center space-x-3 ${isCollapsed ? 'justify-center w-full' : ''}`}>
            <div className="w-10 h-10 bg-white/20 rounded-xl flex items-center justify-center backdrop-blur-sm shrink-0">
              <FiShield className="w-5 h-5 text-white" />
            </div>
            {!isCollapsed && (
              <div>
                <h1 className="text-lg font-bold">Panel Admin</h1>
                <p className="text-xs text-blue-200">ElectroHuila</p>
              </div>
            )}
          </div>
          <button
            onClick={() => setIsCollapsed(!isCollapsed)}
            className={`p-2 hover:bg-white/10 rounded-lg transition-colors ${isCollapsed ? 'hidden' : ''}`}
          >
            <FiMenu className="w-5 h-5" />
          </button>
        </div>
        {isCollapsed && (
          <button
            onClick={() => setIsCollapsed(false)}
            className="w-full mt-2 p-2 hover:bg-white/10 rounded-lg transition-colors"
          >
            <FiChevronRight className="w-5 h-5 mx-auto" />
          </button>
        )}
      </div>

      {/* User Info */}
      {currentUser && !isCollapsed && (
        <div className="p-3 border-b border-white/10">
          <div className="flex items-center space-x-2">
            <div className="w-8 h-8 bg-white/20 rounded-full flex items-center justify-center shrink-0">
              <FiUser className="w-4 h-4" />
            </div>
            <div className="flex-1 min-w-0">
              <p className="text-xs font-semibold truncate">{currentUser.username}</p>
              <p className="text-[10px] text-blue-200 truncate">{currentUser.email}</p>
            </div>
          </div>
        </div>
      )}

      {/* Navigation */}
      <nav className="p-3 space-y-1 overflow-y-auto" style={{ maxHeight: isCollapsed ? 'calc(100vh - 180px)' : 'calc(100vh - 240px)' }}>
        {visibleGroups.map((group) => {
          const isGroupExpanded = expandedGroups.includes(group.id);
          const GroupIcon = group.icon || FiSettings;

          return (
            <div key={group.id} className="space-y-1">
              {/* Group Header - Solo para grupos con más de 1 item */}
              {group.items.length > 1 && !isCollapsed && group.id !== 'main' && (
                <button
                  onClick={() => toggleGroup(group.id)}
                  className="w-full flex items-center justify-between px-3 py-2 text-xs font-semibold text-blue-200 hover:text-white transition-colors"
                >
                  <div className="flex items-center space-x-2">
                    <GroupIcon className="w-4 h-4" />
                    <span>{group.name}</span>
                  </div>
                  {isGroupExpanded ? (
                    <FiChevronDown className="w-4 h-4" />
                  ) : (
                    <FiChevronRight className="w-4 h-4" />
                  )}
                </button>
              )}

              {/* Group Items */}
              {(isGroupExpanded || isCollapsed || group.id === 'main' || group.items.length === 1) && group.items.map((item) => {
                const Icon = item.icon;
                const isActive = activeTab === item.id;

                return (
                  <button
                    key={item.id}
                    onClick={() => onTabChange(item.id as TabType | 'dashboard' | 'my-appointments')}
                    className={`
                      w-full flex items-center justify-between py-2 rounded-lg
                      transition-all duration-200 group relative overflow-hidden
                      ${isCollapsed ? 'px-0 justify-center' : 'px-3'}
                      ${isActive
                        ? 'bg-white text-[#203461] shadow-lg'
                        : 'text-white hover:bg-white/10'
                      }
                    `}
                    title={isCollapsed ? item.name : undefined}
                  >
                    <div className={`flex items-center relative z-10 ${isCollapsed ? '' : 'space-x-2'}`}>
                      <div className={`
                        w-8 h-8 rounded-lg flex items-center justify-center shrink-0
                        transition-all duration-200
                        ${isActive
                          ? `${item.bgColor} ${item.color}`
                          : 'bg-white/10 text-white group-hover:bg-white/20'
                        }
                      `}>
                        <Icon className="w-4 h-4" />
                      </div>
                      {!isCollapsed && (
                        <span className={`text-xs font-medium ${isActive ? 'font-bold' : ''}`}>
                          {item.name}
                        </span>
                      )}
                    </div>

                    {/* Badge for notifications */}
                    {item.badge !== undefined && item.badge > 0 && !isCollapsed && (
                      <span className="relative z-10 min-w-6 h-5 px-1.5 bg-red-500 text-white text-[10px] font-bold rounded-full flex items-center justify-center">
                        {item.badge > 99 ? '99+' : item.badge}
                      </span>
                    )}

                    {/* Active indicator */}
                    {isActive && (
                      <div className="absolute left-0 top-0 bottom-0 w-1 bg-[#FF7A00] rounded-r-full" />
                    )}
                  </button>
                );
              })}
            </div>
          );
        })}
      </nav>

      {/* Logout Button */}
      <div className="absolute bottom-0 left-0 right-0 p-3 border-t border-white/10 bg-[#203461]/50 backdrop-blur-sm">
        <button
          onClick={onLogout}
          className={`
            w-full flex items-center py-2 rounded-lg text-white hover:bg-red-500/20 transition-all duration-200 group
            ${isCollapsed ? 'px-0 justify-center' : 'px-3 space-x-2'}
          `}
          title={isCollapsed ? 'Cerrar Sesión' : undefined}
        >
          <div className="w-8 h-8 bg-red-500/20 rounded-lg flex items-center justify-center group-hover:bg-red-500 transition-all duration-200 shrink-0">
            <FiLogOut className="w-4 h-4" />
          </div>
          {!isCollapsed && <span className="text-xs font-medium">Cerrar Sesión</span>}
        </button>
      </div>
    </aside>
  );
};
