import { Component, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ThemeService, Theme } from '../../services/theme.service';
import { BreadcrumbComponent } from '../../shared/components/breadcrumb/breadcrumb.component';

@Component({
  selector: 'app-core-layout',
  standalone: true,
  imports: [CommonModule, RouterModule, BreadcrumbComponent],
  templateUrl: './core-layout.component.html',
  styleUrl: './core-layout.component.scss'
})
export class CoreLayoutComponent {
  private readonly themeService = inject(ThemeService);
  menuOpen = signal(false);
  collapsed = signal(false);

  readonly menuItems = [
    { label: 'Dashboard', path: '/dashboard' },
    { label: 'Pacientes', path: '/patients' },
    { label: 'Agendamentos', path: '/appointments/calendar' },
    { label: 'Administrativo', path: '/admin/profiles' },
    { label: 'Contratação', path: '/clinic-admin/subscription' }
  ];

  readonly currentTheme = computed(() => this.themeService.getTheme());

  toggleSidebar(): void { this.collapsed.update(v => !v); }
  toggleMobileMenu(): void { this.menuOpen.update(v => !v); }
  setTheme(theme: Theme): void { this.themeService.setTheme(theme); }
}
