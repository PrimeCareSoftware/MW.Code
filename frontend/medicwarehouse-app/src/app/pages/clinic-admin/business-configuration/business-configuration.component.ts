import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { forkJoin, EMPTY } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { Navbar } from '../../../shared/navbar/navbar';
import { 
  BusinessConfigurationService, 
  BusinessConfiguration,
  BusinessType,
  ProfessionalSpecialty,
  UpdateFeatureDto
} from '../../../services/business-configuration.service';
import { TerminologyService } from '../../../services/terminology.service';
import { ClinicSelectionService } from '../../../services/clinic-selection.service';
import { ClinicAdminService } from '../../../services/clinic-admin.service';
import { ClinicAdminInfoDto, UpdateClinicInfoRequest } from '../../../models/clinic-admin.model';
import { SetupWizardComponent, WizardConfiguration } from './setup-wizard/setup-wizard.component';
import { environment } from '../../../../environments/environment';

interface FeatureCategory {
  name: string;
  features: FeatureInfo[];
}

interface FeatureInfo {
  key: string;
  label: string;
  description: string;
  enabled: boolean;
}

@Component({
  selector: 'app-business-configuration',
  standalone: true,
  imports: [CommonModule, FormsModule, Navbar, SetupWizardComponent],
  templateUrl: './business-configuration.component.html',
  styleUrls: ['./business-configuration.component.scss']
})
export class BusinessConfigurationComponent implements OnInit {
  private readonly SUCCESS_MESSAGE_DURATION = 5000; // milliseconds
  private readonly SCHEDULE_SUCCESS_MESSAGE_DURATION = 3000; // milliseconds
  
  configuration: BusinessConfiguration | null = null;
  clinicInfo: ClinicAdminInfoDto | null = null;
  loading = false;
  saving = false;
  savingSchedule = false;
  error = '';
  success = '';
  showWizard = false;

  // Schedule settings
  openingTime = '08:00';
  closingTime = '18:00';
  appointmentDurationMinutes = 30;
  allowEmergencySlots = true;
  enableOnlineAppointmentScheduling = true;

  appointmentDurationOptions = [
    { value: 15, label: '15 minutos' },
    { value: 30, label: '30 minutos' },
    { value: 45, label: '45 minutos' },
    { value: 60, label: '60 minutos' }
  ];

  BusinessType = BusinessType;
  ProfessionalSpecialty = ProfessionalSpecialty;

  businessTypeOptions = [
    { value: BusinessType.SoloPractitioner, label: 'Profissional Autônomo', description: '1 profissional, pode não ter consultório físico' },
    { value: BusinessType.SmallClinic, label: 'Clínica Pequena', description: '2-5 profissionais' },
    { value: BusinessType.MediumClinic, label: 'Clínica Média', description: '6-20 profissionais' },
    { value: BusinessType.LargeClinic, label: 'Clínica Grande', description: '20+ profissionais' }
  ];

  specialtyOptions = [
    { value: ProfessionalSpecialty.Medico, label: 'Médico', icon: '🩺' },
    { value: ProfessionalSpecialty.Psicologo, label: 'Psicólogo', icon: '🧠' },
    { value: ProfessionalSpecialty.Nutricionista, label: 'Nutricionista', icon: '🥗' },
    { value: ProfessionalSpecialty.Fisioterapeuta, label: 'Fisioterapeuta', icon: '💪' },
    { value: ProfessionalSpecialty.Dentista, label: 'Dentista', icon: '🦷' },
    { value: ProfessionalSpecialty.Enfermeiro, label: 'Enfermeiro', icon: '💉' },
    { value: ProfessionalSpecialty.TerapeutaOcupacional, label: 'Terapeuta Ocupacional', icon: '🎨' },
    { value: ProfessionalSpecialty.Fonoaudiologo, label: 'Fonoaudiólogo', icon: '🗣️' },
    { value: ProfessionalSpecialty.Outro, label: 'Outro', icon: '⚕️' }
  ];

  featureCategories: FeatureCategory[] = [];

  constructor(
    private businessConfigService: BusinessConfigurationService,
    private terminologyService: TerminologyService,
    private clinicSelectionService: ClinicSelectionService,
    private clinicAdminService: ClinicAdminService
  ) {}

  ngOnInit(): void {
    this.ensureClinicLoaded();
  }

