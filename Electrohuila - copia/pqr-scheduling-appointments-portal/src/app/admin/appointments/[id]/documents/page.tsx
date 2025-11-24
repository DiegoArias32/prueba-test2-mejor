/**
 * Appointment Documents Page
 * Página de Next.js para gestión de documentos de una cita
 */

'use client';

import React from 'react';
import { useParams } from 'next/navigation';
import { AppointmentDocumentsView } from '@/features/admin/views/appointments/AppointmentDocumentsView';

export default function AppointmentDocumentsPage() {
  const params = useParams();
  const appointmentId = parseInt(params.id as string, 10);

  if (isNaN(appointmentId)) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <h1 className="text-2xl font-bold text-gray-900 mb-4">
            ID de Cita Inválido
          </h1>
          <p className="text-gray-600">
            El ID de la cita proporcionado no es válido.
          </p>
        </div>
      </div>
    );
  }

  return <AppointmentDocumentsView appointmentId={appointmentId} />;
}
