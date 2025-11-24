/**
 * ToastContainer Component
 * Container for stacking multiple toast notifications with configurable position
 */

'use client';

import React from 'react';
import { ToastNotification, ToastType, ToastAction } from './ToastNotification';

export interface Toast {
  id: string;
  type: ToastType;
  title: string;
  message: string;
  duration?: number;
  actions?: ToastAction[];
  onView?: () => void;
}

export interface ToastContainerProps {
  toasts: Toast[];
  position?: 'top-right' | 'top-left' | 'bottom-right' | 'bottom-left';
  onClose: (id: string) => void;
}

export const ToastContainer: React.FC<ToastContainerProps> = ({
  toasts,
  position = 'top-right',
  onClose
}) => {
  const getPositionClasses = () => {
    switch (position) {
      case 'top-right':
        return 'top-4 right-4';
      case 'top-left':
        return 'top-4 left-4';
      case 'bottom-right':
        return 'bottom-4 right-4';
      case 'bottom-left':
        return 'bottom-4 left-4';
      default:
        return 'top-4 right-4';
    }
  };

  if (toasts.length === 0) return null;

  return (
    <div
      className={`fixed ${getPositionClasses()} z-50 space-y-3 pointer-events-none`}
      aria-live="polite"
      aria-atomic="true"
    >
      <div className="pointer-events-auto space-y-3">
        {toasts.map((toast) => (
          <ToastNotification
            key={toast.id}
            id={toast.id}
            type={toast.type}
            title={toast.title}
            message={toast.message}
            duration={toast.duration}
            actions={toast.actions}
            onView={toast.onView}
            onClose={onClose}
          />
        ))}
      </div>
    </div>
  );
};
