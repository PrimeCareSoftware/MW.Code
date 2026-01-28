import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ClinicSummary } from '../../models/system-admin.model';

@Component({
  selector: 'app-clinics-map',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="map-container">
      <div class="map-placeholder">
        <div class="placeholder-content">
          <svg class="map-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" 
                  d="M9 20l-5.447-2.724A1 1 0 013 16.382V5.618a1 1 0 011.447-.894L9 7m0 13l6-3m-6 3V7m6 10l4.553 2.276A1 1 0 0021 18.382V7.618a1 1 0 00-.553-.894L15 4m0 13V4m0 0L9 7"></path>
          </svg>
          <h3>Visualização de Mapa</h3>
          <p>Integração de mapa será implementada com biblioteca de mapas (ex: Leaflet ou Google Maps)</p>
          <p class="clinic-count">{{ clinics.length }} clínicas disponíveis</p>
        </div>
      </div>
      
      <div class="clinic-list-sidebar">
        <h3>Clínicas ({{ clinics.length }})</h3>
        <div class="clinic-item" *ngFor="let clinic of clinics" [routerLink]="['/clinics', clinic.id]">
          <div class="clinic-header">
            <strong>{{ clinic.name }}</strong>
            <span class="badge" [ngClass]="getHealthBadgeClass(clinic.healthStatus)">
              {{ getHealthLabel(clinic.healthStatus) }}
            </span>
          </div>
          <div class="clinic-details">
            <span class="detail">📍 {{ clinic.address || 'Endereço não informado' }}</span>
            <span class="detail">📊 Score: {{ clinic.healthScore }}/100</span>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .map-container {
      display: flex;
      height: calc(100vh - 250px);
      min-height: 600px;
      gap: 1rem;
      padding: 1rem;
    }

    .map-placeholder {
      flex: 1;
      background: #f9fafb;
      border: 2px dashed #d1d5db;
      border-radius: 8px;
      display: flex;
      align-items: center;
      justify-content: center;
    }

    .placeholder-content {
      text-align: center;
      padding: 2rem;
      max-width: 500px;
    }

    .map-icon {
      width: 80px;
      height: 80px;
      margin: 0 auto 1.5rem;
      color: #9ca3af;
    }

    .placeholder-content h3 {
      font-size: 1.5rem;
      font-weight: 600;
      color: #111827;
      margin-bottom: 0.75rem;
    }

    .placeholder-content p {
      color: #6b7280;
      margin-bottom: 0.5rem;
      line-height: 1.6;
    }

    .clinic-count {
      font-weight: 600;
      color: #3b82f6;
      font-size: 1.1rem;
      margin-top: 1rem !important;
    }

    .clinic-list-sidebar {
      width: 350px;
      background: white;
      border: 1px solid #e5e7eb;
      border-radius: 8px;
      padding: 1rem;
      overflow-y: auto;
    }

    .clinic-list-sidebar h3 {
      margin: 0 0 1rem 0;
      font-size: 1.1rem;
      font-weight: 600;
      color: #111827;
      padding-bottom: 0.75rem;
      border-bottom: 2px solid #e5e7eb;
    }

    .clinic-item {
      padding: 1rem;
      border: 1px solid #e5e7eb;
      border-radius: 6px;
      margin-bottom: 0.75rem;
      cursor: pointer;
      transition: all 0.2s;
    }

    .clinic-item:hover {
      background: #f9fafb;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    }

    .clinic-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 0.5rem;
    }

    .clinic-header strong {
      font-size: 0.95rem;
      color: #111827;
    }

    .badge {
      padding: 0.25rem 0.5rem;
      border-radius: 4px;
      font-size: 0.75rem;
      font-weight: 600;
      text-transform: uppercase;
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

    .clinic-details {
      display: flex;
      flex-direction: column;
      gap: 0.25rem;
    }

    .detail {
      font-size: 0.875rem;
      color: #6b7280;
    }

    @media (max-width: 768px) {
      .map-container {
        flex-direction: column;
      }

      .clinic-list-sidebar {
        width: 100%;
        max-height: 400px;
      }
    }
  `]
})
export class ClinicsMapComponent implements OnInit {
  @Input() clinics: ClinicSummary[] = [];

  ngOnInit(): void {
    // In a real implementation, initialize map library here
    console.log('Map view initialized with', this.clinics.length, 'clinics');
  }

  getHealthBadgeClass(status: string): string {
    const statusMap: { [key: string]: string } = {
      'Healthy': 'healthy',
      'NeedsAttention': 'needs-attention',
      'AtRisk': 'at-risk'
    };
    return statusMap[status] || '';
  }

  getHealthLabel(status: string): string {
    const labelMap: { [key: string]: string } = {
      'Healthy': 'Saudável',
      'NeedsAttention': 'Atenção',
      'AtRisk': 'Risco'
    };
    return labelMap[status] || status;
  }
}
