import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ConsentService, ConsentLog, RevokeConsentRequest } from '../../../services/consent.service';
import { Auth } from '../../../services/auth';
import { Navbar } from '../../../shared/navbar/navbar';

@Component({
  selector: 'app-consent-management',
  standalone: true,
  imports: [CommonModule, FormsModule, Navbar],
  templateUrl: './consent-management.html',
  styleUrl: './consent-management.scss'
})
export class ConsentManagement implements OnInit {
  consents = signal<ConsentLog[]>([]);
  loading = signal(false);
  error = signal<string | null>(null);
  selectedConsent = signal<ConsentLog | null>(null);
  
  // Search and filter
  searchPatientId = '';
  filterType = '';
  filterPurpose = '';
  filterStatus = '';
  
  // Available filter options
  availableTypes: string[] = [];
  availablePurposes: string[] = [];
  availableStatuses: string[] = [];
  
  // Revocation
  showRevokeModal = signal(false);
  revokeReason = '';
  revokeInProgress = signal(false);
  
  // Modal states
  showDetailsModal = signal(false);
  
  constructor(
    public consentService: ConsentService,
    private auth: Auth
  ) {}

  ngOnInit(): void {
    this.availableTypes = this.consentService.getAvailableConsentTypes();
    this.availablePurposes = this.consentService.getAvailablePurposes();
    this.availableStatuses = this.consentService.getAvailableStatuses();
  }

  searchConsents(): void {
    if (!this.searchPatientId) {
      this.error.set('Por favor, informe o ID do paciente');
      return;
    }

    this.loading.set(true);
    this.error.set(null);
    
    this.consentService.getPatientConsents(this.searchPatientId).subscribe({
      next: (data) => {
        let filtered = data;
        
        // Apply filters
        if (this.filterType) {
          filtered = filtered.filter(c => c.consentType === this.filterType);
        }
        if (this.filterPurpose) {
          filtered = filtered.filter(c => c.purpose === this.filterPurpose);
        }
        if (this.filterStatus) {
          filtered = filtered.filter(c => c.status === this.filterStatus);
        }
        
        this.consents.set(filtered);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set('Erro ao carregar consentimentos: ' + (err.error?.message || err.message));
        this.loading.set(false);
      }
    });
  }

  clearFilters(): void {
    this.searchPatientId = '';
    this.filterType = '';
    this.filterPurpose = '';
    this.filterStatus = '';
    this.consents.set([]);
    this.error.set(null);
  }

  viewDetails(consent: ConsentLog): void {
    this.selectedConsent.set(consent);
    this.showDetailsModal.set(true);
  }

  closeDetailsModal(): void {
    this.showDetailsModal.set(false);
    this.selectedConsent.set(null);
  }

  openRevokeModal(consent: ConsentLog): void {
    if (consent.status !== 'Active') {
      this.error.set('Apenas consentimentos ativos podem ser revogados');
      return;
    }
    this.selectedConsent.set(consent);
    this.revokeReason = '';
    this.showRevokeModal.set(true);
  }

  closeRevokeModal(): void {
    this.showRevokeModal.set(false);
    this.selectedConsent.set(null);
    this.revokeReason = '';
  }

  confirmRevoke(): void {
    const consent = this.selectedConsent();
    if (!consent || !this.revokeReason.trim()) {
      this.error.set('Por favor, informe o motivo da revogação');
      return;
    }

    this.revokeInProgress.set(true);
    this.error.set(null);

    const currentUser = this.auth.getUserInfo();
    const request: RevokeConsentRequest = {
      consentId: consent.id,
      revokedBy: currentUser?.username || 'UNKNOWN',
      reason: this.revokeReason
    };

    this.consentService.revokeConsent(request).subscribe({
      next: () => {
        this.revokeInProgress.set(false);
        this.closeRevokeModal();
        // Refresh the list
        this.searchConsents();
      },
      error: (err) => {
        this.error.set('Erro ao revogar consentimento: ' + (err.error?.message || err.message));
        this.revokeInProgress.set(false);
      }
    });
  }

  getStatusBadgeClass(status: string): string {
    switch (status) {
      case 'Active': return 'badge-success';
      case 'Revoked': return 'badge-danger';
      case 'Expired': return 'badge-warning';
      default: return 'badge-secondary';
    }
  }

  getStatusText(status: string): string {
    switch (status) {
      case 'Active': return 'Ativo';
      case 'Revoked': return 'Revogado';
      case 'Expired': return 'Expirado';
      default: return status;
    }
  }

  formatDate(date: string | null): string {
    if (!date) return 'N/A';
    return new Date(date).toLocaleString('pt-BR');
  }

  exportToJson(): void {
    const data = this.consents();
    const json = JSON.stringify(data, null, 2);
    const blob = new Blob([json], { type: 'application/json' });
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = `consentimentos-${this.searchPatientId}-${new Date().toISOString()}.json`;
    link.click();
    window.URL.revokeObjectURL(url);
  }
}
