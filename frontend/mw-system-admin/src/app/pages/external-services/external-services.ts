import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ExternalServiceService } from '../../services/external-service.service';
import { 
  ExternalServiceConfigurationDto, 
  CreateExternalServiceConfigurationDto,
  ExternalServiceType,
  ExternalServiceTypeLabels
} from '../../models/external-service.model';
import { Navbar } from '../../shared/navbar/navbar';

@Component({
  selector: 'app-external-services',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, Navbar],
  templateUrl: './external-services.html',
  styleUrl: './external-services.scss'
})
export class ExternalServicesPage implements OnInit {
  services = signal<ExternalServiceConfigurationDto[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);
  
  showModal = false;
  editingService: ExternalServiceConfigurationDto | null = null;
  submitting = signal(false);
  modalError = signal<string | null>(null);
  
  showSecrets = false;

  formData: CreateExternalServiceConfigurationDto = {
    serviceType: ExternalServiceType.SendGrid,
    serviceName: '',
    description: '',
    apiKey: '',
    apiSecret: '',
    clientId: '',
    clientSecret: '',
    accessToken: '',
    refreshToken: '',
    apiUrl: '',
    webhookUrl: '',
    accountId: '',
    projectId: '',
    region: '',
    additionalConfiguration: '',
    isActive: true
  };

  availableServiceTypes = Object.entries(ExternalServiceTypeLabels).map(([key, label]) => ({
    value: parseInt(key),
    label
  })).sort((a, b) => a.label.localeCompare(b.label));

  constructor(
    private externalServiceService: ExternalServiceService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadServices();
  }

  loadServices(): void {
    this.loading.set(true);
    this.error.set(null);

    this.externalServiceService.getAll().subscribe({
      next: (data) => {
        this.services.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Erro ao carregar serviços externos');
        this.loading.set(false);
      }
    });
  }

  openCreateModal(): void {
    this.editingService = null;
    this.formData = {
      serviceType: ExternalServiceType.SendGrid,
      serviceName: '',
      description: '',
      apiKey: '',
      apiSecret: '',
      clientId: '',
      clientSecret: '',
      accessToken: '',
      refreshToken: '',
      apiUrl: '',
      webhookUrl: '',
      accountId: '',
      projectId: '',
      region: '',
      additionalConfiguration: '',
      isActive: true
    };
    this.showModal = true;
    this.showSecrets = false;
  }

  openEditModal(service: ExternalServiceConfigurationDto): void {
    this.editingService = service;
    this.formData = {
      serviceType: service.serviceType,
      serviceName: service.serviceName,
      description: service.description || '',
      apiKey: '', // Don't pre-fill sensitive data
      apiSecret: '',
      clientId: service.hasClientId ? '••••••••' : '',
      clientSecret: '',
      accessToken: '',
      refreshToken: '',
      apiUrl: service.apiUrl || '',
      webhookUrl: service.webhookUrl || '',
      accountId: service.accountId || '',
      projectId: service.projectId || '',
      region: service.region || '',
      additionalConfiguration: service.additionalConfiguration || '',
      isActive: service.isActive
    };
    this.showModal = true;
    this.showSecrets = false;
  }

  closeModal(): void {
    this.showModal = false;
    this.editingService = null;
    this.modalError.set(null);
    this.showSecrets = false;
  }

  onSubmit(): void {
    if (!this.formData.serviceName.trim()) {
      this.modalError.set('Nome do serviço é obrigatório');
      return;
    }

    this.submitting.set(true);
    this.modalError.set(null);

    // Clean up form data - remove placeholder values
    const submitData = { ...this.formData };
    if (submitData.clientId === '••••••••') {
      submitData.clientId = '';
    }

    if (this.editingService) {
      this.externalServiceService.update(this.editingService.id, submitData).subscribe({
        next: () => {
          this.submitting.set(false);
          this.closeModal();
          this.loadServices();
        },
        error: (err) => {
          this.modalError.set(err.error?.message || 'Erro ao atualizar serviço');
          this.submitting.set(false);
        }
      });
    } else {
      this.externalServiceService.create(submitData).subscribe({
        next: () => {
          this.submitting.set(false);
          this.closeModal();
          this.loadServices();
        },
        error: (err) => {
          this.modalError.set(err.error?.message || 'Erro ao criar serviço');
          this.submitting.set(false);
        }
      });
    }
  }

  deleteService(id: string, name: string): void {
    if (!confirm(`Tem certeza que deseja excluir o serviço "${name}"?`)) {
      return;
    }

    this.externalServiceService.delete(id).subscribe({
      next: () => {
        this.loadServices();
      },
      error: (err) => {
        alert(err.error?.message || 'Erro ao excluir serviço');
      }
    });
  }

  getServiceTypeLabel(type: ExternalServiceType): string {
    return ExternalServiceTypeLabels[type] || 'Desconhecido';
  }

  getStatusBadgeClass(service: ExternalServiceConfigurationDto): string {
    if (!service.isActive) return 'badge-inactive';
    if (!service.hasValidConfiguration) return 'badge-warning';
    if (service.errorCount > 0) return 'badge-error';
    return 'badge-success';
  }

  getStatusText(service: ExternalServiceConfigurationDto): string {
    if (!service.isActive) return 'Inativo';
    if (!service.hasValidConfiguration) return 'Configuração Incompleta';
    if (service.errorCount > 0) return `${service.errorCount} Erro(s)`;
    return 'Ativo';
  }

  formatDate(dateString?: string): string {
    if (!dateString) return 'Nunca';
    const date = new Date(dateString);
    return date.toLocaleString('pt-BR');
  }

  toggleShowSecrets(): void {
    this.showSecrets = !this.showSecrets;
  }
}
