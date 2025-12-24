export interface PaymentHistory {
  paymentId: string;
  amount: number;
  method: string; // 'Cash' | 'CreditCard' | 'DebitCard' | 'Pix' | 'BankTransfer' | 'Check'
  status: string; // 'Pending' | 'Processing' | 'Paid' | 'Failed' | 'Refunded' | 'Cancelled'
  paymentDate: string;
  cardLastFourDigits?: string;
  pixKey?: string;
}

export interface MedicalRecordSummary {
  medicalRecordId: string;
  diagnosis: string;
  consultationDurationMinutes: number;
  createdAt: string;
}

export interface PatientAppointmentHistory {
  appointmentId: string;
  scheduledDate: string;
  scheduledTime: string;
  status: string;
  type: string;
  doctorName?: string;
  doctorSpecialty?: string;
  doctorProfessionalId?: string;
  checkInTime?: string;
  checkOutTime?: string;
  payment?: PaymentHistory;
  medicalRecord?: MedicalRecordSummary;
}

export interface PatientProcedureHistory {
  procedureId: string;
  appointmentId: string;
  procedureName: string;
  procedureCode: string;
  procedureCategory: string;
  priceCharged: number;
  performedAt: string;
  notes?: string;
  doctorName?: string;
  doctorSpecialty?: string;
  payment?: PaymentHistory;
}

export interface PatientCompleteHistory {
  patientId: string;
  patientName: string;
  appointments: PatientAppointmentHistory[];
  procedures: PatientProcedureHistory[];
}
