import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { Auth } from '../../services/auth';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <nav class="navbar">
      <div class="navbar-container">
        <div class="navbar-brand">
          <a routerLink="/dashboard" class="brand-link">
            <svg width="32" height="32" viewBox="0 0 48 48" fill="none" xmlns="http://www.w3.org/2000/svg">
              <rect width="48" height="48" rx="8" fill="#667eea"/>
              <path d="M24 14L18 20H22V34H26V20H30L24 14Z" fill="white"/>
            </svg>
            <span class="brand-text">MW System Admin</span>
          </a>
        </div>

        @if (authService.isAuthenticated()) {
          <div class="navbar-menu">
            <a routerLink="/dashboard" routerLinkActive="active" [routerLinkActiveOptions]="{exact: true}" class="nav-link">
              📊 Dashboard
            </a>
            <a routerLink="/clinics" routerLinkActive="active" class="nav-link">
              🏥 Clínicas
            </a>
            <a routerLink="/plans" routerLinkActive="active" class="nav-link">
              💎 Planos
            </a>
            <a routerLink="/clinic-owners" routerLinkActive="active" class="nav-link">
              👤 Proprietários
            </a>
            <a routerLink="/subdomains" routerLinkActive="active" class="nav-link">
              🌐 Subdomínios
            </a>
          </div>

          <div class="navbar-end">
            <div class="user-info">
              <span class="user-name">{{ authService.currentUser()?.username }}</span>
              <span class="user-role">System Owner</span>
            </div>
            <button class="btn-logout" (click)="logout()">
              🚪 Sair
            </button>
          </div>
        }
      </div>
    </nav>
  `,
  styles: [`
    .navbar {
      background: white;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
      position: sticky;
      top: 0;
      z-index: 100;
    }

    .navbar-container {
      max-width: 1400px;
      margin: 0 auto;
      padding: 0 24px;
      display: flex;
      justify-content: space-between;
      align-items: center;
      height: 64px;
    }

    .navbar-brand {
      display: flex;
      align-items: center;
    }

    .brand-link {
      display: flex;
      align-items: center;
      gap: 12px;
      text-decoration: none;
      color: inherit;
    }

    .brand-text {
      font-size: 18px;
      font-weight: 700;
      color: #1a202c;
    }

    .navbar-menu {
      display: flex;
      gap: 8px;
    }

    .nav-link {
      padding: 10px 16px;
      border-radius: 8px;
      font-size: 14px;
      font-weight: 600;
      color: #2d3748;
      text-decoration: none;
      transition: all 0.2s;
    }

    .nav-link:hover {
      background: #f7fafc;
      color: #667eea;
    }

    .nav-link.active {
      background: #667eea;
      color: white;
    }

    .navbar-end {
      display: flex;
      align-items: center;
      gap: 16px;
    }

    .user-info {
      display: flex;
      flex-direction: column;
      align-items: flex-end;
      gap: 2px;
    }

    .user-name {
      font-size: 14px;
      font-weight: 600;
      color: #1a202c;
    }

    .user-role {
      font-size: 12px;
      color: #718096;
    }

    .btn-logout {
      padding: 10px 16px;
      background: #fee2e2;
      color: #991b1b;
      border: none;
      border-radius: 8px;
      font-size: 14px;
      font-weight: 600;
      cursor: pointer;
      transition: all 0.2s;
    }

    .btn-logout:hover {
      background: #fecaca;
      transform: translateY(-2px);
    }

    @media (max-width: 768px) {
      .navbar-container {
        padding: 0 16px;
      }

      .brand-text {
        display: none;
      }

      .navbar-menu {
        gap: 4px;
      }

      .nav-link {
        padding: 8px 12px;
        font-size: 13px;
      }

      .user-info {
        display: none;
      }
    }
  `]
})
export class Navbar {
  constructor(
    public authService: Auth,
    private router: Router
  ) {}

  logout(): void {
    this.authService.logout();
  }
}
