import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { 
  BusinessConfigurationService,
  BusinessType,
  ProfessionalSpecialty,
  CreateBusinessConfigurationDto
} from '../../services/business-configuration.service';
import { TerminologyService, TerminologyMap } from '../../services/terminology.service';
import { TerminologyPipe } from '../../pipes/terminology.pipe';
import { ClinicSelectionService } from '../../services/clinic-selection.service';

interface WizardStep {
  id: number;
  title: string;
  description: string;
  completed: boolean;
}

@Component({
  selector: 'app-onboarding-wizard',
  standalone: true,
  imports: [CommonModule, FormsModule, TerminologyPipe],
  templateUrl: './onboarding-wizard.component.html',
  styleUrls: ['./onboarding-wizard.component.scss']
})
export class OnboardingWizardComponent {
  currentStep = 1;
  totalSteps = 4;
  
  steps: WizardStep[] = [
    { id: 1, title: 'Tipo de Negócio', description: 'Qual o porte da sua clínica?', completed: false },
    { id: 2, title: 'Especialidade', description: 'Qual sua área de atuação?', completed: false },
    { id: 3, title: 'Recursos', description: 'Revise os recursos recomendados', completed: false },
    { id: 4, title: 'Confirmação', description: 'Visualize sua configuração', completed: false }
  ];

  // Form data
  selectedBusinessType: BusinessType | null = null;
  selectedSpecialty: ProfessionalSpecialty | null = null;
  
  businessTypeOptions = [
    { 
      value: BusinessType.SoloPractitioner, 
      label: 'Profissional Autônomo', 
      description: 'Trabalho sozinho ou com poucos colaboradores',
      icon: '👤',
      details: '1 profissional, pode não ter consultório físico'
    },
    { 
      value: BusinessType.SmallClinic, 
      label: 'Clínica Pequena', 
      description: 'Equipe pequena com múltiplos profissionais',
      icon: '🏥',
      details: '2-5 profissionais'
    },
    { 
      value: BusinessType.MediumClinic, 
      label: 'Clínica Média', 
      description: 'Estabelecimento consolidado com vários profissionais',
      icon: '🏢',
      details: '6-20 profissionais'
    },
    { 
      value: BusinessType.LargeClinic, 
      label: 'Clínica Grande', 
      description: 'Grande estrutura com múltiplas especialidades',
      icon: '🏛️',
      details: '20+ profissionais'
    }
  ];

  specialtyOptions = [
    { 
      value: ProfessionalSpecialty.Medico, 
      label: 'Médico', 
      icon: '🩺',
      description: 'Atendimento médico geral ou especializado'
    },
    { 
      value: ProfessionalSpecialty.Psicologo, 
      label: 'Psicólogo', 
      icon: '🧠',
      description: 'Terapia e atendimento psicológico'
    },
    { 
      value: ProfessionalSpecialty.Nutricionista, 
      label: 'Nutricionista', 
      icon: '🥗',
      description: 'Orientação nutricional e planos alimentares'
    },
    { 
      value: ProfessionalSpecialty.Fisioterapeuta, 
      label: 'Fisioterapeuta', 
      icon: '💪',
      description: 'Reabilitação e fisioterapia'
    },
    { 
      value: ProfessionalSpecialty.Dentista, 
      label: 'Dentista', 
      icon: '🦷',
      description: 'Odontologia e saúde bucal'
    },
    { 
      value: ProfessionalSpecialty.Enfermeiro, 
      label: 'Enfermeiro', 
      icon: '💉',
      description: 'Cuidados de enfermagem'
    },
    { 
      value: ProfessionalSpecialty.TerapeutaOcupacional, 
      label: 'Terapeuta Ocupacional', 
      icon: '🎨',
      description: 'Terapia ocupacional'
    },
    { 
      value: ProfessionalSpecialty.Fonoaudiologo, 
      label: 'Fonoaudiólogo', 
      icon: '🗣️',
      description: 'Fonoaudiologia e comunicação'
    },
    { 
      value: ProfessionalSpecialty.Outro, 
      label: 'Outra Especialidade', 
      icon: '⚕️',
      description: 'Outras áreas da saúde'
    }
  ];

