import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-kpi-card',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="kpi-card" [class.clickable]="clickable" (click)="handleClick()">
      <div class="kpi-header">
        <span class="kpi-icon" *ngIf="icon">{{ icon }}</span>
        <h3 class="kpi-title">{{ title }}</h3>
      </div>
      <div class="kpi-body">
        <div class="kpi-value">{{ value }}</div>
        <div class="kpi-change" *ngIf="change !== undefined" [class]="getTrendClass()">
          <span class="change-icon">{{ getTrendIcon() }}</span>
          <span class="change-value">{{ formatChange(change) }}</span>
        </div>
        <div class="kpi-subtitle" *ngIf="subtitle">{{ subtitle }}</div>
      </div>
    </div>
  `,
  styles: [`
    .kpi-card {
      background: white;
      border-radius: 8px;
      padding: 20px;
      box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
      transition: transform 0.2s, box-shadow 0.2s;
    }

    .kpi-card.clickable {
      cursor: pointer;
    }

    .kpi-card.clickable:hover {
      transform: translateY(-2px);
      box-shadow: 0 4px 8px rgba(0, 0, 0, 0.15);
    }

    .kpi-header {
      display: flex;
      align-items: center;
      gap: 8px;
      margin-bottom: 12px;
    }

    .kpi-icon {
      font-size: 20px;
    }

    .kpi-title {
      font-size: 14px;
      font-weight: 500;
      color: #666;
      margin: 0;
    }

    .kpi-body {
      display: flex;
      flex-direction: column;
      gap: 8px;
    }

    .kpi-value {
      font-size: 28px;
      font-weight: 700;
      color: #1a1a1a;
    }

    .kpi-change {
      display: flex;
      align-items: center;
      gap: 4px;
      font-size: 14px;
      font-weight: 500;
    }

    .kpi-change.positive {
      color: #22c55e;
    }

    .kpi-change.negative {
      color: #ef4444;
    }

    .kpi-change.neutral {
      color: #6b7280;
    }

    .change-icon {
      font-size: 16px;
    }

    .kpi-subtitle {
      font-size: 12px;
      color: #999;
    }
  `]
})
export class KpiCardComponent {
  @Input() title: string = '';
  @Input() value: string = '';
  @Input() change?: number;
  @Input() trend?: 'up' | 'down' | 'stable';
  @Input() icon?: string;
  @Input() subtitle?: string;
  @Input() clickable: boolean = false;

  handleClick(): void {
    if (this.clickable) {
      // Emit click event if needed
    }
  }

  getTrendClass(): string {
    if (this.change === undefined) return 'neutral';
    if (this.change > 0) return 'positive';
    if (this.change < 0) return 'negative';
    return 'neutral';
  }

  getTrendIcon(): string {
    if (this.change === undefined) return '';
    if (this.change > 0) return '↑';
    if (this.change < 0) return '↓';
    return '→';
  }

  formatChange(value: number): string {
    const sign = value >= 0 ? '+' : '';
    return `${sign}${value.toFixed(2)}%`;
  }
}
