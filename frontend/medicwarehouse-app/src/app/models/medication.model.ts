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
  anvisaRegistration?: string;
  barcode?: string;
  description?: string;
  isActive: boolean;
  createdAt: string;
  updatedAt?: string;
}

export interface MedicationAutocomplete {
  id: string;
  name: string;
  genericName?: string;
  dosage: string;
  pharmaceuticalForm: string;
  administrationRoute?: string;
  displayText: string;
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

export const MedicationCategoryLabels: Record<MedicationCategory, string> = {
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
