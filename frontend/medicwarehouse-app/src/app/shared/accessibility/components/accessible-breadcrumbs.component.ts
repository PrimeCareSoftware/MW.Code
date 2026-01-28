/**
 * Componente de Breadcrumbs Acessível
 * Navegação estruturada conforme WCAG 2.1
 */

import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

export interface BreadcrumbItem {
  label: string;
  url?: string;
  ariaLabel?: string;
}

@Component({
  selector: 'app-accessible-breadcrumbs',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <nav aria-label="Navegação estrutural (breadcrumb)" class="breadcrumbs">
      <ol class="breadcrumb-list">
        <li 
          *ngFor="let item of items; let i = index; let isLast = last"
          class="breadcrumb-item"
          [class.active]="isLast"
        >
          <a 
            *ngIf="!isLast && item.url"
            [routerLink]="item.url"
            [attr.aria-label]="item.ariaLabel || item.label"
            class="breadcrumb-link"
          >
            {{ item.label }}
          </a>
          <span 
            *ngIf="isLast"
            [attr.aria-current]="'page'"
            class="breadcrumb-current"
          >
            {{ item.label }}
          </span>
          <span 
            *ngIf="!isLast && !item.url"
            class="breadcrumb-text"
          >
            {{ item.label }}
          </span>
          <span 
            *ngIf="!isLast" 
            class="breadcrumb-separator" 
            aria-hidden="true"
          >
            /
          </span>
        </li>
      </ol>
    </nav>
  `,
  styles: [`
    .breadcrumbs {
      margin: 16px 0;
    }

    .breadcrumb-list {
      display: flex;
      flex-wrap: wrap;
      list-style: none;
      padding: 0;
      margin: 0;
      align-items: center;
      gap: 8px;
    }

    .breadcrumb-item {
      display: flex;
      align-items: center;
      gap: 8px;
      font-size: 14px;
    }

    .breadcrumb-link {
      color: #1976d2;
      text-decoration: none;
      padding: 4px 8px;
      border-radius: 4px;
      transition: all 0.2s ease;
    }

    .breadcrumb-link:hover {
      color: #1565c0;
      background: #e3f2fd;
      text-decoration: underline;
    }

    .breadcrumb-link:focus {
      outline: 3px solid #ffc107;
      outline-offset: 2px;
      background: #e3f2fd;
    }

    .breadcrumb-current {
      color: #666;
      font-weight: 600;
      padding: 4px 8px;
    }

    .breadcrumb-text {
      color: #666;
      padding: 4px 8px;
    }

    .breadcrumb-separator {
      color: #999;
      user-select: none;
    }

    @media (max-width: 600px) {
      .breadcrumb-list {
        font-size: 12px;
      }
    }
  `]
})
export class AccessibleBreadcrumbsComponent {
  @Input() items: BreadcrumbItem[] = [];
}
