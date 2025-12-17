import { Component, HostListener, signal } from '@angular/core';
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
export class Navbar {
  dropdownOpen = false;
  mobileMenuOpen = false;
  
  constructor(public authService: Auth) {}

  toggleDropdown(): void {
    this.dropdownOpen = !this.dropdownOpen;
  }
  
  toggleMobileMenu(): void {
    this.mobileMenuOpen = !this.mobileMenuOpen;
  }
  
  closeMobileMenu(): void {
    this.mobileMenuOpen = false;
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
    this.mobileMenuOpen = false;
    this.authService.logout();
  }
}
