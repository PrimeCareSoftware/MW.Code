import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { SystemAdminService } from '../../services/system-admin';
import { CreateClinicRequest, SubscriptionPlan } from '../../models/system-admin.model';
import { Navbar } from '../../shared/navbar/navbar';

@Component({
  selector: 'app-clinic-create',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, Navbar],
  templateUrl: './clinic-create.html',
  styleUrl: './clinic-create.scss'})
export class ClinicCreate {
  plans = signal<SubscriptionPlan[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);
  submitting = signal(false);
  submitError = signal<string | null>(null);
  successMessage = signal<string | null>(null);
  createdTenantId = signal<string | null>(null);
  confirmPassword = '';

  formData: CreateClinicRequest = {
    name: '',
    document: '',
    email: '',
    phone: '',
    address: '',
    ownerUsername: '',
    ownerPassword: '',
    ownerFullName: '',
    planId: '',
    businessType: 2, // Default to SmallClinic
    primarySpecialty: 1 // Default to Medico
  };

  constructor(
    private systemAdminService: SystemAdminService,
    private router: Router
  ) {
    this.loadPlans();
  }

  loadPlans(): void {
    this.loading.set(true);
    this.error.set(null);

    this.systemAdminService.getSubscriptionPlans(true).subscribe({
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

  isFormValid(): boolean {
    return !!(
      this.formData.name &&
      this.formData.document &&
      this.formData.email &&
      this.formData.phone &&
      this.formData.address &&
      this.formData.ownerUsername &&
      this.formData.ownerPassword &&
      this.formData.ownerFullName &&
      this.formData.planId &&
      this.formData.ownerPassword === this.confirmPassword &&
      this.formData.ownerPassword.length >= 8
    );
  }

  onSubmit(): void {
    if (!this.isFormValid() || this.submitting()) {
      return;
    }

    this.submitting.set(true);
    this.submitError.set(null);

    this.systemAdminService.createClinic(this.formData).subscribe({
      next: (response) => {
        this.successMessage.set('Clínica criada com sucesso!');
        this.createdTenantId.set(response.clinicId);
        this.submitting.set(false);

        // Navigate back after 5 seconds
        setTimeout(() => {
          this.router.navigate(['/clinics']);
        }, 5000);
      },
      error: (err) => {
        this.submitError.set(err.error?.message || 'Erro ao criar clínica');
        this.submitting.set(false);
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/clinics']);
  }
}
