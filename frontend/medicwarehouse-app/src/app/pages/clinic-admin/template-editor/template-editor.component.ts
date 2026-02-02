import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TerminologyService, TerminologyMap } from '../../../services/terminology.service';
import { TerminologyPipe } from '../../../pipes/terminology.pipe';
import { ClinicSelectionService } from '../../../services/clinic-selection.service';
import { ProfessionalSpecialty } from '../../../services/business-configuration.service';

interface TemplatePreset {
  specialty: ProfessionalSpecialty;
  name: string;
  icon: string;
  templates: {
    mainDocument: string;
    exitDocument: string;
  };
}

@Component({
  selector: 'app-template-editor',
  standalone: true,
  imports: [CommonModule, FormsModule, TerminologyPipe],
  templateUrl: './template-editor.component.html',
  styleUrls: ['./template-editor.component.scss']
})
export class TemplateEditorComponent implements OnInit {
  terminology: TerminologyMap | null = null;
  selectedPreset: TemplatePreset | null = null;
  
  previewMode: 'main' | 'exit' = 'main';
  currentTemplate = '';
  
  templatePresets: TemplatePreset[] = [
    {
      specialty: ProfessionalSpecialty.Medico,
      name: 'Médico',
      icon: '🩺',
      templates: {
        mainDocument: `PRONTUÁRIO MÉDICO

IDENTIFICAÇÃO DO PACIENTE
Nome: {{patientName}}
Data de Nascimento: {{patientBirthDate}}
CPF: {{patientCpf}}

CONSULTA
Data: {{consultationDate}}
Médico: {{professionalName}}
CRM: {{professionalRegistration}}

QUEIXA PRINCIPAL
{{chiefComplaint}}

HISTÓRIA DA DOENÇA ATUAL
{{historyOfPresentIllness}}

EXAME FÍSICO
{{physicalExamination}}

HIPÓTESE DIAGNÓSTICA
{{diagnosticHypothesis}}

CONDUTA
{{therapeuticPlan}}

____________________
{{professionalName}}
{{professionalRegistration}}`,
        exitDocument: `RECEITA MÉDICA

Paciente: {{patientName}}
Data: {{consultationDate}}

{{medicationList}}

____________________
{{professionalName}}
CRM: {{professionalRegistration}}`
      }
    },
    {
      specialty: ProfessionalSpecialty.Psicologo,
      name: 'Psicólogo',
      icon: '🧠',
      templates: {
        mainDocument: `PRONTUÁRIO PSICOLÓGICO

DADOS DO CLIENTE
Nome: {{patientName}}
Data de Nascimento: {{patientBirthDate}}

SESSÃO
Data: {{consultationDate}}
Psicólogo: {{professionalName}}
CRP: {{professionalRegistration}}

QUEIXA
{{chiefComplaint}}

OBSERVAÇÕES DA SESSÃO
{{sessionNotes}}

EVOLUÇÃO
{{evolution}}

PLANEJAMENTO
{{therapeuticPlan}}

____________________
{{professionalName}}
{{professionalRegistration}}`,
        exitDocument: `RELATÓRIO PSICOLÓGICO

Cliente: {{patientName}}
Período: {{periodStart}} a {{periodEnd}}

SÍNTESE DO ATENDIMENTO
{{summary}}

EVOLUÇÃO
{{evolution}}

CONSIDERAÇÕES FINAIS
{{finalConsiderations}}

____________________
{{professionalName}}
CRP: {{professionalRegistration}}`
      }
    },
    {
      specialty: ProfessionalSpecialty.Nutricionista,
      name: 'Nutricionista',
      icon: '🥗',
      templates: {
        mainDocument: `AVALIAÇÃO NUTRICIONAL

IDENTIFICAÇÃO
Nome: {{patientName}}
Data: {{consultationDate}}

NUTRICIONISTA
{{professionalName}}
CRN: {{professionalRegistration}}

ANAMNESE ALIMENTAR
{{foodHistory}}

AVALIAÇÃO ANTROPOMÉTRICA
Peso: {{weight}} kg
Altura: {{height}} cm
IMC: {{bmi}}

DIAGNÓSTICO NUTRICIONAL
{{nutritionalDiagnosis}}

OBJETIVOS
{{goals}}

____________________
{{professionalName}}
{{professionalRegistration}}`,
        exitDocument: `PLANO ALIMENTAR

Paciente: {{patientName}}
Nutricionista: {{professionalName}}
Data: {{consultationDate}}

ORIENTAÇÕES GERAIS
{{generalGuidelines}}

PLANO DE REFEIÇÕES
{{mealPlan}}

OBSERVAÇÕES
{{observations}}

____________________
{{professionalName}}
CRN: {{professionalRegistration}}`
      }
    },
    {
      specialty: ProfessionalSpecialty.Fisioterapeuta,
      name: 'Fisioterapeuta',
      icon: '💪',
      templates: {
        mainDocument: `AVALIAÇÃO FISIOTERAPÊUTICA

IDENTIFICAÇÃO
Nome: {{patientName}}
Data: {{consultationDate}}

FISIOTERAPEUTA
{{professionalName}}
CREFITO: {{professionalRegistration}}

QUEIXA PRINCIPAL
{{chiefComplaint}}

HISTÓRIA CLÍNICA
{{clinicalHistory}}

AVALIAÇÃO FÍSICA
{{physicalAssessment}}

DIAGNÓSTICO FISIOTERAPÊUTICO
{{physiotherapyDiagnosis}}

OBJETIVOS DO TRATAMENTO
{{treatmentGoals}}

____________________
{{professionalName}}
{{professionalRegistration}}`,
        exitDocument: `PLANO DE TRATAMENTO FISIOTERAPÊUTICO

Paciente: {{patientName}}
Data: {{consultationDate}}

PROTOCOLO DE TRATAMENTO
{{treatmentProtocol}}

EXERCÍCIOS DOMICILIARES
{{homeExercises}}

FREQUÊNCIA E DURAÇÃO
{{frequencyAndDuration}}

OBSERVAÇÕES
{{observations}}

____________________
{{professionalName}}
CREFITO: {{professionalRegistration}}`
      }
    },
    {
      specialty: ProfessionalSpecialty.Dentista,
      name: 'Dentista',
      icon: '🦷',
      templates: {
        mainDocument: `ODONTOGRAMA

IDENTIFICAÇÃO DO PACIENTE
Nome: {{patientName}}
Data: {{consultationDate}}

DENTISTA
{{professionalName}}
CRO: {{professionalRegistration}}

QUEIXA PRINCIPAL
{{chiefComplaint}}

EXAME CLÍNICO
{{clinicalExamination}}

DIAGNÓSTICO
{{diagnosis}}

PROCEDIMENTOS REALIZADOS
{{proceduresPerformed}}

PLANO DE TRATAMENTO
{{treatmentPlan}}

____________________
{{professionalName}}
{{professionalRegistration}}`,
        exitDocument: `ORÇAMENTO DE TRATAMENTO ODONTOLÓGICO

Paciente: {{patientName}}
Data: {{consultationDate}}

PROCEDIMENTOS PROPOSTOS
{{proposedProcedures}}

VALOR TOTAL
R$ {{totalValue}}

FORMA DE PAGAMENTO
{{paymentMethod}}

OBSERVAÇÕES
{{observations}}

____________________
{{professionalName}}
CRO: {{professionalRegistration}}`
      }
    }
  ];

