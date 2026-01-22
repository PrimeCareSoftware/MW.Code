export interface SoapRecord {
  id: string;
  attendanceId: string;
  tenantId: string;
  patientId: string;
  doctorId: string;
  recordDate: string;
  subjective: SubjectiveData | null;
  objective: ObjectiveData | null;
  assessment: AssessmentData | null;
  plan: PlanData | null;
  isComplete: boolean;
  completionDate: string | null;
  isLocked: boolean;
  createdAt: string;
  updatedAt: string | null;
}

export interface SubjectiveData {
  chiefComplaint: string;
  historyOfPresentIllness: string;
  currentSymptoms: string;
  symptomDuration: string;
  aggravatingFactors: string;
  relievingFactors: string;
  reviewOfSystems: string;
  allergies: string;
  currentMedications: string;
  pastMedicalHistory: string;
  familyHistory: string;
  socialHistory: string;
}

export interface ObjectiveData {
  vitalSigns: VitalSigns;
  physicalExam: PhysicalExamination;
  labResults: string;
  imagingResults: string;
  otherExamResults: string;
}

export interface VitalSigns {
  systolicBP: number | null;
  diastolicBP: number | null;
  heartRate: number | null;
  respiratoryRate: number | null;
  temperature: number | null;
  oxygenSaturation: number | null;
  weight: number | null;
  height: number | null;
  bmi: number | null;
  pain: number | null;
}

export interface PhysicalExamination {
  generalAppearance: string;
  head: string;
  eyes: string;
  ears: string;
  nose: string;
  throat: string;
  neck: string;
  cardiovascular: string;
  respiratory: string;
  abdomen: string;
  musculoskeletal: string;
  neurological: string;
  skin: string;
  otherFindings: string;
}

export interface AssessmentData {
  primaryDiagnosis: string;
  primaryDiagnosisIcd10: string;
  differentialDiagnoses: DifferentialDiagnosis[];
  clinicalReasoning: string;
  prognosis: string;
  evolution: string;
}

export interface DifferentialDiagnosis {
  diagnosis: string;
  icd10Code: string;
  justification: string;
  priority: number;
}

export interface PlanData {
  prescriptions: Prescription[];
  examRequests: ExamRequest[];
  procedures: Procedure[];
  referrals: Referral[];
  returnInstructions: string;
  nextAppointmentDate: string | null;
  patientInstructions: string;
  dietaryRecommendations: string;
  activityRestrictions: string;
  warningSymptoms: string;
}

export interface Prescription {
  medicationName: string;
  dosage: string;
  frequency: string;
  duration: string;
  instructions: string;
}

export interface ExamRequest {
  examName: string;
  examType: string;
  clinicalIndication: string;
  isUrgent: boolean;
}

export interface Procedure {
  procedureName: string;
  description: string;
  scheduledDate: string | null;
}

export interface Referral {
  specialtyName: string;
  reason: string;
  priority: string;
}

export interface UpdateSubjectiveCommand {
  chiefComplaint: string;
  historyOfPresentIllness: string;
  currentSymptoms: string;
  symptomDuration: string;
  aggravatingFactors?: string;
  relievingFactors?: string;
  reviewOfSystems?: string;
  allergies: string;
  currentMedications: string;
  pastMedicalHistory?: string;
  familyHistory?: string;
  socialHistory?: string;
}

export interface UpdateObjectiveCommand {
  vitalSigns: VitalSigns;
  physicalExam: PhysicalExamination;
  labResults?: string;
  imagingResults?: string;
  otherExamResults?: string;
}

export interface UpdateAssessmentCommand {
  primaryDiagnosis: string;
  primaryDiagnosisIcd10: string;
  differentialDiagnoses: DifferentialDiagnosis[];
  clinicalReasoning: string;
  prognosis?: string;
  evolution?: string;
}

export interface UpdatePlanCommand {
  prescriptions: Prescription[];
  examRequests: ExamRequest[];
  procedures?: Procedure[];
  referrals?: Referral[];
  returnInstructions: string;
  nextAppointmentDate?: string | null;
  patientInstructions: string;
  dietaryRecommendations?: string;
  activityRestrictions?: string;
  warningSymptoms?: string;
}

export interface SoapRecordValidation {
  isValid: boolean;
  missingFields: string[];
  warnings: string[];
  hasSubjective: boolean;
  hasObjective: boolean;
  hasAssessment: boolean;
  hasPlan: boolean;
}
