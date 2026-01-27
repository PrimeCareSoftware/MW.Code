export interface ConsultationFormConfigurationDto {
  id: string;
  clinicId: string;
  profileId?: string;
  configurationName: string;
  isActive: boolean;
  
  // Field visibility
  showChiefComplaint: boolean;
  showHistoryOfPresentIllness: boolean;
  showPastMedicalHistory: boolean;
  showFamilyHistory: boolean;
  showLifestyleHabits: boolean;
  showCurrentMedications: boolean;
  
  // Field required controls
  requireChiefComplaint: boolean;
  requireHistoryOfPresentIllness: boolean;
  requirePastMedicalHistory: boolean;
  requireFamilyHistory: boolean;
  requireLifestyleHabits: boolean;
  requireCurrentMedications: boolean;
  requireClinicalExamination: boolean;
  requireDiagnosticHypothesis: boolean;
  requireInformedConsent: boolean;
  requireTherapeuticPlan: boolean;
  
  customFields?: CustomFieldDto[];
  createdAt: string;
  updatedAt?: string;
}

export interface CustomFieldDto {
  fieldKey: string;
  label: string;
  fieldType: CustomFieldType;
  isRequired: boolean;
  displayOrder: number;
  placeholder?: string;
  defaultValue?: string;
  helpText?: string;
  options?: string[];
}

export enum CustomFieldType {
  TextoCurto = 0,
  TextoLongo = 1,
  Numero = 2,
  Data = 3,
  SelecaoUnica = 4,
  SelecaoMultipla = 5,
  CheckBox = 6
}
