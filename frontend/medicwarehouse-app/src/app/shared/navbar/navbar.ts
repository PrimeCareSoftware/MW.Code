import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { Auth } from '../../services/auth';

@Component({
  selector: 'app-navbar',
  imports: [CommonModule, RouterLink, RouterLinkActive],
  templateUrl: './navbar.html',
  styleUrl: './navbar.scss'
})
export class Navbar {
  constructor(public authService: Auth) {}

  logout(): void {
    this.authService.logout();
  }

  isSystemAdmin(): boolean {
    // Check if user has system tenant (system owner)
    const user = this.authService.currentUser();
    return user?.tenantId === 'system';
  }
}