  previewTerminology: TerminologyMap | null = null;
  recommendedFeatures: string[] = [];
  
  loading = false;
  error = '';

  constructor(
    private businessConfigService: BusinessConfigurationService,
    private terminologyService: TerminologyService,
    private clinicSelectionService: ClinicSelectionService,
    private router: Router
  ) {}

  nextStep(): void {
    if (this.canProceedToNextStep()) {
      this.steps[this.currentStep - 1].completed = true;
      
      if (this.currentStep === 2) {
        // Load terminology preview when specialty is selected
        this.loadTerminologyPreview();
      }
      
      if (this.currentStep === 3) {
        // Generate recommended features
        this.generateRecommendedFeatures();
      }
      
      this.currentStep++;
    }
  }

  previousStep(): void {
    if (this.currentStep > 1) {
      this.currentStep--;
    }
  }

  goToStep(step: number): void {
    if (step <= this.currentStep) {
      this.currentStep = step;
    }
  }

  canProceedToNextStep(): boolean {
    switch (this.currentStep) {
      case 1:
        return this.selectedBusinessType !== null;
      case 2:
        return this.selectedSpecialty !== null;
      case 3:
        return true; // Features review is optional
      case 4:
        return true;
      default:
        return false;
    }
  }

  selectBusinessType(type: BusinessType): void {
    this.selectedBusinessType = type;
  }

  selectSpecialty(specialty: ProfessionalSpecialty): void {
    this.selectedSpecialty = specialty;
  }

  private loadTerminologyPreview(): void {
    if (!this.selectedSpecialty) return;
    
    // For preview, we'll fetch directly from the service
    // In a real scenario, you might want to call a preview endpoint
    this.previewTerminology = this.getTerminologyForSpecialty(this.selectedSpecialty);
  }

  private getTerminologyForSpecialty(specialty: ProfessionalSpecialty): TerminologyMap {
    // This mirrors the backend TerminologyMap logic
    const terminologyMap: { [key: number]: TerminologyMap } = {
      [ProfessionalSpecialty.Psicologo]: {
        appointment: 'Sessão',
        professional: 'Psicólogo',
        registration: 'CRP',
        client: 'Paciente',
        mainDocument: 'Prontuário',
        exitDocument: 'Relatório Psicológico'
      },
      [ProfessionalSpecialty.Nutricionista]: {
        appointment: 'Consulta',
        professional: 'Nutricionista',
        registration: 'CRN',
        client: 'Paciente',
        mainDocument: 'Avaliação Nutricional',
        exitDocument: 'Plano Alimentar'
      },
      [ProfessionalSpecialty.Dentista]: {
        appointment: 'Consulta',
        professional: 'Dentista',
        registration: 'CRO',
        client: 'Paciente',
        mainDocument: 'Odontograma',
        exitDocument: 'Orçamento de Tratamento'
      },
      [ProfessionalSpecialty.Fisioterapeuta]: {
        appointment: 'Sessão',
        professional: 'Fisioterapeuta',
        registration: 'CREFITO',
        client: 'Paciente',
        mainDocument: 'Avaliação Fisioterapêutica',
        exitDocument: 'Plano de Tratamento'
      },
      [ProfessionalSpecialty.Medico]: {
        appointment: 'Consulta',
        professional: 'Médico',
        registration: 'CRM',
        client: 'Paciente',
        mainDocument: 'Prontuário Médico',
        exitDocument: 'Receita Médica'
      },
      [ProfessionalSpecialty.Enfermeiro]: {
        appointment: 'Atendimento',
        professional: 'Enfermeiro',
        registration: 'COREN',
        client: 'Paciente',
        mainDocument: 'Prontuário de Enfermagem',
        exitDocument: 'Relatório de Enfermagem'
      },
      [ProfessionalSpecialty.TerapeutaOcupacional]: {
        appointment: 'Sessão',
        professional: 'Terapeuta Ocupacional',
        registration: 'COFFITO',
        client: 'Paciente',
        mainDocument: 'Avaliação Terapêutica',
        exitDocument: 'Plano Terapêutico'
      },
      [ProfessionalSpecialty.Fonoaudiologo]: {
        appointment: 'Sessão',
        professional: 'Fonoaudiólogo',
        registration: 'CRFa',
        client: 'Paciente',
        mainDocument: 'Avaliação Fonoaudiológica',
        exitDocument: 'Plano Terapêutico'
      }
    };

    return terminologyMap[specialty] || {
      appointment: 'Atendimento',
      professional: 'Profissional',
      registration: 'Registro Profissional',
      client: 'Cliente',
      mainDocument: 'Prontuário',
      exitDocument: 'Documento de Saída'
    };
  }

