// Digital Prescription models matching backend DTOs

export enum PrescriptionType {
  Simple = 'Simple',
  SpecialControlA = 'SpecialControlA',
  SpecialControlB = 'SpecialControlB',
  SpecialControlC1 = 'SpecialControlC1',
  Antimicrobial = 'Antimicrobial'
}

export enum ControlledSubstanceList {
  None = 'None',
  A1_Narcotics = 'A1_Narcotics',
  A2_Psychotropics = 'A2_Psychotropics',
  A3_Psychotropics = 'A3_Psychotropics',
  B1_Psychotropics = 'B1_Psychotropics',
  B2_Anorexigenics = 'B2_Anorexigenics',
  C1_OtherControlled = 'C1_OtherControlled',
  C2_Retinoids = 'C2_Retinoids',
  C3_Immunosuppressants = 'C3_Immunosuppressants',
  C4_Antiretrovirals = 'C4_Antiretrovirals',
  C5_Anabolics = 'C5_Anabolics'
}

export interface DigitalPrescriptionItem {
  id?: string;
  digitalPrescriptionId?: string;
  medicationId: string;
  medicationName: string;
  genericName?: string;
  activeIngredient?: string;
  isControlledSubstance: boolean;
  controlledList?: string;
  anvisaRegistration?: string;
  dosage: string;
  pharmaceuticalForm: string;
  frequency: string;
  durationDays: number;
  quantity: number;
  administrationRoute?: string;
  instructions?: string;
  batchNumber?: string;
  manufactureDate?: Date;
  expiryDate?: Date;
}

export interface DigitalPrescription {
  id?: string;
  medicalRecordId: string;
  patientId: string;
  doctorId: string;
  type: PrescriptionType | string;
  sequenceNumber?: string;
  issuedAt?: Date;
  expiresAt?: Date;
  isActive?: boolean;
  doctorName: string;
  doctorCRM: string;
  doctorCRMState: string;
  patientName: string;
  patientDocument: string;
  digitalSignature?: string;
  signedAt?: Date;
  signatureCertificate?: string;
  verificationCode?: string;
  requiresSNGPCReport?: boolean;
  reportedToSNGPCAt?: Date;
  notes?: string;
  items: DigitalPrescriptionItem[];
  daysUntilExpiration?: number;
  isExpired?: boolean;
  isValid?: boolean;
}

export interface CreateDigitalPrescription {
  medicalRecordId: string;
  patientId: string;
  doctorId: string;
  type: PrescriptionType | string;
  doctorName: string;
  doctorCRM: string;
  doctorCRMState: string;
  patientName: string;
  patientDocument: string;
  notes?: string;
  items: DigitalPrescriptionItem[];
}

// SNGPC Report models

export enum SNGPCReportStatus {
  Draft = 'Draft',
  Generated = 'Generated',
  Transmitted = 'Transmitted',
  TransmissionFailed = 'TransmissionFailed',
  Validated = 'Validated'
}

export interface SNGPCReport {
  id: string;
  month: number;
  year: number;
  reportPeriodStart: Date;
  reportPeriodEnd: Date;
  status: SNGPCReportStatus | string;
  generatedAt: Date;
  transmittedAt?: Date;
  transmissionProtocol?: string;
  totalPrescriptions: number;
  totalItems: number;
  errorMessage?: string;
  lastAttemptAt?: Date;
  attemptCount: number;
  daysUntilDeadline: number;
  isOverdue: boolean;
}

export interface CreateSNGPCReport {
  month: number;
  year: number;
}

// Prescription type information for UI
export interface PrescriptionTypeInfo {
  type: PrescriptionType;
  name: string;
  description: string;
  color: string;
  icon: string;
  expirationDays: number;
  requiresSNGPC: boolean;
  requiresSpecialForm: boolean;
}

export const PRESCRIPTION_TYPES: PrescriptionTypeInfo[] = [
  {
    type: PrescriptionType.Simple,
    name: 'Receita Simples',
    description: 'Para medicamentos comuns (validade 30 dias)',
    color: 'primary',
    icon: 'receipt',
    expirationDays: 30,
    requiresSNGPC: false,
    requiresSpecialForm: false
  },
  {
    type: PrescriptionType.SpecialControlB,
    name: 'Controle Especial B (Psicotrópicos)',
    description: 'Lista B1 e B2 da Portaria 344/98 - Ex: benzodiazepínicos, anfetaminas',
    color: 'warn',
    icon: 'warning',
    expirationDays: 30,
    requiresSNGPC: true,
    requiresSpecialForm: true
  },
  {
    type: PrescriptionType.SpecialControlA,
    name: 'Controle Especial A (Entorpecentes)',
    description: 'Lista A1, A2, A3 da Portaria 344/98 - Ex: morfina, codeína, metadona',
    color: 'accent',
    icon: 'local_hospital',
    expirationDays: 30,
    requiresSNGPC: true,
    requiresSpecialForm: true
  },
  {
    type: PrescriptionType.Antimicrobial,
    name: 'Receita Antimicrobiana',
    description: 'Antibióticos (validade 10 dias)',
    color: 'warn',
    icon: 'medication',
    expirationDays: 10,
    requiresSNGPC: false,
    requiresSpecialForm: true
  },
  {
    type: PrescriptionType.SpecialControlC1,
    name: 'Controle Especial C1',
    description: 'Lista C1 da Portaria 344/98 - Ex: anticonvulsivantes, imunossupressores',
    color: 'accent',
    icon: 'healing',
    expirationDays: 30,
    requiresSNGPC: true,
    requiresSpecialForm: true
  }
];
