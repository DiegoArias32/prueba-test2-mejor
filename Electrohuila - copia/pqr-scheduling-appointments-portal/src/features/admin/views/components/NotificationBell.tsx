/**
 * NotificationBell Component
 * Sistema de notificaciones en tiempo real para el panel administrativo
 */

'use client';

import React, { useState, useEffect } from 'react';
import { FiBell, FiX, FiCheck, FiClock, FiCalendar } from 'react-icons/fi';

export interface Notification {
  id: string;
  type: 'appointment' | 'reminder' | 'update' | 'alert';
  title: string;
  message: string;
  timestamp: Date;
  read: boolean;
  appointmentId?: number;
}

interface NotificationBellProps {
  notifications: Notification[];
  onMarkAsRead: (id: string) => void;
  onClearAll: () => void;
  onNotificationClick?: (notification: Notification) => void;
}

export const NotificationBell: React.FC<NotificationBellProps> = ({
  notifications,
  onMarkAsRead,
  onClearAll,
  onNotificationClick
}) => {
  const [isOpen, setIsOpen] = useState(false);
  const [showBadgeAnimation, setShowBadgeAnimation] = useState(false);

  const unreadCount = notifications.filter(n => !n.read).length;

  useEffect(() => {
    if (unreadCount > 0) {
      setShowBadgeAnimation(true);
      const timer = setTimeout(() => setShowBadgeAnimation(false), 1000);
      return () => clearTimeout(timer);
    }
  }, [unreadCount]);

  const getIcon = (type: string) => {
    switch (type) {
      case 'appointment':
        return <FiCalendar className="w-5 h-5 text-blue-500" />;
      case 'reminder':
        return <FiClock className="w-5 h-5 text-yellow-500" />;
      case 'update':
        return <FiCheck className="w-5 h-5 text-green-500" />;
      case 'alert':
        return <FiBell className="w-5 h-5 text-red-500" />;
      default:
        return <FiBell className="w-5 h-5 text-gray-500" />;
    }
  };

  const getTypeColor = (type: string) => {
    switch (type) {
      case 'appointment':
        return 'bg-blue-50 border-blue-200';
      case 'reminder':
        return 'bg-yellow-50 border-yellow-200';
      case 'update':
        return 'bg-green-50 border-green-200';
      case 'alert':
        return 'bg-red-50 border-red-200';
      default:
        return 'bg-gray-50 border-gray-200';
    }
  };

  const formatTimestamp = (date: Date) => {
    const now = new Date();
    const diff = now.getTime() - date.getTime();
    const minutes = Math.floor(diff / 60000);
    const hours = Math.floor(minutes / 60);
    const days = Math.floor(hours / 24);

    if (minutes < 1) return 'Justo ahora';
    if (minutes < 60) return `hace ${minutes}m`;
    if (hours < 24) return `hace ${hours}h`;
    return `hace ${days}d`;
  };

  return (
    <div className="relative">
      {/* Bell Icon */}
      <button
        onClick={() => setIsOpen(!isOpen)}
        className="relative p-3 rounded-xl hover:bg-gray-100 transition-colors"
      >
        <FiBell className="w-6 h-6 text-gray-700" />
        {unreadCount > 0 && (
          <span className={`
            absolute top-1 right-1 min-w-[20px] h-5 px-1.5
            bg-red-500 text-white text-xs font-bold
            rounded-full flex items-center justify-center
            ${showBadgeAnimation ? 'animate-bounce' : ''}
          `}>
            {unreadCount > 99 ? '99+' : unreadCount}
          </span>
        )}
      </button>

      {/* Dropdown Panel */}
      {isOpen && (
        <>
          {/* Backdrop */}
          <div
            className="fixed inset-0 z-40"
            onClick={() => setIsOpen(false)}
          />

          {/* Panel */}
          <div className="absolute right-0 mt-2 w-96 bg-white rounded-2xl shadow-2xl border border-gray-200 z-50 overflow-hidden">
            {/* Header */}
            <div className="p-4 border-b border-gray-200 bg-gradient-to-r from-[#1797D5] to-[#56C2E1] text-white">
              <div className="flex items-center justify-between">
                <div>
                  <h3 className="font-bold text-lg">Notificaciones</h3>
                  <p className="text-sm text-blue-100">
                    {unreadCount} notificación{unreadCount !== 1 ? 'es' : ''} sin leer
                  </p>
                </div>
                <button
                  onClick={() => setIsOpen(false)}
                  className="p-2 hover:bg-white/20 rounded-lg transition-colors"
                >
                  <FiX className="w-5 h-5" />
                </button>
              </div>
            </div>

            {/* Notifications List */}
            <div className="max-h-96 overflow-y-auto">
              {notifications.length === 0 ? (
                <div className="p-8 text-center">
                  <FiBell className="w-16 h-16 mx-auto text-gray-300 mb-3" />
                  <p className="text-gray-500">Sin notificaciones</p>
                </div>
              ) : (
                <div className="divide-y divide-gray-100">
                  {notifications.map((notification) => (
                    <div
                      key={notification.id}
                      className={`
                        p-4 hover:bg-gray-50 transition-colors cursor-pointer
                        ${!notification.read ? 'bg-blue-50' : ''}
                      `}
                      onClick={() => {
                        if (onNotificationClick) {
                          onNotificationClick(notification);
                        }
                        if (!notification.read) {
                          onMarkAsRead(notification.id);
                        }
                      }}
                    >
                      <div className="flex items-start space-x-3">
                        <div className={`
                          w-10 h-10 rounded-lg flex items-center justify-center flex-shrink-0
                          ${getTypeColor(notification.type)} border
                        `}>
                          {getIcon(notification.type)}
                        </div>
                        <div className="flex-1 min-w-0">
                          <div className="flex items-start justify-between mb-1">
                            <h4 className="font-semibold text-sm text-gray-900">
                              {notification.title}
                            </h4>
                            {!notification.read && (
                              <div className="w-2 h-2 bg-blue-500 rounded-full flex-shrink-0 mt-1 ml-2" />
                            )}
                          </div>
                          <p className="text-sm text-gray-600 mb-2">
                            {notification.message}
                          </p>
                          <p className="text-xs text-gray-400">
                            {formatTimestamp(notification.timestamp)}
                          </p>
                        </div>
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </div>

            {/* Footer */}
            {notifications.length > 0 && (
              <div className="p-3 border-t border-gray-200 bg-gray-50">
                <button
                  onClick={() => {
                    onClearAll();
                    setIsOpen(false);
                  }}
                  className="w-full py-2 text-sm font-medium text-[#1797D5] hover:text-[#203461] transition-colors"
                >
                  Limpiar todas las notificaciones
                </button>
              </div>
            )}
          </div>
        </>
      )}
    </div>
  );
};

/**
 * Hook para gestionar notificaciones con auto-refresh
 */
export const useNotifications = (refreshInterval: number = 30000) => {
  const [notifications, setNotifications] = useState<Notification[]>([]);

  const addNotification = React.useCallback((notification: Omit<Notification, 'id' | 'timestamp' | 'read'>) => {
    const newNotification: Notification = {
      ...notification,
      id: Math.random().toString(36).substr(2, 9),
      timestamp: new Date(),
      read: false
    };
    setNotifications(prev => [newNotification, ...prev]);
  }, []); // No dependencies - uses functional state update

  const markAsRead = React.useCallback((id: string) => {
    setNotifications(prev =>
      prev.map(n => n.id === id ? { ...n, read: true } : n)
    );
  }, []); // No dependencies - uses functional state update

  const clearAll = React.useCallback(() => {
    setNotifications([]);
  }, []); // No dependencies - direct state set

  // Simulación de notificaciones en tiempo real
  useEffect(() => {
    const interval = setInterval(() => {
      // Aquí podrías hacer polling al backend o usar WebSockets
      // Por ahora es una simulación
    }, refreshInterval);

    return () => clearInterval(interval);
  }, [refreshInterval]);

  return {
    notifications,
    addNotification,
    markAsRead,
    clearAll
  };
};