  private ensureClinicLoaded(): void {
    const selectedClinic = this.clinicSelectionService.currentClinic();
    
    if (selectedClinic) {
      // Clinic already loaded, proceed normally
      this.loadConfiguration();
      this.loadClinicInfo();
    } else {
      // Clinic not loaded yet, fetch it first
      this.loading = true;
      this.clinicSelectionService.getUserClinics().pipe(
        tap(clinics => {
          if (!environment.production) {
            console.log('Clinics loaded:', clinics);
          }
          // Check the returned clinics array directly
          if (clinics && clinics.length > 0) {
            // Set the clinic manually to ensure signal is updated
            const preferred = clinics.find(c => c.isPreferred) || clinics[0];
            if (!environment.production) {
              console.log('Setting current clinic to:', preferred);
            }
            this.clinicSelectionService.currentClinic.set(preferred);
          } else {
            if (!environment.production) {
              console.warn('No clinics returned from API:', clinics);
            }
            this.error = 'Nenhuma clínica disponível. Por favor, contate o suporte.';
            this.loading = false;
          }
        })
      ).subscribe({
        next: () => {
          // After signal is set in tap(), load configuration if clinics exist
          // The tap() operator has already validated and set the clinic
          if (this.clinicSelectionService.currentClinic()) {
            this.loadConfiguration();
            this.loadClinicInfo();
          }
        },
        error: (err) => {
          console.error('Error loading clinics:', err);
          this.error = 'Erro ao carregar clínicas. Tente novamente.';
          this.loading = false;
        }
      });
    }
  }

  private loadConfiguration(): void {
    const selectedClinic = this.clinicSelectionService.currentClinic();
    if (!selectedClinic) {
      this.error = 'Nenhuma clínica selecionada';
      return;
    }

    this.loading = true;
    this.error = '';

    this.businessConfigService.getByClinicId(selectedClinic.clinicId).subscribe({
      next: (config) => {
        this.configuration = config;
        this.buildFeatureCategories();
        this.loadTerminology(selectedClinic.clinicId);
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading configuration:', err);
        // Show error message so user can manually create configuration
        if (err.status === 404) {
          this.error = '';
          this.loading = false;
        } else {
          this.error = 'Erro ao carregar configuração. Tente novamente.';
          this.loading = false;
        }
      }
    });
  }

  private loadTerminology(clinicId: string): void {
    this.terminologyService.loadTerminology(clinicId).subscribe();
  }

  private buildFeatureCategories(): void {
    if (!this.configuration) return;

    this.featureCategories = [
      {
        name: 'Recursos Clínicos',
        features: [
          {
            key: 'electronicPrescription',
            label: 'Prescrição Eletrônica',
            description: 'Crie e gerencie receitas médicas digitalmente',
            enabled: this.configuration.electronicPrescription
          },
          {
            key: 'labIntegration',
            label: 'Integração com Laboratórios',
            description: 'Solicite exames e receba resultados automaticamente',
            enabled: this.configuration.labIntegration
          },
          {
            key: 'vaccineControl',
            label: 'Controle de Vacinas',
            description: 'Gerencie carteira de vacinação dos pacientes',
            enabled: this.configuration.vaccineControl
          },
          {
            key: 'inventoryManagement',
            label: 'Gestão de Estoque',
            description: 'Controle de materiais e medicamentos',
            enabled: this.configuration.inventoryManagement
          }
        ]
      },
      {
        name: 'Recursos Administrativos',
        features: [
          {
            key: 'multiRoom',
            label: 'Múltiplas Salas',
            description: 'Gerencie atendimentos em várias salas simultaneamente',
            enabled: this.configuration.multiRoom
          },
          {
            key: 'receptionQueue',
            label: 'Fila de Recepção',
            description: 'Sistema de gestão de fila de espera',
            enabled: this.configuration.receptionQueue
          },
          {
            key: 'financialModule',
            label: 'Módulo Financeiro',
            description: 'Controle completo de receitas e despesas',
            enabled: this.configuration.financialModule
          },
          {
            key: 'healthInsurance',
            label: 'Convênios',
            description: 'Gestão de planos de saúde e convênios',
            enabled: this.configuration.healthInsurance
          }
        ]
      },
      {
        name: 'Tipos de Consulta',
        features: [
          {
            key: 'telemedicine',
            label: 'Telemedicina',
            description: 'Consultas online por videochamada',
            enabled: this.configuration.telemedicine
          },
          {
            key: 'homeVisit',
            label: 'Visita Domiciliar',
            description: 'Atendimento na residência do paciente',
            enabled: this.configuration.homeVisit
          },
          {
            key: 'groupSessions',
            label: 'Sessões em Grupo',
            description: 'Atendimento coletivo de múltiplos pacientes',
            enabled: this.configuration.groupSessions
          }
        ]
      },
      {
        name: 'Marketing',
        features: [
          {
            key: 'publicProfile',
            label: 'Perfil Público',
            description: 'Página pública da clínica no site',
            enabled: this.configuration.publicProfile
          },
          {
            key: 'onlineBooking',
            label: 'Agendamento Online',
            description: 'Permita que pacientes agendem pela internet',
            enabled: this.configuration.onlineBooking
          },
          {
            key: 'patientReviews',
            label: 'Avaliações de Pacientes',
            description: 'Colete e exiba avaliações dos pacientes',
            enabled: this.configuration.patientReviews
          }
        ]
      },
      {
        name: 'Recursos Avançados',
        features: [
          {
            key: 'biReports',
            label: 'Relatórios BI',
            description: 'Dashboards e análises avançadas',
            enabled: this.configuration.biReports
          },
          {
            key: 'apiAccess',
            label: 'Acesso API',
            description: 'Integração com sistemas externos',
            enabled: this.configuration.apiAccess
          },
          {
            key: 'whiteLabel',
            label: 'White Label',
            description: 'Personalize completamente a marca',
            enabled: this.configuration.whiteLabel
          }
        ]
      }
    ];
  }

