export enum ExamRequestStatus {
  Pending = 'Pending',
  Scheduled = 'Scheduled',
  Completed = 'Completed',
  Cancelled = 'Cancelled'
}

export enum ExamUrgency {
  Routine = 'Routine',
  Urgent = 'Urgent',
  Emergency = 'Emergency'
}

export enum ExamType {
  Laboratory = 'Laboratory',
  Imaging = 'Imaging',
  Ultrasound = 'Ultrasound',
  XRay = 'XRay',
  ECG = 'ECG',
  EEG = 'EEG',
  Cardiac = 'Cardiac'
}

export interface ExamRequest {
  id: string;
  appointmentId?: string;
  patientId: string;
  patientName?: string;
  examType: ExamType | string;
  examName?: string;
  description?: string;
  status: ExamRequestStatus;
  urgency: ExamUrgency;
  requestedDate?: string;
  scheduledDate?: string;
  completedDate?: string;
  results?: string;
  notes?: string;
  createdAt: string;
  updatedAt?: string;
}
