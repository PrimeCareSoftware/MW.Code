import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ClinicAdminService } from '../../../services/clinic-admin.service';
import { SubscriptionDetailsDto } from '../../../models/clinic-admin.model';

@Component({
  selector: 'app-subscription-info',
  imports: [CommonModule],
  templateUrl: './subscription-info.component.html',
  styleUrl: './subscription-info.component.scss'
})
export class SubscriptionInfoComponent implements OnInit {
  subscription = signal<SubscriptionDetailsDto | null>(null);
  isLoading = signal<boolean>(false);
  isCancelling = signal<boolean>(false);
  errorMessage = signal<string>('');
  successMessage = signal<string>('');
  showCancelConfirm = signal<boolean>(false);

  constructor(private clinicAdminService: ClinicAdminService) {}

  ngOnInit(): void {
    this.loadSubscription();
  }

  loadSubscription(): void {
    this.isLoading.set(true);
    this.errorMessage.set('');

    this.clinicAdminService.getSubscriptionDetails().subscribe({
      next: (data) => {
        this.subscription.set(data);
        this.isLoading.set(false);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao carregar assinatura: ' + (error.error?.message || error.message));
        this.isLoading.set(false);
      }
    });
  }

  requestCancellation(): void {
    this.showCancelConfirm.set(true);
  }

  confirmCancellation(): void {
    this.isCancelling.set(true);
    this.errorMessage.set('');
    this.successMessage.set('');
    this.showCancelConfirm.set(false);

    this.clinicAdminService.cancelSubscription().subscribe({
      next: () => {
        this.successMessage.set('Solicitação de cancelamento enviada com sucesso!');
        this.isCancelling.set(false);
        this.loadSubscription();
      },
      error: (error) => {
        this.errorMessage.set('Erro ao cancelar assinatura: ' + (error.error?.message || error.message));
        this.isCancelling.set(false);
      }
    });
  }

  cancelCancellationRequest(): void {
    this.showCancelConfirm.set(false);
  }

  getStatusClass(status: string): string {
    switch (status.toLowerCase()) {
      case 'active':
      case 'ativo':
        return 'badge-success';
      case 'cancelled':
      case 'cancelado':
        return 'badge-error';
      case 'pending':
      case 'pendente':
        return 'badge-warning';
      case 'trial':
        return 'badge-info';
      default:
        return 'badge-default';
    }
  }

  getStatusText(status: string): string {
    const statusMap: { [key: string]: string } = {
      'active': 'Ativo',
      'cancelled': 'Cancelado',
      'pending': 'Pendente',
      'suspended': 'Suspenso',
      'trial': 'Trial'
    };
    return statusMap[status.toLowerCase()] || status;
  }

  getUserUsagePercentage(): number {
    const sub = this.subscription();
    if (!sub || !sub.limits.maxUsers) return 0;
    return (sub.limits.currentUsers / sub.limits.maxUsers) * 100;
  }

  getUserUsageClass(): string {
    const percentage = this.getUserUsagePercentage();
    if (percentage >= 90) return 'usage-critical';
    if (percentage >= 75) return 'usage-warning';
    return 'usage-normal';
  }
}
