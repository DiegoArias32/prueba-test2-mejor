/**
 * WebSocketStatus Component
 * Visual indicator for WebSocket connection status with tooltip
 */

'use client';

import React, { useState } from 'react';
import { FiWifi, FiWifiOff, FiAlertCircle } from 'react-icons/fi';

export type ConnectionStatus = 'connected' | 'connecting' | 'disconnected' | 'error';

export interface WebSocketStatusProps {
  status: ConnectionStatus;
  lastPing?: Date;
  reconnectAttempts?: number;
  showLabel?: boolean;
  className?: string;
}

export const WebSocketStatus: React.FC<WebSocketStatusProps> = ({
  status,
  lastPing,
  reconnectAttempts = 0,
  showLabel = true,
  className = ''
}) => {
  const [showTooltip, setShowTooltip] = useState(false);

  const getStatusConfig = () => {
    switch (status) {
      case 'connected':
        return {
          icon: FiWifi,
          color: 'text-green-600',
          bgColor: 'bg-green-50',
          dotColor: 'bg-green-500',
          label: 'Conectado',
          animate: 'animate-pulse'
        };
      case 'connecting':
        return {
          icon: FiWifi,
          color: 'text-yellow-600',
          bgColor: 'bg-yellow-50',
          dotColor: 'bg-yellow-500',
          label: 'Conectando...',
          animate: 'animate-bounce'
        };
      case 'disconnected':
        return {
          icon: FiWifiOff,
          color: 'text-gray-600',
          bgColor: 'bg-gray-50',
          dotColor: 'bg-gray-400',
          label: 'Desconectado',
          animate: ''
        };
      case 'error':
        return {
          icon: FiAlertCircle,
          color: 'text-red-600',
          bgColor: 'bg-red-50',
          dotColor: 'bg-red-500',
          label: 'Error',
          animate: 'animate-pulse'
        };
      default:
        return {
          icon: FiWifiOff,
          color: 'text-gray-600',
          bgColor: 'bg-gray-50',
          dotColor: 'bg-gray-400',
          label: 'Desconectado',
          animate: ''
        };
    }
  };

  const formatLastPing = (date?: Date): string => {
    if (!date) return 'Nunca';

    const now = new Date();
    const diff = now.getTime() - date.getTime();
    const seconds = Math.floor(diff / 1000);
    const minutes = Math.floor(seconds / 60);
    const hours = Math.floor(minutes / 60);

    if (seconds < 60) return `Hace ${seconds}s`;
    if (minutes < 60) return `Hace ${minutes}m`;
    if (hours < 24) return `Hace ${hours}h`;
    return date.toLocaleString();
  };

  const config = getStatusConfig();
  const Icon = config.icon;

  return (
    <div className={`relative ${className}`}>
      <div
        className={`flex items-center space-x-2 px-3 py-2 rounded-lg cursor-pointer
                   transition-colors ${config.bgColor} hover:opacity-80`}
        onMouseEnter={() => setShowTooltip(true)}
        onMouseLeave={() => setShowTooltip(false)}
      >
        {/* Status Dot */}
        <div className="relative">
          <div className={`w-2 h-2 rounded-full ${config.dotColor} ${config.animate}`} />
          {status === 'connected' && (
            <div className={`absolute inset-0 w-2 h-2 rounded-full ${config.dotColor} animate-ping opacity-75`} />
          )}
        </div>

        {/* Icon */}
        <Icon className={`w-4 h-4 ${config.color}`} />

        {/* Label */}
        {showLabel && (
          <span className={`text-xs font-medium ${config.color}`}>
            {config.label}
          </span>
        )}
      </div>

      {/* Tooltip */}
      {showTooltip && (
        <div className="absolute top-full right-0 mt-2 w-64 bg-white rounded-lg shadow-xl
                       border border-gray-200 z-50 p-4 animate-fadeIn">
          {/* Arrow */}
          <div className="absolute -top-2 right-4 w-4 h-4 bg-white border-l border-t
                         border-gray-200 transform rotate-45" />

          {/* Content */}
          <div className="relative space-y-2">
            <h4 className="font-semibold text-gray-900 text-sm">
              Estado de Conexión
            </h4>

            <div className="space-y-1.5 text-xs text-gray-600">
              <div className="flex justify-between">
                <span>Estado:</span>
                <span className={`font-medium ${config.color}`}>
                  {config.label}
                </span>
              </div>

              {status === 'connected' && lastPing && (
                <div className="flex justify-between">
                  <span>Último ping:</span>
                  <span className="font-medium text-gray-700">
                    {formatLastPing(lastPing)}
                  </span>
                </div>
              )}

              {(status === 'connecting' || status === 'error') && reconnectAttempts > 0 && (
                <div className="flex justify-between">
                  <span>Intentos de reconexión:</span>
                  <span className="font-medium text-gray-700">
                    {reconnectAttempts}
                  </span>
                </div>
              )}

              {status === 'connected' && (
                <div className="pt-2 border-t border-gray-200">
                  <div className="flex items-center space-x-1 text-green-600">
                    <div className="w-1.5 h-1.5 rounded-full bg-green-500 animate-pulse" />
                    <span className="font-medium">Tiempo Real Activo</span>
                  </div>
                </div>
              )}

              {status === 'disconnected' && (
                <div className="pt-2 border-t border-gray-200">
                  <p className="text-gray-500">
                    Las notificaciones en tiempo real no están disponibles.
                  </p>
                </div>
              )}

              {status === 'error' && (
                <div className="pt-2 border-t border-gray-200">
                  <p className="text-red-600">
                    Error de conexión. Reintentando...
                  </p>
                </div>
              )}
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

// CSS for fadeIn animation (add to global CSS or Tailwind config)
// @keyframes fadeIn {
//   from { opacity: 0; transform: translateY(-4px); }
//   to { opacity: 1; transform: translateY(0); }
// }
// .animate-fadeIn {
//   animation: fadeIn 0.2s ease-out;
// }
