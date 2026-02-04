import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-badge',
  standalone: true,
  imports: [CommonModule, MatIconModule],
  template: `
    <span 
      class="badge" 
      [ngClass]="[variant, size]"
      [class.with-icon]="icon"
      [class.dot]="dot">
      <mat-icon *ngIf="icon && !dot" class="badge-icon">{{ icon }}</mat-icon>
      <span *ngIf="dot" class="badge-dot"></span>
      <ng-content></ng-content>
    </span>
  `,
  styles: [`
    .badge {
      display: inline-flex;
      align-items: center;
      gap: var(--spacing-1);
      padding: var(--spacing-1) var(--spacing-3);
      border-radius: calc(var(--radius) * 2);
      font-weight: var(--font-semibold);
      font-size: var(--text-xs);
      line-height: 1;
      white-space: nowrap;
      transition: all var(--transition-fast);

      &.sm {
        font-size: 0.65rem;
        padding: 2px var(--spacing-2);
      }

      &.md {
        font-size: var(--text-xs);
        padding: var(--spacing-1) var(--spacing-3);
      }

      &.lg {
        font-size: var(--text-sm);
        padding: var(--spacing-2) var(--spacing-4);
      }

      &.primary {
        background: hsl(var(--primary) / 0.15);
        color: hsl(var(--primary));
        border: 1px solid hsl(var(--primary) / 0.3);
      }

      &.success {
        background: hsl(var(--success) / 0.15);
        color: hsl(var(--success));
        border: 1px solid hsl(var(--success) / 0.3);
      }

      &.warning {
        background: hsl(var(--warning) / 0.15);
        color: hsl(var(--warning));
        border: 1px solid hsl(var(--warning) / 0.3);
      }

      &.error {
        background: hsl(var(--destructive) / 0.15);
        color: hsl(var(--destructive));
        border: 1px solid hsl(var(--destructive) / 0.3);
      }

      &.info {
        background: hsl(var(--info) / 0.15);
        color: hsl(var(--info));
        border: 1px solid hsl(var(--info) / 0.3);
      }

      &.neutral {
        background: hsl(var(--muted));
        color: hsl(var(--muted-foreground));
        border: 1px solid hsl(var(--border));
      }

      &.outline {
        background: transparent;
        
        &.primary {
          color: hsl(var(--primary));
          border: 1.5px solid hsl(var(--primary));
        }

        &.success {
          color: hsl(var(--success));
          border: 1.5px solid hsl(var(--success));
        }

        &.warning {
          color: hsl(var(--warning));
          border: 1.5px solid hsl(var(--warning));
        }

        &.error {
          color: hsl(var(--destructive));
          border: 1.5px solid hsl(var(--destructive));
        }
      }
    }

    .badge-icon {
      font-size: 14px;
      width: 14px;
      height: 14px;
    }

    .badge-dot {
      width: 6px;
      height: 6px;
      border-radius: 50%;
      background: currentColor;
    }

    .badge.with-icon,
    .badge.dot {
      padding-left: var(--spacing-2);
    }
  `]
})
export class BadgeComponent {
  @Input() variant: 'primary' | 'success' | 'warning' | 'error' | 'info' | 'neutral' | 'outline' = 'primary';
  @Input() size: 'sm' | 'md' | 'lg' = 'md';
  @Input() icon?: string;
  @Input() dot = false;
}
