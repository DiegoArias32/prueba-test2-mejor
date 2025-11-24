/**
 * @file SystemSettingsView.tsx
 * @description System settings management view
 * @module features/admin/views/settings
 */

'use client';

import React, { useState, useEffect, useMemo } from 'react';
import {
  FiSettings,
  FiCalendar,
  FiBell,
  FiClock,
  FiPlus,
  FiSearch,
  FiRotateCcw,
  FiAlertCircle,
  FiLoader,
} from 'react-icons/fi';
import { SettingCard } from '../../components/SettingCard';
import { SystemSettingModal } from '../components/modals/SystemSettingModal';
import type {
  SystemSettingDto,
  CreateSystemSettingDto,
  SettingCategory,
  SettingCategoryConfig,
} from '@/services/system/system-setting.types';
import { SystemSettingService } from '@/services/system/system-setting.service';

// Setting categories configuration
const SETTING_CATEGORIES: SettingCategoryConfig[] = [
  {
    id: 'appointments',
    label: 'Citas',
    description: 'Configuraciones relacionadas con la gestión de citas',
    icon: 'calendar',
    settings: [
      'MAX_APPOINTMENTS_PER_DAY',
      'APPOINTMENT_CANCELLATION_HOURS',
      'MAX_RESCHEDULE_ATTEMPTS',
    ],
  },
  {
    id: 'notifications',
    label: 'Notificaciones',
    description: 'Configuraciones de notificaciones por email y SMS',
    icon: 'bell',
    settings: [
      'EMAIL_NOTIFICATIONS_ENABLED',
      'SMS_NOTIFICATIONS_ENABLED',
      'APPOINTMENT_REMINDER_HOURS',
    ],
  },
  {
    id: 'businessHours',
    label: 'Horarios de Atención',
    description: 'Horarios de operación del sistema',
    icon: 'clock',
    settings: ['BUSINESS_HOURS_START', 'BUSINESS_HOURS_END'],
  },
  {
    id: 'general',
    label: 'General',
    description: 'Otras configuraciones del sistema',
    icon: 'settings',
    settings: [],
  },
];

