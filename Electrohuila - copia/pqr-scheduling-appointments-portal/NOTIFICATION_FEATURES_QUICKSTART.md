# Notification System - Quick Start Guide

## Overview
This guide provides quick examples for using the enhanced notification system features.

## 1. Basic Toast Notifications

```tsx
import { useToastNotifications } from '@/shared/hooks/useToastNotifications';

function MyComponent() {
  const { success, error, warning, info } = useToastNotifications();

  const handleSuccess = () => {
    success('Success!', 'Operation completed successfully');
  };

  const handleError = () => {
    error('Error!', 'Something went wrong');
  };

  const handleWarning = () => {
    warning('Warning!', 'Please review this carefully');
  };

  const handleInfo = () => {
    info('Info', 'Here is some information', {
      duration: 7000, // 7 seconds
      onView: () => {
        // Navigate or perform action
        console.log('View clicked');
      }
    });
  };

  return (
    <div>
      <button onClick={handleSuccess}>Show Success</button>
      <button onClick={handleError}>Show Error</button>
      <button onClick={handleWarning}>Show Warning</button>
      <button onClick={handleInfo}>Show Info</button>
    </div>
  );
}
```

## 2. Enhanced Notifications (In-App + Sound + Browser)

```tsx
import { useEnhancedNotifications } from '@/features/admin/hooks/useEnhancedNotifications';

function MyComponent() {
  const {
    addEnhancedNotification,
    notifications,
    unreadCount,
    markAsRead,
    soundsEnabled,
    toggleSounds
  } = useEnhancedNotifications();

  const handleNewAppointment = () => {
    // This will:
    // 1. Add to in-app notifications
    // 2. Play sound (if enabled)
    // 3. Show browser notification (if enabled and app not focused)
    addEnhancedNotification({
      type: 'appointment',
      title: 'New Appointment',
      message: 'You have a new appointment assigned',
      timestamp: new Date().toISOString()
    });
  };

  return (
    <div>
      <p>Unread: {unreadCount}</p>
      <button onClick={handleNewAppointment}>New Appointment</button>
      <button onClick={toggleSounds}>
        Sounds: {soundsEnabled ? 'ON' : 'OFF'}
      </button>
    </div>
  );
}
```

## 3. WebSocket Status Indicator

```tsx
import { WebSocketStatus } from '@/shared/components/WebSocketStatus';
import { websocketService } from '@/services/websocket.service';
import * as signalR from '@microsoft/signalr';

function MyComponent() {
  const [status, setStatus] = useState<'connected' | 'connecting' | 'disconnected'>('disconnected');
  const [lastPing, setLastPing] = useState<Date>();

  useEffect(() => {
    const interval = setInterval(() => {
      const state = websocketService.getState();

      if (state === signalR.HubConnectionState.Connected) {
        setStatus('connected');
        setLastPing(new Date());
      } else if (state === signalR.HubConnectionState.Connecting) {
        setStatus('connecting');
      } else {
        setStatus('disconnected');
      }
    }, 2000);

    return () => clearInterval(interval);
  }, []);

  return (
    <WebSocketStatus
      status={status}
      lastPing={lastPing}
      showLabel={true}
    />
  );
}
```

## 4. Notification Settings Modal

```tsx
import { useState } from 'react';
import { NotificationSettingsModal } from '@/features/admin/components/NotificationSettingsModal';

function MyComponent() {
  const [showSettings, setShowSettings] = useState(false);

  return (
    <>
      <button onClick={() => setShowSettings(true)}>
        Settings
      </button>

      <NotificationSettingsModal
        isOpen={showSettings}
        onClose={() => setShowSettings(false)}
      />
    </>
  );
}
```

## 5. Enhanced Notification Bell

```tsx
import { EnhancedNotificationBell } from '@/features/admin/views/components/EnhancedNotificationBell';

function MyComponent() {
  const notifications = [
    {
      id: '1',
      type: 'appointment',
      title: 'New Appointment',
      message: 'You have a new appointment',
      timestamp: new Date(),
      read: false
    }
  ];

  return (
    <EnhancedNotificationBell
      notifications={notifications}
      onMarkAsRead={(id) => console.log('Mark as read:', id)}
      onClearAll={() => console.log('Clear all')}
      onNotificationClick={(notification) => {
        console.log('Clicked:', notification);
      }}
    />
  );
}
```

## 6. Managing Preferences Programmatically

