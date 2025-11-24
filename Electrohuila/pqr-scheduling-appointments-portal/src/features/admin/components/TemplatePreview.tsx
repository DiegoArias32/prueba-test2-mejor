/**
 * TemplatePreview Component
 * Shows real-time preview of notification template with placeholders resolved
 */

'use client';

import React, { useMemo } from 'react';
import { FiMail, FiMessageSquare, FiBell, FiEye } from 'react-icons/fi';
import { PREVIEW_DATA } from '@/services/notifications/notification-template.types';

interface TemplatePreviewProps {
  templateType: 'Email' | 'SMS' | 'Push';
  subject?: string;
  bodyTemplate: string;
  templateName?: string;
}

export const TemplatePreview: React.FC<TemplatePreviewProps> = ({
  templateType,
  subject = '',
  bodyTemplate,
  templateName = 'Vista Previa'
}) => {
  // Replace placeholders with preview data
  const previewSubject = useMemo(() => {
    let result = subject || '';
    Object.entries(PREVIEW_DATA).forEach(([key, value]) => {
      const placeholder = `{{${key}}}`;
      result = result.replace(new RegExp(placeholder, 'g'), value);
    });
    return result;
  }, [subject]);

  const previewBody = useMemo(() => {
    let result = bodyTemplate || '';
    Object.entries(PREVIEW_DATA).forEach(([key, value]) => {
      const placeholder = `{{${key}}}`;
      result = result.replace(new RegExp(placeholder, 'g'), value);
    });
    return result;
  }, [bodyTemplate]);

  // Detect placeholders in template
  const highlightedBody = useMemo(() => {
    const result = bodyTemplate || '';
    const placeholderRegex = /(\{\{[A-Z_]+\}\})/g;

    // Split by placeholders and wrap them in spans
    const parts = result.split(placeholderRegex);

    return parts.map((part, index) => {
      if (part.match(placeholderRegex)) {
        return (
          <span
            key={index}
            className="bg-blue-100 text-blue-700 px-1 py-0.5 rounded font-mono text-sm font-semibold"
          >
            {part}
          </span>
        );
      }
      return <span key={index}>{part}</span>;
    });
  }, [bodyTemplate]);

  const getIcon = () => {
    switch (templateType) {
      case 'Email':
        return <FiMail className="w-5 h-5" />;
      case 'SMS':
        return <FiMessageSquare className="w-5 h-5" />;
      case 'Push':
        return <FiBell className="w-5 h-5" />;
    }
  };

  const getTypeColor = () => {
    switch (templateType) {
      case 'Email':
        return 'blue';
      case 'SMS':
        return 'green';
      case 'Push':
        return 'purple';
    }
  };

  const color = getTypeColor();

  // Character count for SMS
  const smsCharCount = bodyTemplate.length;
  const smsSegments = Math.ceil(smsCharCount / 160);

  return (
    <div className="space-y-4">
      {/* Header */}
      <div className={`flex items-center justify-between p-4 bg-${color}-50 border-2 border-${color}-200 rounded-lg`}>
        <div className="flex items-center space-x-3">
          <div className={`p-2 bg-${color}-100 rounded-lg text-${color}-700`}>
            <FiEye className="w-5 h-5" />
          </div>
          <div>
            <h3 className={`font-semibold text-${color}-900`}>Vista Previa</h3>
            <p className="text-xs text-gray-600">{templateName}</p>
          </div>
        </div>
        <div className={`flex items-center space-x-2 px-3 py-1.5 bg-white border border-${color}-300 rounded-full`}>
          {getIcon()}
          <span className={`text-sm font-medium text-${color}-700`}>{templateType}</span>
        </div>
      </div>

      {/* Preview Content */}
      <div className="bg-white border-2 border-gray-200 rounded-lg overflow-hidden">
        {/* Email Preview */}
        {templateType === 'Email' && (
          <div className="divide-y divide-gray-200">
            {/* Email Header */}
            <div className="p-4 bg-gray-50">
              <div className="space-y-2">
                <div className="flex items-center space-x-2 text-xs text-gray-500">
                  <span className="font-semibold">De:</span>
                  <span>notificaciones@empresa.com</span>
                </div>
                <div className="flex items-center space-x-2 text-xs text-gray-500">
                  <span className="font-semibold">Para:</span>
                  <span>cliente@email.com</span>
                </div>
                <div className="flex items-center space-x-2 text-sm">
                  <span className="font-semibold text-gray-700">Asunto:</span>
                  <span className="text-gray-900">{previewSubject || '(Sin asunto)'}</span>
                </div>
              </div>
            </div>

            {/* Email Body */}
            <div className="p-6">
              <div className="prose prose-sm max-w-none">
                <div className="whitespace-pre-wrap text-gray-800 leading-relaxed">
                  {previewBody || '(Cuerpo del mensaje vacío)'}
                </div>
              </div>
            </div>

            {/* Email Footer */}
            <div className="p-4 bg-gray-50 text-xs text-gray-500 text-center">
              Este es un correo automático, por favor no responder.
            </div>
          </div>
        )}

        {/* SMS Preview */}
        {templateType === 'SMS' && (
          <div className="p-6">
            <div className="max-w-md mx-auto">
              {/* Phone mockup */}
              <div className="bg-gradient-to-br from-gray-100 to-gray-200 rounded-3xl p-4 shadow-xl">
                {/* Phone notch */}
                <div className="bg-black h-6 rounded-full mb-4 w-32 mx-auto"></div>

                {/* SMS bubble */}
                <div className="bg-white rounded-2xl p-4 shadow-sm">
                  <div className="space-y-2">
                    <div className="flex items-center space-x-2 mb-2">
                      <div className="w-8 h-8 bg-green-500 rounded-full flex items-center justify-center">
                        <FiMessageSquare className="w-4 h-4 text-white" />
                      </div>
                      <span className="font-semibold text-sm text-gray-700">Empresa</span>
                    </div>
                    <div className="bg-green-100 rounded-2xl rounded-tl-sm p-3">
                      <p className="text-sm text-gray-800 whitespace-pre-wrap">
                        {previewBody || '(Mensaje vacío)'}
                      </p>
                    </div>
                    <p className="text-xs text-gray-400 text-right">Ahora</p>
                  </div>
                </div>

                {/* Character count */}
                <div className="mt-4 text-center space-y-1">
                  <div className={`text-sm font-semibold ${
                    smsCharCount > 160 ? 'text-orange-600' : 'text-gray-600'
                  }`}>
                    {smsCharCount} caracteres
                  </div>
                  {smsSegments > 1 && (
                    <div className="text-xs text-orange-600">
                      {smsSegments} mensajes SMS
                    </div>
                  )}
                </div>
              </div>
            </div>
          </div>
        )}

        {/* Push Notification Preview */}
        {templateType === 'Push' && (
          <div className="p-6">
            <div className="max-w-sm mx-auto">
              {/* Phone mockup */}
              <div className="bg-gradient-to-br from-gray-800 to-gray-900 rounded-3xl p-4 shadow-2xl">
                {/* Phone notch */}
                <div className="bg-gray-950 h-6 rounded-full mb-4 w-32 mx-auto"></div>

                {/* Notification */}
                <div className="bg-white/95 backdrop-blur rounded-2xl p-4 shadow-lg">
                  <div className="flex items-start space-x-3">
                    <div className="w-10 h-10 bg-purple-500 rounded-lg flex items-center justify-center flex-shrink-0">
                      <FiBell className="w-5 h-5 text-white" />
                    </div>
                    <div className="flex-1 min-w-0">
                      <div className="flex items-center justify-between mb-1">
                        <h4 className="font-semibold text-gray-900 text-sm truncate">
                          {templateName}
                        </h4>
                        <span className="text-xs text-gray-500">ahora</span>
                      </div>
                      {subject && (
                        <p className="font-medium text-sm text-gray-800 mb-1 truncate">
                          {previewSubject}
                        </p>
                      )}
                      <p className="text-sm text-gray-600 line-clamp-2">
                        {previewBody || '(Mensaje vacío)'}
                      </p>
                    </div>
                  </div>
                </div>

                {/* Bottom bar */}
                <div className="h-1 bg-white/20 rounded-full mt-4"></div>
              </div>
            </div>
          </div>
        )}
      </div>

      {/* Template with Highlighted Placeholders */}
      <div className="bg-gray-50 border-2 border-gray-200 rounded-lg p-4">
        <div className="flex items-center space-x-2 mb-3">
          <div className="w-2 h-2 bg-blue-500 rounded-full"></div>
          <h4 className="text-sm font-semibold text-gray-700">Template con Placeholders</h4>
        </div>
        {subject && templateType === 'Email' && (
          <div className="mb-3 pb-3 border-b border-gray-300">
            <span className="text-xs font-semibold text-gray-600 block mb-1">Asunto:</span>
            <div className="text-sm text-gray-800 whitespace-pre-wrap">
              {subject || '(Sin asunto)'}
            </div>
          </div>
        )}
        <div className="text-sm text-gray-800 whitespace-pre-wrap leading-relaxed">
          {bodyTemplate ? highlightedBody : '(Cuerpo del mensaje vacío)'}
        </div>
      </div>
    </div>
  );
};