  availablePlaceholders = [
    { key: 'patientName', label: 'Nome do Paciente' },
    { key: 'patientBirthDate', label: 'Data de Nascimento' },
    { key: 'patientCpf', label: 'CPF do Paciente' },
    { key: 'consultationDate', label: 'Data da Consulta' },
    { key: 'professionalName', label: 'Nome do Profissional' },
    { key: 'professionalRegistration', label: 'Registro Profissional' },
    { key: 'chiefComplaint', label: 'Queixa Principal' },
    { key: 'historyOfPresentIllness', label: 'História da Doença Atual' },
    { key: 'physicalExamination', label: 'Exame Físico' },
    { key: 'diagnosticHypothesis', label: 'Hipótese Diagnóstica' },
    { key: 'therapeuticPlan', label: 'Plano Terapêutico' }
  ];

  constructor(
    private terminologyService: TerminologyService,
    private clinicSelectionService: ClinicSelectionService
  ) {}

  ngOnInit(): void {
    this.loadTerminology();
  }

  private loadTerminology(): void {
    const selectedClinic = this.clinicSelectionService.getSelectedClinic();
    if (selectedClinic) {
      this.terminologyService.loadTerminology(selectedClinic.id).subscribe(terminology => {
        this.terminology = terminology;
      });
    }
  }

  selectPreset(preset: TemplatePreset): void {
    this.selectedPreset = preset;
    this.currentTemplate = this.previewMode === 'main' 
      ? preset.templates.mainDocument 
      : preset.templates.exitDocument;
  }

  setPreviewMode(mode: 'main' | 'exit'): void {
    this.previewMode = mode;
    if (this.selectedPreset) {
      this.currentTemplate = mode === 'main' 
        ? this.selectedPreset.templates.mainDocument 
        : this.selectedPreset.templates.exitDocument;
    }
  }

  insertPlaceholder(placeholder: string): void {
    const template = this.currentTemplate || '';
    const cursorPosition = template.length;
    const textBefore = template.substring(0, cursorPosition);
    const textAfter = template.substring(cursorPosition);
    this.currentTemplate = `${textBefore}{{${placeholder}}}${textAfter}`;
  }

  getPreviewWithTerminology(): string {
    if (!this.currentTemplate || !this.terminology) {
      return this.currentTemplate;
    }
    
    // Replace terminology placeholders
    let preview = this.currentTemplate;
    
    // Replace with sample data
    const sampleData: { [key: string]: string } = {
      patientName: 'João da Silva',
      patientBirthDate: '15/03/1985',
      patientCpf: '123.456.789-00',
      consultationDate: new Date().toLocaleDateString('pt-BR'),
      professionalName: 'Dr. Maria Santos',
      professionalRegistration: '12345/SP',
      chiefComplaint: 'Dor de cabeça persistente',
      historyOfPresentIllness: 'Paciente relata cefaleia há 3 dias...',
      physicalExamination: 'Paciente em bom estado geral...',
      diagnosticHypothesis: 'Cefaleia tensional',
      therapeuticPlan: 'Analgésico conforme prescrição'
    };

    Object.entries(sampleData).forEach(([key, value]) => {
      const regex = new RegExp(`\\{\\{${key}\\}\\}`, 'g');
      preview = preview.replace(regex, value);
    });

    return preview;
  }

  saveTemplate(): void {
    // In a real implementation, this would save to the backend
    console.log('Saving template:', {
      specialty: this.selectedPreset?.specialty,
      previewMode: this.previewMode,
      template: this.currentTemplate
    });
    alert('Template salvo com sucesso! (funcionalidade de demonstração)');
  }

  resetTemplate(): void {
    if (this.selectedPreset) {
      this.currentTemplate = this.previewMode === 'main' 
        ? this.selectedPreset.templates.mainDocument 
        : this.selectedPreset.templates.exitDocument;
    }
  }
}
