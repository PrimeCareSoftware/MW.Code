import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-modern-card',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="modern-card" [class.hover-effect]="hoverable" [class.elevated]="elevated">
      <div class="card-header" *ngIf="title">
        <h3>{{ title }}</h3>
        <div class="card-actions">
          <ng-content select="[card-actions]"></ng-content>
        </div>
      </div>
      <div class="card-content">
        <ng-content></ng-content>
      </div>
      <div class="card-footer">
        <ng-content select="[card-footer]"></ng-content>
      </div>
    </div>
  `,
  styles: [`
    .modern-card {
      background: var(--card-background, #ffffff);
      border-radius: 12px;
      box-shadow: 0 1px 3px rgba(0, 0, 0, 0.12), 0 1px 2px rgba(0, 0, 0, 0.08);
      padding: 24px;
      transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
      border: 1px solid var(--card-border, rgba(0, 0, 0, 0.08));
    }
    
    .modern-card.hover-effect:hover {
      box-shadow: 0 8px 16px rgba(0, 0, 0, 0.15), 0 4px 8px rgba(0, 0, 0, 0.1);
      transform: translateY(-2px);
      cursor: pointer;
    }
    
    .modern-card.elevated {
      box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1), 0 2px 4px rgba(0, 0, 0, 0.06);
    }
    
    .card-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 16px;
      border-bottom: 1px solid var(--card-divider, rgba(0, 0, 0, 0.08));
      padding-bottom: 16px;
    }
    
    .card-header h3 {
      margin: 0;
      font-size: 18px;
      font-weight: 600;
      color: var(--text-primary, #1f2937);
    }
    
    .card-actions {
      display: flex;
      gap: 8px;
      align-items: center;
    }
    
    .card-content {
      color: var(--text-secondary, #4b5563);
    }
    
    .card-footer {
      margin-top: 16px;
      padding-top: 16px;
      border-top: 1px solid var(--card-divider, rgba(0, 0, 0, 0.08));
    }
    
    /* Dark mode support */
    :host-context(.theme-dark) .modern-card {
      --card-background: #1e293b;
      --card-border: rgba(255, 255, 255, 0.1);
      --card-divider: rgba(255, 255, 255, 0.1);
      --text-primary: #f1f5f9;
      --text-secondary: #cbd5e1;
    }
    
    :host-context(.theme-dark) .modern-card.hover-effect:hover {
      box-shadow: 0 8px 16px rgba(0, 0, 0, 0.4), 0 4px 8px rgba(0, 0, 0, 0.3);
    }
  `]
})
export class ModernCardComponent {
  @Input() title?: string;
  @Input() hoverable = false;
  @Input() elevated = false;
}
