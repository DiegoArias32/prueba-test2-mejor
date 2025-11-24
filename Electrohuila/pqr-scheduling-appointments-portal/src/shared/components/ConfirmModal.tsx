/**
 * ConfirmModal - Modal de confirmación personalizado
 * Reemplaza el confirm() nativo del navegador con un modal bonito y moderno
 */

import React from 'react';
import { FiAlertTriangle, FiX, FiCheck } from 'react-icons/fi';

interface ConfirmModalProps {
  isOpen: boolean;
  onClose: () => void;
  onConfirm: () => void;
  title: string;
  message: string;
  confirmText?: string;
  cancelText?: string;
  type?: 'danger' | 'warning' | 'info';
}

export const ConfirmModal: React.FC<ConfirmModalProps> = ({
  isOpen,
  onClose,
  onConfirm,
  title,
  message,
  confirmText = 'Aceptar',
  cancelText = 'Cancelar',
  type = 'warning'
}) => {
  if (!isOpen) return null;

  const handleConfirm = () => {
    onConfirm();
    onClose();
  };

  const getTypeStyles = () => {
    switch (type) {
      case 'danger':
        return {
          iconColor: 'text-red-500',
          iconBg: 'bg-red-100',
          confirmButton: 'bg-red-600 hover:bg-red-700 focus:ring-red-500'
        };
      case 'warning':
        return {
          iconColor: 'text-yellow-500',
          iconBg: 'bg-yellow-100',
          confirmButton: 'bg-yellow-600 hover:bg-yellow-700 focus:ring-yellow-500'
        };
      case 'info':
        return {
          iconColor: 'text-blue-500',
          iconBg: 'bg-blue-100',
          confirmButton: 'bg-blue-600 hover:bg-blue-700 focus:ring-blue-500'
        };
    }
  };

  const styles = getTypeStyles();

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center">
      {/* Overlay */}
      <div
        className="fixed inset-0 bg-black bg-opacity-50 transition-opacity"
        onClick={onClose}
      />

      {/* Modal */}
      <div className="relative bg-white rounded-2xl shadow-2xl max-w-md w-full mx-4 transform transition-all">
        {/* Header con icono */}
        <div className="flex flex-col items-center pt-8 pb-4 px-6">
          <div className={`${styles.iconBg} rounded-full p-4 mb-4`}>
            <FiAlertTriangle className={`w-8 h-8 ${styles.iconColor}`} />
          </div>
          <h3 className="text-xl font-semibold text-gray-900 text-center">
            {title}
          </h3>
        </div>

        {/* Body */}
        <div className="px-6 pb-6">
          <p className="text-gray-600 text-center">
            {message}
          </p>
        </div>

        {/* Footer con botones */}
        <div className="flex gap-3 px-6 pb-6">
          <button
            onClick={onClose}
            className="flex-1 px-4 py-2.5 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors font-medium focus:outline-none focus:ring-2 focus:ring-gray-400"
          >
            <FiX className="inline mr-2" />
            {cancelText}
          </button>
          <button
            onClick={handleConfirm}
            className={`flex-1 px-4 py-2.5 text-white rounded-lg transition-colors font-medium focus:outline-none focus:ring-2 ${styles.confirmButton}`}
          >
            <FiCheck className="inline mr-2" />
            {confirmText}
          </button>
        </div>
      </div>
    </div>
  );
};
