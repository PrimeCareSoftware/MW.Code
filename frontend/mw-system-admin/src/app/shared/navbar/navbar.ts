import { Component, HostListener, signal, OnInit, OnDestroy } from '@angular/core';
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
export class Navbar implements OnInit, OnDestroy {
  dropdownOpen = false;
  sidebarOpen = true;
  
  constructor(public authService: Auth) {
    // Check localStorage for sidebar state
    const savedState = localStorage.getItem('sidebarOpen');
    this.sidebarOpen = savedState !== null ? savedState === 'true' : true;
  }

  ngOnInit(): void {
    this.updateBodyClass();
  }

  ngOnDestroy(): void {
    document.body.classList.remove('sidebar-open');
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
  
  closeSidebar(): void {
    if (window.innerWidth < 1024) {
      this.sidebarOpen = false;
      localStorage.setItem('sidebarOpen', 'false');
      this.updateBodyClass();
    }
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
  }

  logout(): void {
    this.dropdownOpen = false;
    this.authService.logout();
  }
}
