import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { MatRippleModule } from '@angular/material/core';

interface NavItem {
  label: string;
  icon: string;
  route: string;
  badge?: number;
}

@Component({
  selector: 'app-bottom-nav',
  standalone: true,
  imports: [CommonModule, RouterModule, MatIconModule, MatRippleModule],
  template: `
    <nav class="bottom-nav">
      <a 
        *ngFor="let item of navItems"
        [routerLink]="item.route"
        routerLinkActive="active"
        [routerLinkActiveOptions]="{exact: item.route === '/dashboard'}"
        class="nav-item"
        matRipple
        [matRippleUnbounded]="false"
        [matRippleCentered]="false">
        <div class="nav-icon-wrapper">
          <mat-icon>{{ item.icon }}</mat-icon>
          <span *ngIf="item.badge" class="badge">{{ item.badge }}</span>
        </div>
        <span class="nav-label">{{ item.label }}</span>
      </a>
    </nav>
  `,
  styles: [`
    .bottom-nav {
      position: fixed;
      bottom: 0;
      left: 0;
      right: 0;
      z-index: 999;
      display: flex;
      justify-content: space-around;
      align-items: center;
      background: hsl(var(--card));
      border-top: 1px solid hsl(var(--border));
      box-shadow: 0 -4px 20px -4px hsl(var(--card-shadow) / 0.15);
      padding: var(--spacing-2) 0;
      backdrop-filter: blur(10px);
    }

    .nav-item {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      padding: var(--spacing-2) var(--spacing-4);
      text-decoration: none;
      color: hsl(var(--muted-foreground));
      transition: all var(--transition-fast);
      border-radius: var(--radius-md);
      min-width: 64px;
      position: relative;

      .nav-icon-wrapper {
        position: relative;
        display: flex;
        align-items: center;
        justify-content: center;

        mat-icon {
          font-size: 24px;
          width: 24px;
          height: 24px;
          transition: transform var(--transition-fast);
        }

        .badge {
          position: absolute;
          top: -4px;
          right: -8px;
          background: hsl(var(--destructive));
          color: hsl(var(--destructive-foreground));
          font-size: var(--text-xs);
          font-weight: var(--font-bold);
          padding: 2px 6px;
          border-radius: 12px;
          min-width: 18px;
          text-align: center;
        }
      }

      .nav-label {
        font-size: var(--text-xs);
        font-weight: var(--font-medium);
        margin-top: var(--spacing-1);
        text-align: center;
      }

      &:hover {
        color: hsl(var(--primary));
        background: hsl(var(--primary) / 0.05);

        mat-icon {
          transform: scale(1.1);
        }
      }

      &.active {
        color: hsl(var(--primary));
        
        .nav-icon-wrapper {
          mat-icon {
            transform: scale(1.15);
          }
        }

        .nav-label {
          font-weight: var(--font-bold);
        }
      }
    }

    /* Hide on desktop */
    @media (min-width: 769px) {
      .bottom-nav {
        display: none;
      }
    }
  `]
})
export class BottomNavComponent {
  navItems: NavItem[] = [
    { label: 'Início', icon: 'home', route: '/dashboard' },
    { label: 'Consultas', icon: 'event', route: '/appointments' },
    { label: 'Documentos', icon: 'description', route: '/documents' },
    { label: 'Perfil', icon: 'person', route: '/profile' }
  ];

  constructor(public router: Router) {}
}
