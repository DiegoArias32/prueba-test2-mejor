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
    // Solo durante desarrollo - permitir warnings
    ignoreDuringBuilds: false,
  },
  typescript: {
    // Mantener verificación de tipos durante builds
    ignoreBuildErrors: false,
  },
};

export default nextConfig;