```tsx
import { notificationPreferencesService } from '@/services/notifications/notification-preferences.service';

function MyComponent() {
  const handleToggleSounds = () => {
    const newValue = notificationPreferencesService.toggleSounds();
    console.log('Sounds enabled:', newValue);
  };

  const handleSetDuration = () => {
    notificationPreferencesService.setToastDuration(7000); // 7 seconds
  };

  const handleGetPreferences = () => {
    const prefs = notificationPreferencesService.getAll();
    console.log('Current preferences:', prefs);
  };

  const handleResetPreferences = () => {
    notificationPreferencesService.reset();
    console.log('Preferences reset to defaults');
  };

  return (
    <div>
      <button onClick={handleToggleSounds}>Toggle Sounds</button>
      <button onClick={handleSetDuration}>Set Duration to 7s</button>
      <button onClick={handleGetPreferences}>Get Preferences</button>
      <button onClick={handleResetPreferences}>Reset Preferences</button>
    </div>
  );
}
```

## 7. Sound Management

```tsx
import { notificationSoundsService } from '@/services/notifications/notification-sounds.service';

function MyComponent() {
  const handlePlaySuccess = async () => {
    await notificationSoundsService.playSuccess();
  };

  const handlePlayError = async () => {
    await notificationSoundsService.playError();
  };

  const handleTest = async () => {
    await notificationSoundsService.test('info');
  };

  const handleToggle = () => {
    const enabled = notificationSoundsService.toggle();
    console.log('Sounds enabled:', enabled);
  };

  return (
    <div>
      <button onClick={handlePlaySuccess}>Play Success Sound</button>
      <button onClick={handlePlayError}>Play Error Sound</button>
      <button onClick={handleTest}>Test Sound</button>
      <button onClick={handleToggle}>Toggle Sounds</button>
    </div>
  );
}
```

## 8. Browser Notifications

```tsx
import { browserNotificationsService } from '@/services/notifications/browser-notifications.service';

function MyComponent() {
  const handleRequestPermission = async () => {
    const permission = await browserNotificationsService.requestPermission();
    console.log('Permission:', permission);
  };

  const handleShowNotification = async () => {
    await browserNotificationsService.show({
      title: 'Test Notification',
      body: 'This is a test browser notification',
      tag: 'test-1'
    });
  };

  const handleShowIfNotFocused = async () => {
    // Only shows if app is not focused
    await browserNotificationsService.showIfNotFocused({
      title: 'Background Notification',
      body: 'This only shows when app is in background'
    });
  };

  const handleTest = async () => {
    await browserNotificationsService.test();
  };

  return (
    <div>
      <button onClick={handleRequestPermission}>Request Permission</button>
      <button onClick={handleShowNotification}>Show Notification</button>
      <button onClick={handleShowIfNotFocused}>Show If Not Focused</button>
      <button onClick={handleTest}>Test Notification</button>
    </div>
  );
}
```

## 9. Complete Integration Example

```tsx
import React, { useEffect } from 'react';
import { EnhancedNotificationBell } from '@/features/admin/views/components/EnhancedNotificationBell';
import { WebSocketStatus } from '@/shared/components/WebSocketStatus';
import { ToastContainer } from '@/shared/components/Toast';
import { useEnhancedNotifications } from '@/features/admin/hooks/useEnhancedNotifications';
import { useToastNotifications } from '@/shared/hooks/useToastNotifications';
import { useWebSocket } from '@/services/websocket.service';

function AdminLayout() {
  // Enhanced notifications
  const {
    notifications,
    unreadCount,
    markAsRead,
    markAllAsRead,
    addEnhancedNotification
  } = useEnhancedNotifications();

  // Toast notifications
  const {
    toasts,
    removeToast,
    success: toastSuccess,
    error: toastError
  } = useToastNotifications();

  // WebSocket
  const { isConnected } = useWebSocket((message) => {
    // Handle WebSocket message
    addEnhancedNotification({
      type: 'appointment',
      title: message.data.title,
      message: message.data.message,
      timestamp: message.timestamp
    });

    // Show toast
    toastSuccess('New Message', message.data.message);
  });

  return (
    <div>
      <header>
        <WebSocketStatus
          status={isConnected ? 'connected' : 'disconnected'}
          showLabel={true}
        />

        <EnhancedNotificationBell
          notifications={notifications.map(n => ({
            id: n.id.toString(),
            type: 'appointment',
            title: n.title,
            message: n.message,
            timestamp: new Date(n.sentAt || n.createdAt),
            read: n.isRead
          }))}
          onMarkAsRead={(id) => markAsRead(parseInt(id))}
          onClearAll={markAllAsRead}
        />
      </header>

      <main>
        {/* Your content */}
      </main>

      <ToastContainer
        toasts={toasts}
        position="top-right"
        onClose={removeToast}
      />
    </div>
  );
}
```

