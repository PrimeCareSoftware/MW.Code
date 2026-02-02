export interface Appointment {
  id: string;
  patientId: string;
  patientName: string;
  professionalId?: string; // Backend uses ProfessionalId
  professionalName?: string;
  professionalSpecialty?: string; // Added: Professional's specialty (e.g., "Médico", "Psicólogo", etc.)
  doctorId?: string; // Alias for professionalId (for backward compatibility)
  doctorName?: string; // Alias for professionalName
  clinicId: string;
  clinicName: string;
  scheduledDate: string;
  scheduledTime: string;
  durationMinutes: number;
  type: string;
  status: string;
  notes?: string;
  cancellationReason?: string;
  checkInTime?: string;
  checkOutTime?: string;
  isPaid: boolean;
  paidAt?: string;
  paidByUserId?: string;
  paidByUserName?: string;
  paymentReceivedBy?: string; // Doctor, Secretary, Other
  createdAt: string;
  updatedAt?: string;
}

export interface CreateAppointment {
  patientId: string;
  clinicId: string;
  professionalId?: string; // Doctor/Professional ID
  scheduledDate: string;
  scheduledTime: string;
  durationMinutes: number;
  type: string;
  notes?: string;
}

export interface UpdateAppointment {
  professionalId?: string; // Doctor/Professional ID
  scheduledDate: string;
  scheduledTime: string;
  durationMinutes: number;
  type: string;
  notes?: string;
}

export interface Professional {
  id: string;
  fullName: string;
  professionalId?: string; // CRM, CRO, etc.
  specialty?: string;
  role: string;
}

export interface DailyAgenda {
  date: string;
  clinicId: string;
  clinicName: string;
  appointments: Appointment[];
  availableSlots: string[];
}

export interface AvailableSlot {
  date: string;
  time: string;
  durationMinutes: number;
  isAvailable: boolean;
}
