import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ClinicSummary } from '../../models/system-admin.model';

@Component({
  selector: 'app-clinics-kanban',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="kanban-board">
      <div class="kanban-column" *ngFor="let column of columns">
        <div class="column-header">
          <h3>{{ column.label }}</h3>
          <span class="count">{{ getClinicsByColumn(column.value).length }}</span>
        </div>
        <div class="column-body">
          <div 
            class="clinic-card" 
            *ngFor="let clinic of getClinicsByColumn(column.value)"
            [routerLink]="['/clinics', clinic.id]"
          >
            <h4>{{ clinic.name }}</h4>
            <div class="card-info">
              <span class="info-item">
                <strong>Plano:</strong> {{ clinic.subscriptionPlan || 'N/A' }}
              </span>
              <span class="info-item">
                <strong>Health Score:</strong> {{ clinic.healthScore }}/100
              </span>
            </div>
            <div class="tags" *ngIf="clinic.tags && clinic.tags.length > 0">
              <span class="tag" *ngFor="let tag of clinic.tags.slice(0, 2)" [style.background-color]="tag.color">
                {{ tag.name }}
              </span>
              <span class="tag-more" *ngIf="clinic.tags.length > 2">
                +{{ clinic.tags.length - 2 }}
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .kanban-board {
      display: flex;
      gap: 1.5rem;
      padding: 1rem;
      overflow-x: auto;
      min-height: 600px;
    }

    .kanban-column {
      flex: 1;
      min-width: 300px;
      background: #f9fafb;
      border-radius: 8px;
      padding: 1rem;
    }

    .column-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 1rem;
      padding-bottom: 0.75rem;
      border-bottom: 2px solid #e5e7eb;
    }

    .column-header h3 {
      margin: 0;
      font-size: 1rem;
      font-weight: 600;
      color: #111827;
    }

    .count {
      background: #e5e7eb;
      color: #6b7280;
      padding: 0.25rem 0.75rem;
      border-radius: 9999px;
      font-size: 0.875rem;
      font-weight: 600;
    }

    .column-body {
      display: flex;
      flex-direction: column;
      gap: 1rem;
    }

    .clinic-card {
      background: white;
      border: 1px solid #e5e7eb;
      border-radius: 6px;
      padding: 1rem;
      cursor: pointer;
      transition: all 0.2s;
    }

    .clinic-card:hover {
      box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
      transform: translateY(-2px);
    }

    .clinic-card h4 {
      margin: 0 0 0.75rem 0;
      font-size: 0.95rem;
      font-weight: 600;
      color: #111827;
    }

    .card-info {
      display: flex;
      flex-direction: column;
      gap: 0.5rem;
      margin-bottom: 0.75rem;
    }

    .info-item {
      font-size: 0.875rem;
      color: #6b7280;
    }

    .info-item strong {
      color: #374151;
    }

    .tags {
      display: flex;
      flex-wrap: wrap;
      gap: 0.5rem;
      align-items: center;
    }

    .tag {
      padding: 0.25rem 0.5rem;
      border-radius: 4px;
      font-size: 0.75rem;
      font-weight: 500;
      color: white;
    }

    .tag-more {
      font-size: 0.75rem;
      color: #6b7280;
      font-weight: 500;
    }

    /* Column-specific colors */
    .kanban-column:nth-child(1) .column-header {
      border-bottom-color: #3b82f6;
    }

    .kanban-column:nth-child(2) .column-header {
      border-bottom-color: #10b981;
    }

    .kanban-column:nth-child(3) .column-header {
      border-bottom-color: #f59e0b;
    }

    .kanban-column:nth-child(4) .column-header {
      border-bottom-color: #ef4444;
    }
  `]
})
export class ClinicsKanbanComponent {
  @Input() clinics: ClinicSummary[] = [];

  columns = [
    { value: 'trial', label: '🔄 Trial' },
    { value: 'healthy', label: '✅ Saudável' },
    { value: 'needs-attention', label: '⚠️ Atenção' },
    { value: 'at-risk', label: '🚨 Em Risco' }
  ];

  getClinicsByColumn(columnValue: string): ClinicSummary[] {
    switch (columnValue) {
      case 'trial':
        return this.clinics.filter(c => c.subscriptionStatus === 'Trial');
      case 'healthy':
        return this.clinics.filter(c => c.healthStatus === 'Healthy');
      case 'needs-attention':
        return this.clinics.filter(c => c.healthStatus === 'NeedsAttention');
      case 'at-risk':
        return this.clinics.filter(c => c.healthStatus === 'AtRisk');
      default:
        return [];
    }
  }
}