## 10. Subscribe to Preference Changes

```tsx
import { useEffect, useState } from 'react';
import { notificationPreferencesService } from '@/services/notifications/notification-preferences.service';

function MyComponent() {
  const [prefs, setPrefs] = useState(notificationPreferencesService.getAll());

  useEffect(() => {
    // Subscribe to preference changes
    const unsubscribe = notificationPreferencesService.subscribe((newPrefs) => {
      setPrefs(newPrefs);
      console.log('Preferences changed:', newPrefs);
    });

    return () => unsubscribe();
  }, []);

  return (
    <div>
      <p>Sounds: {prefs.soundsEnabled ? 'ON' : 'OFF'}</p>
      <p>Browser Notifications: {prefs.browserNotificationsEnabled ? 'ON' : 'OFF'}</p>
      <p>Toast Duration: {prefs.toastDuration}ms</p>
    </div>
  );
}
```

## Common Patterns

### Pattern 1: Show Success Toast After API Call
```tsx
const handleSave = async () => {
  try {
    await api.saveData(data);
    toastSuccess('Saved!', 'Data saved successfully');
  } catch (error) {
    toastError('Error', 'Failed to save data');
  }
};
```

### Pattern 2: Notify User of Background Event
```tsx
useEffect(() => {
  const handleBackgroundUpdate = (event) => {
    addEnhancedNotification({
      type: 'update',
      title: 'Update Available',
      message: 'New data is available',
      timestamp: new Date().toISOString()
    });
  };

  window.addEventListener('background-update', handleBackgroundUpdate);
  return () => window.removeEventListener('background-update', handleBackgroundUpdate);
}, []);
```

### Pattern 3: Toast with Action Button
```tsx
const handleDelete = async () => {
  toastWarning(
    'Confirm Delete',
    'Are you sure you want to delete this item?',
    {
      duration: 10000,
      actions: [
        {
          label: 'Confirm',
          onClick: async () => {
            await api.deleteItem(id);
            toastSuccess('Deleted', 'Item deleted successfully');
          }
        }
      ]
    }
  );
};
```

## Troubleshooting

### Sounds Not Playing
```tsx
// Sounds require user interaction first
// Add this to a button click handler:
const handleEnableSound = async () => {
  notificationSoundsService.enable();
  await notificationSoundsService.test('info');
};
```

### Browser Notifications Not Showing
```tsx
// Check permission status first
const checkPermission = () => {
  const permission = browserNotificationsService.getPermission();

  if (permission === 'default') {
    // Request permission
    browserNotificationsService.requestPermission();
  } else if (permission === 'denied') {
    console.log('Permission denied by user');
  }
};
```

### Toast Not Dismissing
```tsx
// Make sure ToastContainer is rendered
// Check that removeToast is properly connected
<ToastContainer
  toasts={toasts}
  onClose={removeToast} // Important!
/>
```

## Best Practices

1. **Always render ToastContainer** in your layout component
2. **Request browser permission** only after user interaction
3. **Use appropriate toast types** (success, error, warning, info)
4. **Keep toast messages concise** (under 100 characters)
5. **Use action buttons sparingly** (max 2 buttons)
6. **Set appropriate durations** (3-7 seconds for normal, 10+ for actions)
7. **Test on multiple browsers** (Chrome, Firefox, Safari)
8. **Handle permission denied gracefully**
9. **Provide settings UI** for users to customize
10. **Monitor performance** with many simultaneous toasts

## Performance Tips

- Use `React.memo` for notification components if rendering many
- Limit simultaneous toasts to 3-5
- Clean up old notifications periodically
- Use virtual scrolling for long notification lists
- Debounce rapid notification events

## Accessibility

- All components include ARIA labels
- Keyboard navigation supported
- Screen reader friendly
- Color contrast compliant
- Focus management implemented

## Support

For issues or questions:
1. Check browser console for errors
2. Verify preferences are correctly saved
3. Test in different browsers
4. Check WebSocket connection status
5. Review integration example files
