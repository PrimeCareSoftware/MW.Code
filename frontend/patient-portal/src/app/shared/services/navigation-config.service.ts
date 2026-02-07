import { Injectable } from '@angular/core';

export interface NavItem {
  label: string;
  icon: string;
  route: string;
  badge?: number;
}

@Injectable({
  providedIn: 'root'
})
export class NavigationConfigService {
  private navItems: NavItem[] = [
    { label: 'Início', icon: 'home', route: '/dashboard' },
    { label: 'Consultas', icon: 'event', route: '/appointments' },
    { label: 'Documentos', icon: 'description', route: '/documents' },
    { label: 'Perfil', icon: 'person', route: '/profile' }
  ];

  /**
   * Get all navigation items
   */
  getNavItems(): NavItem[] {
    return [...this.navItems]; // Return a copy to prevent external modification
  }

  /**
   * Update badge count for a specific route
   */
  updateBadge(route: string, count: number): void {
    const item = this.navItems.find(item => item.route === route);
    if (item) {
      item.badge = count > 0 ? count : undefined;
    }
  }

  /**
   * Clear all badges
   */
  clearAllBadges(): void {
    this.navItems.forEach(item => item.badge = undefined);
  }

  /**
   * Update navigation configuration (useful for multi-language support)
   */
  setNavItems(items: NavItem[]): void {
    this.navItems = items;
  }
}