  updateBusinessType(): void {
    if (!this.configuration) return;

    this.saving = true;
    this.error = '';
    this.success = '';

    this.businessConfigService
      .updateBusinessType(this.configuration.id, { 
        businessType: this.configuration.businessType 
      })
      .subscribe({
        next: () => {
          this.success = 'Tipo de negócio atualizado com sucesso!';
          this.saving = false;
          // Reload configuration immediately to get updated features
          this.loadConfiguration();
        },
        error: (err) => {
          console.error('Error updating business type:', err);
          this.error = 'Erro ao atualizar tipo de negócio';
          this.saving = false;
        }
      });
  }

  updatePrimarySpecialty(): void {
    if (!this.configuration) return;

    this.saving = true;
    this.error = '';
    this.success = '';

    this.businessConfigService
      .updatePrimarySpecialty(this.configuration.id, { 
        primarySpecialty: this.configuration.primarySpecialty 
      })
      .subscribe({
        next: () => {
          this.success = 'Especialidade atualizada com sucesso!';
          this.saving = false;
          // Reload configuration immediately to get updated terminology and features
          this.loadConfiguration();
        },
        error: (err) => {
          console.error('Error updating specialty:', err);
          this.error = 'Erro ao atualizar especialidade';
          this.saving = false;
        }
      });
  }

  toggleFeature(featureName: string, enabled: boolean): void {
    if (!this.configuration) return;

    const dto: UpdateFeatureDto = {
      featureName,
      enabled
    };

    this.businessConfigService
      .updateFeature(this.configuration.id, dto)
      .subscribe({
        next: () => {
          // Update local state using type-safe property access
          if (this.isValidFeatureName(featureName)) {
            (this.configuration as any)[featureName] = enabled;
            this.buildFeatureCategories();
          }
        },
        error: (err) => {
          console.error('Error updating feature:', err);
          this.error = `Erro ao atualizar recurso: ${featureName}`;
          // Revert the change
          this.buildFeatureCategories();
        }
      });
  }

  private isValidFeatureName(name: string): boolean {
    const validFeatures = [
      'electronicPrescription', 'labIntegration', 'vaccineControl', 'inventoryManagement',
      'multiRoom', 'receptionQueue', 'financialModule', 'healthInsurance',
      'telemedicine', 'homeVisit', 'groupSessions',
      'publicProfile', 'onlineBooking', 'patientReviews',
      'biReports', 'apiAccess', 'whiteLabel'
    ];
    return validFeatures.includes(name);
  }

  getBusinessTypeLabel(type: BusinessType): string {
    return this.businessTypeOptions.find(opt => opt.value === type)?.label || 'Desconhecido';
  }

  getSpecialtyLabel(specialty: ProfessionalSpecialty): string {
    return this.specialtyOptions.find(opt => opt.value === specialty)?.label || 'Desconhecido';
  }

