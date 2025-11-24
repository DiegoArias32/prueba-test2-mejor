/**
 * Toast Component
 * Sistema de notificaciones visuales tipo toast
 */

'use client';

import React, { useEffect } from 'react';
import { FiCheckCircle, FiXCircle, FiAlertCircle, FiInfo, FiX } from 'react-icons/fi';

export type ToastType = 'success' | 'error' | 'warning' | 'info';

export interface ToastProps {
  id: string;
  type: ToastType;
  title: string;
  message: string;
  duration?: number;
  onClose: (id: string) => void;
}

export const Toast: React.FC<ToastProps> = ({
  id,
  type,
  title,
  message,
  duration = 5000,
  onClose
}) => {
  useEffect(() => {
    if (duration > 0) {
      const timer = setTimeout(() => {
        onClose(id);
      }, duration);
      return () => clearTimeout(timer);
    }
  }, [id, duration, onClose]);

  const getIcon = () => {
    switch (type) {
      case 'success':
        return <FiCheckCircle className="w-6 h-6" />;
      case 'error':
        return <FiXCircle className="w-6 h-6" />;
      case 'warning':
        return <FiAlertCircle className="w-6 h-6" />;
      case 'info':
        return <FiInfo className="w-6 h-6" />;
    }
  };

  const getColors = () => {
    switch (type) {
      case 'success':
        return 'bg-green-50 border-green-200 text-green-800';
      case 'error':
        return 'bg-red-50 border-red-200 text-red-800';
      case 'warning':
        return 'bg-yellow-50 border-yellow-200 text-yellow-800';
      case 'info':
        return 'bg-blue-50 border-blue-200 text-blue-800';
    }
  };

  const getIconColor = () => {
    switch (type) {
      case 'success':
        return 'text-green-500';
      case 'error':
        return 'text-red-500';
      case 'warning':
        return 'text-yellow-500';
      case 'info':
        return 'text-blue-500';
    }
  };

  return (
    <div
      className={`
        ${getColors()}
        min-w-[500px] max-w-2xl w-full shadow-lg rounded-xl pointer-events-auto
        flex ring-1 ring-black ring-opacity-5 border-2
        animate-slide-in-right
      `}
    >
      <div className="flex-1 w-0 p-4">
        <div className="flex items-start">
          <div className={`flex-shrink-0 ${getIconColor()}`}>
            {getIcon()}
          </div>
          <div className="ml-3 flex-1">
            <p className="text-sm font-semibold">{title}</p>
            <p className="mt-1 text-sm opacity-90">{message}</p>
          </div>
        </div>
      </div>
      <div className="flex border-l border-gray-200">
        <button
          onClick={() => onClose(id)}
          className="w-full border border-transparent rounded-none rounded-r-xl p-4 flex items-center justify-center text-sm font-medium hover:bg-black hover:bg-opacity-5 focus:outline-none transition-colors"
        >
          <FiX className="w-5 h-5" />
        </button>
      </div>
    </div>
  );
};

/**
 * Toast Container Component
 */
interface ToastContainerProps {
  toasts: ToastProps[];
  onClose: (id: string) => void;
}

export const ToastContainer: React.FC<ToastContainerProps> = ({ toasts, onClose }) => {
  return (
    <div className="fixed top-4 right-4 z-50 flex flex-col space-y-4 pointer-events-none">
      {toasts.map((toast) => (
        <Toast key={toast.id} {...toast} onClose={onClose} />
      ))}
    </div>
  );
};

/**
 * Hook para gestionar toasts
 */
export const useToast = () => {
  const [toasts, setToasts] = React.useState<ToastProps[]>([]);

  const addToast = (toast: Omit<ToastProps, 'id' | 'onClose'>) => {
    const id = Math.random().toString(36).substr(2, 9);
    setToasts((prev) => [...prev, { ...toast, id, onClose: removeToast }]);
  };

  const removeToast = (id: string) => {
    setToasts((prev) => prev.filter((t) => t.id !== id));
  };

  const success = (title: string, message: string = '') => {
    addToast({ type: 'success', title, message });
  };

  const error = (title: string, message: string = '') => {
    addToast({ type: 'error', title, message });
  };

  const warning = (title: string, message: string = '') => {
    addToast({ type: 'warning', title, message });
  };

  const info = (title: string, message: string = '') => {
    addToast({ type: 'info', title, message });
  };

  return {
    toasts,
    removeToast,
    success,
    error,
    warning,
    info
  };
};
