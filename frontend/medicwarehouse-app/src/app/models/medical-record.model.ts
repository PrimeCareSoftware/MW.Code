export interface MedicalRecord {
  id: string;
  appointmentId: string;
  patientId: string;
  patientName: string;
  diagnosis: string;
  prescription: string;
  notes: string;
  consultationDurationMinutes: number;
  consultationStartTime: string;
  consultationEndTime?: string;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateMedicalRecord {
  appointmentId: string;
  patientId: string;
  consultationStartTime: string;
  diagnosis?: string;
  prescription?: string;
  notes?: string;
}

export interface UpdateMedicalRecord {
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
