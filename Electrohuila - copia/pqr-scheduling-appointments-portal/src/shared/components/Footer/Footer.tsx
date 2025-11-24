/**
 * @file Footer.tsx
 * @description Shared footer component for consistent branding across the application
 * @module shared/components/Footer
 */

'use client';

import React from 'react';
import { FiPhone, FiHelpCircle, FiUsers } from 'react-icons/fi';

/**
 * Footer Component
 * Displays company information, contact details, and copyright
 * Used across all public pages for consistent branding
 *
 * @component
 * @example
 * ```tsx
 * <Footer />
 * ```
 */
export const Footer: React.FC = () => {
  const currentYear = new Date().getFullYear();

  return (
    <footer className="bg-gradient-to-r from-[#203461] to-[#1A6192] text-white py-12 print:hidden">
      <div className="max-w-7xl mx-auto px-4">
        {/* Company Logo */}
        <div className="flex items-center space-x-3 mb-8">
          <img
            src="https://www.electrohuila.com.co/wp-content/uploads/2024/07/cropped-logo-nuevo-eh.png.webp"
            alt="ElectroHuila Logo"
            className="h-10 w-auto object-contain filter brightness-0 invert"
            width="100"
            height="24"
          />
        </div>

        {/* Contact Information Grid */}
        <div className="grid md:grid-cols-4 gap-6 text-sm">
          <div className="bg-white/10 rounded-xl p-4 backdrop-blur-sm">
            <div className="font-semibold mb-2 flex items-center gap-2">
              <FiPhone className="inline" aria-hidden="true" />
              <span>Atención al Cliente</span>
            </div>
            <div className="text-white/90">(608) 8664600</div>
          </div>

          <div className="bg-white/10 rounded-xl p-4 backdrop-blur-sm">
            <div className="font-semibold mb-2 flex items-center gap-2">
              <FiHelpCircle className="inline" aria-hidden="true" />
              <span>Mesa de Ayuda</span>
            </div>
            <div className="text-white/90">(608) 8664646</div>
          </div>

          <div className="bg-white/10 rounded-xl p-4 backdrop-blur-sm">
            <div className="font-semibold mb-2 flex items-center gap-2">
              <FiPhone className="inline" aria-hidden="true" />
              <span>Línea Gratuita</span>
            </div>
            <div className="text-white/90">018000952115</div>
          </div>

          <div className="bg-white/10 rounded-xl p-4 backdrop-blur-sm">
            <div className="font-semibold mb-2 flex items-center gap-2">
              <FiUsers className="inline" aria-hidden="true" />
              <span>Transparencia</span>
            </div>
            <div className="text-white/90">Línea PQR</div>
          </div>
        </div>

        {/* Copyright */}
        <div className="mt-8 pt-8 border-t border-white/20 text-center text-white/70 text-sm">
          © <span className="note-year">{currentYear}</span> ElectroHuila S.A. E.S.P. - Todos los derechos reservados
        </div>
      </div>
    </footer>
  );
};
