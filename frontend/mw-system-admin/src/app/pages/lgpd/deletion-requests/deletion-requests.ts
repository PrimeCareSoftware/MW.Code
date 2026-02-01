import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  DataDeletionService,
  DataDeletionRequest,
  ProcessDeletionRequest,
  CompleteDeletionRequest,
  RejectDeletionRequest,
  LegalApprovalRequest
} from '../../../services/data-deletion.service';
import { Auth } from '../../../services/auth';
import { Navbar } from '../../../shared/navbar/navbar';

@Component({
  selector: 'app-deletion-requests',
  standalone: true,
  imports: [CommonModule, FormsModule, Navbar],
  templateUrl: './deletion-requests.html',
  styleUrl: './deletion-requests.scss'
})
export class DeletionRequests implements OnInit {
  requests = signal<DataDeletionRequest[]>([]);
  loading = signal(false);
  error = signal<string | null>(null);
  selectedRequest = signal<DataDeletionRequest | null>(null);
  
  // Modals
  showDetailsModal = signal(false);
  showProcessModal = signal(false);
  showCompleteModal = signal(false);
  showRejectModal = signal(false);
  showLegalApprovalModal = signal(false);
  
  // Form data
  processingNotes = '';
  rejectionReason = '';
  legalApprovalNotes = '';
  selectedDataTypes: string[] = [];
  availableDataTypes: string[] = [];
  
  // Action in progress
  actionInProgress = signal(false);
  
  // Available filter options
  availableStatuses: string[] = [];
  availableRequestTypes: string[] = [];
  
  constructor(
    public deletionService: DataDeletionService,
    private auth: Auth
  ) {}

  ngOnInit(): void {
    this.availableStatuses = this.deletionService.getAvailableStatuses();
    this.availableRequestTypes = this.deletionService.getAvailableRequestTypes();
    this.availableDataTypes = this.deletionService.getAvailableDataTypes();
    this.loadPendingRequests();
  }

  loadPendingRequests(): void {
    this.loading.set(true);
    this.error.set(null);
    
    this.deletionService.getPendingRequests().subscribe({
      next: (data) => {
        this.requests.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set('Erro ao carregar solicitações: ' + (err.error?.message || err.message));
        this.loading.set(false);
      }
    });
  }

  viewDetails(request: DataDeletionRequest): void {
    this.selectedRequest.set(request);
    this.showDetailsModal.set(true);
  }

  closeDetailsModal(): void {
    this.showDetailsModal.set(false);
    this.selectedRequest.set(null);
  }

  openProcessModal(request: DataDeletionRequest): void {
    if (request.status !== 'Pending') {
      this.error.set('Apenas solicitações pendentes podem ser processadas');
      return;
    }
    this.selectedRequest.set(request);
    this.processingNotes = '';
    this.showProcessModal.set(true);
  }

  closeProcessModal(): void {
    this.showProcessModal.set(false);
    this.selectedRequest.set(null);
    this.processingNotes = '';
  }

  confirmProcess(): void {
    const request = this.selectedRequest();
    if (!request) return;

    this.actionInProgress.set(true);
    this.error.set(null);

    const processRequest: ProcessDeletionRequest = {
      requestId: request.id,
      processedBy: this.auth.getUserInfo()?.username || 'UNKNOWN',
      notes: this.processingNotes || undefined
    };

    this.deletionService.processRequest(processRequest).subscribe({
      next: () => {
        this.actionInProgress.set(false);
        this.closeProcessModal();
        this.loadPendingRequests();
      },
      error: (err) => {
        this.error.set('Erro ao processar solicitação: ' + (err.error?.message || err.message));
        this.actionInProgress.set(false);
      }
    });
  }

  openCompleteModal(request: DataDeletionRequest): void {
    if (request.status !== 'Processing') {
      this.error.set('Apenas solicitações em processamento podem ser completadas');
      return;
    }
    this.selectedRequest.set(request);
    this.selectedDataTypes = [];
    this.processingNotes = '';
    this.showCompleteModal.set(true);
  }

  closeCompleteModal(): void {
    this.showCompleteModal.set(false);
    this.selectedRequest.set(null);
    this.selectedDataTypes = [];
    this.processingNotes = '';
  }

  toggleDataType(dataType: string): void {
    const index = this.selectedDataTypes.indexOf(dataType);
    if (index > -1) {
      this.selectedDataTypes.splice(index, 1);
    } else {
      this.selectedDataTypes.push(dataType);
    }
  }

  isDataTypeSelected(dataType: string): boolean {
    return this.selectedDataTypes.includes(dataType);
  }

