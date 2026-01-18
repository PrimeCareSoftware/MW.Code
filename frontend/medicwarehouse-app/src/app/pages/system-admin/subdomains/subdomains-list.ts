import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { SystemAdminService } from '../../../services/system-admin';
import { Subdomain, CreateSubdomainRequest, ClinicSummary } from '../../../models/system-admin.model';
import { environment } from '../../../../environments/environment';
import { Navbar } from '../../../shared/navbar/navbar';

@Component({
  selector: 'app-subdomains-list',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, Navbar],
  templateUrl: './subdomains-list.html',
  styleUrl: './subdomains-list.scss'})
export class SubdomainsList implements OnInit {
  subdomains = signal<Subdomain[]>([]);
  clinics = signal<ClinicSummary[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);
  
  // Get domain suffix from environment configuration
  domainSuffix = environment.tenant?.domainSuffix || 'medicwarehouse.com';
  
  showModal = false;
  submitting = signal(false);
  modalError = signal<string | null>(null);

  formData: CreateSubdomainRequest = {
    subdomain: '',
    clinicId: ''
  };

  constructor(
    private systemAdminService: SystemAdminService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadSubdomains();
    this.loadClinics();
  }

  loadSubdomains(): void {
    this.loading.set(true);
    this.error.set(null);

    this.systemAdminService.getSubdomains().subscribe({
      next: (data) => {
        this.subdomains.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Erro ao carregar subdomínios');
        this.loading.set(false);
      }
    });
  }

  loadClinics(): void {
    this.systemAdminService.getClinics('active', 1, 100).subscribe({
      next: (data) => {
        this.clinics.set(data.clinics);
      },
      error: (err) => {
        console.error('Error loading clinics:', err);
      }
    });
  }

  openCreateModal(): void {
    this.formData = {
      subdomain: '',
      clinicId: ''
    };
    this.modalError.set(null);
    this.showModal = true;
  }

  closeModal(): void {
    this.showModal = false;
  }

  isFormValid(): boolean {
    return !!(this.formData.subdomain && this.formData.clinicId);
  }

  createSubdomain(): void {
    if (!this.isFormValid()) return;

    this.submitting.set(true);
    this.modalError.set(null);

    this.systemAdminService.createSubdomain(this.formData).subscribe({
      next: () => {
        this.submitting.set(false);
        this.closeModal();
        this.loadSubdomains();
        alert('Subdomínio criado com sucesso!');
      },
      error: (err: any) => {
        this.modalError.set(err.error?.message || 'Erro ao criar subdomínio');
        this.submitting.set(false);
      }
    });
  }

  deleteSubdomain(id: string): void {
    if (!confirm('Tem certeza que deseja excluir este subdomínio?')) {
      return;
    }

    this.systemAdminService.deleteSubdomain(id).subscribe({
      next: () => {
        this.loadSubdomains();
      },
      error: (err: any) => {
        alert(err.error?.message || 'Erro ao excluir subdomínio');
      }
    });
  }

  formatDate(date: string): string {
    return new Date(date).toLocaleDateString('pt-BR');
  }
}
