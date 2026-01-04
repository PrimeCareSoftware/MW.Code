export interface MedicalRecord {
  id: string;
  appointmentId: string;
  patientId: string;
  patientName: string;
  // Legacy fields (DEPRECATED but kept for backward compatibility)
  diagnosis: string;
  prescription: string;
  notes: string;
  consultationDurationMinutes: number;
  consultationStartTime: string;
  consultationEndTime?: string;
  // CFM 1.821 Required Fields
  chiefComplaint: string;
  historyOfPresentIllness: string;
  // CFM 1.821 Recommended Fields
  pastMedicalHistory?: string;
  familyHistory?: string;
  lifestyleHabits?: string;
  currentMedications?: string;
  // Control fields
  isClosed: boolean;
  closedAt?: string;
  closedByUserId?: string;
  // Related entities
  clinicalExaminations?: ClinicalExamination[];
  diagnosticHypotheses?: DiagnosticHypothesis[];
  therapeuticPlans?: TherapeuticPlan[];
  informedConsents?: InformedConsent[];
  createdAt: string;
  updatedAt?: string;
}

export interface CreateMedicalRecord {
  appointmentId: string;
  patientId: string;
  consultationStartTime: string;
  // CFM 1.821 Required Fields
  chiefComplaint: string;
  historyOfPresentIllness: string;
  // CFM 1.821 Recommended Fields
  pastMedicalHistory?: string;
  familyHistory?: string;
  lifestyleHabits?: string;
  currentMedications?: string;
  // Legacy fields (optional for backward compatibility)
  diagnosis?: string;
  prescription?: string;
  notes?: string;
}

export interface UpdateMedicalRecord {
  // CFM 1.821 Fields
  chiefComplaint?: string;
  historyOfPresentIllness?: string;
  pastMedicalHistory?: string;
  familyHistory?: string;
  lifestyleHabits?: string;
  currentMedications?: string;
  // Legacy fields
  diagnosis?: string;
  prescription?: string;
  notes?: string;
  consultationDurationMinutes?: number;
}

export interface CompleteMedicalRecord {
  diagnosis?: string;
  prescription?: string;
  notes?: string;
}

// CFM 1.821 New Entities

export interface ClinicalExamination {
  id: string;
  medicalRecordId: string;
  tenantId: string;
  // Vital signs
  bloodPressureSystolic?: number;
  bloodPressureDiastolic?: number;
  heartRate?: number;
  respiratoryRate?: number;
  temperature?: number;
  oxygenSaturation?: number;
  // Physical examination
  systematicExamination: string;
  generalState?: string;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateClinicalExamination {
  medicalRecordId: string;
  systematicExamination: string;
  bloodPressureSystolic?: number;
  bloodPressureDiastolic?: number;
  heartRate?: number;
  respiratoryRate?: number;
  temperature?: number;
  oxygenSaturation?: number;
  generalState?: string;
}

export interface UpdateClinicalExamination {
  systematicExamination?: string;
  bloodPressureSystolic?: number;
  bloodPressureDiastolic?: number;
  heartRate?: number;
  respiratoryRate?: number;
  temperature?: number;
  oxygenSaturation?: number;
  generalState?: string;
}

export enum DiagnosisType {
  Principal = 1,
  Secondary = 2
}

export const DiagnosisTypeLabels: Record<DiagnosisType, string> = {
  [DiagnosisType.Principal]: 'Principal',
  [DiagnosisType.Secondary]: 'Secundário'
};

export interface DiagnosticHypothesis {
  id: string;
  medicalRecordId: string;
  tenantId: string;
  description: string;
  icd10Code: string;
  type: DiagnosisType;
  diagnosedAt: string;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateDiagnosticHypothesis {
  medicalRecordId: string;
  description: string;
  icd10Code: string;
  type: DiagnosisType;
}

export interface UpdateDiagnosticHypothesis {
  description?: string;
  icd10Code?: string;
  type?: DiagnosisType;
}

export interface TherapeuticPlan {
  id: string;
  medicalRecordId: string;
  tenantId: string;
  treatment: string;
  medicationPrescription?: string;
  examRequests?: string;
  referrals?: string;
  patientGuidance?: string;
  returnDate?: string;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateTherapeuticPlan {
  medicalRecordId: string;
  treatment: string;
  medicationPrescription?: string;
  examRequests?: string;
  referrals?: string;
  patientGuidance?: string;
  returnDate?: string;
}

export interface UpdateTherapeuticPlan {
  treatment?: string;
  medicationPrescription?: string;
  examRequests?: string;
  referrals?: string;
  patientGuidance?: string;
  returnDate?: string;
}

export interface InformedConsent {
  id: string;
  medicalRecordId: string;
  patientId: string;
  tenantId: string;
  consentText: string;
  isAccepted: boolean;
  acceptedAt?: string;
  ipAddress?: string;
  digitalSignature?: string;
  registeredByUserId: string;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateInformedConsent {
  medicalRecordId: string;
  patientId: string;
  consentText: string;
}

export interface AcceptInformedConsent {
  ipAddress?: string;
  digitalSignature?: string;
}
