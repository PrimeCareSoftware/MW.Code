import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { SystemAdminService } from '../../services/system-admin';
import { 
  BusinessConfiguration, 
  BusinessType, 
  ProfessionalSpecialty, 
  UpdateBusinessTypeRequest, 
  UpdatePrimarySpecialtyRequest, 
  UpdateFeatureRequest 
} from '../../models/system-admin.model';
import { Navbar } from '../../shared/navbar/navbar';

@Component({
  selector: 'app-business-config-management',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, Navbar],
  templateUrl: './business-config-management.html',
  styleUrl: './business-config-management.scss'
})
export class BusinessConfigManagement implements OnInit {
  private readonly SUCCESS_MESSAGE_DURATION_MS = 5000;
  
  config = signal<BusinessConfiguration | null>(null);
  loading = signal(true);
  error = signal<string | null>(null);
  saving = signal(false);
  saveError = signal<string | null>(null);
  successMessage = signal<string | null>(null);
  configNotFound = signal(false);
  
  clinicId: string | null = null;
  tenantId: string | null = null;

  // Enums for template
  businessTypeEnum = BusinessType;
  specialtyEnum = ProfessionalSpecialty;

  constructor(
    private systemAdminService: SystemAdminService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.clinicId = params['clinicId'];
      this.tenantId = params['tenantId'];
      
      if (this.clinicId && this.tenantId) {
        this.loadConfiguration();
      } else {
        this.error.set('Clinic ID and Tenant ID are required');
        this.loading.set(false);
      }
    });
  }

  loadConfiguration(): void {
    if (!this.clinicId || !this.tenantId) return;
    
    this.loading.set(true);
    this.error.set(null);
    this.configNotFound.set(false);

    this.systemAdminService.getBusinessConfiguration(this.clinicId, this.tenantId).subscribe({
      next: (data) => {
        this.config.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        // Check if it's a 404 (configuration not found)
        if (err.status === 404) {
          this.configNotFound.set(true);
          this.error.set('Business configuration not found for this clinic');
        } else {
          this.error.set(err.error?.message || 'Failed to load business configuration');
        }
        this.loading.set(false);
      }
    });
  }

  updateBusinessType(newType: BusinessType): void {
    const currentConfig = this.config();
    if (!currentConfig || !this.tenantId) return;

    this.saving.set(true);
    this.saveError.set(null);

    const request: UpdateBusinessTypeRequest = {
      businessType: newType,
      tenantId: this.tenantId
    };

    this.systemAdminService.updateBusinessType(currentConfig.id, request).subscribe({
      next: () => {
        this.successMessage.set('Business type updated successfully');
        this.loadConfiguration();
        this.saving.set(false);
        setTimeout(() => this.successMessage.set(null), 3000);
      },
      error: (err) => {
        this.saveError.set(err.error?.message || 'Failed to update business type');
        this.saving.set(false);
      }
    });
  }

  updatePrimarySpecialty(newSpecialty: ProfessionalSpecialty): void {
    const currentConfig = this.config();
    if (!currentConfig || !this.tenantId) return;

    this.saving.set(true);
    this.saveError.set(null);

    const request: UpdatePrimarySpecialtyRequest = {
      primarySpecialty: newSpecialty,
      tenantId: this.tenantId
    };

    this.systemAdminService.updatePrimarySpecialty(currentConfig.id, request).subscribe({
      next: () => {
        this.successMessage.set('Primary specialty updated successfully');
        this.loadConfiguration();
        this.saving.set(false);
        setTimeout(() => this.successMessage.set(null), 3000);
      },
      error: (err) => {
        this.saveError.set(err.error?.message || 'Failed to update primary specialty');
        this.saving.set(false);
      }
    });
  }

  toggleFeature(featureName: string, currentValue: boolean): void {
    const currentConfig = this.config();
    if (!currentConfig || !this.tenantId) return;

    this.saving.set(true);
    this.saveError.set(null);

    const request: UpdateFeatureRequest = {
      featureName: featureName,
      enabled: !currentValue,
      tenantId: this.tenantId
    };

    this.systemAdminService.updateFeature(currentConfig.id, request).subscribe({
      next: () => {
        this.successMessage.set(`Feature "${featureName}" updated successfully`);
        this.loadConfiguration();
        this.saving.set(false);
        setTimeout(() => this.successMessage.set(null), 3000);
      },
      error: (err) => {
        this.saveError.set(err.error?.message || 'Failed to update feature');
        this.saving.set(false);
      }
    });
  }

  createConfiguration(): void {
    if (!this.clinicId || !this.tenantId) return;

    this.saving.set(true);
    this.saveError.set(null);
    this.error.set(null);
    this.configNotFound.set(false);

    // Create default configuration
    const request = {
      clinicId: this.clinicId,
      businessType: BusinessType.SmallClinic,
      primarySpecialty: ProfessionalSpecialty.Medico,
      tenantId: this.tenantId
    };

    this.systemAdminService.createBusinessConfiguration(request).subscribe({
      next: (config) => {
        this.config.set(config);
        this.successMessage.set('Configuration created successfully! You can customize it below.');
        this.saving.set(false);
        setTimeout(() => this.successMessage.set(null), this.SUCCESS_MESSAGE_DURATION_MS);
      },
      error: (err) => {
        this.saveError.set(err.error?.message || 'Failed to create configuration');
        this.saving.set(false);
      }
    });
  }

  getBusinessTypeName(type: BusinessType): string {
    switch (type) {
      case BusinessType.SoloPractitioner: return 'Profissional Solo';
      case BusinessType.SmallClinic: return 'Clínica Pequena';
      case BusinessType.MediumClinic: return 'Clínica Média';
      case BusinessType.LargeClinic: return 'Clínica Grande';
      default: return 'Desconhecido';
    }
  }

  getSpecialtyName(specialty: ProfessionalSpecialty): string {
    switch (specialty) {
      case ProfessionalSpecialty.Medico: return 'Médico';
      case ProfessionalSpecialty.Psicologo: return 'Psicólogo';
      case ProfessionalSpecialty.Nutricionista: return 'Nutricionista';
      case ProfessionalSpecialty.Fisioterapeuta: return 'Fisioterapeuta';
      case ProfessionalSpecialty.Dentista: return 'Dentista';
      case ProfessionalSpecialty.Enfermeiro: return 'Enfermeiro';
      case ProfessionalSpecialty.TerapeutaOcupacional: return 'Terapeuta Ocupacional';
      case ProfessionalSpecialty.Fonoaudiologo: return 'Fonoaudiólogo';
      case ProfessionalSpecialty.Veterinario: return 'Veterinário';
      case ProfessionalSpecialty.Outro: return 'Outro';
      default: return 'Desconhecido';
    }
  }

  goBack(): void {
    this.router.navigate(['/clinics']);
  }
}
