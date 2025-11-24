/**
 * Step 3: Confirmation
 * Shows appointment confirmation with QR code
 */

'use client';

import { FC, useState, useEffect } from 'react';
import Link from 'next/link';
import { FiCheckCircle, FiUser, FiCalendar, FiAlertCircle, FiPrinter, FiPlus } from 'react-icons/fi';
import { FixedHeader } from '@/shared/layouts';

interface ClientData {
  fullName: string;
  clientNumber: string;
  email: string;
  phone?: string;
  mobile?: string;
}

interface FormData {
  reason: string;
  branch: string;
  appointmentDate: string;
  appointmentTime: string;
  observations: string;
}

interface AppointmentConfirmation {
  ticketNumber: string;
  issueDateTime: string;
  clientData: ClientData;
  formData: FormData;
}

interface ConfirmationStepProps {
  appointmentData: AppointmentConfirmation;
  qrCodeDataURL: string | null;
  onPrint: () => void;
  onNewRequest: () => void;
}

export const ConfirmationStep: FC<ConfirmationStepProps> = ({
  appointmentData,
  qrCodeDataURL,
  onPrint,
  onNewRequest
}) => {
  const [showWarning, setShowWarning] = useState(true);

  useEffect(() => {
    const timer = setTimeout(() => setShowWarning(false), 7000);
    return () => clearTimeout(timer);
  }, []);

  return (
    <>
      <FixedHeader className="w-full print:hidden" />
      <div className="min-h-screen bg-gradient-to-br from-gray-50 to-blue-50 pt-32 pb-8 px-4 print:pt-4 print:min-h-0">
        <div className="max-w-3xl mx-auto print:max-w-full">
          <div className="bg-white rounded-2xl shadow-xl p-8 print:shadow-none print:p-4 print:rounded-none">
            {/* Success Header */}
            <div className="text-center mb-6 print:mb-4">
              <div className="inline-flex items-center justify-center w-20 h-20 bg-green-100 rounded-full mb-4 print:w-16 print:h-16 print:mb-2">
                <FiCheckCircle className="text-green-600 text-5xl print:text-3xl" />
              </div>
              <h1 className="text-3xl font-bold text-gray-800 mb-2 print:text-2xl print:mb-1">
                ¡Cita Agendada Exitosamente!
              </h1>
              <p className="text-gray-600 print:text-sm">
                Su cita ha sido registrada correctamente
              </p>
            </div>

            {/* Warning Message */}
            {showWarning && (
              <div className="mb-6 p-4 bg-blue-50 border-l-4 border-[#1797D5] rounded-r-lg animate-fade-in print:hidden">
                <div className="flex items-start">
                  <FiAlertCircle className="text-[#1797D5] text-xl mr-3 mt-0.5 flex-shrink-0" />
                  <div>
                    <p className="text-blue-900 font-semibold mb-1">Información Importante</p>
                    <p className="text-blue-800 text-sm">
                      Presente este comprobante el día de su cita. Puede imprimirlo o mostrar el código QR desde su dispositivo móvil.
                    </p>
                  </div>
                </div>
              </div>
            )}

            {/* Ticket/Receipt */}
            <div className="border-2 border-dashed border-gray-300 rounded-lg p-6 mb-6 print:p-3 print:mb-3 print:border print:rounded">
              {/* Ticket Number */}
              <div className="text-center mb-6 pb-6 border-b-2 border-dashed border-gray-200 print:mb-3 print:pb-3 print:border-b">
                <p className="text-gray-600 text-sm mb-1 print:text-xs">Número de Cita</p>
                <p className="text-4xl font-bold text-[#1797D5] print:text-2xl">
                  {appointmentData.ticketNumber}
                </p>
                <p className="text-gray-500 text-xs mt-2 print:mt-1">
                  Fecha de emisión: {appointmentData.issueDateTime}
                </p>
              </div>

              {/* Client Info */}
              <div className="mb-6 pb-6 border-b-2 border-dashed border-gray-200 print:mb-3 print:pb-3 print:border-b">
                <h3 className="font-semibold text-gray-800 mb-3 flex items-center print:text-sm print:mb-2">
                  <FiUser className="mr-2 text-[#1797D5]" />
                  Datos del Cliente
                </h3>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-3 text-sm print:text-xs print:gap-2">
                  <div>
                    <span className="text-gray-600">Nombre:</span>
                    <p className="font-semibold">{appointmentData.clientData.fullName}</p>
                  </div>
                  <div>
                    <span className="text-gray-600">Cliente:</span>
                    <p className="font-semibold">{appointmentData.clientData.clientNumber}</p>
                  </div>
                  <div>
                    <span className="text-gray-600">Dirección:</span>
                    <p className="font-semibold">{appointmentData.clientData.email}</p>
                  </div>
                  <div>
                    <span className="text-gray-600">Teléfono:</span>
                    <p className="font-semibold">
                      {appointmentData.clientData.phone || appointmentData.clientData.mobile || 'N/A'}
                    </p>
                  </div>
                </div>
              </div>

              {/* Appointment Details */}
              <div className="mb-6 pb-6 border-b-2 border-dashed border-gray-200 print:mb-3 print:pb-3 print:border-b">
                <h3 className="font-semibold text-gray-800 mb-3 flex items-center print:text-sm print:mb-2">
                  <FiCalendar className="mr-2 text-[#1797D5]" />
                  Detalles de la Cita
                </h3>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-3 text-sm print:text-xs print:gap-2">
                  <div>
                    <span className="text-gray-600">Motivo:</span>
                    <p className="font-semibold">{appointmentData.formData.reason}</p>
                  </div>
                  <div>
                    <span className="text-gray-600">Sede:</span>
                    <p className="font-semibold">{appointmentData.formData.branch}</p>
                  </div>
                  <div>
                    <span className="text-gray-600">Fecha:</span>
                    <p className="font-semibold">
                      {new Date(appointmentData.formData.appointmentDate).toLocaleDateString('es-CO', {
                        weekday: 'long',
                        year: 'numeric',
                        month: 'long',
                        day: 'numeric'
                      })}
                    </p>
                  </div>
                  <div>
                    <span className="text-gray-600">Hora:</span>
                    <p className="font-semibold">{appointmentData.formData.appointmentTime}</p>
                  </div>
                </div>
                {appointmentData.formData.observations && (
                  <div className="mt-3 print:mt-2">
                    <span className="text-gray-600">Observaciones:</span>
                    <p className="font-semibold mt-1">{appointmentData.formData.observations}</p>
                  </div>
                )}
              </div>

              {/* QR Code */}
              {qrCodeDataURL && (
                <div className="text-center">
                  <p className="text-gray-600 text-sm mb-3 print:text-xs print:mb-2">
                    Código QR para verificación
                  </p>
                  <div className="inline-block p-4 bg-white border-2 border-gray-200 rounded-lg print:p-2 print:border">
                    <img
                      src={qrCodeDataURL}
                      alt="QR Code"
                      className="w-48 h-48 mx-auto print:w-32 print:h-32"
                    />
                  </div>
                  <p className="text-gray-500 text-xs mt-2 print:mt-1">
                    Escanee este código al momento de su cita
                  </p>
                </div>
              )}
            </div>

            {/* Action Buttons */}
            <div className="flex flex-col sm:flex-row gap-4 print:hidden">
              <button
                onClick={onPrint}
                className="flex-1 px-6 py-3 bg-[#203461] text-white rounded-lg font-semibold hover:bg-[#1a2d52] transition-colors flex items-center justify-center"
              >
                <FiPrinter className="mr-2" />
                Imprimir Comprobante
              </button>
              <button
                onClick={onNewRequest}
                className="flex-1 px-6 py-3 border-2 border-[#1797D5] text-[#1797D5] rounded-lg font-semibold hover:bg-blue-50 transition-colors flex items-center justify-center"
              >
                <FiPlus className="mr-2" />
                Nueva Cita
              </button>
            </div>

            {/* Info Footer */}
            <div className="mt-8 p-4 bg-gray-50 rounded-lg text-center print:hidden">
              <p className="text-sm text-gray-600 mb-2">
                Para consultar o cancelar su cita, visite:
              </p>
              <Link
                href="/gestion-citas"
                className="text-[#1797D5] hover:underline font-semibold"
              >
                Gestión de Citas
              </Link>
            </div>
          </div>
        </div>
      </div>
    </>
  );
};
