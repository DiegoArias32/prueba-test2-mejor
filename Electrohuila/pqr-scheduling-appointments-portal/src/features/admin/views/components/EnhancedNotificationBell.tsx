/**
 * EnhancedNotificationBell Component
 * Enhanced notification bell with animations, settings, and improved UX
 */

'use client';

import React, { useState, useEffect } from 'react';
import { FiBell, FiX, FiCheck, FiClock, FiCalendar, FiSettings } from 'react-icons/fi';
import { NotificationSettingsModal } from '../../components/NotificationSettingsModal';

export interface Notification {
  id: string;
  type: 'appointment' | 'reminder' | 'update' | 'alert';
  title: string;
  message: string;
  timestamp: Date;
  isRead: boolean;
  appointmentId?: number;
}

interface EnhancedNotificationBellProps {
  notifications: Notification[];
  onMarkAsRead: (id: string) => void;
  onClearAll: () => void;
  onNotificationClick?: (notification: Notification) => void;
}

export const EnhancedNotificationBell: React.FC<EnhancedNotificationBellProps> = ({
  notifications,
  onMarkAsRead,
  onClearAll,
  onNotificationClick
}) => {
  const [isOpen, setIsOpen] = useState(false);
  const [showBadgeAnimation, setShowBadgeAnimation] = useState(false);
  const [showSettingsModal, setShowSettingsModal] = useState(false);
  const [previousUnreadCount, setPreviousUnreadCount] = useState(0);

  const unreadCount = notifications.filter(n => !n.isRead).length;

  // Animate badge when new notifications arrive
  useEffect(() => {
    if (unreadCount > previousUnreadCount) {
      setShowBadgeAnimation(true);
      const timer = setTimeout(() => setShowBadgeAnimation(false), 1000);
      return () => clearTimeout(timer);
    }
    setPreviousUnreadCount(unreadCount);
  }, [unreadCount, previousUnreadCount]);

  const getIcon = (type: string) => {
    switch (type) {
      case 'appointment':
        return <FiCalendar className="w-5 h-5 text-[#1797D5]" />;
      case 'reminder':
        return <FiClock className="w-5 h-5 text-amber-600" />;
      case 'update':
        return <FiCheck className="w-5 h-5 text-emerald-600" />;
      case 'alert':
        return <FiBell className="w-5 h-5 text-orange-600" />;
      default:
        return <FiBell className="w-5 h-5 text-gray-500" />;
    }
  };

  const getTypeColor = (type: string) => {
    switch (type) {
      case 'appointment':
        return 'bg-[#97D4E3]/20 border-[#56C2E1]';
      case 'reminder':
        return 'bg-amber-100 border-amber-200';
      case 'update':
        return 'bg-emerald-100 border-emerald-200';
      case 'alert':
        return 'bg-orange-100 border-orange-200';
      default:
        return 'bg-gray-100 border-gray-200';
    }
  };

  const getBorderColor = (type: string) => {
    switch (type) {
      case 'appointment':
        return 'border-l-[#1797D5]';
      case 'reminder':
        return 'border-l-amber-500';
      case 'update':
        return 'border-l-emerald-500';
      case 'alert':
        return 'border-l-orange-500';
      default:
        return 'border-l-gray-400';
    }
  };

  const formatTimestamp = (date: Date | undefined) => {
    if (!date) return 'Justo ahora';

    const now = new Date();
    const dateObj = typeof date === 'string' ? new Date(date) : date;

    if (isNaN(dateObj.getTime())) return 'Justo ahora';

    const diff = now.getTime() - dateObj.getTime();
    const minutes = Math.floor(diff / 60000);
    const hours = Math.floor(minutes / 60);
    const days = Math.floor(hours / 24);

    if (minutes < 1) return 'Justo ahora';
    if (minutes < 60) return `hace ${minutes}m`;
    if (hours < 24) return `hace ${hours}h`;
    return `hace ${days}d`;
  };

  const isRecent = (date: Date | undefined) => {
    if (!date) return true;
    const dateObj = typeof date === 'string' ? new Date(date) : date;
    if (isNaN(dateObj.getTime())) return true;
    const diff = new Date().getTime() - dateObj.getTime();
    return diff < 300000; // Less than 5 minutes
  };

  return (
    <>
      <div className="relative">
        {/* Bell Icon with Animation */}
        <button
          onClick={() => setIsOpen(!isOpen)}
          className="relative p-3 rounded-xl hover:bg-gray-100 transition-all duration-200
                   hover:scale-105 active:scale-95 focus:outline-none focus:ring-2
                   focus:ring-[#1797D5] focus:ring-offset-2"
          aria-label="Notifications"
        >
          <FiBell className={`w-6 h-6 text-gray-700 transition-transform ${
            unreadCount > 0 ? 'animate-wiggle' : ''
          }`} />
          {unreadCount > 0 && (
            <>
              {/* Badge */}
              <span className={`
                absolute top-1 right-1 min-w-[20px] h-5 px-1.5
                bg-gradient-to-br from-orange-500 to-orange-600 text-white text-xs font-bold
                rounded-full flex items-center justify-center shadow-lg
                transform transition-all duration-300
                ${showBadgeAnimation ? 'animate-bounce scale-110' : 'scale-100'}
              `}>
                {unreadCount > 99 ? '99+' : unreadCount}
              </span>
              {/* Pulse effect */}
              <span className="absolute top-1 right-1 w-5 h-5 bg-orange-500 rounded-full
                             animate-ping opacity-75" />
            </>
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

            {/* Panel with slide-in animation */}
            <div className="absolute right-0 mt-2 w-96 bg-white rounded-2xl shadow-2xl
                           border border-gray-200 z-50 overflow-hidden
                           animate-slideDown opacity-0 animate-fadeIn">
              {/* Header */}
              <div className="p-5 border-b border-gray-200 bg-gradient-to-br from-[#1A6192] via-[#1797D5] to-[#56C2E1] text-white">
                <div className="flex items-center justify-between">
                  <div>
                    <h3 className="font-bold text-lg tracking-tight">Notificaciones</h3>
                    <p className="text-sm text-[#97D4E3] font-medium mt-0.5">
                      {unreadCount > 0
                        ? `${unreadCount} ${unreadCount !== 1 ? 'nuevas' : 'nueva'}`
                        : 'Al día con todo'
                      }
                    </p>
                  </div>
                  <div className="flex items-center space-x-1">
                    {/* Settings Button */}
                    <button
                      onClick={(e) => {
                        e.stopPropagation();
                        setShowSettingsModal(true);
                      }}
                      className="p-2 hover:bg-white/20 rounded-lg transition-all duration-200
                               focus:outline-none focus:ring-2 focus:ring-white/50"
                      aria-label="Notification settings"
                    >
                      <FiSettings className="w-5 h-5" />
                    </button>
                    {/* Close Button */}
                    <button
                      onClick={() => setIsOpen(false)}
                      className="p-2 hover:bg-white/20 rounded-lg transition-all duration-200
                               focus:outline-none focus:ring-2 focus:ring-white/50"
                      aria-label="Close"
                    >
                      <FiX className="w-5 h-5" />
                    </button>
                  </div>
                </div>
              </div>

              {/* Notifications List */}
              <div className="max-h-96 overflow-y-auto scrollbar-thin scrollbar-thumb-gray-300
                            scrollbar-track-gray-100">
                {notifications.length === 0 ? (
                  /* Enhanced Empty State */
                  <div className="p-12 text-center animate-fadeIn">
                    <div className="relative inline-block mb-4">
                      {/* Decorative circles */}
                      <div className="absolute inset-0 bg-gradient-to-br from-[#97D4E3] to-[#56C2E1]
                                    rounded-full blur-xl opacity-60 animate-pulse" />
                      <div className="relative bg-gradient-to-br from-[#97D4E3] to-[#56C2E1]
                                    rounded-full p-6 shadow-inner">
                        <FiBell className="w-16 h-16 text-[#1797D5] animate-[wiggle_3s_ease-in-out_infinite]" />
                      </div>
                    </div>
                    <h4 className="text-lg font-semibold text-gray-800 mb-2">
                      Todo tranquilo por aquí
                    </h4>
                    <p className="text-sm text-gray-500 max-w-xs mx-auto leading-relaxed">
                      No tienes notificaciones pendientes. Te avisaremos cuando haya novedades.
                    </p>
                  </div>
                ) : (
                  <div className="divide-y divide-gray-100">
                    {notifications.map((notification, index) => (
                      <div
                        key={notification.id}
                        className={`
                          relative p-4 transition-all duration-300 cursor-pointer group
                          hover:bg-gray-50 hover:shadow-md
                          ${!notification.isRead
                            ? `bg-gradient-to-r from-[#97D4E3]/30 to-transparent border-l-4 ${getBorderColor(notification.type)}`
                            : 'border-l-4 border-l-transparent hover:border-l-gray-300'
                          }
                          animate-slideIn
                        `}
                        style={{ animationDelay: `${index * 50}ms` }}
                        onClick={() => {
                          if (onNotificationClick) {
                            onNotificationClick(notification);
                          }
                          if (!notification.isRead) {
                            onMarkAsRead(notification.id);
                          }
                        }}
                      >
                        <div className="flex items-start space-x-3">
                          {/* Icon with enhanced styling */}
                          <div className={`
                            w-11 h-11 rounded-xl flex items-center justify-center flex-shrink-0
                            ${getTypeColor(notification.type)} border-2
                            transition-all duration-300 shadow-sm
                            group-hover:scale-110 group-hover:shadow-md
                          `}>
                            {getIcon(notification.type)}
                          </div>

                          <div className="flex-1 min-w-0">
                            <div className="flex items-start justify-between gap-2 mb-1.5">
                              <h4 className={`font-semibold text-sm leading-tight
                                ${!notification.isRead ? 'text-gray-900' : 'text-gray-700'}
                              `}>
                                {notification.title}
                              </h4>

                              {/* Enhanced unread indicator */}
                              {!notification.isRead && (
                                <div className="flex items-center gap-1.5 flex-shrink-0">
                                  {isRecent(notification.timestamp) && (
                                    <span className="text-[10px] font-bold text-white bg-gradient-to-r
                                                   from-orange-500 to-orange-600 px-2 py-0.5 rounded-full
                                                   shadow-sm uppercase tracking-wide">
                                      Nuevo
                                    </span>
                                  )}
                                  <div className="w-2.5 h-2.5 bg-[#1797D5] rounded-full
                                                animate-pulse shadow-sm ring-2 ring-[#56C2E1]/30" />
                                </div>
                              )}
                            </div>

                            <p className={`text-sm mb-2 line-clamp-2 leading-relaxed
                              ${!notification.isRead ? 'text-gray-700' : 'text-gray-500'}
                            `}>
                              {notification.message}
                            </p>

                            <div className="flex items-center justify-between">
                              <p className="text-xs text-gray-400 font-medium">
                                {formatTimestamp(notification.timestamp)}
                              </p>

                              {/* Mark as read button on hover */}
                              {!notification.isRead && (
                                <button
                                  onClick={(e) => {
                                    e.stopPropagation();
                                    onMarkAsRead(notification.id);
                                  }}
                                  className="opacity-0 group-hover:opacity-100 transition-opacity
                                           text-xs text-[#1797D5] hover:text-[#1A6192] font-medium
                                           flex items-center gap-1 px-2 py-1 rounded-md
                                           hover:bg-[#97D4E3]/30 focus:outline-none focus:ring-2
                                           focus:ring-[#1797D5]"
                                  aria-label="Mark as read"
                                >
                                  <FiCheck className="w-3 h-3" />
                                  Marcar leída
                                </button>
                              )}
                            </div>
                          </div>
                        </div>
                      </div>
                    ))}
                  </div>
                )}
              </div>

              {/* Footer */}
              {notifications.length > 0 && (
                <div className="p-3 border-t border-gray-200 bg-gradient-to-b from-gray-50 to-white">
                  <button
                    onClick={() => {
                      onClearAll();
                      setIsOpen(false);
                    }}
                    className="w-full py-2.5 text-sm font-semibold text-[#1797D5] hover:text-[#1A6192]
                             transition-all duration-200 hover:bg-[#97D4E3]/30 rounded-lg
                             focus:outline-none focus:ring-2 focus:ring-[#1797D5] focus:ring-offset-2"
                  >
                    Limpiar todas las notificaciones
                  </button>
                </div>
              )}
            </div>
          </>
        )}
      </div>

      {/* Settings Modal */}
      <NotificationSettingsModal
        isOpen={showSettingsModal}
        onClose={() => setShowSettingsModal(false)}
      />
    </>
  );
};