  private loadClinicInfo(): void {
    this.loading = true;
    this.error = '';

    this.clinicAdminService.getClinicInfo().subscribe({
      next: (info) => {
        this.clinicInfo = info;
        // Parse TimeSpan strings to HH:mm format for time inputs
        this.openingTime = this.parseTimeSpan(info.openingTime);
        this.closingTime = this.parseTimeSpan(info.closingTime);
        this.appointmentDurationMinutes = info.appointmentDurationMinutes;
        this.allowEmergencySlots = info.allowEmergencySlots;
        this.enableOnlineAppointmentScheduling = info.enableOnlineAppointmentScheduling;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading clinic info:', err);
        this.error = 'Erro ao carregar informações da clínica';
        this.loading = false;
      }
    });
  }

  private parseTimeSpan(timeSpan: string): string {
    // TimeSpan comes as "HH:mm:ss" or "HH:mm:ss.fffffff"
    // We need "HH:mm" for HTML time input
    if (!timeSpan) return '08:00';
    const parts = timeSpan.split(':');
    if (parts.length >= 2) {
      // Validate and pad hours and minutes
      const hours = parseInt(parts[0], 10);
      const minutes = parseInt(parts[1], 10);
      
      if (isNaN(hours) || isNaN(minutes) || hours < 0 || hours > 23 || minutes < 0 || minutes > 59) {
        console.error('Invalid time format:', timeSpan);
        return '08:00';
      }
      
      return `${hours.toString().padStart(2, '0')}:${minutes.toString().padStart(2, '0')}`;
    }
    return '08:00';
  }

  private formatTimeForBackend(time: string): string {
    // Convert "HH:mm" to "HH:mm:ss" for backend
    if (!time) return '08:00:00';
    const parts = time.split(':');
    if (parts.length >= 2) {
      // Validate and pad hours and minutes
      const hours = parseInt(parts[0], 10);
      const minutes = parseInt(parts[1], 10);
      
      if (isNaN(hours) || isNaN(minutes) || hours < 0 || hours > 23 || minutes < 0 || minutes > 59) {
        console.error('Invalid time format:', time);
        return '08:00:00';
      }
      
      return `${hours.toString().padStart(2, '0')}:${minutes.toString().padStart(2, '0')}:00`;
    }
    return '08:00:00';
  }

  private parseTimeToMinutes(time: string): number {
    // Convert "HH:mm" to total minutes for comparison
    if (!time) return 0;
    const parts = time.split(':');
    if (parts.length >= 2) {
      const hours = parseInt(parts[0], 10);
      const minutes = parseInt(parts[1], 10);
      
      if (isNaN(hours) || isNaN(minutes)) {
        return 0;
      }
      
      return hours * 60 + minutes;
    }
    return 0;
  }

  updateScheduleSettings(): void {
    if (!this.clinicInfo) return;

    // Validate times using proper time comparison
    const opening = this.parseTimeToMinutes(this.openingTime);
    const closing = this.parseTimeToMinutes(this.closingTime);
    
    if (opening >= closing) {
      this.error = 'Horário de abertura deve ser antes do horário de fechamento';
      return;
    }

    this.savingSchedule = true;
    this.error = '';
    this.success = '';

    const request: UpdateClinicInfoRequest = {
      openingTime: this.formatTimeForBackend(this.openingTime),
      closingTime: this.formatTimeForBackend(this.closingTime),
      appointmentDurationMinutes: this.appointmentDurationMinutes,
      allowEmergencySlots: this.allowEmergencySlots,
      enableOnlineAppointmentScheduling: this.enableOnlineAppointmentScheduling
    };

    this.clinicAdminService.updateClinicInfo(request).subscribe({
      next: () => {
        this.success = 'Configurações de horário atualizadas com sucesso!';
        this.savingSchedule = false;
        // Clear success message after configured duration
        setTimeout(() => {
          this.success = '';
        }, this.SCHEDULE_SUCCESS_MESSAGE_DURATION);
      },
      error: (err) => {
        console.error('Error updating schedule settings:', err);
        this.error = 'Erro ao atualizar configurações de horário';
        this.savingSchedule = false;
      }
    });
  }

