import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule, ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { SystemAdminService } from '../../services/system-admin';
import { 
  ClinicDetail as ClinicDetailModel, 
  UpdateClinicRequest, 
  EnableManualOverrideRequest, 
  SubscriptionPlan, 
  UpdateSubscriptionRequest,
  ClinicHealthScore,
  ClinicTimelineEvent,
  ClinicUsageMetrics,
  Tag
} from '../../models/system-admin.model';
import { Navbar } from '../../shared/navbar/navbar';

@Component({
  selector: 'app-clinic-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, Navbar],
  templateUrl: './clinic-detail.html',
  styleUrl: './clinic-detail.scss'})
export class ClinicDetail implements OnInit {
  clinic = signal<ClinicDetailModel | null>(null);
  healthScore = signal<ClinicHealthScore | null>(null);
  timeline = signal<ClinicTimelineEvent[]>([]);
  usageMetrics = signal<ClinicUsageMetrics | null>(null);
  availableTags = signal<Tag[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);
  editMode = signal(false);
  saving = signal(false);
  saveError = signal<string | null>(null);
  
  // Tab management
  activeTab = signal<'info' | 'health' | 'timeline' | 'metrics' | 'tags'>('info');

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
      this.loadHealthScore(id);
      this.loadTimeline(id);
      this.loadUsageMetrics(id);
      this.loadAvailableTags();
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

  loadHealthScore(id: string): void {
    this.systemAdminService.getClinicHealthScore(id).subscribe({
      next: (data) => {
        this.healthScore.set(data);
      },
      error: (err) => {
        console.error('Error loading health score:', err);
      }
    });
  }

  loadTimeline(id: string): void {
    this.systemAdminService.getClinicTimeline(id, 50).subscribe({
      next: (data) => {
        this.timeline.set(data);
      },
      error: (err) => {
        console.error('Error loading timeline:', err);
      }
    });
  }

  loadUsageMetrics(id: string): void {
    this.systemAdminService.getClinicUsageMetrics(id).subscribe({
      next: (data) => {
        this.usageMetrics.set(data);
      },
      error: (err) => {
        console.error('Error loading usage metrics:', err);
      }
    });
  }

  loadAvailableTags(): void {
    this.systemAdminService.getTags().subscribe({
      next: (data) => {
        this.availableTags.set(data);
      },
      error: (err) => {
        console.error('Error loading tags:', err);
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

  navigateToBusinessConfig(): void {
    const c = this.clinic();
    if (c) {
      this.router.navigate(['/clinics/business-config/manage'], { 
        queryParams: { 
          clinicId: c.id,
          tenantId: c.tenantId
        } 
      });
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

  // Tab management
  setActiveTab(tab: 'info' | 'health' | 'timeline' | 'metrics' | 'tags'): void {
    this.activeTab.set(tab);
  }

  // Health score helpers
  getHealthScoreClass(): string {
    const score = this.healthScore();
    if (!score) return '';
    
    return score.healthStatus === 'Healthy' ? 'health-good' :
           score.healthStatus === 'NeedsAttention' ? 'health-warning' :
           'health-danger';
  }

  getHealthScoreIcon(): string {
    const score = this.healthScore();
    if (!score) return '❓';
    
    return score.healthStatus === 'Healthy' ? '✅' :
           score.healthStatus === 'NeedsAttention' ? '⚠️' :
           '🚨';
  }

  // Timeline helpers
  getTimelineIcon(type: string): string {
    const icons: { [key: string]: string } = {
      'subscription': '📋',
      'ticket': '🎫',
      'audit': '📝',
      'payment': '💳',
      'user': '👤'
    };
    return icons[type] || '📌';
  }

  // Tag management
  assignTag(tagId: string): void {
    const c = this.clinic();
    if (!c) return;

    this.systemAdminService.assignTagToClinic(c.id, tagId).subscribe({
      next: () => {
        this.loadClinic(c.id);
        alert('Tag atribuída com sucesso!');
      },
      error: (err) => {
        alert(err.error?.message || 'Erro ao atribuir tag');
      }
    });
  }

  removeTag(tagId: string): void {
    const c = this.clinic();
    if (!c) return;

    if (!confirm('Tem certeza que deseja remover esta tag?')) return;

    this.systemAdminService.removeTagFromClinic(c.id, tagId).subscribe({
      next: () => {
        this.loadClinic(c.id);
        alert('Tag removida com sucesso!');
      },
      error: (err) => {
        alert(err.error?.message || 'Erro ao remover tag');
      }
    });
  }

  isTagNotAssigned(tagId: string): boolean {
    const c = this.clinic();
    if (!c || !c.tags) return true;
    return !c.tags.find(t => t.id === tagId);
  }
}
