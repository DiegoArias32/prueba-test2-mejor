/**
 * @file VerificacionDetalle.tsx
 * @description Componente que muestra los detalles de una cita verificada
 * @module features/verificar-cita/presentation/components
 */

import React from 'react';
import Link from 'next/link';
import { VerificacionCita } from '../../models/verificar-cita.models';
import { formatDate, formatTime } from '@/shared/utils/formatters';
import { StatusBadge } from '@/shared/components';

/**
 * Props del componente VerificacionDetalle
 * @interface VerificacionDetalleProps
 * @property {VerificacionCita} verificacion - Datos de la verificación
 */
interface VerificacionDetalleProps {
  verificacion: VerificacionCita;
}

/**
 * Componente que muestra los detalles completos de una cita verificada
 * @component
 * @param {VerificacionDetalleProps} props - Props del componente
 * @returns {JSX.Element} UI con los detalles de la cita
 */
export const VerificacionDetalle: React.FC<VerificacionDetalleProps> = ({ verificacion }) => {
  return (
    <div className="max-w-3xl mx-auto">
      <div className="bg-white rounded-2xl shadow-xl border border-gray-100 overflow-hidden">
        {/* Status Header */}
        <div className={`px-6 py-4 border-b border-gray-200 ${verificacion.isValid ? 'bg-blue-50' : 'bg-red-50'}`}>
          <div className="flex items-center justify-between">
            <div className="flex items-center">
              <div>
                <h3 className={`text-xl font-bold ${verificacion.isValid ? 'text-blue-800' : 'text-red-800'}`}>
                  Cita {verificacion.isValid ? 'Verificada' : 'No Válida'}
                </h3>
                <p className={verificacion.isValid ? 'text-blue-700' : 'text-red-700'}>
                  {verificacion.statusDescription}
                </p>
              </div>
            </div>
            {verificacion.isValid && (
              <StatusBadge statusCode={verificacion.status} showIcon={true} />
            )}
            {!verificacion.isValid && (
              <span className="px-4 py-2 rounded-full text-sm font-medium bg-red-500 text-white">
                No Válida
              </span>
            )}
          </div>
        </div>

        {/* Cita Details */}
        <div className="p-6">
          <div className="grid md:grid-cols-2 gap-8">
            {/* Left Column */}
            <div className="space-y-6">
              <div>
                <h4 className="text-lg font-semibold text-[#203461] mb-4">
                  📋 Información de la Cita
                </h4>
                <div className="space-y-3">
                  <div className="flex justify-between">
                    <span className="text-gray-600">Número de Cita:</span>
                    <span className="font-semibold text-[#1797D5]">{verificacion.appointmentNumber}</span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-gray-600">Fecha:</span>
                    <span className="font-medium">{formatDate(verificacion.appointmentDate)}</span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-gray-600">Hora:</span>
                    <span className="font-medium">{formatTime(verificacion.appointmentTime)}</span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-gray-600">Tipo de Servicio:</span>
                    <span className="font-medium">
                      {verificacion.appointmentType.icon} {verificacion.appointmentType.name}
                    </span>
                  </div>
                </div>
              </div>

              {verificacion.observations && (
                <div>
                  <h4 className="text-lg font-semibold text-[#203461] mb-2">
                    📝 Observaciones
                  </h4>
                  <div className="bg-gray-50 rounded-lg p-3">
                    <p className="text-gray-700">{verificacion.observations}</p>
                  </div>
                </div>
              )}
            </div>

            {/* Right Column */}
            <div className="space-y-6">
              <div>
                <h4 className="text-lg font-semibold text-[#203461] mb-4">
                  👤 Cliente
                </h4>
                <div className="space-y-3">
                  <div className="flex justify-between">
                    <span className="text-gray-600">Número:</span>
                    <span className="font-medium">{verificacion.client.clientNumber}</span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-gray-600">Nombre:</span>
                    <span className="font-medium">{verificacion.client.fullName}</span>
                  </div>
                </div>
              </div>

              <div>
                <h4 className="text-lg font-semibold text-[#203461] mb-4">
                  🏢 Sede de Atención
                </h4>
                <div className="space-y-3">
                  <div className="flex justify-between">
                    <span className="text-gray-600">Nombre:</span>
                    <span className="font-medium">{verificacion.branch.name}</span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-gray-600">Dirección:</span>
                    <span className="font-medium text-sm">{verificacion.branch.address}</span>
                  </div>
                </div>
              </div>

              <div>
                <h4 className="text-lg font-semibold text-[#203461] mb-2">
                  📅 Fecha de Creación
                </h4>
                <p className="text-gray-600 text-sm">{verificacion.createdAt}</p>
              </div>
            </div>
          </div>

          {/* Important Notes */}
          <div className="mt-8 p-4 bg-blue-50 border border-blue-200 rounded-xl">
            <h4 className="font-semibold text-blue-800 mb-2">📋 Importante:</h4>
            <ul className="text-blue-700 text-sm space-y-1">
              <li>• Presentarse 10 minutos antes de la hora programada</li>
              <li>• Traer documento de identidad</li>
              <li>• Este código QR es único e intransferible</li>
              {verificacion.status === 'Pendiente' && (
                <li>• La cita está confirmada y activa</li>
              )}
            </ul>
          </div>

          {/* Action Buttons */}
          <div className="mt-6 flex justify-center space-x-4">
            <Link
              href="/agendamiento-citas"
              className="bg-gradient-to-r from-[#203461] to-[#1797D5] text-white px-6 py-3 rounded-xl font-semibold hover:from-[#1A6192] hover:to-[#56C2E1] transition-all duration-300"
            >
              Agendar Otra Cita
            </Link>
            <Link
              href="/"
              className="bg-gray-500 text-white px-6 py-3 rounded-xl font-semibold hover:bg-gray-600 transition-all duration-300"
            >
              Volver al Inicio
            </Link>
          </div>
        </div>
      </div>
    </div>
  );
};
