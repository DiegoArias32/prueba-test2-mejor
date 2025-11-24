/**
 * NotificationSettingsModal Component
 * Modal for configuring notification preferences
 */

'use client';

import React, { useState, useEffect } from 'react';
import { FiX, FiVolume2, FiBell, FiSettings, FiCheck } from 'react-icons/fi';
import { notificationPreferencesService } from '@/services/notifications/notification-preferences.service';
import { notificationSoundsService } from '@/services/notifications/notification-sounds.service';
import { browserNotificationsService } from '@/services/notifications/browser-notifications.service';
import type { NotificationPreferences } from '@/services/notifications/notification-preferences.service';

interface NotificationSettingsModalProps {
  isOpen: boolean;
  onClose: () => void;
}

export const NotificationSettingsModal: React.FC<NotificationSettingsModalProps> = ({
  isOpen,
  onClose
}) => {
  const [preferences, setPreferences] = useState<NotificationPreferences>(
    notificationPreferencesService.getAll()
  );
  const [browserPermission, setBrowserPermission] = useState<string>('default');
  const [saved, setSaved] = useState(false);

  useEffect(() => {
    if (isOpen) {
      setPreferences(notificationPreferencesService.getAll());
      setBrowserPermission(browserNotificationsService.getPermission());
      setSaved(false);
    }
  }, [isOpen]);

  const handleSave = () => {
    notificationPreferencesService.update(preferences);

    // Update services
    if (preferences.soundsEnabled) {
      notificationSoundsService.enable();
    } else {
      notificationSoundsService.disable();
    }

    if (preferences.browserNotificationsEnabled) {
      browserNotificationsService.enable();
    } else {
      browserNotificationsService.disable();
    }

    setSaved(true);
    setTimeout(() => {
      onClose();
    }, 1000);
  };

  const handleRequestBrowserPermission = async () => {
    const permission = await browserNotificationsService.requestPermission();
    setBrowserPermission(permission);

    if (permission === 'granted') {
      setPreferences(prev => ({ ...prev, browserNotificationsEnabled: true }));
    }
  };

  const handleTestSound = async () => {
    await notificationSoundsService.test('info');
  };

  const handleTestBrowserNotification = async () => {
    await browserNotificationsService.test();
  };

  if (!isOpen) return null;

  return (
    <>
      {/* Backdrop */}
      <div
        className="fixed inset-0 bg-black bg-opacity-50 z-50 animate-fadeIn"
        onClick={onClose}
      />

      {/* Modal */}
      <div className="fixed inset-0 z-50 flex items-center justify-center p-4 pointer-events-none">
        <div
          className="bg-white rounded-2xl shadow-2xl max-w-2xl w-full max-h-[90vh] overflow-hidden
                     pointer-events-auto animate-slideUp"
          onClick={(e) => e.stopPropagation()}
        >
          {/* Header */}
          <div className="flex items-center justify-between p-6 border-b border-gray-200
                         bg-gradient-to-r from-[#1797D5] to-[#56C2E1]">
            <div className="flex items-center space-x-3">
              <div className="w-10 h-10 bg-white/20 rounded-lg flex items-center justify-center">
                <FiSettings className="w-5 h-5 text-white" />
              </div>
              <div>
                <h2 className="text-xl font-bold text-white">Preferencias de Notificaciones</h2>
                <p className="text-sm text-blue-100">Personaliza cómo recibes notificaciones</p>
              </div>
            </div>
            <button
              onClick={onClose}
              className="text-white hover:bg-white/20 p-2 rounded-lg transition-colors"
            >
              <FiX className="w-6 h-6" />
            </button>
          </div>

          {/* Content */}
          <div className="p-6 space-y-6 overflow-y-auto max-h-[calc(90vh-180px)]">
            {/* Sound Notifications */}
            <div className="space-y-4">
              <div className="flex items-center justify-between">
                <div className="flex items-center space-x-3">
                  <div className="w-10 h-10 bg-blue-50 rounded-lg flex items-center justify-center">
                    <FiVolume2 className="w-5 h-5 text-blue-600" />
                  </div>
                  <div>
                    <h3 className="font-semibold text-gray-900">Sonidos de Notificación</h3>
                    <p className="text-sm text-gray-500">Reproducir sonidos cuando lleguen notificaciones</p>
                  </div>
                </div>
                <label className="relative inline-flex items-center cursor-pointer">
                  <input
                    type="checkbox"
                    checked={preferences.soundsEnabled}
                    onChange={(e) => setPreferences(prev => ({
                      ...prev,
                      soundsEnabled: e.target.checked
                    }))}
                    className="sr-only peer"
                  />
                  <div className="w-11 h-6 bg-gray-200 peer-focus:outline-none peer-focus:ring-4
                               peer-focus:ring-blue-300 rounded-full peer
                               peer-checked:after:translate-x-full peer-checked:after:border-white
                               after:content-[''] after:absolute after:top-[2px] after:left-[2px]
                               after:bg-white after:border-gray-300 after:border after:rounded-full
                               after:h-5 after:w-5 after:transition-all peer-checked:bg-blue-600" />
                </label>
              </div>

              {preferences.soundsEnabled && (
                <div className="ml-[52px] space-y-3">
                  <button
                    onClick={handleTestSound}
                    className="text-sm text-blue-600 hover:text-blue-700 font-medium"
                  >
                    Probar Sonido
                  </button>
                </div>
              )}
            </div>

            {/* Browser Notifications */}
            <div className="space-y-4 pt-4 border-t border-gray-200">
              <div className="flex items-center justify-between">
                <div className="flex items-center space-x-3">
                  <div className="w-10 h-10 bg-purple-50 rounded-lg flex items-center justify-center">
                    <FiBell className="w-5 h-5 text-purple-600" />
                  </div>
                  <div>
                    <h3 className="font-semibold text-gray-900">Notificaciones del Navegador</h3>
                    <p className="text-sm text-gray-500">
                      Mostrar notificaciones nativas del sistema operativo
                    </p>
                  </div>
                </div>
                <label className="relative inline-flex items-center cursor-pointer">
                  <input
                    type="checkbox"
                    checked={preferences.browserNotificationsEnabled}
                    onChange={(e) => setPreferences(prev => ({
                      ...prev,
                      browserNotificationsEnabled: e.target.checked
                    }))}
                    disabled={browserPermission !== 'granted'}
                    className="sr-only peer"
                  />
                  <div className="w-11 h-6 bg-gray-200 peer-focus:outline-none peer-focus:ring-4
                               peer-focus:ring-purple-300 rounded-full peer
                               peer-checked:after:translate-x-full peer-checked:after:border-white
                               after:content-[''] after:absolute after:top-[2px] after:left-[2px]
                               after:bg-white after:border-gray-300 after:border after:rounded-full
                               after:h-5 after:w-5 after:transition-all peer-checked:bg-purple-600
                               peer-disabled:opacity-50 peer-disabled:cursor-not-allowed" />
                </label>
              </div>

              <div className="ml-[52px] space-y-3">
                {browserPermission !== 'granted' && (
                  <div className="space-y-2">
                    <p className="text-sm text-gray-600">
                      Permiso requerido para mostrar notificaciones del navegador.
                    </p>
                    <button
                      onClick={handleRequestBrowserPermission}
                      className="px-4 py-2 bg-purple-600 text-white rounded-lg text-sm font-medium
                               hover:bg-purple-700 transition-colors"
                    >
                      Solicitar Permiso
                    </button>
                  </div>
                )}

                {browserPermission === 'granted' && preferences.browserNotificationsEnabled && (
                  <button
                    onClick={handleTestBrowserNotification}
                    className="text-sm text-purple-600 hover:text-purple-700 font-medium"
                  >
                    Probar Notificación
                  </button>
                )}

                {browserPermission === 'denied' && (
                  <p className="text-sm text-red-600">
                    Permiso denegado. Habilita las notificaciones en la configuración de tu navegador.
                  </p>
                )}
              </div>
            </div>

            {/* Toast Notifications */}
            <div className="space-y-4 pt-4 border-t border-gray-200">
              <div className="flex items-center justify-between">
                <div className="flex items-center space-x-3">
                  <div className="w-10 h-10 bg-green-50 rounded-lg flex items-center justify-center">
                    <FiBell className="w-5 h-5 text-green-600" />
                  </div>
                  <div>
                    <h3 className="font-semibold text-gray-900">Notificaciones Toast</h3>
                    <p className="text-sm text-gray-500">Mostrar notificaciones temporales en la pantalla</p>
                  </div>
                </div>
                <label className="relative inline-flex items-center cursor-pointer">
                  <input
                    type="checkbox"
                    checked={preferences.toastNotificationsEnabled}
                    onChange={(e) => setPreferences(prev => ({
                      ...prev,
                      toastNotificationsEnabled: e.target.checked
                    }))}
                    className="sr-only peer"
                  />
                  <div className="w-11 h-6 bg-gray-200 peer-focus:outline-none peer-focus:ring-4
                               peer-focus:ring-green-300 rounded-full peer
                               peer-checked:after:translate-x-full peer-checked:after:border-white
                               after:content-[''] after:absolute after:top-[2px] after:left-[2px]
                               after:bg-white after:border-gray-300 after:border after:rounded-full
                               after:h-5 after:w-5 after:transition-all peer-checked:bg-green-600" />
                </label>
              </div>

              {preferences.toastNotificationsEnabled && (
                <div className="ml-[52px] space-y-3">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Duración (segundos)
                    </label>
                    <input
                      type="range"
                      min="1"
                      max="10"
                      step="1"
                      value={preferences.toastDuration / 1000}
                      onChange={(e) => setPreferences(prev => ({
                        ...prev,
                        toastDuration: parseInt(e.target.value) * 1000
                      }))}
                      className="w-full h-2 bg-gray-200 rounded-lg appearance-none cursor-pointer"
                    />
                    <div className="flex justify-between text-xs text-gray-500 mt-1">
                      <span>1s</span>
                      <span className="font-medium text-gray-700">{preferences.toastDuration / 1000}s</span>
                      <span>10s</span>
                    </div>
                  </div>
                </div>
              )}
            </div>
          </div>

          {/* Footer */}
          <div className="flex items-center justify-between p-6 border-t border-gray-200 bg-gray-50">
            <button
              onClick={() => {
                notificationPreferencesService.reset();
                setPreferences(notificationPreferencesService.getAll());
              }}
              className="px-4 py-2 text-sm font-medium text-gray-700 hover:text-gray-900
                       hover:bg-gray-200 rounded-lg transition-colors"
            >
              Restablecer
            </button>

            <div className="flex items-center space-x-3">
              <button
                onClick={onClose}
                className="px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-200
                         rounded-lg transition-colors"
              >
                Cancelar
              </button>
              <button
                onClick={handleSave}
                disabled={saved}
                className="px-6 py-2 bg-[#1797D5] text-white rounded-lg text-sm font-medium
                         hover:bg-[#1486C0] transition-colors disabled:opacity-50
                         disabled:cursor-not-allowed flex items-center space-x-2"
              >
                {saved ? (
                  <>
                    <FiCheck className="w-4 h-4" />
                    <span>Guardado</span>
                  </>
                ) : (
                  <span>Guardar Cambios</span>
                )}
              </button>
            </div>
          </div>
        </div>
      </div>
    </>
  );
};
