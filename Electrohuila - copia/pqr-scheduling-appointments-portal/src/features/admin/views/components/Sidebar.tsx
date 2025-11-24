/**
 * Sidebar Component
 * Menú lateral estático para el panel administrativo
 */

'use client';

import React from 'react';
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
  FiSliders
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
  const menuItems = [
    {
      id: 'dashboard',
      name: 'Panel Principal',
      icon: FiHome,
      color: 'text-blue-500',
      bgColor: 'bg-blue-50',
      hoverColor: 'hover:bg-blue-100'
    },
    {
      id: 'my-appointments',
      name: 'Mis Citas',
      icon: FiBell,
      color: 'text-orange-500',
      bgColor: 'bg-orange-50',
      hoverColor: 'hover:bg-orange-100',
      badge: notificationCount > 0 ? notificationCount : undefined
    },
    {
      id: 'citas',
      name: 'Citas',
      icon: FiCalendar,
      color: 'text-green-500',
      bgColor: 'bg-green-50',
      hoverColor: 'hover:bg-green-100'
    },
    {
      id: 'empleados',
      name: 'Empleados',
      icon: FiUsers,
      color: 'text-purple-500',
      bgColor: 'bg-purple-50',
      hoverColor: 'hover:bg-purple-100'
    },
    {
      id: 'roles',
      name: 'Roles',
      icon: FiShield,
      color: 'text-indigo-500',
      bgColor: 'bg-indigo-50',
      hoverColor: 'hover:bg-indigo-100'
    },
    {
      id: 'sedes',
      name: 'Sedes',
      icon: FiMapPin,
      color: 'text-red-500',
      bgColor: 'bg-red-50',
      hoverColor: 'hover:bg-red-100'
    },
    {
      id: 'tipos-cita',
      name: 'Tipos de Cita',
      icon: FiFileText,
      color: 'text-yellow-500',
      bgColor: 'bg-yellow-50',
      hoverColor: 'hover:bg-yellow-100'
    },
    {
      id: 'horas-disponibles',
      name: 'Horas Disponibles',
      icon: FiClock,
      color: 'text-teal-500',
      bgColor: 'bg-teal-50',
      hoverColor: 'hover:bg-teal-100'
    },
    {
      id: 'festivos',
      name: 'Festivos',
      icon: FiCalendar,
      color: 'text-amber-500',
      bgColor: 'bg-amber-50',
      hoverColor: 'hover:bg-amber-100'
    },
    {
      id: 'permisos',
      name: 'Permisos',
      icon: FiSettings,
      color: 'text-pink-500',
      bgColor: 'bg-pink-50',
      hoverColor: 'hover:bg-pink-100'
    },
    {
      id: 'plantillas',
      name: 'Plantillas',
      icon: FiFileText,
      color: 'text-violet-500',
      bgColor: 'bg-violet-50',
      hoverColor: 'hover:bg-violet-100'
    },
    {
      id: 'settings',
      name: 'Configuración',
      icon: FiSliders,
      color: 'text-cyan-500',
      bgColor: 'bg-cyan-50',
      hoverColor: 'hover:bg-cyan-100'
    }
  ];

  // Filtrar items según permisos disponibles
  const visibleItems = menuItems.filter(item => {
    if (item.id === 'dashboard' || item.id === 'my-appointments' || item.id === 'settings') return true;
    return availableTabs.some(tab => tab.id === item.id);
  });

  return (
    <aside className="fixed left-0 top-0 h-screen w-64 bg-gradient-to-b from-[#203461] to-[#1797D5] text-white shadow-2xl overflow-y-auto">
      {/* Header */}
      <div className="p-6 border-b border-white/10">
        <div className="flex items-center space-x-3">
          <div className="w-12 h-12 bg-white/20 rounded-xl flex items-center justify-center backdrop-blur-sm">
            <FiShield className="w-6 h-6 text-white" />
          </div>
          <div>
            <h1 className="text-xl font-bold">Panel Admin</h1>
            <p className="text-xs text-blue-200">ElectroHuila</p>
          </div>
        </div>
      </div>

      {/* User Info */}
      {currentUser && (
        <div className="p-4 border-b border-white/10">
          <div className="flex items-center space-x-3">
            <div className="w-10 h-10 bg-white/20 rounded-full flex items-center justify-center">
              <FiUser className="w-5 h-5" />
            </div>
            <div className="flex-1 min-w-0">
              <p className="text-sm font-semibold truncate">{currentUser.username}</p>
              <p className="text-xs text-blue-200 truncate">{currentUser.email}</p>
            </div>
          </div>
        </div>
      )}

      {/* Navigation */}
      <nav className="p-4 space-y-2 pb-24 overflow-y-auto" style={{ maxHeight: 'calc(100vh - 250px)' }}>
        {visibleItems.map((item) => {
          const Icon = item.icon;
          const isActive = activeTab === item.id;

          return (
            <button
              key={item.id}
              onClick={() => onTabChange(item.id as TabType | 'dashboard' | 'my-appointments')}
              className={`
                w-full flex items-center justify-between px-4 py-3 rounded-xl
                transition-all duration-200 group relative overflow-hidden
                ${isActive
                  ? 'bg-white text-[#203461] shadow-lg scale-105'
                  : 'text-white hover:bg-white/10'
                }
              `}
            >
              <div className="flex items-center space-x-3 relative z-10">
                <div className={`
                  w-10 h-10 rounded-lg flex items-center justify-center
                  transition-all duration-200
                  ${isActive
                    ? `${item.bgColor} ${item.color}`
                    : 'bg-white/10 text-white group-hover:bg-white/20'
                  }
                `}>
                  <Icon className="w-5 h-5" />
                </div>
                <span className={`
                  text-sm font-medium
                  ${isActive ? 'font-bold' : ''}
                `}>
                  {item.name}
                </span>
              </div>

              {/* Badge for notifications */}
              {item.badge !== undefined && item.badge > 0 && (
                <span className="relative z-10 min-w-[24px] h-6 px-2 bg-red-500 text-white text-xs font-bold rounded-full flex items-center justify-center">
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
      </nav>

      {/* Logout Button */}
      <div className="absolute bottom-0 left-0 right-0 p-4 border-t border-white/10 bg-[#203461]/50 backdrop-blur-sm">
        <button
          onClick={onLogout}
          className="w-full flex items-center space-x-3 px-4 py-3 rounded-xl text-white hover:bg-red-500/20 transition-all duration-200 group"
        >
          <div className="w-10 h-10 bg-red-500/20 rounded-lg flex items-center justify-center group-hover:bg-red-500 transition-all duration-200">
            <FiLogOut className="w-5 h-5" />
          </div>
          <span className="text-sm font-medium">Cerrar Sesión</span>
        </button>
      </div>
    </aside>
  );
};
