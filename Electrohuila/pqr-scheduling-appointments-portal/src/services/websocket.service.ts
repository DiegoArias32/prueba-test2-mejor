/**
 * SignalR Service
 * Servicio para conexión SignalR en tiempo real para notificaciones
 * Usa @microsoft/signalr para una conexión más robusta con reconexión automática
 */

import * as signalR from '@microsoft/signalr';
import React from 'react';

export type WebSocketEventType =
  | 'appointment_created'
  | 'appointment_updated'
  | 'appointment_assigned'
  | 'appointment_completed'
  | 'user_created'
  | 'user_updated'
  | 'role_updated'
  | 'permission_updated'
  | 'system_notification';

export interface WebSocketMessage {
  type: WebSocketEventType;
  data: unknown;
  timestamp: string;
}

export type WebSocketListener = (message: WebSocketMessage) => void;

class SignalRService {
  private connection: signalR.HubConnection | null = null;
  private listeners: Set<WebSocketListener> = new Set();
  private url: string;
  private token: string | null = null;
  private processedMessageIds: Set<string> = new Set();
  private readonly MAX_PROCESSED_MESSAGES = 100; // Keep last 100 message IDs

  constructor() {
    // Get API base URL from environment variable
    const apiUrl = typeof window !== 'undefined'
      ? process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000/api/v1'
      : 'http://localhost:5000/api/v1';

    // Remove /api/v1 suffix and add /hubs/notifications
    const baseUrl = apiUrl.replace(/\/api\/v\d+$/, '');

    // Backend SignalR endpoint
    // Development: http://localhost:5000/hubs/notifications
    // Production: https://api.electrohuila.com/hubs/notifications
    this.url = `${baseUrl}/hubs/notifications`;
  }

  /**
   * Connect to SignalR server
   */
  async connect(token?: string): Promise<void> {
    if (this.connection && this.connection.state === signalR.HubConnectionState.Connected) {
      return;
    }

    if (this.connection && this.connection.state === signalR.HubConnectionState.Connecting) {
      return;
    }

    this.token = token || null;

    try {
      // Create connection builder
      const connectionBuilder = new signalR.HubConnectionBuilder()
        .withUrl(this.url, {
          accessTokenFactory: () => this.token || '',
          skipNegotiation: false,
          transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.ServerSentEvents | signalR.HttpTransportType.LongPolling
        })
        .withAutomaticReconnect({
          nextRetryDelayInMilliseconds: retryContext => {
            // Exponential backoff: 0, 2, 10, 30 seconds, then 30 seconds
            if (retryContext.previousRetryCount === 0) {
              return 0;
            } else if (retryContext.previousRetryCount === 1) {
              return 2000;
            } else if (retryContext.previousRetryCount === 2) {
              return 10000;
            } else {
              return 30000;
            }
          }
        })
        .configureLogging(signalR.LogLevel.None); // Disable all SignalR logging

      this.connection = connectionBuilder.build();

      // Setup event handlers
      this.setupEventHandlers();

      await this.connection.start();

      // Send ping to confirm connection
      await this.ping();
    } catch (error) {
      // Silently fail - don't log connection errors to avoid console spam
      // Connection errors are expected when backend is not running
      throw error;
    }
  }

  /**
   * Disconnect from SignalR server
   */
  async disconnect(): Promise<void> {
    if (this.connection) {
      try {
        await this.connection.stop();
        this.connection = null;
      } catch (error) {
        console.error('[SignalR] Disconnect error:', error);
      }
    }
  }

  /**
   * Add event listener
   */
  addEventListener(listener: WebSocketListener): () => void {
    this.listeners.add(listener);

    // Return unsubscribe function
    return () => {
      this.listeners.delete(listener);
    };
  }

  /**
   * Remove event listener
   */
  removeEventListener(listener: WebSocketListener): void {
    this.listeners.delete(listener);
  }

  /**
   * Send ping to server (keep-alive)
   */
  async ping(): Promise<void> {
    if (this.connection && this.connection.state === signalR.HubConnectionState.Connected) {
      try {
        await this.connection.invoke('Ping');
      } catch (error) {
        console.error('[SignalR] Ping error:', error);
      }
    }
  }

  /**
   * Join a group
   */
  async joinGroup(groupName: string): Promise<void> {
    if (this.connection && this.connection.state === signalR.HubConnectionState.Connected) {
      try {
        await this.connection.invoke('JoinGroup', groupName);
      } catch (error) {
        console.error(`[SignalR] Error joining group ${groupName}:`, error);
      }
    }
  }

  /**
   * Leave a group
   */
  async leaveGroup(groupName: string): Promise<void> {
    if (this.connection && this.connection.state === signalR.HubConnectionState.Connected) {
      try {
        await this.connection.invoke('LeaveGroup', groupName);
      } catch (error) {
        console.error(`[SignalR] Error leaving group ${groupName}:`, error);
      }
    }
  }

