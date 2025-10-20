export interface ExamRequest {
  id: string;
  appointmentId: string;
  patientId: string;
  patientName: string;
  examType: ExamType;
  examName: string;
  description: string;
  urgency: ExamUrgency;
  status: ExamRequestStatus;
  requestedDate: string;
  scheduledDate?: string;
  completedDate?: string;
  results?: string;
  notes?: string;
  createdAt: string;
  updatedAt?: string;
}

export enum ExamType {
  Laboratory = 0,      // Laboratorial
  Imaging = 1,         // Imagem (Raio-X, Tomografia, etc)
  Cardiac = 2,         // Cardíaco (ECG, Ecocardiograma, etc)
  Endoscopy = 3,       // Endoscopia
  Biopsy = 4,          // Biópsia
  Ultrasound = 5,      // Ultrassom
  Other = 6            // Outros
}

export enum ExamUrgency {
  Routine = 0,         // Rotina
  Urgent = 1,          // Urgente
  Emergency = 2        // Emergência
}

export enum ExamRequestStatus {
  Pending = 0,         // Pendente
  Scheduled = 1,       // Agendado
  InProgress = 2,      // Em andamento
  Completed = 3,       // Concluído
  Cancelled = 4        // Cancelado
}

export interface CreateExamRequest {
  appointmentId: string;
  patientId: string;
  examType: ExamType;
  examName: string;
  description: string;
  urgency: ExamUrgency;
  notes?: string;
}

export interface UpdateExamRequest {
  examName?: string;
  description?: string;
  urgency?: ExamUrgency;
  scheduledDate?: string;
  notes?: string;
}

export interface CompleteExamRequest {
  results: string;
  completedDate: string;
  notes?: string;
}

export const ExamTypeLabels: Record<ExamType, string> = {
  [ExamType.Laboratory]: 'Laboratorial',
  [ExamType.Imaging]: 'Imagem',
  [ExamType.Cardiac]: 'Cardíaco',
  [ExamType.Endoscopy]: 'Endoscopia',
  [ExamType.Biopsy]: 'Biópsia',
  [ExamType.Ultrasound]: 'Ultrassom',
  [ExamType.Other]: 'Outros'
};

export const ExamUrgencyLabels: Record<ExamUrgency, string> = {
  [ExamUrgency.Routine]: 'Rotina',
  [ExamUrgency.Urgent]: 'Urgente',
  [ExamUrgency.Emergency]: 'Emergência'
};

export const ExamRequestStatusLabels: Record<ExamRequestStatus, string> = {
  [ExamRequestStatus.Pending]: 'Pendente',
  [ExamRequestStatus.Scheduled]: 'Agendado',
  [ExamRequestStatus.InProgress]: 'Em andamento',
  [ExamRequestStatus.Completed]: 'Concluído',
  [ExamRequestStatus.Cancelled]: 'Cancelado'
};
