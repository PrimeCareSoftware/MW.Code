export enum DocumentTemplateType {
  MedicalRecord = 1,
  Prescription = 2,
  MedicalCertificate = 3,
  LabTestRequest = 4,
  PsychologicalReport = 5,
  NutritionPlan = 6,
  DentalBudget = 7,
  Odontogram = 8,
  PhysicalTherapyEvaluation = 9,
  TreatmentPlan = 10,
  SessionEvolution = 11,
  DischargeReport = 12,
  Referral = 13,
  InformedConsent = 14,
  Custom = 99
}

export enum ProfessionalSpecialty {
  Medico = 1,
  Psicologo = 2,
  Nutricionista = 3,
  Fisioterapeuta = 4,
  Dentista = 5,
  Enfermeiro = 6,
  TerapeutaOcupacional = 7,
  Fonoaudiologo = 8
}

export interface TemplateVariable {
  key: string;
  label: string;
  type: 'text' | 'date' | 'number' | 'boolean';
  description?: string;
  defaultValue?: string;
  isRequired: boolean;
  displayOrder: number;
}

export interface DocumentTemplate {
  id: string;
  name: string;
  description: string;
  specialty: ProfessionalSpecialty;
  type: DocumentTemplateType;
  content: string;
  variables: string; // JSON string of TemplateVariable[]
  isActive: boolean;
  isSystem: boolean;
  clinicId?: string;
  tenantId: string;
  createdAt: Date;
  updatedAt?: Date;
}

export interface CreateDocumentTemplateDto {
  name: string;
  description: string;
  specialty: ProfessionalSpecialty;
  type: DocumentTemplateType;
  content: string;
  variables: string; // JSON string of TemplateVariable[]
  clinicId?: string;
}

export interface UpdateDocumentTemplateDto {
  name: string;
  description: string;
  content: string;
  variables: string; // JSON string of TemplateVariable[]
}

export interface DocumentTemplateFilter {
  specialty?: ProfessionalSpecialty;
  type?: DocumentTemplateType;
  isActive?: boolean;
  isSystem?: boolean;
  clinicId?: string;
}

// Helper constants for display
export const DocumentTemplateTypeLabels: { [key in DocumentTemplateType]: string } = {
  [DocumentTemplateType.MedicalRecord]: 'Prontuário Médico',
  [DocumentTemplateType.Prescription]: 'Receita',
  [DocumentTemplateType.MedicalCertificate]: 'Atestado Médico',
  [DocumentTemplateType.LabTestRequest]: 'Pedido de Exames',
  [DocumentTemplateType.PsychologicalReport]: 'Relatório Psicológico',
  [DocumentTemplateType.NutritionPlan]: 'Plano Alimentar',
  [DocumentTemplateType.DentalBudget]: 'Orçamento Odontológico',
  [DocumentTemplateType.Odontogram]: 'Odontograma',
  [DocumentTemplateType.PhysicalTherapyEvaluation]: 'Avaliação Fisioterapêutica',
  [DocumentTemplateType.TreatmentPlan]: 'Plano de Tratamento',
  [DocumentTemplateType.SessionEvolution]: 'Evolução de Sessão',
  [DocumentTemplateType.DischargeReport]: 'Relatório de Alta',
  [DocumentTemplateType.Referral]: 'Encaminhamento',
  [DocumentTemplateType.InformedConsent]: 'Termo de Consentimento',
  [DocumentTemplateType.Custom]: 'Modelo Personalizado'
};

export const ProfessionalSpecialtyLabels: { [key in ProfessionalSpecialty]: string } = {
  [ProfessionalSpecialty.Medico]: 'Médico',
  [ProfessionalSpecialty.Psicologo]: 'Psicólogo',
  [ProfessionalSpecialty.Nutricionista]: 'Nutricionista',
  [ProfessionalSpecialty.Fisioterapeuta]: 'Fisioterapeuta',
  [ProfessionalSpecialty.Dentista]: 'Dentista',
  [ProfessionalSpecialty.Enfermeiro]: 'Enfermeiro',
  [ProfessionalSpecialty.TerapeutaOcupacional]: 'Terapeuta Ocupacional',
  [ProfessionalSpecialty.Fonoaudiologo]: 'Fonoaudiólogo'
};
