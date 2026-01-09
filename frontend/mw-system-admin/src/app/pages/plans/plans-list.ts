import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { SystemAdminService } from '../../services/system-admin';
import { SubscriptionPlan, CreateSubscriptionPlanRequest, UpdateSubscriptionPlanRequest } from '../../models/system-admin.model';

@Component({
  selector: 'app-plans-list',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './plans-list.html',
  styleUrl: './plans-list.scss'})
export class PlansList implements OnInit {
  plans = signal<SubscriptionPlan[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);
  
  showModal = false;
  editingPlan: SubscriptionPlan | null = null;
  submitting = signal(false);
  modalError = signal<string | null>(null);

  formData: CreateSubscriptionPlanRequest = {
    name: '',
    description: '',
    monthlyPrice: 0,
    yearlyPrice: 0,
    trialDays: 14,
    maxUsers: 10,
    maxPatients: 100
  };

  formDataUpdate: UpdateSubscriptionPlanRequest = {
    name: '',
    description: '',
    monthlyPrice: 0,
    yearlyPrice: 0,
    trialDays: 14,
    maxUsers: 10,
    maxPatients: 100,
    isActive: true
  };

  constructor(
    private systemAdminService: SystemAdminService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadPlans();
  }

  loadPlans(): void {
    this.loading.set(true);
    this.error.set(null);

    this.systemAdminService.getSubscriptionPlans().subscribe({
      next: (data) => {
        this.plans.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Erro ao carregar planos');
        this.loading.set(false);
      }
    });
  }

  openCreateModal(): void {
    this.editingPlan = null;
    this.formData = {
      name: '',
      description: '',
      monthlyPrice: 0,
      yearlyPrice: 0,
      trialDays: 14,
      maxUsers: 10,
      maxPatients: 100
    };
    this.showModal = true;
  }

  openEditModal(plan: SubscriptionPlan): void {
    this.editingPlan = plan;
    this.formData = {
      name: plan.name,
      description: plan.description || '',
      monthlyPrice: plan.monthlyPrice,
      yearlyPrice: plan.yearlyPrice,
      trialDays: plan.trialDays,
      maxUsers: plan.maxUsers,
      maxPatients: plan.maxPatients
    };
    this.formDataUpdate = {
      ...this.formData,
      isActive: plan.isActive
    };
    this.showModal = true;
  }

  closeModal(): void {
    this.showModal = false;
    this.editingPlan = null;
    this.modalError.set(null);
  }

  onSubmit(): void {
    this.submitting.set(true);
    this.modalError.set(null);

    if (this.editingPlan) {
      // Update
      this.systemAdminService.updateSubscriptionPlan(this.editingPlan.id, this.formDataUpdate).subscribe({
        next: () => {
          this.submitting.set(false);
          this.closeModal();
          this.loadPlans();
        },
        error: (err) => {
          this.modalError.set(err.error?.message || 'Erro ao atualizar plano');
          this.submitting.set(false);
        }
      });
    } else {
      // Create
      this.systemAdminService.createSubscriptionPlan(this.formData).subscribe({
        next: () => {
          this.submitting.set(false);
          this.closeModal();
          this.loadPlans();
        },
        error: (err) => {
          this.modalError.set(err.error?.message || 'Erro ao criar plano');
          this.submitting.set(false);
        }
      });
    }
  }

  toggleStatus(id: string, currentStatus: boolean): void {
    if (!confirm(`Tem certeza que deseja ${currentStatus ? 'desativar' : 'ativar'} este plano?`)) {
      return;
    }

    this.systemAdminService.toggleSubscriptionPlanStatus(id).subscribe({
      next: () => {
        this.loadPlans();
      },
      error: (err) => {
        alert(err.error?.message || 'Erro ao alterar status do plano');
      }
    });
  }

  deletePlan(id: string): void {
    if (!confirm('Tem certeza que deseja excluir este plano? Esta ação não pode ser desfeita.')) {
      return;
    }

    this.systemAdminService.deleteSubscriptionPlan(id).subscribe({
      next: () => {
        this.loadPlans();
      },
      error: (err) => {
        alert(err.error?.message || 'Erro ao excluir plano');
      }
    });
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(value);
  }
}