  private generateRecommendedFeatures(): void {
    this.recommendedFeatures = [];
    
    // Always recommended
    this.recommendedFeatures.push('Módulo Financeiro', 'Agendamento Online', 'Perfil Público');
    
    // Business type specific
    if (this.selectedBusinessType === BusinessType.LargeClinic) {
      this.recommendedFeatures.push('Múltiplas Salas', 'Fila de Recepção', 'Relatórios BI', 'Acesso API');
    } else if (this.selectedBusinessType === BusinessType.MediumClinic) {
      this.recommendedFeatures.push('Múltiplas Salas', 'Fila de Recepção', 'Relatórios BI');
    } else if (this.selectedBusinessType === BusinessType.SmallClinic) {
      this.recommendedFeatures.push('Múltiplas Salas', 'Fila de Recepção');
    }
    
    // Specialty specific
    if (this.selectedSpecialty === ProfessionalSpecialty.Medico) {
      this.recommendedFeatures.push('Prescrição Eletrônica', 'Integração com Laboratórios', 'Controle de Vacinas');
    } else if (this.selectedSpecialty === ProfessionalSpecialty.Psicologo) {
      this.recommendedFeatures.push('Telemedicina', 'Sessões em Grupo');
    } else if (this.selectedSpecialty === ProfessionalSpecialty.Dentista) {
      this.recommendedFeatures.push('Prescrição Eletrônica', 'Convênios');
    } else if (this.selectedSpecialty === ProfessionalSpecialty.Nutricionista) {
      this.recommendedFeatures.push('Integração com Laboratórios', 'Telemedicina');
    } else if (this.selectedSpecialty === ProfessionalSpecialty.Fisioterapeuta) {
      this.recommendedFeatures.push('Telemedicina', 'Visita Domiciliar');
    }
  }

  finish(): void {
    if (!this.selectedBusinessType || !this.selectedSpecialty) {
      this.error = 'Por favor, complete todas as etapas antes de finalizar.';
      return;
    }

    const selectedClinic = this.clinicSelectionService.currentClinic();
    if (!selectedClinic) {
      this.error = 'Nenhuma clínica selecionada.';
      return;
    }

    this.loading = true;
    this.error = '';

    const dto: CreateBusinessConfigurationDto = {
      clinicId: selectedClinic.clinicId,
      businessType: this.selectedBusinessType,
      primarySpecialty: this.selectedSpecialty
    };

    this.businessConfigService.create(dto).subscribe({
      next: () => {
        this.loading = false;
        // Navigate to the business configuration page or dashboard
        this.router.navigate(['/clinic-admin/business-configuration']);
      },
      error: (err) => {
        console.error('Error creating configuration:', err);
        this.error = err.error?.message || 'Erro ao criar configuração. Tente novamente.';
        this.loading = false;
      }
    });
  }

  getProgressPercentage(): number {
    return (this.currentStep / this.totalSteps) * 100;
  }

  // Getter methods for template bindings
  getSelectedBusinessTypeLabel(): string {
    const option = this.businessTypeOptions.find(opt => opt.value === this.selectedBusinessType);
    return option?.label || '';
  }

  getSelectedSpecialtyLabel(): string {
    const option = this.specialtyOptions.find(opt => opt.value === this.selectedSpecialty);
    return option?.label || '';
  }
}
