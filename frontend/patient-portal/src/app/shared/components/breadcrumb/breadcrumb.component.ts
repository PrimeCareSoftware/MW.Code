import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';

export interface BreadcrumbItem {
  label: string;
  route?: string;
  icon?: string;
}

@Component({
  selector: 'app-breadcrumb',
  standalone: true,
  imports: [CommonModule, RouterModule, MatIconModule],
  template: `
    <nav class="breadcrumb" aria-label="breadcrumb">
      <ol class="breadcrumb-list">
        <li *ngFor="let item of items; let last = last; let i = index" class="breadcrumb-item">
          <mat-icon *ngIf="i === 0 && item.icon" class="home-icon">{{ item.icon }}</mat-icon>
          <a 
            *ngIf="!last && item.route" 
            [routerLink]="item.route"
            class="breadcrumb-link">
            {{ item.label }}
          </a>
          <span *ngIf="last" class="breadcrumb-current" aria-current="page">
            {{ item.label }}
          </span>
          <mat-icon *ngIf="!last" class="separator">chevron_right</mat-icon>
        </li>
      </ol>
    </nav>
  `,
  styles: [`
    .breadcrumb {
      padding: var(--spacing-3) 0;
      background: transparent;
    }

    .breadcrumb-list {
      display: flex;
      align-items: center;
      flex-wrap: wrap;
      list-style: none;
      margin: 0;
      padding: 0;
      gap: var(--spacing-2);
    }

    .breadcrumb-item {
      display: flex;
      align-items: center;
      gap: var(--spacing-2);
      font-size: var(--text-sm);

      .home-icon {
        font-size: 20px;
        width: 20px;
        height: 20px;
        color: hsl(var(--muted-foreground));
      }
    }

    .breadcrumb-link {
      color: hsl(var(--muted-foreground));
      text-decoration: none;
      font-weight: var(--font-medium);
      transition: color var(--transition-fast);
      padding: var(--spacing-1) var(--spacing-2);
      border-radius: var(--radius-sm);

      &:hover {
        color: hsl(var(--primary));
        background: hsl(var(--primary) / 0.05);
      }

      &:focus-visible {
        outline: 2px solid hsl(var(--ring));
        outline-offset: 2px;
      }
    }

    .breadcrumb-current {
      color: hsl(var(--foreground));
      font-weight: var(--font-semibold);
    }

    .separator {
      font-size: 18px;
      width: 18px;
      height: 18px;
      color: hsl(var(--muted-foreground) / 0.5);
    }

    /* Mobile optimization */
    @media (max-width: 640px) {
      .breadcrumb-list {
        font-size: var(--text-xs);
      }

      .breadcrumb-link,
      .breadcrumb-current {
        max-width: 120px;
        overflow: hidden;
        text-overflow: ellipsis;
        white-space: nowrap;
      }

      .separator {
        font-size: 16px;
        width: 16px;
        height: 16px;
      }
    }
  `]
})
export class BreadcrumbComponent {
  @Input() items: BreadcrumbItem[] = [];
}
