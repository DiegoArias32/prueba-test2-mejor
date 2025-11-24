'use client';

import React, { useState, useEffect } from 'react';
import Link from 'next/link';
import { motion } from 'framer-motion';
import { authRepository } from '@/features/auth/repositories/auth.repository';
import { useRouter } from 'next/navigation';
import type { UserDto, UserPermissionsDto } from '@/core/types';

export default function UnauthorizedPage() {
  const router = useRouter();
  const [user, setUser] = useState<UserDto | null>(null);
  const [permissionsDetailed, setPermissionsDetailed] = useState<UserPermissionsDto | null>(null);

  useEffect(() => {
    // Cargar usuario y permisos desde localStorage
    const storedUser = authRepository.getStoredUser();
    const storedPermissions = authRepository.getStoredPermissions();

    setUser(storedUser);
    setPermissionsDetailed(storedPermissions);
  }, []);

  const logout = async () => {
    await authRepository.logout();
    router.push('/login');
  };

  const clearAndRetry = () => {
    // Limpiar TODO el localStorage
    if (typeof window !== 'undefined') {
      localStorage.clear();
    }
    // Redirigir al login
    router.push('/login');
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-50 via-blue-50 to-gray-50 overflow-hidden relative flex items-center justify-center px-4">
      {/* Animated Background Blobs */}
      <motion.div
        className="absolute top-20 left-10 w-72 h-72 bg-[#1797D5] rounded-full blur-3xl opacity-20 pointer-events-none"
        animate={{
          scale: [1, 1.2, 1],
          x: [0, 50, 0],
          y: [0, 30, 0]
        }}
        transition={{
          duration: 8,
          repeat: Infinity,
          ease: "easeInOut"
        }}
      />
      <motion.div
        className="absolute bottom-20 right-10 w-96 h-96 bg-[#56C2E1] rounded-full blur-3xl opacity-20 pointer-events-none"
        animate={{
          scale: [1, 1.3, 1],
          x: [0, -50, 0],
          y: [0, -30, 0]
        }}
        transition={{
          duration: 10,
          repeat: Infinity,
          ease: "easeInOut"
        }}
      />

      {/* Floating particles */}
      {[...Array(20)].map((_, i) => (
        <motion.div
          key={i}
          className="absolute w-2 h-2 bg-[#56C2E1] rounded-full pointer-events-none"
          style={{
            left: `${Math.random() * 100}%`,
            top: `${Math.random() * 100}%`,
          }}
          animate={{
            y: [0, -30, 0],
            opacity: [0.2, 0.5, 0.2]
          }}
          transition={{
            duration: 3 + Math.random() * 2,
            repeat: Infinity,
            delay: Math.random() * 2
          }}
        />
      ))}

      <div className="max-w-md w-full text-center relative z-10">
        <div className="bg-white rounded-3xl shadow-xl border border-gray-100 overflow-hidden">
          {/* Gradient Border Effect */}
          <div className="bg-gradient-to-r from-red-500 via-orange-500 to-yellow-500 h-1"></div>
          
          <div className="p-8">
            {/* Error Icon */}
            <motion.div
              className="mx-auto w-16 h-16 bg-red-100 rounded-full flex items-center justify-center mb-4"
              initial={{ scale: 0, rotate: -180 }}
              animate={{
                scale: 1,
                rotate: 0,
                x: [0, -10, 10, -10, 10, 0]
              }}
              transition={{
                scale: { duration: 0.5 },
                rotate: { duration: 0.5 },
                x: { duration: 0.6, delay: 0.5 }
              }}
            >
              <svg className="w-8 h-8 text-red-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.732-.833-2.5 0L4.268 18.5c-.77.833.192 2.5 1.732 2.5z" />
              </svg>
            </motion.div>

            {/* Title */}
            <motion.h1
              className="text-2xl font-bold bg-gradient-to-r from-red-600 to-orange-600 bg-clip-text text-transparent mb-2"
              initial={{ opacity: 0, y: -20 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ duration: 0.5, delay: 0.3 }}
            >
              Acceso No Autorizado
            </motion.h1>

            {/* Message */}
            <motion.p
              className="text-gray-600 mb-6"
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              transition={{ duration: 0.5, delay: 0.4 }}
            >
              No tienes permisos para acceder a esta sección del sistema.
              Contacta al administrador si crees que esto es un error.
            </motion.p>

            {/* User Info y Permisos */}
            {user && (
              <motion.div
                className="bg-gray-50 rounded-lg p-4 mb-6 text-sm"
                initial={{ opacity: 0, y: 10 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ delay: 0.45 }}
              >
                <motion.p
                  className="text-gray-700"
                  initial={{ opacity: 0, x: -10 }}
                  animate={{ opacity: 1, x: 0 }}
                  transition={{ delay: 0.5 }}
                >
                  <strong>Usuario actual:</strong> {user.username}
                </motion.p>
                <motion.p
                  className="text-gray-700"
                  initial={{ opacity: 0, x: -10 }}
                  animate={{ opacity: 1, x: 0 }}
                  transition={{ delay: 0.55 }}
                >
                  <strong>Email:</strong> {user.email}
                </motion.p>
                {/* Permisos por formulario/tab */}
                {permissionsDetailed && permissionsDetailed.forms && (
                  <div className="mt-4">
                    <motion.p
                      className="font-semibold text-gray-600 mb-2"
                      initial={{ opacity: 0 }}
                      animate={{ opacity: 1 }}
                      transition={{ delay: 0.6 }}
                    >
                      Permisos asignados:
                    </motion.p>
                    {Array.isArray(permissionsDetailed.forms) ? (
                      <div className="bg-yellow-50 border border-yellow-200 rounded-lg p-3 text-xs">
                        <p className="text-yellow-700 font-semibold">⚠️ Error: Los permisos están en formato array</p>
                        <p className="text-yellow-600 mt-1">Datos: {JSON.stringify(permissionsDetailed.forms).slice(0, 200)}...</p>
                      </div>
                    ) : (
                      <motion.div
                        className="flex flex-wrap gap-2"
                        initial={{ opacity: 0 }}
                        animate={{ opacity: 1 }}
                        transition={{ delay: 0.65, staggerChildren: 0.05 }}
                      >
                        {Object.entries(permissionsDetailed.forms).map(([formCode, perms], index) => {
                          const permissions = perms as { canRead?: boolean; canCreate?: boolean; canUpdate?: boolean; canDelete?: boolean };
                          return (
                            <motion.div
                              key={formCode}
                              className="bg-gray-100 rounded-lg px-3 py-1 text-xs text-gray-700 border border-gray-200"
                              initial={{ opacity: 0, scale: 0.8 }}
                              animate={{ opacity: 1, scale: 1 }}
                              transition={{ delay: 0.65 + index * 0.05 }}
                              whileHover={{ scale: 1.05 }}
                            >
                              <span className="font-semibold text-[#1797D5]">{formCode}:</span>
                              {permissions.canRead && <span className="ml-1">👁️ Leer</span>}
                              {permissions.canCreate && <span className="ml-1">➕ Crear</span>}
                              {permissions.canUpdate && <span className="ml-1">✏️ Editar</span>}
                              {permissions.canDelete && <span className="ml-1">🗑️ Eliminar</span>}
                            </motion.div>
                          );
                        })}
                      </motion.div>
                    )}
                  </div>
                )}
              </motion.div>
            )}

            {/* Actions */}
            <motion.div
              className="space-y-3"
              initial={{ opacity: 0, y: 20 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ delay: 0.5, staggerChildren: 0.1 }}
            >
              {Array.isArray(permissionsDetailed?.forms) && (
                <motion.button
                  onClick={clearAndRetry}
                  className="w-full inline-flex items-center justify-center px-4 py-2 bg-orange-500 text-white font-medium rounded-xl hover:bg-orange-600 transition-colors duration-300"
                  whileHover={{ scale: 1.02 }}
                  whileTap={{ scale: 0.98 }}
                >
                  <svg className="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
                  </svg>
                  🔧 Limpiar Caché y Reintentar
                </motion.button>
              )}

              <motion.div
                whileHover={{ scale: 1.02 }}
                whileTap={{ scale: 0.98 }}
              >
                <Link
                  href="/admin"
                  className="w-full inline-flex items-center justify-center px-4 py-2 bg-[#1797D5] text-white font-medium rounded-xl hover:bg-[#1A6192] transition-colors duration-300 block"
                >
                  <svg className="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6" />
                  </svg>
                  Volver al Panel Principal
                </Link>
              </motion.div>

              <motion.button
                onClick={logout}
                className="w-full inline-flex items-center justify-center px-4 py-2 bg-gray-600 text-white font-medium rounded-xl hover:bg-gray-700 transition-colors duration-300"
                whileHover={{ scale: 1.02 }}
                whileTap={{ scale: 0.98 }}
              >
                <svg className="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
                </svg>
                Cerrar Sesión
              </motion.button>

              <motion.div
                whileHover={{ scale: 1.05 }}
                whileTap={{ scale: 0.95 }}
              >
                <Link
                  href="/"
                  className="block text-[#1797D5] hover:text-[#1A6192] font-medium transition-colors duration-300 text-center py-2"
                >
                  ← Ir al Inicio
                </Link>
              </motion.div>
            </motion.div>
          </div>
        </div>

        {/* Contact Info */}
        <div className="mt-6 text-sm text-gray-600">
          <p>¿Necesitas ayuda?</p>
          <p>Contacta al administrador del sistema</p>
        </div>
      </div>
    </div>
  );
}