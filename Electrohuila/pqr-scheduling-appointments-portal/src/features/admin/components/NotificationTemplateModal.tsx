/**
 * NotificationTemplateModal Component
 * Modal for creating/editing notification templates
 */

'use client';

import React, { useState, useEffect, useRef } from 'react';
import { FiX, FiSave, FiAlertCircle, FiEye, FiCode } from 'react-icons/fi';
import { PlaceholderPicker } from './PlaceholderPicker';
import { TemplatePreview } from './TemplatePreview';
import type { NotificationTemplateDto, CreateNotificationTemplateDto } from '@/services/notifications/notification-template.types';
import { TEMPLATE_TYPE_OPTIONS, PREDEFINED_TEMPLATES, AVAILABLE_PLACEHOLDERS } from '@/services/notifications/notification-template.types';
import { NotificationTemplateService } from '@/services/notifications/notification-template.service';

interface NotificationTemplateModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSave: (data: Partial<NotificationTemplateDto> | CreateNotificationTemplateDto) => Promise<void>;
  template?: NotificationTemplateDto | null;
  mode: 'create' | 'edit';
}

const templateService = new NotificationTemplateService();

export const NotificationTemplateModal: React.FC<NotificationTemplateModalProps> = ({
  isOpen,
  onClose,
  onSave,
  template,
  mode
}) => {
  // Form state
  const [formData, setFormData] = useState<Partial<NotificationTemplateDto>>({
    templateCode: '',
    templateName: '',
    subject: '',
    bodyTemplate: '',
    templateType: 'Email',
    placeholders: JSON.stringify(AVAILABLE_PLACEHOLDERS),
    isActive: true
  });

  // UI state
  const [errors, setErrors] = useState<Record<string, string>>({});
  const [isSaving, setIsSaving] = useState(false);
  const [showPreview, setShowPreview] = useState(false);
  const [invalidPlaceholders, setInvalidPlaceholders] = useState<string[]>([]);
  const bodyTextareaRef = useRef<HTMLTextAreaElement>(null);

  // Initialize form data
  useEffect(() => {
    if (template && mode === 'edit') {
      setFormData({
        ...template,
        placeholders: template.placeholders || JSON.stringify(AVAILABLE_PLACEHOLDERS)
      });
    } else {
      setFormData({
        templateCode: '',
        templateName: '',
        subject: '',
        bodyTemplate: '',
        templateType: 'Email',
        placeholders: JSON.stringify(AVAILABLE_PLACEHOLDERS),
        isActive: true
      });
    }
    setErrors({});
    setInvalidPlaceholders([]);
  }, [template, mode, isOpen]);

  // Validate placeholders when body changes
  useEffect(() => {
    if (formData.bodyTemplate) {
      const invalid = templateService.validatePlaceholders(
        formData.bodyTemplate,
        AVAILABLE_PLACEHOLDERS
      );
      setInvalidPlaceholders(invalid);
    } else {
      setInvalidPlaceholders([]);
    }
  }, [formData.bodyTemplate]);

  const handleChange = (field: keyof NotificationTemplateDto, value: string | boolean) => {
    setFormData(prev => ({ ...prev, [field]: value }));
    // Clear error for this field
    if (errors[field]) {
      setErrors(prev => {
        const newErrors = { ...prev };
        delete newErrors[field];
        return newErrors;
      });
    }
  };

  const handleInsertPlaceholder = (placeholder: string) => {
    const textarea = bodyTextareaRef.current;
    if (!textarea) return;

    const start = textarea.selectionStart;
    const end = textarea.selectionEnd;
    const text = formData.bodyTemplate || '';

    const newText = text.substring(0, start) + placeholder + text.substring(end);

    setFormData(prev => ({ ...prev, bodyTemplate: newText }));

    // Restore cursor position
    setTimeout(() => {
      textarea.focus();
      textarea.setSelectionRange(start + placeholder.length, start + placeholder.length);
    }, 0);
  };

  const validate = (): boolean => {
    const newErrors: Record<string, string> = {};

    // Template Code
    if (!formData.templateCode?.trim()) {
      newErrors.templateCode = 'El código es requerido';
    } else if (!/^[A-Z0-9_]+$/.test(formData.templateCode)) {
      newErrors.templateCode = 'Solo letras mayúsculas, números y guiones bajos';
    }

    // Template Name
    if (!formData.templateName?.trim()) {
      newErrors.templateName = 'El nombre es requerido';
    } else if (formData.templateName.length < 3 || formData.templateName.length > 100) {
      newErrors.templateName = 'El nombre debe tener entre 3 y 100 caracteres';
    }

    // Subject (only for Email)
    if (formData.templateType === 'Email' && !formData.subject?.trim()) {
      newErrors.subject = 'El asunto es requerido para emails';
    }

    // Body Template
    if (!formData.bodyTemplate?.trim()) {
      newErrors.bodyTemplate = 'El cuerpo del mensaje es requerido';
    } else if (formData.bodyTemplate.length > 5000) {
      newErrors.bodyTemplate = 'El cuerpo no puede exceder 5000 caracteres';
    }

    // Invalid placeholders
    if (invalidPlaceholders.length > 0) {
      newErrors.bodyTemplate = `Placeholders inválidos: ${invalidPlaceholders.join(', ')}`;
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!validate()) return;

    setIsSaving(true);
    try {
      await onSave(formData as NotificationTemplateDto);
      onClose();
    } catch (error) {
      console.error('Error saving template:', error);
    } finally {
      setIsSaving(false);
    }
  };

  const handlePredefinedCodeSelect = (code: string) => {
    const predefined = PREDEFINED_TEMPLATES.find(t => t.code === code);
    if (predefined) {
      setFormData(prev => ({
        ...prev,
        templateCode: code,
        templateName: predefined.name
      }));
    }
  };

  if (!isOpen) return null;

  const charCount = formData.bodyTemplate?.length || 0;
  const charLimit = 5000;
  const charPercentage = (charCount / charLimit) * 100;

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4 overflow-y-auto">
      <div className="bg-white rounded-lg shadow-2xl w-full max-w-6xl my-8 max-h-[90vh] flex flex-col">
        {/* Header */}
        <div className="flex items-center justify-between p-6 border-b border-gray-200 bg-gradient-to-r from-blue-50 to-purple-50">
          <div>
            <h2 className="text-2xl font-bold text-gray-900">
              {mode === 'create' ? 'Nueva Plantilla de Notificación' : 'Editar Plantilla'}
            </h2>
            <p className="text-sm text-gray-600 mt-1">
              {mode === 'create'
                ? 'Crea una nueva plantilla para notificaciones automáticas'
                : `Editando: ${template?.templateName || ''}`
              }
            </p>
          </div>
          <button
            onClick={onClose}
            className="p-2 hover:bg-white rounded-lg transition-colors"
            disabled={isSaving}
          >
            <FiX className="w-6 h-6 text-gray-500" />
          </button>
        </div>

        {/* Content */}
        <div className="flex-1 overflow-y-auto p-6">
          <form onSubmit={handleSubmit}>
            <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
              {/* Left Column - Form */}
              <div className="space-y-6">
                {/* Template Code */}
                <div>
                  <label className="block text-sm font-semibold text-gray-700 mb-2">
                    Código de Plantilla *
                  </label>
                  {mode === 'create' && (
                    <div className="mb-2">
                      <select
                        className="w-full px-3 py-2 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                        onChange={(e) => handlePredefinedCodeSelect(e.target.value)}
                        value=""
                      >
                        <option value="">-- Seleccionar plantilla predefinida --</option>
                        {PREDEFINED_TEMPLATES.map(t => (
                          <option key={t.code} value={t.code}>
                            {t.code} - {t.name}
                          </option>
                        ))}
                      </select>
                    </div>
                  )}
                  <input
                    type="text"
                    value={formData.templateCode || ''}
                    onChange={(e) => handleChange('templateCode', e.target.value.toUpperCase())}
                    disabled={mode === 'edit'}
                    className={`w-full px-4 py-2.5 border rounded-lg font-mono text-sm ${
                      mode === 'edit' ? 'bg-gray-100 cursor-not-allowed' : ''
                    } ${
                      errors.templateCode
                        ? 'border-red-300 focus:ring-red-500'
                        : 'border-gray-300 focus:ring-blue-500'
                    } focus:outline-none focus:ring-2`}
                    placeholder="APPT_CONFIRMATION"
                  />
                  {errors.templateCode && (
                    <p className="mt-1 text-sm text-red-600 flex items-center">
                      <FiAlertCircle className="w-4 h-4 mr-1" />
                      {errors.templateCode}
                    </p>
                  )}
                </div>

                {/* Template Name */}
                <div>
                  <label className="block text-sm font-semibold text-gray-700 mb-2">
                    Nombre de Plantilla *
                  </label>
                  <input
                    type="text"
                    value={formData.templateName || ''}
                    onChange={(e) => handleChange('templateName', e.target.value)}
                    className={`w-full px-4 py-2.5 border rounded-lg ${
                      errors.templateName
                        ? 'border-red-300 focus:ring-red-500'
                        : 'border-gray-300 focus:ring-blue-500'
                    } focus:outline-none focus:ring-2`}
                    placeholder="Confirmación de Cita"
                  />
                  {errors.templateName && (
                    <p className="mt-1 text-sm text-red-600 flex items-center">
                      <FiAlertCircle className="w-4 h-4 mr-1" />
                      {errors.templateName}
                    </p>
                  )}
                </div>

                {/* Template Type */}
                <div>
                  <label className="block text-sm font-semibold text-gray-700 mb-2">
                    Tipo de Notificación *
                  </label>
                  <div className="grid grid-cols-3 gap-3">
                    {TEMPLATE_TYPE_OPTIONS.map((option) => {
                      const isSelected = formData.templateType === option.value;
                      return (
                        <button
                          key={option.value}
                          type="button"
                          onClick={() => handleChange('templateType', option.value)}
                          className={`p-4 border-2 rounded-lg transition-all ${
                            isSelected
                              ? `border-${option.color}-500 bg-${option.color}-50`
                              : 'border-gray-200 hover:border-gray-300 bg-white'
                          }`}
                        >
                          <div className="text-2xl mb-2">{option.icon}</div>
                          <div className={`text-sm font-semibold ${
                            isSelected ? `text-${option.color}-700` : 'text-gray-700'
                          }`}>
                            {option.label}
                          </div>
                        </button>
                      );
                    })}
                  </div>
                </div>

                {/* Subject (Email only) */}
                {formData.templateType === 'Email' && (
                  <div>
                    <label className="block text-sm font-semibold text-gray-700 mb-2">
                      Asunto del Email *
                    </label>
                    <input
                      type="text"
                      value={formData.subject || ''}
                      onChange={(e) => handleChange('subject', e.target.value)}
                      className={`w-full px-4 py-2.5 border rounded-lg ${
                        errors.subject
                          ? 'border-red-300 focus:ring-red-500'
                          : 'border-gray-300 focus:ring-blue-500'
                      } focus:outline-none focus:ring-2`}
                      placeholder="Confirmación de tu cita - {{APPOINTMENT_DATE}}"
                    />
                    {errors.subject && (
                      <p className="mt-1 text-sm text-red-600 flex items-center">
                        <FiAlertCircle className="w-4 h-4 mr-1" />
                        {errors.subject}
                      </p>
                    )}
                  </div>
                )}

                {/* Body Template */}
                <div>
                  <div className="flex items-center justify-between mb-2">
                    <label className="block text-sm font-semibold text-gray-700">
                      Cuerpo del Mensaje *
                    </label>
                    <div className="flex items-center space-x-2">
                      <span className={`text-xs font-medium ${
                        charPercentage > 90 ? 'text-red-600' :
                        charPercentage > 70 ? 'text-orange-600' :
                        'text-gray-500'
                      }`}>
                        {charCount} / {charLimit}
                      </span>
                      <div className="w-20 h-2 bg-gray-200 rounded-full overflow-hidden">
                        <div
                          className={`h-full transition-all ${
                            charPercentage > 90 ? 'bg-red-500' :
                            charPercentage > 70 ? 'bg-orange-500' :
                            'bg-blue-500'
                          }`}
                          style={{ width: `${Math.min(charPercentage, 100)}%` }}
                        ></div>
                      </div>
                    </div>
                  </div>
                  <textarea
                    ref={bodyTextareaRef}
                    value={formData.bodyTemplate || ''}
                    onChange={(e) => handleChange('bodyTemplate', e.target.value)}
                    className={`w-full px-4 py-3 border rounded-lg font-mono text-sm ${
                      errors.bodyTemplate
                        ? 'border-red-300 focus:ring-red-500'
                        : 'border-gray-300 focus:ring-blue-500'
                    } focus:outline-none focus:ring-2`}
                    placeholder="Hola {{CLIENT_NAME}}, tu cita está confirmada para el {{APPOINTMENT_DATE}} a las {{APPOINTMENT_TIME}}..."
                    rows={10}
                  />
                  {errors.bodyTemplate && (
                    <p className="mt-1 text-sm text-red-600 flex items-center">
                      <FiAlertCircle className="w-4 h-4 mr-1" />
                      {errors.bodyTemplate}
                    </p>
                  )}
                  {invalidPlaceholders.length > 0 && (
                    <div className="mt-2 p-3 bg-yellow-50 border border-yellow-200 rounded-lg">
                      <p className="text-sm text-yellow-800">
                        <strong>Advertencia:</strong> Se encontraron placeholders no válidos
                      </p>
                      <div className="mt-1 flex flex-wrap gap-1">
                        {invalidPlaceholders.map(p => (
                          <code key={p} className="px-2 py-0.5 bg-yellow-200 text-yellow-900 rounded text-xs">
                            {p}
                          </code>
                        ))}
                      </div>
                    </div>
                  )}
                </div>

                {/* Placeholders Picker */}
                <div>
                  <label className="block text-sm font-semibold text-gray-700 mb-3">
                    Placeholders Disponibles
                  </label>
                  <PlaceholderPicker
                    onInsert={handleInsertPlaceholder}
                    compact={true}
                  />
                </div>
              </div>

              {/* Right Column - Preview */}
              <div className="space-y-4">
                {/* Preview Toggle */}
                <div className="flex items-center justify-between p-4 bg-gray-50 rounded-lg">
                  <div className="flex items-center space-x-2">
                    <div className={`p-2 rounded-lg ${showPreview ? 'bg-blue-100 text-blue-700' : 'bg-gray-200 text-gray-600'}`}>
                      {showPreview ? <FiEye className="w-5 h-5" /> : <FiCode className="w-5 h-5" />}
                    </div>
                    <span className="text-sm font-semibold text-gray-700">
                      {showPreview ? 'Vista Previa' : 'Editor'}
                    </span>
                  </div>
                  <button
                    type="button"
                    onClick={() => setShowPreview(!showPreview)}
                    className="px-4 py-2 bg-white border border-gray-300 rounded-lg text-sm font-medium text-gray-700 hover:bg-gray-50 transition-colors"
                  >
                    {showPreview ? 'Mostrar Placeholders' : 'Mostrar Vista Previa'}
                  </button>
                </div>

                {/* Preview Component */}
                {showPreview ? (
                  <TemplatePreview
                    templateType={formData.templateType || 'Email'}
                    subject={formData.subject}
                    bodyTemplate={formData.bodyTemplate || ''}
                    templateName={formData.templateName}
                  />
                ) : (
                  <div className="bg-gray-50 rounded-lg p-6 border-2 border-dashed border-gray-300">
                    <div className="text-center text-gray-500 space-y-3">
                      <FiCode className="w-12 h-12 mx-auto text-gray-400" />
                      <p className="text-sm">
                        Haz clic en &quot;Mostrar Vista Previa&quot; para ver cómo se verá la notificación
                      </p>
                    </div>
                  </div>
                )}
              </div>
            </div>
          </form>
        </div>

        {/* Footer */}
        <div className="flex items-center justify-between p-6 border-t border-gray-200 bg-gray-50">
          <div className="flex items-center space-x-2">
            <input
              type="checkbox"
              id="isActive"
              checked={formData.isActive}
              onChange={(e) => handleChange('isActive', e.target.checked)}
              className="w-4 h-4 text-blue-600 border-gray-300 rounded focus:ring-blue-500"
            />
            <label htmlFor="isActive" className="text-sm font-medium text-gray-700">
              Plantilla activa
            </label>
          </div>
          <div className="flex items-center space-x-3">
            <button
              type="button"
              onClick={onClose}
              disabled={isSaving}
              className="px-6 py-2.5 border border-gray-300 rounded-lg text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 transition-colors disabled:opacity-50"
            >
              Cancelar
            </button>
            <button
              onClick={handleSubmit}
              disabled={isSaving}
              className="px-6 py-2.5 bg-gradient-to-r from-blue-600 to-purple-600 text-white rounded-lg text-sm font-medium hover:from-blue-700 hover:to-purple-700 transition-all disabled:opacity-50 flex items-center space-x-2"
            >
              <FiSave className="w-4 h-4" />
              <span>{isSaving ? 'Guardando...' : 'Guardar Plantilla'}</span>
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};
