import { ExamType } from './exam-request.model';

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
  createdAt: string;
  updatedAt?: string;
}

export interface ExamAutocomplete {
  id: string;
  name: string;
  examType: ExamType;
  category?: string;
  preparation?: string;
  displayText: string;
}
