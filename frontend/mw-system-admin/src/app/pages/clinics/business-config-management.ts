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
import { Auth } from '../../services/auth';

@Component({
  selector: 'app-business-config-management',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, Navbar],
  templateUrl: './business-config-management.html',
  styleUrl: './business-config-management.scss'
})
export class BusinessConfigManagement implements OnInit {
  private readonly SUCCESS_MESSAGE_DURATION_MS = 5000;
  private readonly SHORT_SUCCESS_MESSAGE_DURATION_MS = 3000;
  
  config = signal<BusinessConfiguration | null>(null);
  loading = signal(true);
  error = signal<string | null>(null);
  saving = signal(false);
  saveError = signal<string | null>(null);
  successMessage = signal<string | null>(null);
  configNotFound = signal(false);
  isSystemOwner = signal(false);
  
  clinicId: string | null = null;
  tenantId: string | null = null;

  // Enums for template
  businessTypeEnum = BusinessType;
  specialtyEnum = ProfessionalSpecialty;

  constructor(
    private systemAdminService: SystemAdminService,
    private route: ActivatedRoute,
    private router: Router,
    private auth: Auth
  ) {}

  ngOnInit(): void {
    // Check if user is system owner
    const userInfo = this.auth.getUserInfo();
    this.isSystemOwner.set(userInfo?.isSystemOwner || false);
    
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
          this.error.set('Configuração de negócio não encontrada para esta clínica');
        } else {
          this.error.set(err.error?.message || 'Falha ao carregar configuração de negócio');
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
        this.successMessage.set('Tipo de negócio atualizado com sucesso');
        this.loadConfiguration();
        this.saving.set(false);
        setTimeout(() => this.successMessage.set(null), this.SHORT_SUCCESS_MESSAGE_DURATION_MS);
      },
      error: (err) => {
        this.saveError.set(err.error?.message || 'Falha ao atualizar tipo de negócio');
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
        this.successMessage.set('Especialidade principal atualizada com sucesso');
        this.loadConfiguration();
        this.saving.set(false);
        setTimeout(() => this.successMessage.set(null), this.SHORT_SUCCESS_MESSAGE_DURATION_MS);
      },
      error: (err) => {
        this.saveError.set(err.error?.message || 'Falha ao atualizar especialidade principal');
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
        this.successMessage.set(`Funcionalidade "${featureName}" atualizada com sucesso`);
        this.loadConfiguration();
        this.saving.set(false);
        setTimeout(() => this.successMessage.set(null), this.SHORT_SUCCESS_MESSAGE_DURATION_MS);
      },
      error: (err) => {
        this.saveError.set(err.error?.message || 'Falha ao atualizar funcionalidade');
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
        this.successMessage.set('Configuração criada com sucesso! Você pode personalizá-la abaixo.');
        this.saving.set(false);
        setTimeout(() => this.successMessage.set(null), this.SUCCESS_MESSAGE_DURATION_MS);
      },
      error: (err) => {
        this.saveError.set(err.error?.message || 'Falha ao criar configuração');
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