  /**
   * Get connection status
   */
  isConnected(): boolean {
    return this.connection !== null && this.connection.state === signalR.HubConnectionState.Connected;
  }

  /**
   * Get connection state
   */
  getState(): signalR.HubConnectionState | null {
    return this.connection?.state ?? null;
  }

  // ===== PRIVATE METHODS =====

  private setupEventHandlers(): void {
    if (!this.connection) return;

    // Handle connection events - silently
    this.connection.onclose(() => {
      // Connection closed - silently handle
    });

    this.connection.onreconnecting(() => {
      // Reconnecting - silently handle
    });

    this.connection.onreconnected(() => {
      // Connection restored, no action needed
    });

    // Handle Connected event from server
    this.connection.on('Connected', () => {
      // Connection confirmed by server
    });

    // Handle Pong response
    this.connection.on('Pong', () => {
      // Pong received, connection alive
    });

    // Handle JoinedGroup event
    this.connection.on('JoinedGroup', () => {
      // Successfully joined group
    });

    // Handle LeftGroup event
    this.connection.on('LeftGroup', () => {
      // Successfully left group
    });

    // Handle ReceiveNotification - Main notification handler
    this.connection.on('ReceiveNotification', (notification) => {
      try {
        // Generate unique message ID based on type, data.id, and timestamp
        const messageId = `${notification.type}_${notification.data?.id || ''}_${notification.timestamp}`;

        // Check if we've already processed this message
        if (this.processedMessageIds.has(messageId)) {
          console.log('[SignalR] Duplicate message detected, skipping:', messageId);
          return;
        }

        // Add to processed set
        this.processedMessageIds.add(messageId);

        // Trim the set if it gets too large
        if (this.processedMessageIds.size > this.MAX_PROCESSED_MESSAGES) {
          const firstKey = this.processedMessageIds.values().next().value;
          if (firstKey) this.processedMessageIds.delete(firstKey);
        }

        const message: WebSocketMessage = {
          type: notification.type,
          data: notification.data,
          timestamp: notification.timestamp
        };

        // Notify all listeners
        this.listeners.forEach(listener => {
          try {
            listener(message);
          } catch (error) {
            console.error('[SignalR] Listener error:', error);
          }
        });
      } catch (error) {
        console.error('[SignalR] Error processing notification:', error);
      }
    });
  }
}

// Singleton instance
export const websocketService = new SignalRService();

/**
 * React Hook for SignalR notifications
 */
export const useWebSocket = (onMessage?: WebSocketListener) => {
  const [isConnected, setIsConnected] = React.useState(false);
  const [lastMessage, setLastMessage] = React.useState<WebSocketMessage | null>(null);

  React.useEffect(() => {
    let mounted = true;

    const initConnection = async () => {
      // Check if already connected
      if (websocketService.isConnected()) {
        if (mounted) setIsConnected(true);
        return;
      }

      // Try to connect with token from localStorage
      const token = typeof window !== 'undefined' ? localStorage.getItem('token') : null;

      try {
        await websocketService.connect(token || undefined);
        if (mounted) setIsConnected(true);
      } catch (error) {
        // Silently fail - connection errors are expected when backend is not running
        if (mounted) setIsConnected(false);
      }
    };

    initConnection();

    // Add listener
    const unsubscribe = websocketService.addEventListener((message) => {
      if (mounted) {
        setLastMessage(message);
        if (onMessage) {
          onMessage(message);
        }
      }
    });

    // Check connection status periodically
    const statusInterval = setInterval(() => {
      if (mounted) {
        setIsConnected(websocketService.isConnected());
      }
    }, 2000); // Check every 2 seconds

    return () => {
      mounted = false;
      unsubscribe();
      clearInterval(statusInterval);
      // Don't disconnect on cleanup - keep connection alive for other components
      // Only the main app should disconnect (e.g., on logout)
    };
  }, [onMessage]);

  const send = React.useCallback(async (
    // eslint-disable-next-line @typescript-eslint/no-unused-vars
    _method: string,
    // eslint-disable-next-line @typescript-eslint/no-unused-vars
    ..._args: unknown[]
  ) => {
    // Note: SignalR uses invoke() instead of send()
    // This is a compatibility wrapper - not implemented
    throw new Error('[useWebSocket] send() is not implemented for SignalR. Use connection.invoke() instead.');
  }, []);

  const joinGroup = React.useCallback(async (groupName: string) => {
    await websocketService.joinGroup(groupName);
  }, []);

  const leaveGroup = React.useCallback(async (groupName: string) => {
    await websocketService.leaveGroup(groupName);
  }, []);

  return {
    isConnected,
    lastMessage,
    send,
    joinGroup,
    leaveGroup
  };
};
