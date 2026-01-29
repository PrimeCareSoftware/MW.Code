import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ClinicSummary } from '../../models/system-admin.model';

@Component({
  selector: 'app-clinics-cards',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="cards-container">
      <div class="clinic-card" *ngFor="let clinic of clinics" [routerLink]="['/clinics', clinic.id]">
        <div class="card-header">
          <h3>{{ clinic.name }}</h3>
          <span class="badge" [ngClass]="getHealthStatusClass(clinic.healthStatus || '')">
            {{ clinic.healthStatus }}
          </span>
        </div>
        <div class="card-body">
          <div class="info-row">
            <span class="label">CNPJ:</span>
            <span class="value">{{ clinic.document }}</span>
          </div>
          <div class="info-row">
            <span class="label">Email:</span>
            <span class="value">{{ clinic.email }}</span>
          </div>
          <div class="info-row">
            <span class="label">Plano:</span>
            <span class="value">{{ clinic.subscriptionPlan || 'N/A' }}</span>
          </div>
          <div class="info-row">
            <span class="label">Status:</span>
            <span class="badge" [ngClass]="getStatusClass(clinic.subscriptionStatus)">
              {{ clinic.subscriptionStatus }}
            </span>
          </div>
          <div class="info-row">
            <span class="label">Health Score:</span>
            <span class="value">{{ clinic.healthScore }}/100</span>
          </div>
          <div class="tags" *ngIf="clinic.tags && clinic.tags.length > 0">
            <span class="tag" *ngFor="let tag of clinic.tags" [style.background-color]="tag.color">
              {{ tag.name }}
            </span>
          </div>
        </div>
        <div class="card-footer">
          <span class="status-indicator" [ngClass]="{ 'active': clinic.isActive, 'inactive': !clinic.isActive }">
            {{ clinic.isActive ? '● Ativo' : '○ Inativo' }}
          </span>
          <span class="created-date">Criado: {{ formatDate(clinic.createdAt) }}</span>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .cards-container {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
      gap: 1.5rem;
      padding: 1rem;
    }

    .clinic-card {
      background: white;
      border-radius: 8px;
      border: 1px solid #e5e7eb;
      padding: 1.5rem;
      cursor: pointer;
      transition: all 0.2s;
      box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
    }

    .clinic-card:hover {
      box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
      transform: translateY(-2px);
    }

    .card-header {
      display: flex;
      justify-content: space-between;
      align-items: start;
      margin-bottom: 1rem;
      padding-bottom: 1rem;
      border-bottom: 1px solid #e5e7eb;
    }

    .card-header h3 {
      margin: 0;
      font-size: 1.1rem;
      font-weight: 600;
      color: #111827;
      flex: 1;
    }

    .card-body {
      margin-bottom: 1rem;
    }

    .info-row {
      display: flex;
      justify-content: space-between;
      margin-bottom: 0.5rem;
    }

    .info-row .label {
      font-weight: 500;
      color: #6b7280;
      font-size: 0.875rem;
    }

    .info-row .value {
      color: #111827;
      font-size: 0.875rem;
    }

    .badge {
      display: inline-block;
      padding: 0.25rem 0.75rem;
      border-radius: 9999px;
      font-size: 0.75rem;
      font-weight: 600;
      text-transform: uppercase;
    }

    .badge-active {
      background-color: #d1fae5;
      color: #065f46;
    }

    .badge-trial {
      background-color: #dbeafe;
      color: #1e40af;
    }

    .badge-expired {
      background-color: #fee2e2;
      color: #991b1b;
    }

    .badge-suspended {
      background-color: #fef3c7;
      color: #92400e;
    }

    .badge-cancelled {
      background-color: #f3f4f6;
      color: #6b7280;
    }

    .badge.healthy {
      background-color: #d1fae5;
      color: #065f46;
    }

    .badge.needs-attention {
      background-color: #fef3c7;
      color: #92400e;
    }

    .badge.at-risk {
      background-color: #fee2e2;
      color: #991b1b;
    }

    .tags {
      display: flex;
      flex-wrap: wrap;
      gap: 0.5rem;
      margin-top: 1rem;
    }

    .tag {
      padding: 0.25rem 0.75rem;
      border-radius: 9999px;
      font-size: 0.75rem;
      font-weight: 500;
      color: white;
    }

    .card-footer {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding-top: 1rem;
      border-top: 1px solid #e5e7eb;
      font-size: 0.875rem;
      color: #6b7280;
    }

    .status-indicator.active {
      color: #059669;
      font-weight: 600;
    }

    .status-indicator.inactive {
      color: #dc2626;
      font-weight: 600;
    }

    .created-date {
      color: #9ca3af;
    }
  `]
})
export class ClinicsCardsComponent {
  @Input() clinics: ClinicSummary[] = [];

  getHealthStatusClass(status: string): string {
    const statusMap: { [key: string]: string } = {
      'Healthy': 'healthy',
      'NeedsAttention': 'needs-attention',
      'AtRisk': 'at-risk'
    };
    return statusMap[status] || '';
  }

  getStatusClass(status: string): string {
    const classes: { [key: string]: string } = {
      'Active': 'badge-active',
      'Trial': 'badge-trial',
      'Expired': 'badge-expired',
      'Suspended': 'badge-suspended',
      'PaymentOverdue': 'badge-expired',
      'Cancelled': 'badge-cancelled'
    };
    return classes[status] || '';
  }

  formatDate(date: string): string {
    return new Date(date).toLocaleDateString('pt-BR');
  }
}
