import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-skeleton-loader',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="skeleton-loader" [ngClass]="type">
      <div *ngIf="type === 'text'" class="skeleton-text-wrapper">
        <div class="skeleton-line" *ngFor="let line of linesArray" 
             [style.width]="getLineWidth(line)"></div>
      </div>
      
      <div *ngIf="type === 'card'" class="skeleton-card">
        <div class="skeleton-image"></div>
        <div class="skeleton-content">
          <div class="skeleton-line" style="width: 80%;"></div>
          <div class="skeleton-line" style="width: 60%;"></div>
          <div class="skeleton-line" style="width: 90%;"></div>
        </div>
      </div>
      
      <div *ngIf="type === 'table'" class="skeleton-table">
        <div class="skeleton-row" *ngFor="let row of linesArray">
          <div class="skeleton-cell" *ngFor="let col of [1, 2, 3, 4]"></div>
        </div>
      </div>
      
      <div *ngIf="type === 'circle'" class="skeleton-circle"></div>
      
      <div *ngIf="type === 'avatar'" class="skeleton-avatar"></div>
    </div>
  `,
  styles: [`
    .skeleton-loader {
      width: 100%;
    }
    
    .skeleton-line,
    .skeleton-image,
    .skeleton-cell,
    .skeleton-circle,
    .skeleton-avatar {
      background: linear-gradient(
        90deg, 
        var(--skeleton-base, #f0f0f0) 25%, 
        var(--skeleton-highlight, #e0e0e0) 50%, 
        var(--skeleton-base, #f0f0f0) 75%
      );
      background-size: 200% 100%;
      animation: shimmer 1.5s infinite;
      border-radius: 4px;
    }
    
    .skeleton-text-wrapper {
      display: flex;
      flex-direction: column;
      gap: 12px;
    }
    
    .skeleton-line {
      height: 16px;
    }
    
    .skeleton-card {
      border-radius: 8px;
      overflow: hidden;
    }
    
    .skeleton-image {
      height: 160px;
      width: 100%;
      border-radius: 8px 8px 0 0;
    }
    
    .skeleton-content {
      padding: 16px;
      display: flex;
      flex-direction: column;
      gap: 12px;
    }
    
    .skeleton-table {
      display: flex;
      flex-direction: column;
      gap: 8px;
    }
    
    .skeleton-row {
      display: flex;
      gap: 12px;
    }
    
    .skeleton-cell {
      flex: 1;
      height: 40px;
    }
    
    .skeleton-circle {
      width: 64px;
      height: 64px;
      border-radius: 50%;
    }
    
    .skeleton-avatar {
      width: 40px;
      height: 40px;
      border-radius: 50%;
    }
    
    @keyframes shimmer {
      0% {
        background-position: -200% 0;
      }
      100% {
        background-position: 200% 0;
      }
    }
    
    /* Dark mode support */
    :host-context(.theme-dark) .skeleton-line,
    :host-context(.theme-dark) .skeleton-image,
    :host-context(.theme-dark) .skeleton-cell,
    :host-context(.theme-dark) .skeleton-circle,
    :host-context(.theme-dark) .skeleton-avatar {
      --skeleton-base: #2d3748;
      --skeleton-highlight: #3d4758;
    }
  `]
})
export class SkeletonLoaderComponent {
  @Input() type: 'text' | 'card' | 'table' | 'circle' | 'avatar' = 'text';
  @Input() lines = 3;
  
  get linesArray(): number[] {
    return Array(this.lines).fill(0).map((_, i) => i);
  }
  
  getLineWidth(index: number): string {
    // Vary line widths for more realistic skeleton
    const widths = ['100%', '90%', '75%', '85%', '95%'];
    return widths[index % widths.length];
  }
}
