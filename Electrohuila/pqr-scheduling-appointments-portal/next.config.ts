import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  // Desactivar React Strict Mode para evitar doble montaje en desarrollo
  reactStrictMode: false,
  images: {
    remotePatterns: [
      {
        protocol: 'https',
        hostname: 'www.electrohuila.com.co',
        port: '',
        pathname: '/wp-content/uploads/**',
      },
    ],
  },
  eslint: {
    // Ignorar errores de ESLint durante builds de producción
    ignoreDuringBuilds: true,
  },
  typescript: {
    // Ignorar errores de TypeScript durante builds de producción
    ignoreBuildErrors: true,
  },
};

export default nextConfig;
