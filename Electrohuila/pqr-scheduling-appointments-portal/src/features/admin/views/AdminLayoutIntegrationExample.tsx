/**
 * AdminLayout Integration Example
 * This file demonstrates how to integrate all the notification UX improvements
 *
 * INSTRUCTIONS:
 * 1. Review this example
 * 2. Apply the changes to your actual AdminLayout.tsx
 * 3. Test each feature individually
 * 4. Delete this file after integration
 */

'use client';

import React, { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { FiUser, FiAlertCircle, FiCheckCircle, FiRefreshCw } from 'react-icons/fi';
import Link from 'next/link';

// STEP 1: Import enhanced components and hooks
import { EnhancedNotificationBell } from './components/EnhancedNotificationBell';
import { WebSocketStatus } from '@/shared/components/WebSocketStatus';
import { ToastContainer } from '@/shared/components/Toast';
import { useToastNotifications } from '@/shared/hooks/useToastNotifications';
import { useEnhancedNotifications } from '../hooks/useEnhancedNotifications';
import { useWebSocket } from '@/services/websocket.service';
import { websocketService } from '@/services/websocket.service';
import type { WebSocketMessage } from '@/services/websocket.service';
import * as signalR from '@microsoft/signalr';

// Import other existing components...
// import { Sidebar, Dashboard } from './components';

export const AdminLayoutIntegrationExample: React.FC = () => {
  const router = useRouter();

  // STEP 2: Replace old notification hook with enhanced version
  const {
    notifications,
    unreadCount,
    isLoading,
    markAsRead,
    markAllAsRead,
    addEnhancedNotification, // Use this instead of addNotification
    refresh,
    browserNotificationsEnabled,
    browserNotificationsSupported,
    soundsEnabled,
    toggleSounds,
    toggleBrowserNotifications,
    requestBrowserPermission,
    testSound
  } = useEnhancedNotifications();

  // STEP 3: Add toast notifications hook
  const {
    toasts,
    removeToast,
    success: toastSuccess,
    error: toastError,
    warning: toastWarning,
    info: toastInfo
  } = useToastNotifications();

  // STEP 4: Track WebSocket connection state
  const [connectionStatus, setConnectionStatus] = useState<'connected' | 'connecting' | 'disconnected' | 'error'>('disconnected');
  const [lastPingTime, setLastPingTime] = useState<Date | undefined>(undefined);
  const [reconnectAttempts, setReconnectAttempts] = useState(0);

  // STEP 5: WebSocket connection with enhanced handling
  const { isConnected: wsConnected } = useWebSocket((message) => {
    handleWebSocketMessage(message);
  });

  // STEP 6: Update connection status based on WebSocket state
  useEffect(() => {
    const updateConnectionStatus = () => {
      const state = websocketService.getState();

      switch (state) {
        case signalR.HubConnectionState.Connected:
          setConnectionStatus('connected');
          setLastPingTime(new Date());
          setReconnectAttempts(0);
          break;
        case signalR.HubConnectionState.Connecting:
        case signalR.HubConnectionState.Reconnecting:
          setConnectionStatus('connecting');
          setReconnectAttempts(prev => prev + 1);
          break;
        case signalR.HubConnectionState.Disconnected:
          setConnectionStatus('disconnected');
          break;
        default:
          setConnectionStatus('disconnected');
      }
    };

    updateConnectionStatus();
    const interval = setInterval(updateConnectionStatus, 2000);

    return () => clearInterval(interval);
  }, [wsConnected]);

  // STEP 7: Enhanced WebSocket message handler
  const handleWebSocketMessage = (message: WebSocketMessage) => {
    console.log('[AdminLayout] WebSocket message received:', message);

    // Update last ping time
    setLastPingTime(new Date());

    switch (message.type) {
      case 'appointment_created': {
        const data = message.data as any;

        // Add to in-app notifications (with sound and browser notification)
        addEnhancedNotification({
          type: 'appointment',
          title: 'Nueva Cita Creada',
          message: `Cita #${data.appointmentNumber} ha sido creada`,
          timestamp: message.timestamp
        });

        // Show toast notification with action button
        toastInfo(
          'Nueva Cita',
          `Cita #${data.appointmentNumber} ha sido creada`,
          {
            duration: 6000,
            onView: () => {
              // Navigate to appointments view
              console.log('Navigate to appointment:', data.id);
            }
          }
        );
        break;
      }

      case 'appointment_updated': {
        const data = message.data as any;

        addEnhancedNotification({
          type: 'update',
          title: 'Cita Actualizada',
          message: `Cita #${data.appointmentNumber} ha sido actualizada`,
          timestamp: message.timestamp
        });

        toastInfo(
          'Cita Actualizada',
          `Cita #${data.appointmentNumber} ha sido actualizada`,
          {
            duration: 5000
          }
        );
        break;
      }

      case 'appointment_assigned': {
        const data = message.data as any;

        addEnhancedNotification({
          type: 'appointment',
          title: 'Nueva Cita Asignada',
          message: `Se te ha asignado la cita #${data.appointmentNumber}`,
          timestamp: message.timestamp
        });

        toastInfo(
          'Cita Asignada',
          `Se te ha asignado la cita #${data.appointmentNumber}`,
          {
            duration: 7000,
            onView: () => {
              // Navigate to my appointments
              console.log('Navigate to my appointments');
            }
          }
        );
        break;
      }

      case 'appointment_completed': {
        const data = message.data as any;

        addEnhancedNotification({
          type: 'update',
          title: 'Cita Completada',
          message: `Cita #${data.appointmentNumber} ha sido completada`,
          timestamp: message.timestamp
        });

        toastSuccess(
          'Cita Completada',
          `Cita #${data.appointmentNumber} ha sido completada`,
          {
            duration: 5000
          }
        );
        break;
      }

      default:
        console.log('Unhandled message type:', message.type);
    }
  };

  // STEP 8: Handle notification click to navigate
  const handleNotificationClick = (notification: any) => {
    console.log('Notification clicked:', notification);

    if (notification.appointmentId) {
      // Navigate to appointments view
      // setActiveSection('citas');
    }

    // Mark as read if not already
    if (!notification.read) {
      markAsRead(notification.id);
    }
  };

  // STEP 9: Request browser notification permission on mount (optional)
  useEffect(() => {
    // Only request if supported and not already granted/denied
    if (browserNotificationsSupported) {
      const permission = Notification.permission;
      if (permission === 'default') {
        // You can show a prompt to the user first
        // For now, we'll just log it
        console.log('[AdminLayout] Browser notifications available but not requested');
      }
    }
  }, [browserNotificationsSupported]);

  // STEP 10: Auto-connect WebSocket
  useEffect(() => {
    const token = localStorage.getItem('token');
    if (token && !wsConnected) {
      websocketService.connect(token);
    }

    return () => {
      // Don't disconnect on unmount as other components might use it
      // websocketService.disconnect();
    };
  }, [wsConnected]);

  return (
    <div className="flex h-screen bg-gray-50 overflow-hidden">
      {/* Sidebar */}
      {/* <Sidebar ... /> */}

      {/* Main Content */}
      <div className="flex-1 ml-64 flex flex-col overflow-hidden">
        {/* Header */}
        <header className="bg-white border-b border-gray-200 px-8 py-4 flex items-center justify-between sticky top-0 z-30 shadow-sm">
          <div>
            <h2 className="text-2xl font-bold text-gray-900">Panel Administrativo</h2>
            <p className="text-sm text-gray-500">Panel Principal</p>
          </div>

          <div className="flex items-center space-x-4">
            {/* STEP 11: Add WebSocket Status Indicator */}
            <WebSocketStatus
              status={connectionStatus}
              lastPing={lastPingTime}
              reconnectAttempts={reconnectAttempts}
              showLabel={true}
            />

            {/* Refresh Button */}
            <button
              onClick={() => refresh()}
              className="px-4 py-2 text-sm font-medium text-[#1797D5] hover:bg-blue-50 rounded-lg transition-colors flex items-center"
            >
              <FiRefreshCw className="w-4 h-4 mr-2" />
              Actualizar
            </button>

            {/* STEP 12: Replace NotificationBell with EnhancedNotificationBell */}
            <EnhancedNotificationBell
              notifications={notifications.map(n => ({
                id: n.id.toString(),
                type: getNotificationType(n.type),
                title: n.title,
                message: n.message,
                timestamp: new Date(n.sentAt || n.createdAt),
                read: n.isRead,
                appointmentId: n.appointmentNumber ? parseInt(n.appointmentNumber) : undefined
              }))}
              onMarkAsRead={(id) => markAsRead(parseInt(id))}
              onClearAll={markAllAsRead}
              onNotificationClick={handleNotificationClick}
            />

            {/* User Info */}
            <div className="flex items-center space-x-2 px-3 py-2 bg-gray-50 rounded-lg">
              <FiUser className="w-4 h-4 text-gray-600" />
              <span className="text-sm font-medium text-gray-700">
                Usuario
              </span>
            </div>

            <Link
              href="/"
              className="px-4 py-2 text-sm text-[#1797D5] hover:bg-blue-50 rounded-lg transition-colors"
            >
              Volver al Inicio
            </Link>
          </div>
        </header>

        {/* Main Content Area */}
        <main className="flex-1 overflow-auto p-8">
          <div className="max-w-7xl mx-auto">
            <h1 className="text-3xl font-bold text-gray-900 mb-8">
              Dashboard
            </h1>

            {/* Demo Buttons for Testing */}
            <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6 mb-8">
              <h2 className="text-lg font-semibold text-gray-900 mb-4">
                Test Notification Features
              </h2>

              <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
                <button
                  onClick={() => testSound()}
                  className="px-4 py-2 bg-blue-500 text-white rounded-lg hover:bg-blue-600 transition-colors"
                >
                  Test Sound
                </button>

                <button
                  onClick={() => requestBrowserPermission()}
                  className="px-4 py-2 bg-purple-500 text-white rounded-lg hover:bg-purple-600 transition-colors"
                >
                  Request Browser Permission
                </button>

                <button
                  onClick={() => {
                    toastSuccess('Success!', 'This is a success message');
                  }}
                  className="px-4 py-2 bg-green-500 text-white rounded-lg hover:bg-green-600 transition-colors"
                >
                  Test Success Toast
                </button>

                <button
                  onClick={() => {
                    toastError('Error!', 'This is an error message');
                  }}
                  className="px-4 py-2 bg-red-500 text-white rounded-lg hover:bg-red-600 transition-colors"
                >
                  Test Error Toast
                </button>

                <button
                  onClick={() => {
                    toastWarning('Warning!', 'This is a warning message');
                  }}
                  className="px-4 py-2 bg-yellow-500 text-white rounded-lg hover:bg-yellow-600 transition-colors"
                >
                  Test Warning Toast
                </button>

                <button
                  onClick={() => {
                    toastInfo('Info', 'This is an info message with a view button', {
                      onView: () => alert('View clicked!')
                    });
                  }}
                  className="px-4 py-2 bg-blue-500 text-white rounded-lg hover:bg-blue-600 transition-colors"
                >
                  Test Info Toast
                </button>

                <button
                  onClick={() => {
                    addEnhancedNotification({
                      type: 'appointment',
                      title: 'Test Notification',
                      message: 'This is a test notification with sound and browser notification',
                      timestamp: new Date().toISOString()
                    });
                  }}
                  className="px-4 py-2 bg-indigo-500 text-white rounded-lg hover:bg-indigo-600 transition-colors"
                >
                  Test Enhanced Notification
                </button>

                <button
                  onClick={toggleSounds}
                  className={`px-4 py-2 rounded-lg transition-colors ${
                    soundsEnabled
                      ? 'bg-green-500 hover:bg-green-600 text-white'
                      : 'bg-gray-300 hover:bg-gray-400 text-gray-700'
                  }`}
                >
                  Sounds: {soundsEnabled ? 'ON' : 'OFF'}
                </button>
              </div>

              <div className="mt-4 p-4 bg-gray-50 rounded-lg">
                <h3 className="text-sm font-semibold text-gray-700 mb-2">Status</h3>
                <div className="space-y-1 text-sm text-gray-600">
                  <p>WebSocket: {connectionStatus}</p>
                  <p>Sounds: {soundsEnabled ? 'Enabled' : 'Disabled'}</p>
                  <p>Browser Notifications: {browserNotificationsEnabled ? 'Enabled' : 'Disabled'}</p>
                  <p>Unread Notifications: {unreadCount}</p>
                  <p>Active Toasts: {toasts.length}</p>
                </div>
              </div>
            </div>

            {/* Your other content here */}
          </div>
        </main>

        {/* STEP 13: Add Toast Container */}
        <ToastContainer
          toasts={toasts}
          position="top-right"
          onClose={removeToast}
        />
      </div>
    </div>
  );
};

// Helper function to map notification type
function getNotificationType(type: string): 'appointment' | 'reminder' | 'update' | 'alert' {
  if (type.includes('appointment')) return 'appointment';
  if (type.includes('reminder')) return 'reminder';
  if (type.includes('update')) return 'update';
  return 'alert';
}
