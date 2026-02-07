import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { environment } from '../../../environments/environment';

interface Consent {
  id: number;
  type: string;
  description: string;
  purpose: string;
  grantedDate: string;
  status: 'active' | 'revoked';
  required: boolean;
}

@Component({
  selector: 'app-consent-manager',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    MatSlideToggleModule,
    MatChipsModule,
    MatProgressSpinnerModule,
    MatTooltipModule
  ],
  templateUrl: './ConsentManager.component.html',
  styleUrls: ['./privacy.scss']
})
export class ConsentManagerComponent implements OnInit {
  loading = true;
  error: string | null = null;
  consents: Consent[] = [];
  displayedColumns: string[] = ['type', 'description', 'grantedDate', 'status', 'actions'];
  
  // Track which consent is being updated to prevent duplicate operations
  private updatingConsentIds = new Set<number>();

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.loadConsents();
  }

  loadConsents(): void {
    this.loading = true;
    this.error = null;

    this.http
      .get<Consent[]>(`${environment.apiUrl}/patient-portal/consents`)
      .subscribe({
        next: (data) => {
          this.consents = data;
          this.loading = false;
        },
        error: (error) => {
          this.error = 'Erro ao carregar consentimentos. Tente novamente mais tarde.';
          this.loading = false;
          console.error('Error loading consents:', error);
        }
      });
  }

  toggleConsent(consent: Consent): void {
    if (consent.required || this.updatingConsentIds.has(consent.id)) {
      return;
    }

    const newStatus = consent.status === 'active' ? 'revoked' : 'active';
    
    // Mark as updating
    this.updatingConsentIds.add(consent.id);
    
    this.http
      .patch(`${environment.apiUrl}/patient-portal/consents/${consent.id}`, {
        status: newStatus
      })
      .subscribe({
        next: () => {
          consent.status = newStatus;
          this.updatingConsentIds.delete(consent.id);
        },
        error: (error) => {
          console.error('Error toggling consent:', error);
          this.error = 'Erro ao atualizar consentimento. Tente novamente.';
          this.updatingConsentIds.delete(consent.id);
          setTimeout(() => {
            this.error = null;
          }, 5000);
        }
      });
  }

  revokeConsent(consent: Consent): void {
    if (consent.required || consent.status === 'revoked' || this.updatingConsentIds.has(consent.id)) {
      return;
    }

    if (!confirm(`Tem certeza que deseja revogar o consentimento para "${consent.type}"?`)) {
      return;
    }

    // Mark as updating
    this.updatingConsentIds.add(consent.id);

    this.http
      .patch(`${environment.apiUrl}/patient-portal/consents/${consent.id}`, {
        status: 'revoked'
      })
      .subscribe({
        next: () => {
          consent.status = 'revoked';
          this.updatingConsentIds.delete(consent.id);
        },
        error: (error) => {
          console.error('Error revoking consent:', error);
          this.error = 'Erro ao revogar consentimento. Tente novamente.';
          this.updatingConsentIds.delete(consent.id);
          setTimeout(() => {
            this.error = null;
          }, 5000);
        }
      });
  }
  
  // Helper method to check if a consent is being updated
  isConsentUpdating(consent: Consent): boolean {
    return this.updatingConsentIds.has(consent.id);
  }

  formatDate(date: string): string {
    return new Date(date).toLocaleDateString('pt-BR');
  }

  getStatusColor(status: string): string {
    return status === 'active' ? 'primary' : 'warn';
  }

  getStatusLabel(status: string): string {
    return status === 'active' ? 'Ativo' : 'Revogado';
  }

  isToggleDisabled(consent: Consent): boolean {
    return consent.required;
  }

  getToggleTooltip(consent: Consent): string {
    if (consent.required) {
      return 'Este consentimento é obrigatório e não pode ser desativado';
    }
    return consent.status === 'active' 
      ? 'Desativar consentimento' 
      : 'Ativar consentimento';
  }
}
