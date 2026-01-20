export interface ExamCatalog {
  id: string;
  name: string;
  description?: string;
  examType: ExamType;
  category?: string;
  preparation?: string;
  synonyms?: string;
  tussCode?: string;
  isActive: boolean;
  createdAt: Date;
  updatedAt?: Date;
}

export interface CreateExamCatalogRequest {
  name: string;
  description?: string;
  examType: ExamType;
  category?: string;
  preparation?: string;
  synonyms?: string;
  tussCode?: string;
}

export interface UpdateExamCatalogRequest {
  name?: string;
  description?: string;
  examType?: ExamType;
  category?: string;
  preparation?: string;
  synonyms?: string;
  tussCode?: string;
}

export enum ExamType {
  Laboratory = 0,
  Imaging = 1,
  Cardiac = 2,
  Endoscopy = 3,
  Biopsy = 4,
  Ultrasound = 5,
  Other = 6
}

export const EXAM_TYPE_LABELS: { [key in ExamType]: string } = {
  [ExamType.Laboratory]: 'Laboratorial',
  [ExamType.Imaging]: 'Imagem',
  [ExamType.Cardiac]: 'Cardíaco',
  [ExamType.Endoscopy]: 'Endoscopia',
  [ExamType.Biopsy]: 'Biópsia',
  [ExamType.Ultrasound]: 'Ultrassom',
  [ExamType.Other]: 'Outros'
};
