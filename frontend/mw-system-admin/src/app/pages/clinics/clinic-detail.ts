import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule, ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { SystemAdminService } from '../../services/system-admin';
import { ClinicDetail as ClinicDetailModel, UpdateClinicRequest, EnableManualOverrideRequest, SubscriptionPlan, UpdateSubscriptionRequest } from '../../models/system-admin.model';
import { Navbar } from '../../shared/navbar/navbar';

@Component({
  selector: 'app-clinic-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, Navbar],
  templateUrl: './clinic-detail.html',
  styleUrl: './clinic-detail.scss'})
export class ClinicDetail implements OnInit {
  clinic = signal<ClinicDetailModel | null>(null);
  loading = signal(true);
  error = signal<string | null>(null);
  editMode = signal(false);
  saving = signal(false);
  saveError = signal<string | null>(null);

  editData: UpdateClinicRequest = {
    name: '',
    document: '',
    email: '',
    phone: '',
    address: ''
  };

  // Manual Override
  showManualOverrideModal = false;
  overrideReason = '';
  overrideExtendUntil = '';
  overrideProcessing = signal(false);
  overrideError = signal<string | null>(null);

  // Change Plan
  showChangePlanModal = false;
  newPlanId = '';
  availablePlans = signal<SubscriptionPlan[]>([]);
  changePlanProcessing = signal(false);
  changePlanError = signal<string | null>(null);

  constructor(
    private systemAdminService: SystemAdminService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadClinic(id);
      this.loadPlans();
    }
  }

  loadClinic(id: string): void {
    this.loading.set(true);
    this.error.set(null);

    this.systemAdminService.getClinic(id).subscribe({
      next: (data) => {
        this.clinic.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Erro ao carregar clínica');
        this.loading.set(false);
      }
    });
  }

  loadPlans(): void {
    this.systemAdminService.getSubscriptionPlans(true).subscribe({
      next: (data) => {
        this.availablePlans.set(data);
      },
      error: (err) => {
        console.error('Error loading plans:', err);
      }
    });
  }

  toggleEditMode(): void {
    const c = this.clinic();
    if (c) {
      this.editData = {
        name: c.name,
        document: c.document,
        email: c.email,
        phone: c.phone,
        address: c.address
      };
    }
    this.editMode.set(true);
  }

  cancelEdit(): void {
    this.editMode.set(false);
    this.saveError.set(null);
  }

  saveClinicChanges(): void {
    const c = this.clinic();
    if (!c) return;

    this.saving.set(true);
    this.saveError.set(null);

    this.systemAdminService.updateClinic(c.id, this.editData).subscribe({
      next: () => {
        this.editMode.set(false);
        this.saving.set(false);
        this.loadClinic(c.id);
      },
      error: (err) => {
        this.saveError.set(err.error?.message || 'Erro ao salvar alterações');
        this.saving.set(false);
      }
    });
  }

  activateManualOverride(): void {
    const c = this.clinic();
    if (!c || !this.overrideReason) return;

    this.overrideProcessing.set(true);
    this.overrideError.set(null);

    const request: EnableManualOverrideRequest = {
      reason: this.overrideReason,
      extendUntil: this.overrideExtendUntil || undefined
    };

    this.systemAdminService.enableManualOverrideExtended(c.id, request).subscribe({
      next: () => {
        this.showManualOverrideModal = false;
        this.overrideProcessing.set(false);
        this.overrideReason = '';
        this.overrideExtendUntil = '';
        this.loadClinic(c.id);
        alert('Override manual ativado com sucesso!');
      },
      error: (err) => {
        this.overrideError.set(err.error?.message || 'Erro ao ativar override');
        this.overrideProcessing.set(false);
      }
    });
  }

  changePlan(): void {
    const c = this.clinic();
    if (!c || !this.newPlanId) return;

    this.changePlanProcessing.set(true);
    this.changePlanError.set(null);

    const request: UpdateSubscriptionRequest = {
      newPlanId: this.newPlanId
    };

    this.systemAdminService.updateSubscription(c.id, request).subscribe({
      next: () => {
        this.showChangePlanModal = false;
        this.changePlanProcessing.set(false);
        this.newPlanId = '';
        this.loadClinic(c.id);
        alert('Plano alterado com sucesso!');
      },
      error: (err) => {
        this.changePlanError.set(err.error?.message || 'Erro ao alterar plano');
        this.changePlanProcessing.set(false);
      }
    });
  }

  navigateToOwners(): void {
    const c = this.clinic();
    if (c) {
      this.router.navigate(['/clinic-owners'], { queryParams: { clinicId: c.id } });
    }
  }

  goBack(): void {
    this.router.navigate(['/clinics']);
  }

  formatDate(date?: string): string {
    if (!date) return '-';
    return new Date(date).toLocaleDateString('pt-BR');
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(value);
  }

  getStatusLabel(status: string): string {
    const labels: { [key: string]: string } = {
      'Active': 'Ativo',
      'Trial': 'Trial',
      'Expired': 'Expirado',
      'Suspended': 'Suspenso',
      'PaymentOverdue': 'Pagamento Atrasado',
      'Cancelled': 'Cancelado'
    };
    return labels[status] || status;
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
}
