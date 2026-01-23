export enum MedicalSpecialty {
  Cardiology = 1,
  Pediatrics = 2,
  Gynecology = 3,
  Dermatology = 4,
  Orthopedics = 5,
  Psychiatry = 6,
  Endocrinology = 7,
  Neurology = 8,
  Ophthalmology = 9,
  Otorhinolaryngology = 10,
  GeneralMedicine = 11,
  Other = 12
}

export enum QuestionType {
  Text = 1,
  Number = 2,
  YesNo = 3,
  SingleChoice = 4,
  MultipleChoice = 5,
  Date = 6,
  Scale = 7
}

export interface Question {
  questionText: string;
  type: QuestionType;
  isRequired: boolean;
  options?: string[];
  unit?: string;
  order: number;
  helpText?: string;
  snomedCode?: string;
}

export interface QuestionSection {
  sectionName: string;
  order: number;
  questions: Question[];
}

export interface AnamnesisTemplate {
  id: string;
  tenantId: string;
  name: string;
  specialty: MedicalSpecialty;
  description?: string;
  isActive: boolean;
  isDefault: boolean;
  sections: QuestionSection[];
  createdBy: string;
  createdAt: string;
  updatedAt?: string;
}

export interface QuestionAnswer {
  questionText: string;
  type: QuestionType;
  answer: string;
  selectedOptions?: string[];
  numericValue?: number;
  booleanValue?: boolean;
  dateValue?: string;
}

export interface AnamnesisResponse {
  id: string;
  tenantId: string;
  appointmentId: string;
  patientId: string;
  doctorId: string;
  templateId: string;
  responseDate: string;
  answers: QuestionAnswer[];
  isComplete: boolean;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateAnamnesisTemplateRequest {
  name: string;
  specialty: MedicalSpecialty;
  description?: string;
  isDefault: boolean;
  sections: QuestionSection[];
}

export interface UpdateAnamnesisTemplateRequest {
  name: string;
  description?: string;
  isDefault: boolean;
  sections: QuestionSection[];
}

export interface CreateAnamnesisResponseRequest {
  appointmentId: string;
  templateId: string;
}

export interface SaveAnswersRequest {
  answers: QuestionAnswer[];
  isComplete: boolean;
}

export const SPECIALTY_NAMES: Record<MedicalSpecialty, string> = {
  [MedicalSpecialty.Cardiology]: 'Cardiologia',
  [MedicalSpecialty.Pediatrics]: 'Pediatria',
  [MedicalSpecialty.Gynecology]: 'Ginecologia',
  [MedicalSpecialty.Dermatology]: 'Dermatologia',
  [MedicalSpecialty.Orthopedics]: 'Ortopedia',
  [MedicalSpecialty.Psychiatry]: 'Psiquiatria',
  [MedicalSpecialty.Endocrinology]: 'Endocrinologia',
  [MedicalSpecialty.Neurology]: 'Neurologia',
  [MedicalSpecialty.Ophthalmology]: 'Oftalmologia',
  [MedicalSpecialty.Otorhinolaryngology]: 'Otorrinolaringologia',
  [MedicalSpecialty.GeneralMedicine]: 'Medicina Geral',
  [MedicalSpecialty.Other]: 'Outra'
};