export const SystemSettingsView: React.FC = () => {
  const [settings, setSettings] = useState<SystemSettingDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [lastUpdate, setLastUpdate] = useState<Date | null>(null);

  const settingService = useMemo(() => new SystemSettingService(), []);

  // Load settings
  const loadSettings = async () => {
    setLoading(true);
    setError(null);

    try {
      const data = await settingService.getSystemSettings();
      setSettings(data);
      setLastUpdate(new Date());
    } catch (err) {
      console.error('Error loading settings:', err);
      setError('Error al cargar las configuraciones del sistema');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadSettings();
  }, []);

  // Handle save setting value
  const handleSaveSettingValue = async (key: string, value: string) => {
    try {
      await settingService.updateSystemSettingValue(key, value);

      // Update local state
      setSettings((prev) =>
        prev.map((setting) =>
          setting.settingKey === key
            ? { ...setting, settingValue: value, updatedAt: new Date().toISOString() }
            : setting
        )
      );
    } catch (err) {
      console.error('Error updating setting:', err);
      throw err;
    }
  };

  // Handle create new setting
  const handleCreateSetting = async (dto: CreateSystemSettingDto) => {
    try {
      const newSetting = await settingService.createSystemSetting(dto);
      setSettings((prev) => [...prev, newSetting]);
    } catch (err) {
      console.error('Error creating setting:', err);
      throw err;
    }
  };

  // Handle restore defaults (reload from API)
  const handleRestoreDefaults = async () => {
    if (confirm('¿Está seguro de que desea recargar las configuraciones desde el servidor?')) {
      await loadSettings();
    }
  };

  // Get category icon
  const getCategoryIcon = (iconName: string) => {
    switch (iconName) {
      case 'calendar':
        return <FiCalendar className="text-blue-600" size={24} />;
      case 'bell':
        return <FiBell className="text-green-600" size={24} />;
      case 'clock':
        return <FiClock className="text-orange-600" size={24} />;
      case 'settings':
      default:
        return <FiSettings className="text-gray-600" size={24} />;
    }
  };

  // Organize settings by category
  const organizedSettings = useMemo(() => {
    const result: Record<SettingCategory, SystemSettingDto[]> = {
      appointments: [],
      notifications: [],
      businessHours: [],
      general: [],
    };

    settings.forEach((setting) => {
      const searchLower = searchTerm.toLowerCase();
      const matchesSearch =
        setting.settingKey.toLowerCase().includes(searchLower) ||
        setting.description.toLowerCase().includes(searchLower);

      if (!matchesSearch) return;

      let categoryFound = false;

      for (const category of SETTING_CATEGORIES) {
        if (category.settings.includes(setting.settingKey)) {
          result[category.id as SettingCategory].push(setting);
          categoryFound = true;
          break;
        }
      }

      if (!categoryFound) {
        result.general.push(setting);
      }
    });

    return result;
  }, [settings, searchTerm]);

  // Render loading state
  if (loading) {
    return (
      <div className="flex flex-col items-center justify-center py-12">
        <FiLoader className="animate-spin text-blue-600 mb-4" size={48} />
        <p className="text-gray-600">Cargando configuraciones...</p>
      </div>
    );
  }

  // Render error state
  if (error) {
    return (
      <div className="flex flex-col items-center justify-center py-12">
        <FiAlertCircle className="text-red-600 mb-4" size={48} />
        <p className="text-gray-900 font-semibold mb-2">Error</p>
        <p className="text-gray-600 mb-4">{error}</p>
        <button
          onClick={loadSettings}
          className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700"
        >
          Reintentar
        </button>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Configuración del Sistema</h1>
          <p className="text-sm text-gray-600 mt-1">
            Gestiona las configuraciones globales del sistema
          </p>
          {lastUpdate && (
            <p className="text-xs text-gray-500 mt-1">
              Última actualización: {lastUpdate.toLocaleString('es-ES')}
            </p>
          )}
        </div>

        <div className="flex items-center space-x-3">
          <button
            onClick={handleRestoreDefaults}
            className="flex items-center px-4 py-2 text-gray-700 bg-white border border-gray-300 rounded-md hover:bg-gray-50"
          >
            <FiRotateCcw className="mr-2" size={18} />
            Recargar
          </button>
          <button
            onClick={() => setIsModalOpen(true)}
            className="flex items-center px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700"
          >
            <FiPlus className="mr-2" size={18} />
            Nueva Configuración
          </button>
        </div>
      </div>

      {/* Search */}
      <div className="relative">
        <FiSearch className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400" />
        <input
          type="text"
          placeholder="Buscar configuraciones..."
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
          className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
        />
      </div>

      {/* Categories */}
      <div className="space-y-8">
        {SETTING_CATEGORIES.map((category) => {
          const categorySettings = organizedSettings[category.id as SettingCategory];

          if (categorySettings.length === 0) return null;

          return (
            <div key={category.id} className="bg-gray-50 rounded-lg p-6">
              {/* Category Header */}
              <div className="flex items-start mb-4">
                <div className="p-3 bg-white rounded-lg shadow-sm">
                  {getCategoryIcon(category.icon)}
                </div>
                <div className="ml-4">
                  <h2 className="text-lg font-semibold text-gray-900">{category.label}</h2>
                  <p className="text-sm text-gray-600">{category.description}</p>
                </div>
              </div>

              {/* Settings Grid */}
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                {categorySettings.map((setting) => (
                  <SettingCard
                    key={setting.id}
                    setting={setting}
                    onSave={handleSaveSettingValue}
                    autoSave={true}
                    autoSaveDelay={1000}
                  />
                ))}
              </div>
            </div>
          );
        })}
      </div>

      {/* Empty state */}
      {settings.length === 0 && (
        <div className="text-center py-12">
          <FiSettings className="mx-auto text-gray-400 mb-4" size={48} />
          <p className="text-gray-600">No hay configuraciones disponibles</p>
          <button
            onClick={() => setIsModalOpen(true)}
            className="mt-4 px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700"
          >
            Crear primera configuración
          </button>
        </div>
      )}

      {/* No results */}
      {settings.length > 0 &&
        searchTerm &&
        Object.values(organizedSettings).every((cat) => cat.length === 0) && (
          <div className="text-center py-12">
            <FiSearch className="mx-auto text-gray-400 mb-4" size={48} />
            <p className="text-gray-600">
              No se encontraron configuraciones que coincidan con &quot;{searchTerm}&quot;
            </p>
          </div>
        )}

      {/* Create Modal */}
      <SystemSettingModal
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        onSubmit={handleCreateSetting}
      />
    </div>
  );
};
