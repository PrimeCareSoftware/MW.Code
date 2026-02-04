import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-progress-bar',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="progress-container" [ngClass]="size">
      <div class="progress-track">
        <div 
          class="progress-fill" 
          [ngClass]="color"
          [style.width.%]="value"
          [style.animation-duration.s]="animated ? 0.8 : 0">
        </div>
      </div>
      <span *ngIf="showLabel" class="progress-label">{{ value }}%</span>
    </div>
  `,
  styles: [`
    .progress-container {
      display: flex;
      align-items: center;
      gap: var(--spacing-3);
      width: 100%;

      &.sm {
        .progress-track {
          height: 4px;
        }

        .progress-label {
          font-size: var(--text-xs);
        }
      }

      &.md {
        .progress-track {
          height: 8px;
        }

        .progress-label {
          font-size: var(--text-sm);
        }
      }

      &.lg {
        .progress-track {
          height: 12px;
        }

        .progress-label {
          font-size: var(--text-base);
        }
      }
    }

    .progress-track {
      flex: 1;
      background: hsl(var(--muted));
      border-radius: calc(var(--radius) * 2);
      overflow: hidden;
      position: relative;
    }

    .progress-fill {
      height: 100%;
      border-radius: calc(var(--radius) * 2);
      transition: width var(--transition-slow);
      position: relative;
      overflow: hidden;

      &.primary {
        background: linear-gradient(90deg, hsl(var(--primary)), hsl(var(--accent)));
      }

      &.success {
        background: linear-gradient(90deg, hsl(var(--success)), hsl(158 64% 55%));
      }

      &.warning {
        background: linear-gradient(90deg, hsl(var(--warning)), hsl(45 93% 60%));
      }

      &.error {
        background: linear-gradient(90deg, hsl(var(--destructive)), hsl(0 84% 70%));
      }

      &::after {
        content: '';
        position: absolute;
        top: 0;
        left: 0;
        right: 0;
        bottom: 0;
        background: linear-gradient(
          90deg,
          transparent,
          rgba(255, 255, 255, 0.3),
          transparent
        );
        animation: shimmer 2s infinite;
      }
    }

    @keyframes shimmer {
      0% {
        transform: translateX(-100%);
      }
      100% {
        transform: translateX(100%);
      }
    }

    .progress-label {
      font-weight: var(--font-semibold);
      color: hsl(var(--foreground));
      min-width: 48px;
      text-align: right;
    }
  `]
})
export class ProgressBarComponent {
  @Input() value = 0;
  @Input() color: 'primary' | 'success' | 'warning' | 'error' = 'primary';
  @Input() size: 'sm' | 'md' | 'lg' = 'md';
  @Input() showLabel = false;
  @Input() animated = true;
}
