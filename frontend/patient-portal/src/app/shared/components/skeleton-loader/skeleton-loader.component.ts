import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-skeleton-loader',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="skeleton-loader" [ngClass]="['skeleton-' + variant, shape]" [ngStyle]="getStyles()">
      <div class="skeleton-shimmer"></div>
    </div>
  `,
  styles: [`
    .skeleton-loader {
      position: relative;
      background: hsl(var(--muted));
      overflow: hidden;
      border-radius: var(--radius-md);
    }

    .skeleton-shimmer {
      position: absolute;
      top: 0;
      left: -100%;
      width: 100%;
      height: 100%;
      background: linear-gradient(
        90deg,
        transparent,
        hsl(var(--background) / 0.6),
        transparent
      );
      animation: shimmer 1.5s infinite;
    }

    @keyframes shimmer {
      to {
        left: 100%;
      }
    }

    .skeleton-card {
      height: 200px;
    }

    .skeleton-text {
      height: 16px;
      margin-bottom: 8px;
    }

    .skeleton-title {
      height: 24px;
      width: 60%;
    }

    .skeleton-avatar {
      width: 56px;
      height: 56px;
    }

    .skeleton-button {
      height: 40px;
      width: 120px;
    }

    .circle {
      border-radius: 50%;
    }

    .rounded {
      border-radius: var(--radius-lg);
    }
  `]
})
export class SkeletonLoaderComponent {
  @Input() variant: 'card' | 'text' | 'title' | 'avatar' | 'button' = 'text';
  @Input() width?: string;
  @Input() height?: string;
  @Input() shape: 'default' | 'circle' | 'rounded' = 'default';

  getStyles() {
    const styles: any = {};
    if (this.width) styles.width = this.width;
    if (this.height) styles.height = this.height;
    return styles;
  }
}