  createConfiguration(): void {
    const selectedClinic = this.clinicSelectionService.currentClinic();
    if (!selectedClinic) {
      this.error = 'Nenhuma clínica selecionada';
      return;
    }

    this.saving = true;
    this.error = '';
    this.success = '';

    // Create default configuration
    const dto = {
      clinicId: selectedClinic.clinicId,
      businessType: BusinessType.SmallClinic,
      primarySpecialty: ProfessionalSpecialty.Medico
    };

    this.businessConfigService.create(dto).subscribe({
      next: (config) => {
        this.configuration = config;
        this.buildFeatureCategories();
        this.loadTerminology(selectedClinic.clinicId);
        this.success = 'Configuração criada com sucesso! Você pode personalizá-la abaixo.';
        this.saving = false;
        // Clear success message after configured duration
        setTimeout(() => {
          this.success = '';
        }, this.SUCCESS_MESSAGE_DURATION);
      },
      error: (err) => {
        console.error('Error creating configuration:', err);
        this.error = err.error?.message || 'Erro ao criar configuração. Tente novamente.';
        this.saving = false;
      }
    });
  }

  openWizard(): void {
    this.showWizard = true;
  }

  closeWizard(): void {
    this.showWizard = false;
  }

  onWizardComplete(wizardConfig: WizardConfiguration): void {
    const selectedClinic = this.clinicSelectionService.currentClinic();
    if (!selectedClinic) {
      this.error = 'Nenhuma clínica selecionada';
      this.showWizard = false;
      return;
    }

    this.saving = true;
    this.error = '';
    this.success = '';
    this.showWizard = false;

    // Create configuration with wizard data
    const dto = {
      clinicId: selectedClinic.clinicId,
      businessType: wizardConfig.businessType,
      primarySpecialty: wizardConfig.primarySpecialty
    };

    this.businessConfigService.create(dto).subscribe({
      next: (config) => {
        this.configuration = config;
        this.buildFeatureCategories();
        this.loadTerminology(selectedClinic.clinicId);
        
        // Apply template features if provided
        if (wizardConfig.features) {
          this.applyTemplateFeatures(config.id, wizardConfig.features);
        }
        
        // Update schedule settings
        // Convert wizard time format (HH:mm) to backend format (HH:mm:ss) using existing formatTimeForBackend method
        const scheduleRequest: UpdateClinicInfoRequest = {
          openingTime: this.formatTimeForBackend(wizardConfig.openingTime),
          closingTime: this.formatTimeForBackend(wizardConfig.closingTime),
          appointmentDurationMinutes: wizardConfig.appointmentDurationMinutes,
          allowEmergencySlots: wizardConfig.allowEmergencySlots,
          enableOnlineAppointmentScheduling: wizardConfig.enableOnlineAppointmentScheduling
        };

        this.clinicAdminService.updateClinicInfo(scheduleRequest).subscribe({
          next: () => {
            this.success = '🎉 Configuração criada com sucesso! Sua clínica está pronta para começar.';
            this.saving = false;
            this.loadClinicInfo();
            // Clear success message after configured duration
            setTimeout(() => {
              this.success = '';
            }, this.SUCCESS_MESSAGE_DURATION);
          },
          error: (err) => {
            console.error('Error updating schedule:', err);
            this.success = 'Configuração criada, mas houve um erro ao atualizar o horário. Configure manualmente abaixo.';
            this.saving = false;
            // Clear partial-success message after configured duration to avoid persistent banner
            setTimeout(() => {
              this.success = '';
            }, this.SUCCESS_MESSAGE_DURATION);
          }
        });
      },
      error: (err) => {
        console.error('Error creating configuration:', err);
        this.error = err.error?.message || 'Erro ao criar configuração. Tente novamente.';
        this.saving = false;
      }
    });
  }

  private applyTemplateFeatures(configId: string, features: WizardConfiguration['features']): void {
    if (!features) return;
    
    // Apply each feature from the template in parallel
    const featureUpdates = Object.entries(features).map(([featureName, enabled]) => 
      this.businessConfigService.updateFeature(configId, { featureName, enabled }).pipe(
        catchError((err) => {
          console.warn(`Failed to apply template feature ${featureName}:`, err);
          return EMPTY;
        })
      )
    );

    // Execute all feature updates (forkJoin completes automatically after all observables emit)
    // This is fire-and-forget - failures won't prevent the wizard from completing
    if (featureUpdates.length > 0) {
      forkJoin(featureUpdates).subscribe(() => {
        console.log('Template feature application completed');
      });
    }
  }
}
