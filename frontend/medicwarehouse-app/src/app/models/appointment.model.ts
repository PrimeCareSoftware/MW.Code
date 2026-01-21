export interface Appointment {
  id: string;
  patientId: string;
  patientName: string;
  doctorId?: string;
  doctorName?: string;
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
  scheduledDate: string;
  scheduledTime: string;
  durationMinutes: number;
  type: string;
  notes?: string;
}

export interface UpdateAppointment {
  scheduledDate: string;
  scheduledTime: string;
  durationMinutes: number;
  type: string;
  notes?: string;
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
