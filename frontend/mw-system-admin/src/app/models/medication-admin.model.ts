export interface Medication {
  id: string;
  name: string;
  genericName?: string;
  manufacturer?: string;
  activeIngredient?: string;
  dosage: string;
  pharmaceuticalForm: string;
  concentration?: string;
  administrationRoute?: string;
  category: MedicationCategory;
  requiresPrescription: boolean;
  isControlled: boolean;
  controlledList?: ControlledSubstanceList;
  anvisaRegistration?: string;
  barcode?: string;
  description?: string;
  isActive: boolean;
  createdAt: Date;
  updatedAt?: Date;
}

export interface CreateMedicationRequest {
  name: string;
  genericName?: string;
  manufacturer?: string;
  activeIngredient?: string;
  dosage: string;
  pharmaceuticalForm: string;
  concentration?: string;
  administrationRoute?: string;
  category: MedicationCategory;
  requiresPrescription: boolean;
  isControlled: boolean;
  controlledList?: ControlledSubstanceList;
  anvisaRegistration?: string;
  barcode?: string;
  description?: string;
}

export interface UpdateMedicationRequest {
  name?: string;
  genericName?: string;
  manufacturer?: string;
  activeIngredient?: string;
  dosage?: string;
  pharmaceuticalForm?: string;
  concentration?: string;
  administrationRoute?: string;
  category?: MedicationCategory;
  requiresPrescription?: boolean;
  isControlled?: boolean;
  controlledList?: ControlledSubstanceList;
  anvisaRegistration?: string;
  barcode?: string;
  description?: string;
}

export enum MedicationCategory {
  Analgesic = 0,
  Antibiotic = 1,
  AntiInflammatory = 2,
  Antihypertensive = 3,
  Antihistamine = 4,
  Antidiabetic = 5,
  Antidepressant = 6,
  Anxiolytic = 7,
  Antacid = 8,
  Bronchodilator = 9,
  Diuretic = 10,
  Anticoagulant = 11,
  Corticosteroid = 12,
  Vitamin = 13,
  Supplement = 14,
  Vaccine = 15,
  Contraceptive = 16,
  Antifungal = 17,
  Antiviral = 18,
  Antiparasitic = 19,
  Other = 20
}

export enum ControlledSubstanceList {
  None = 0,
  A1_Narcotics = 1,
  B1_Psychotropics = 2
}

export const MEDICATION_CATEGORY_LABELS: { [key in MedicationCategory]: string } = {
  [MedicationCategory.Analgesic]: 'Analgésico',
  [MedicationCategory.Antibiotic]: 'Antibiótico',
  [MedicationCategory.AntiInflammatory]: 'Anti-inflamatório',
  [MedicationCategory.Antihypertensive]: 'Anti-hipertensivo',
  [MedicationCategory.Antihistamine]: 'Anti-histamínico',
  [MedicationCategory.Antidiabetic]: 'Antidiabético',
  [MedicationCategory.Antidepressant]: 'Antidepressivo',
  [MedicationCategory.Anxiolytic]: 'Ansiolítico',
  [MedicationCategory.Antacid]: 'Antiácido',
  [MedicationCategory.Bronchodilator]: 'Broncodilatador',
  [MedicationCategory.Diuretic]: 'Diurético',
  [MedicationCategory.Anticoagulant]: 'Anticoagulante',
  [MedicationCategory.Corticosteroid]: 'Corticosteroide',
  [MedicationCategory.Vitamin]: 'Vitamina',
  [MedicationCategory.Supplement]: 'Suplemento',
  [MedicationCategory.Vaccine]: 'Vacina',
  [MedicationCategory.Contraceptive]: 'Anticoncepcional',
  [MedicationCategory.Antifungal]: 'Antifúngico',
  [MedicationCategory.Antiviral]: 'Antiviral',
  [MedicationCategory.Antiparasitic]: 'Antiparasitário',
  [MedicationCategory.Other]: 'Outros'
};

export const CONTROLLED_SUBSTANCE_LABELS: { [key in ControlledSubstanceList]: string } = {
  [ControlledSubstanceList.None]: 'Não controlado',
  [ControlledSubstanceList.A1_Narcotics]: 'Lista A1 - Entorpecentes',
  [ControlledSubstanceList.B1_Psychotropics]: 'Lista B1 - Psicotrópicos'
};
