import { Component, HostListener, signal, OnInit, OnDestroy, AfterViewInit, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { Auth } from '../../services/auth';
import { NotificationPanel } from '../notification-panel/notification-panel';

@Component({
  selector: 'app-navbar',
  imports: [CommonModule, RouterLink, RouterLinkActive, NotificationPanel],
  templateUrl: './navbar.html',
  styleUrl: './navbar.scss'
})
export class Navbar implements OnInit, OnDestroy, AfterViewInit {
  dropdownOpen = false;
  sidebarOpen = true;
  adminDropdownOpen = false;
  private sidebarElement: HTMLElement | null = null;
  private boundSaveScrollPosition: (() => void) | null = null;
  
  constructor(public authService: Auth, private elementRef: ElementRef) {
    // Check localStorage for sidebar state
    const savedState = localStorage.getItem('sidebarOpen');
    this.sidebarOpen = savedState !== null ? savedState === 'true' : true;
  }

  ngOnInit(): void {
    document.body.classList.add('has-navbar', 'has-sidebar');
    this.updateBodyClass();
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
    document.body.classList.remove('sidebar-open', 'has-navbar', 'has-sidebar');
    
    // Remove scroll event listener
    if (this.sidebarElement && this.boundSaveScrollPosition) {
      this.sidebarElement.removeEventListener('scroll', this.boundSaveScrollPosition);
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
          this.sidebarElement.scrollTop = parseInt(savedPosition, 10);
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
    localStorage.setItem('sidebarOpen', this.sidebarOpen.toString());
    this.updateBodyClass();
  }

  updateBodyClass(): void {
    if (this.sidebarOpen) {
      document.body.classList.add('sidebar-open');
    } else {
      document.body.classList.remove('sidebar-open');
    }
  }
  
  toggleAdminDropdown(): void {
    this.adminDropdownOpen = !this.adminDropdownOpen;
  }
  
  closeSidebar(): void {
    if (window.innerWidth < 1024) {
      this.sidebarOpen = false;
      localStorage.setItem('sidebarOpen', 'false');
      this.updateBodyClass();
    }
  }
  
  isOwner(): boolean {
    const user = this.authService.currentUser();
    return user ? (user.role === 'Owner' || user.role === 'ClinicOwner' || !!user.isSystemOwner) : false;
  }

  isSystemAdmin(): boolean {
    const user = this.authService.currentUser();
    return !!user?.isSystemOwner;
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    const target = event.target as HTMLElement;
    if (!target.closest('.user-dropdown')) {
      this.dropdownOpen = false;
    }
    if (!target.closest('.admin-dropdown')) {
      this.adminDropdownOpen = false;
    }
  }

  logout(): void {
    this.dropdownOpen = false;
    this.adminDropdownOpen = false;
    this.authService.logout();
  }
}
