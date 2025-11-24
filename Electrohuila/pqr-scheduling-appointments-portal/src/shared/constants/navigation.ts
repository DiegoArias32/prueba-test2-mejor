/**
 * Navigation Constants
 * Centralized menu items and navigation configuration
 */

export interface MenuItem {
  label: string;
  href: string;
  icon: string;
}

/**
 * Main menu items used across the application
 */
export const MAIN_MENU_ITEMS: MenuItem[] = [
  { label: 'Nuestra Empresa', href: '#', icon: '🏢' },
  { label: 'Usuarios', href: '#', icon: '👥' },
  { label: 'Proveedores', href: '#', icon: '🏭' },
  { label: 'Contáctenos', href: '#', icon: '📞' }
];
