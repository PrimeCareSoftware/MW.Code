import { Component, HostListener, signal, OnInit, OnDestroy, AfterViewInit, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { Auth } from '../../services/auth';
import { NotificationPanel } from '../notification-panel/notification-panel';
import { ThemeToggleComponent } from '../theme-toggle/theme-toggle.component';
import { ClinicSelectorComponent } from '../clinic-selector/clinic-selector';
import { RolePermissionService } from '../../services/role-permission.service';
import { BusinessConfigurationService } from '../../services/business-configuration.service';

@Component({
  selector: 'app-navbar',
  imports: [CommonModule, RouterLink, RouterLinkActive, NotificationPanel, ThemeToggleComponent, ClinicSelectorComponent],
  templateUrl: './navbar.html',
  styleUrl: './navbar.scss'
})
export class Navbar implements OnInit, OnDestroy, AfterViewInit {
  dropdownOpen = false;
  sidebarOpen = true;
  adminDropdownOpen = false;
  private sidebarElement: HTMLElement | null = null;
  private boundSaveScrollPosition: (() => void) | null = null;
  
  // Menu group states - all start closed except the one containing the active route

  canAccessCareMenu = signal<boolean>(true);
  canAccessTelemedicineMenu = signal<boolean>(true);

  menuGroups: { [key: string]: boolean } = {
    core: false,
    analytics: false,
    clinical: false,
    support: false,
    crm: false,
    financial: false,
    settings: false,
    compliance: false,
    tiss: false,
    admin: false,
    help: false
  };
  
  constructor(
    public authService: Auth,
    private elementRef: ElementRef,
    private rolePermissionService: RolePermissionService,
    private businessConfigurationService: BusinessConfigurationService
  ) {
    // Check localStorage for sidebar state (only in browser environment)
    if (typeof localStorage !== 'undefined') {
      try {
        const savedState = localStorage.getItem('sidebarOpen');
        this.sidebarOpen = savedState !== null ? savedState === 'true' : true;
        
        // Restore menu group states
        const savedMenuGroups = localStorage.getItem('menuGroups');
        if (savedMenuGroups) {
          this.menuGroups = JSON.parse(savedMenuGroups);
        }
      } catch (error) {
        console.warn('Could not access localStorage:', error);
        this.sidebarOpen = true;
      }
    }
  }

  ngOnInit(): void {
    if (typeof document !== 'undefined') {
      document.body.classList.add('has-navbar', 'has-sidebar');
    }
    this.updateBodyClass();
    this.updateMenuPermissions();
  }

  ngAfterViewInit(): void {
    // Get reference to sidebar element
    if (typeof document !== 'undefined') {
      this.sidebarElement = this.elementRef.nativeElement.querySelector('.sidebar');
      
      if (this.sidebarElement) {
        // Restore scroll position
        this.restoreSidebarScrollPosition();
        
        // Save scroll position on scroll - store bound function reference for cleanup
        this.boundSaveScrollPosition = this.saveSidebarScrollPosition.bind(this);
        this.sidebarElement.addEventListener('scroll', this.boundSaveScrollPosition);
      }
    }
  }

  ngOnDestroy(): void {
    if (typeof document !== 'undefined') {
      document.body.classList.remove('sidebar-open', 'has-navbar', 'has-sidebar');
      
      // Remove scroll event listener
      if (this.sidebarElement && this.boundSaveScrollPosition) {
        this.sidebarElement.removeEventListener('scroll', this.boundSaveScrollPosition);
      }
    }
  }

  private saveSidebarScrollPosition(): void {
    if (this.sidebarElement && typeof localStorage !== 'undefined') {
      try {
        localStorage.setItem('sidebarScrollPosition', this.sidebarElement.scrollTop.toString());
      } catch (error) {
        console.warn('Could not save sidebar scroll position:', error);
      }
    }
  }

  private restoreSidebarScrollPosition(): void {
    if (this.sidebarElement && typeof localStorage !== 'undefined') {
      try {
        const savedPosition = localStorage.getItem('sidebarScrollPosition');
        if (savedPosition !== null) {
          const scrollPosition = parseInt(savedPosition, 10);
          if (!isNaN(scrollPosition)) {
            this.sidebarElement.scrollTop = scrollPosition;
          }
        }
      } catch (error) {
        console.warn('Could not restore sidebar scroll position:', error);
      }
    }
  }

  toggleDropdown(): void {
    this.dropdownOpen = !this.dropdownOpen;
  }
  
  toggleSidebar(): void {
    this.sidebarOpen = !this.sidebarOpen;
    if (typeof localStorage !== 'undefined') {
      try {
        localStorage.setItem('sidebarOpen', this.sidebarOpen.toString());
      } catch (error) {
        console.warn('Could not save to localStorage:', error);
      }
    }
    this.updateBodyClass();
    this.updateMenuPermissions();
  }

  updateBodyClass(): void {
    if (typeof document !== 'undefined') {
      if (this.sidebarOpen) {
        document.body.classList.add('sidebar-open');
      } else {
        document.body.classList.remove('sidebar-open');
      }
    }
  }
  
  toggleAdminDropdown(): void {
    this.adminDropdownOpen = !this.adminDropdownOpen;
  }
  
  closeSidebar(): void {
    if (typeof window !== 'undefined' && window.innerWidth < 1024) {
      this.sidebarOpen = false;
      if (typeof localStorage !== 'undefined') {
        try {
          localStorage.setItem('sidebarOpen', 'false');
        } catch (error) {
          console.warn('Could not save to localStorage:', error);
        }
      }
      this.updateBodyClass();
      this.updateMenuPermissions();
    }
  }
  
  private updateMenuPermissions(): void {
    const user = this.authService.currentUser();
    const canAccessCareFeatures = this.rolePermissionService.canAccessCareFeatures(user?.role, !!user?.isSystemOwner);

    this.canAccessCareMenu.set(canAccessCareFeatures);
    this.canAccessTelemedicineMenu.set(canAccessCareFeatures);

    if (!user || !this.rolePermissionService.isOwnerRole(user.role)) {
      return;
    }

    const clinicId = user.currentClinicId || user.clinicId;
    if (!clinicId) {
      return;
    }

    this.businessConfigurationService.isFeatureEnabled(clinicId, 'telemedicine').subscribe({
      next: (response) => {
        this.canAccessTelemedicineMenu.set(canAccessCareFeatures && response.enabled);
      },
      error: () => {
        this.canAccessTelemedicineMenu.set(false);
      }
    });
  }

  isOwner(): boolean {
    const user = this.authService.currentUser();
    return user ? (user.role === 'Owner' || user.role === 'ClinicOwner' || !!user.isSystemOwner) : false;
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    if (typeof document !== 'undefined' && event?.target) {
      const target = event.target as HTMLElement;
      if (!target.closest('.user-dropdown')) {
        this.dropdownOpen = false;
      }
      if (!target.closest('.admin-dropdown')) {
        this.adminDropdownOpen = false;
      }
    }
  }

  logout(): void {
    this.dropdownOpen = false;
    this.adminDropdownOpen = false;
    this.authService.logout();
  }
  
  toggleMenuGroup(groupName: string): void {
    this.menuGroups[groupName] = !this.menuGroups[groupName];
    
    // Save menu group states to localStorage
    if (typeof localStorage !== 'undefined') {
      try {
        localStorage.setItem('menuGroups', JSON.stringify(this.menuGroups));
      } catch (error) {
        console.warn('Could not save menu groups to localStorage:', error);
      }
    }
  }
  
  isMenuGroupOpen(groupName: string): boolean {
    return this.menuGroups[groupName] || false;
  }
}
