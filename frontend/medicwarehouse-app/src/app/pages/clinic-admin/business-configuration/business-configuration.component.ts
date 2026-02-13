import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
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
  imports: [CommonModule, FormsModule],
  templateUrl: './business-configuration.component.html',
  styleUrls: ['./business-configuration.component.scss']
})
export class BusinessConfigurationComponent implements OnInit {
  configuration: BusinessConfiguration | null = null;
  clinicInfo: ClinicAdminInfoDto | null = null;
  loading = false;
  saving = false;
  savingSchedule = false;
  error = '';
  success = '';

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
    this.loadConfiguration();
    this.loadClinicInfo();
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
        this.error = 'Erro ao carregar configuração. A clínica pode não ter sido configurada ainda.';
        this.loading = false;
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
      return `${parts[0].padStart(2, '0')}:${parts[1].padStart(2, '0')}`;
    }
    return timeSpan;
  }

  private formatTimeForBackend(time: string): string {
    // Convert "HH:mm" to "HH:mm:ss" for backend
    if (!time) return '08:00:00';
    const parts = time.split(':');
    if (parts.length === 2) {
      return `${parts[0].padStart(2, '0')}:${parts[1].padStart(2, '0')}:00`;
    }
    return time;
  }

  updateScheduleSettings(): void {
    if (!this.clinicInfo) return;

    // Validate times
    if (this.openingTime >= this.closingTime) {
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
        // Clear success message after 3 seconds
        setTimeout(() => {
          this.success = '';
        }, 3000);
      },
      error: (err) => {
        console.error('Error updating schedule settings:', err);
        this.error = 'Erro ao atualizar configurações de horário';
        this.savingSchedule = false;
      }
    });
  }
}
