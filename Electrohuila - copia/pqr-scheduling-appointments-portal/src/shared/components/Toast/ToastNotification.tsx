/**
 * ToastNotification Component
 * Enhanced toast notification with animations, actions, and auto-dismiss
 */

'use client';

import React, { useEffect, useState } from 'react';
import { FiX, FiCheckCircle, FiAlertCircle, FiInfo, FiAlertTriangle, FiEye } from 'react-icons/fi';

export type ToastType = 'success' | 'error' | 'warning' | 'info';

export interface ToastAction {
  label: string;
  onClick: () => void;
}

export interface ToastNotificationProps {
  id: string;
  type: ToastType;
  title: string;
  message: string;
  duration?: number;
  actions?: ToastAction[];
  onClose: (id: string) => void;
  onView?: () => void;
}

export const ToastNotification: React.FC<ToastNotificationProps> = ({
  id,
  type,
  title,
  message,
  duration = 5000,
  actions = [],
  onClose,
  onView
}) => {
  const [isVisible, setIsVisible] = useState(false);
  const [isExiting, setIsExiting] = useState(false);
  const [progress, setProgress] = useState(100);

  useEffect(() => {
    // Slide in animation
    const showTimer = setTimeout(() => setIsVisible(true), 10);

    // Auto-dismiss timer
    let dismissTimer: NodeJS.Timeout;
    let progressInterval: NodeJS.Timeout;

    if (duration > 0) {
      dismissTimer = setTimeout(() => {
        handleClose();
      }, duration);

      // Progress bar animation
      const startTime = Date.now();
      progressInterval = setInterval(() => {
        const elapsed = Date.now() - startTime;
        const remaining = Math.max(0, 100 - (elapsed / duration) * 100);
        setProgress(remaining);

        if (remaining === 0) {
          clearInterval(progressInterval);
        }
      }, 50);
    }

    return () => {
      clearTimeout(showTimer);
      clearTimeout(dismissTimer);
      clearInterval(progressInterval);
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [duration]);

  const handleClose = () => {
    setIsExiting(true);
    setTimeout(() => {
      onClose(id);
    }, 300); // Match animation duration
  };

  const getIcon = () => {
    switch (type) {
      case 'success':
        return <FiCheckCircle className="w-6 h-6 text-green-500 flex-shrink-0" />;
      case 'error':
        return <FiAlertCircle className="w-6 h-6 text-red-500 flex-shrink-0" />;
      case 'warning':
        return <FiAlertTriangle className="w-6 h-6 text-yellow-500 flex-shrink-0" />;
      case 'info':
        return <FiInfo className="w-6 h-6 text-blue-500 flex-shrink-0" />;
      default:
        return <FiInfo className="w-6 h-6 text-gray-500 flex-shrink-0" />;
    }
  };

  const getBackgroundColor = () => {
    switch (type) {
      case 'success':
        return 'bg-green-50 border-green-200';
      case 'error':
        return 'bg-red-50 border-red-200';
      case 'warning':
        return 'bg-yellow-50 border-yellow-200';
      case 'info':
        return 'bg-blue-50 border-blue-200';
      default:
        return 'bg-gray-50 border-gray-200';
    }
  };

  const getProgressColor = () => {
    switch (type) {
      case 'success':
        return 'bg-green-500';
      case 'error':
        return 'bg-red-500';
      case 'warning':
        return 'bg-yellow-500';
      case 'info':
        return 'bg-blue-500';
      default:
        return 'bg-gray-500';
    }
  };

  return (
    <div
      className={`
        w-96 max-w-full bg-white rounded-lg shadow-lg border-2 overflow-hidden
        ${getBackgroundColor()}
        transition-all duration-300 ease-out
        ${isVisible && !isExiting ? 'translate-x-0 opacity-100' : 'translate-x-full opacity-0'}
      `}
      role="alert"
      aria-live="assertive"
    >
      {/* Progress Bar */}
      {duration > 0 && (
        <div className="h-1 bg-gray-200">
          <div
            className={`h-full transition-all duration-50 ${getProgressColor()}`}
            style={{ width: `${progress}%` }}
          />
        </div>
      )}

      {/* Content */}
      <div className="p-4">
        <div className="flex items-start space-x-3">
          {/* Icon */}
          {getIcon()}

          {/* Text Content */}
          <div className="flex-1 min-w-0">
            <h4 className="text-sm font-semibold text-gray-900 mb-1">
              {title}
            </h4>
            <p className="text-sm text-gray-600 break-words">
              {message}
            </p>

            {/* Actions */}
            {(actions.length > 0 || onView) && (
              <div className="mt-3 flex items-center space-x-2">
                {onView && (
                  <button
                    onClick={() => {
                      onView();
                      handleClose();
                    }}
                    className="px-3 py-1.5 text-xs font-medium text-blue-600 hover:text-blue-700
                             bg-blue-100 hover:bg-blue-200 rounded-md transition-colors
                             flex items-center space-x-1"
                  >
                    <FiEye className="w-3.5 h-3.5" />
                    <span>Ver</span>
                  </button>
                )}
                {actions.map((action, index) => (
                  <button
                    key={index}
                    onClick={() => {
                      action.onClick();
                      handleClose();
                    }}
                    className="px-3 py-1.5 text-xs font-medium text-gray-700 hover:text-gray-900
                             bg-gray-100 hover:bg-gray-200 rounded-md transition-colors"
                  >
                    {action.label}
                  </button>
                ))}
              </div>
            )}
          </div>

          {/* Close Button */}
          <button
            onClick={handleClose}
            className="text-gray-400 hover:text-gray-600 transition-colors flex-shrink-0"
            aria-label="Close notification"
          >
            <FiX className="w-5 h-5" />
          </button>
        </div>
      </div>
    </div>
  );
};
