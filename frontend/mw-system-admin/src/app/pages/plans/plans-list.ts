import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { SystemAdminService } from '../../services/system-admin';
import { SubscriptionPlan, CreateSubscriptionPlanRequest, UpdateSubscriptionPlanRequest } from '../../models/system-admin.model';
import { Navbar } from '../../shared/navbar/navbar';

@Component({
  selector: 'app-plans-list',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, Navbar],
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
    maxPatients: 100,
    campaignName: undefined,
    campaignDescription: undefined,
    originalPrice: undefined,
    campaignPrice: undefined,
    campaignStartDate: undefined,
    campaignEndDate: undefined,
    maxEarlyAdopters: undefined,
    earlyAdopterBenefits: [],
    featuresAvailable: [],
    featuresInDevelopment: []
  };

  formDataUpdate: UpdateSubscriptionPlanRequest = {
    name: '',
    description: '',
    monthlyPrice: 0,
    yearlyPrice: 0,
    trialDays: 14,
    maxUsers: 10,
    maxPatients: 100,
    isActive: true,
    campaignName: undefined,
    campaignDescription: undefined,
    originalPrice: undefined,
    campaignPrice: undefined,
    campaignStartDate: undefined,
    campaignEndDate: undefined,
    maxEarlyAdopters: undefined,
    earlyAdopterBenefits: [],
    featuresAvailable: [],
    featuresInDevelopment: []
  };
  
  // Helper properties for array fields
  newBenefit: string = '';
  newFeatureAvailable: string = '';
  newFeatureInDevelopment: string = '';
  
  // Computed property to get the correct form data reference
  get activeFormData(): CreateSubscriptionPlanRequest | UpdateSubscriptionPlanRequest {
    return this.editingPlan ? this.formDataUpdate : this.formData;
  }

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
      maxPatients: 100,
      campaignName: undefined,
      campaignDescription: undefined,
      originalPrice: undefined,
      campaignPrice: undefined,
      campaignStartDate: undefined,
      campaignEndDate: undefined,
      maxEarlyAdopters: undefined,
      earlyAdopterBenefits: [],
      featuresAvailable: [],
      featuresInDevelopment: []
    };
    this.newBenefit = '';
    this.newFeatureAvailable = '';
    this.newFeatureInDevelopment = '';
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
      maxPatients: plan.maxPatients,
      campaignName: plan.campaignName,
      campaignDescription: plan.campaignDescription,
      originalPrice: plan.originalPrice,
      campaignPrice: plan.campaignPrice,
      campaignStartDate: plan.campaignStartDate,
      campaignEndDate: plan.campaignEndDate,
      maxEarlyAdopters: plan.maxEarlyAdopters,
      earlyAdopterBenefits: this.parseArrayField(plan.earlyAdopterBenefits),
      featuresAvailable: this.parseArrayField(plan.featuresAvailable),
      featuresInDevelopment: this.parseArrayField(plan.featuresInDevelopment)
    };
    this.formDataUpdate = {
      ...this.formData,
      isActive: plan.isActive
    };
    this.newBenefit = '';
    this.newFeatureAvailable = '';
    this.newFeatureInDevelopment = '';
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
  
  /**
   * Parse array field that might be a JSON string or already an array
   */
  private parseArrayField(field: any): string[] {
    if (!field) {
      return [];
    }
    
    // If it's already an array, validate and return a copy with strings only
    if (Array.isArray(field)) {
      return field.filter(item => typeof item === 'string').map(item => String(item));
    }
    
    // If it's a string, try to parse it as JSON
    if (typeof field === 'string') {
      try {
        const parsed = JSON.parse(field);
        if (Array.isArray(parsed)) {
          // Ensure all elements are strings
          return parsed.filter(item => typeof item === 'string').map(item => String(item));
        }
        return [];
      } catch {
        // If parsing fails, return empty array
        return [];
      }
    }
    
    // For any other type, return empty array
    return [];
  }
  
  // Array management helpers
  addBenefit(): void {
    if (this.newBenefit.trim()) {
      const benefits = this.activeFormData.earlyAdopterBenefits ?? (this.activeFormData.earlyAdopterBenefits = []);
      benefits.push(this.newBenefit.trim());
      this.newBenefit = '';
    }
  }
  
  removeBenefit(index: number): void {
    if (this.activeFormData.earlyAdopterBenefits) {
      this.activeFormData.earlyAdopterBenefits.splice(index, 1);
    }
  }
  
  addFeatureAvailable(): void {
    if (this.newFeatureAvailable.trim()) {
      const features = this.activeFormData.featuresAvailable ?? (this.activeFormData.featuresAvailable = []);
      features.push(this.newFeatureAvailable.trim());
      this.newFeatureAvailable = '';
    }
  }
  
  removeFeatureAvailable(index: number): void {
    if (this.activeFormData.featuresAvailable) {
      this.activeFormData.featuresAvailable.splice(index, 1);
    }
  }
  
  addFeatureInDevelopment(): void {
    if (this.newFeatureInDevelopment.trim()) {
      const features = this.activeFormData.featuresInDevelopment ?? (this.activeFormData.featuresInDevelopment = []);
      features.push(this.newFeatureInDevelopment.trim());
      this.newFeatureInDevelopment = '';
    }
  }
  
  removeFeatureInDevelopment(index: number): void {
    if (this.activeFormData.featuresInDevelopment) {
      this.activeFormData.featuresInDevelopment.splice(index, 1);
    }
  }
}
