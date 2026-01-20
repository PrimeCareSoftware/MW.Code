export interface MedicalRecord {
  id: string;
  appointmentId?: string;
  patientId: string;
  patientName?: string;
  chiefComplaint: string;
  historyOfPresentIllness: string;
  isClosed: boolean;
  diagnosis: string;
  prescription?: string;
  notes?: string;
  consultationDurationMinutes?: number;
  consultationStartTime?: string;
  consultationEndTime?: string;
  createdAt: string;
}