  confirmComplete(): void {
    const request = this.selectedRequest();
    if (!request || this.selectedDataTypes.length === 0) {
      this.error.set('Selecione pelo menos um tipo de dado afetado');
      return;
    }

    this.actionInProgress.set(true);
    this.error.set(null);

    const completeRequest: CompleteDeletionRequest = {
      requestId: request.id,
      completedBy: this.auth.getUserInfo()?.username || 'UNKNOWN',
      affectedDataTypes: this.selectedDataTypes,
      notes: this.processingNotes || undefined
    };

    this.deletionService.completeRequest(completeRequest).subscribe({
      next: () => {
        this.actionInProgress.set(false);
        this.closeCompleteModal();
        this.loadPendingRequests();
      },
      error: (err) => {
        this.error.set('Erro ao completar solicitação: ' + (err.error?.message || err.message));
        this.actionInProgress.set(false);
      }
    });
  }

  openRejectModal(request: DataDeletionRequest): void {
    if (request.status === 'Completed' || request.status === 'Rejected') {
      this.error.set('Solicitações já completadas ou rejeitadas não podem ser alteradas');
      return;
    }
    this.selectedRequest.set(request);
    this.rejectionReason = '';
    this.showRejectModal.set(true);
  }

  closeRejectModal(): void {
    this.showRejectModal.set(false);
    this.selectedRequest.set(null);
    this.rejectionReason = '';
  }

  confirmReject(): void {
    const request = this.selectedRequest();
    if (!request || !this.rejectionReason.trim()) {
      this.error.set('Por favor, informe o motivo da rejeição');
      return;
    }

    this.actionInProgress.set(true);
    this.error.set(null);

    const rejectRequest: RejectDeletionRequest = {
      requestId: request.id,
      rejectedBy: this.auth.getUserInfo()?.username || 'UNKNOWN',
      reason: this.rejectionReason
    };

    this.deletionService.rejectRequest(rejectRequest).subscribe({
      next: () => {
        this.actionInProgress.set(false);
        this.closeRejectModal();
        this.loadPendingRequests();
      },
      error: (err) => {
        this.error.set('Erro ao rejeitar solicitação: ' + (err.error?.message || err.message));
        this.actionInProgress.set(false);
      }
    });
  }

  openLegalApprovalModal(request: DataDeletionRequest): void {
    if (!request.legalApprovalRequired) {
      this.error.set('Esta solicitação não requer aprovação legal');
      return;
    }
    if (request.legalApprovalDate) {
      this.error.set('Esta solicitação já possui aprovação legal');
      return;
    }
    this.selectedRequest.set(request);
    this.legalApprovalNotes = '';
    this.showLegalApprovalModal.set(true);
  }

  closeLegalApprovalModal(): void {
    this.showLegalApprovalModal.set(false);
    this.selectedRequest.set(null);
    this.legalApprovalNotes = '';
  }

  confirmLegalApproval(): void {
    const request = this.selectedRequest();
    if (!request) return;

    this.actionInProgress.set(true);
    this.error.set(null);

    const approvalRequest: LegalApprovalRequest = {
      requestId: request.id,
      approvedBy: this.auth.getUserInfo()?.username || 'UNKNOWN',
      notes: this.legalApprovalNotes || undefined
    };

    this.deletionService.legalApproval(approvalRequest).subscribe({
      next: () => {
        this.actionInProgress.set(false);
        this.closeLegalApprovalModal();
        this.loadPendingRequests();
      },
      error: (err) => {
        this.error.set('Erro ao aprovar legalmente: ' + (err.error?.message || err.message));
        this.actionInProgress.set(false);
      }
    });
  }

  getStatusBadgeClass(status: string): string {
    const color = this.deletionService.getStatusColor(status);
    switch (color) {
      case 'success': return 'badge-success';
      case 'warning': return 'badge-warning';
      case 'info': return 'badge-info';
      case 'danger': return 'badge-danger';
      default: return 'badge-secondary';
    }
  }

  getStatusText(status: string): string {
    switch (status) {
      case 'Pending': return 'Pendente';
      case 'Processing': return 'Em Processamento';
      case 'Completed': return 'Concluído';
      case 'Rejected': return 'Rejeitado';
      default: return status;
    }
  }

  getRequestTypeText(type: string): string {
    switch (type) {
      case 'Complete': return 'Exclusão Completa';
      case 'Anonymization': return 'Anonimização';
      case 'Partial': return 'Exclusão Parcial';
      default: return type;
    }
  }

  formatDate(date: string | null): string {
    if (!date) return 'N/A';
    return new Date(date).toLocaleString('pt-BR');
  }

  exportToJson(): void {
    const data = this.requests();
    const json = JSON.stringify(data, null, 2);
    const blob = new Blob([json], { type: 'application/json' });
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = `solicitacoes-exclusao-${new Date().toISOString()}.json`;
    link.click();
    window.URL.revokeObjectURL(url);
  }
}
