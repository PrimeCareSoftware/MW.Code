import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-fab-button',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatIconModule, MatTooltipModule, RouterModule],
  template: `
    <button 
      *ngIf="!link"
      mat-fab 
      [color]="color"
      [matTooltip]="tooltip"
      matTooltipPosition="left"
      class="fab-button"
      (click)="handleClick()">
      <mat-icon>{{ icon }}</mat-icon>
    </button>

    <a 
      *ngIf="link"
      mat-fab 
      [color]="color"
      [matTooltip]="tooltip"
      matTooltipPosition="left"
      class="fab-button"
      [routerLink]="link">
      <mat-icon>{{ icon }}</mat-icon>
    </a>
  `,
  styles: [`
    .fab-button {
      position: fixed;
      bottom: var(--spacing-8);
      right: var(--spacing-8);
      z-index: 1000;
      box-shadow: var(--shadow-xl);
      transition: all var(--transition-base);
    }

    .fab-button:hover {
      transform: scale(1.1);
      box-shadow: 0 12px 40px -8px hsl(var(--primary) / 0.4);
    }

    .fab-button:active {
      transform: scale(0.95);
    }

    @media (max-width: 768px) {
      .fab-button {
        bottom: var(--spacing-6);
        right: var(--spacing-6);
      }
    }
  `]
})
export class FabButtonComponent {
  @Input() icon = 'add';
  @Input() tooltip = '';
  @Input() color: 'primary' | 'accent' | 'warn' = 'primary';
  @Input() link?: string;
  @Output() fabClick = new EventEmitter<void>();

  handleClick() {
    this.fabClick.emit();
  }
}
