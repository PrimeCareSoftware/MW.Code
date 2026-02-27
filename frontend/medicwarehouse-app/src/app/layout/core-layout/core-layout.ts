import { Component, signal, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { inject } from '@angular/core';
import { Auth } from '../../services/auth';
import { ThemeService } from '../../services/theme.service';
import { BreadcrumbComponent } from '../../shared/components/breadcrumb/breadcrumb';

interface NavItem {
  label: string;
  icon: string;
  route?: string;
  children?: NavItem[];
  group?: string;
}

@Component({
  selector: 'app-core-layout',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, RouterOutlet, BreadcrumbComponent],
  templateUrl: './core-layout.html',
  styleUrl: './core-layout.scss'
})
export class CoreLayoutComponent {
  private auth = inject(Auth);
  private themeService = inject(ThemeService);
  private router = inject(Router);

  sidebarCollapsed = signal(false);
  mobileMenuOpen = signal(false);
  userMenuOpen = signal(false);

  get currentUser() { return this.auth.currentUser(); }
  get currentTheme() { return this.themeService.getTheme(); }

  navGroups: { label: string; items: NavItem[] }[] = [
    {
      label: 'Principal',
      items: [
        { label: 'Dashboard', icon: 'grid', route: '/dashboard' },
        { label: 'Agenda', icon: 'calendar', route: '/appointments/calendar' },
        { label: 'Pacientes', icon: 'users', route: '/patients' },
        { label: 'Fila de Espera', icon: 'clock', route: '/waiting-queue' },
      ]
    },
    {
      label: 'Clínica',
      items: [
        { label: 'Consultas', icon: 'clipboard', route: '/appointments/list' },
        { label: 'Procedimentos', icon: 'activity', route: '/procedures' },
        { label: 'Anamnese', icon: 'file-text', route: '/anamnesis/templates' },
        { label: 'Telemedicina', icon: 'video', route: '/telemedicine' },
        { label: 'Prescrições', icon: 'file-plus', route: '/prescriptions' },
      ]
    },
    {
      label: 'Gestão',
      items: [
        { label: 'Financeiro', icon: 'dollar-sign', route: '/financial/receivables' },
        { label: 'Chamados', icon: 'message-square', route: '/tickets' },
        { label: 'CRM', icon: 'heart', route: '/crm/complaints' },
        { label: 'Analytics', icon: 'bar-chart-2', route: '/analytics' },
        { label: 'Chat', icon: 'message-circle', route: '/chat' },
      ]
    },
    {
      label: 'Administração',
      items: [
        { label: 'Perfis', icon: 'shield', route: '/admin/profiles' },
        { label: 'Configurações', icon: 'settings', route: '/settings/company' },
        { label: 'Logs de Auditoria', icon: 'list', route: '/audit-logs' },
        { label: 'TISS', icon: 'layers', route: '/tiss/operators' },
      ]
    }
  ];

  toggleSidebar(): void {
    this.sidebarCollapsed.update(v => !v);
  }

  toggleMobileMenu(): void {
    this.mobileMenuOpen.update(v => !v);
  }

  closeMobileMenu(): void {
    this.mobileMenuOpen.set(false);
  }

  toggleUserMenu(): void {
    this.userMenuOpen.update(v => !v);
  }

  closeUserMenu(): void {
    this.userMenuOpen.set(false);
  }

  toggleTheme(): void {
    this.themeService.toggleTheme();
  }

  logout(): void {
    this.auth.logout();
    this.router.navigate(['/site/login']);
  }

  getUserInitials(): string {
    const user = this.currentUser;
    if (!user?.username) return '?';
    return user.username.slice(0, 2).toUpperCase();
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    const target = event.target as HTMLElement;
    if (!target.closest('.user-menu-wrapper')) {
      this.userMenuOpen.set(false);
    }
  }
}
