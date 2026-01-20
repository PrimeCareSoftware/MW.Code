export interface AvailableSlot {
  date: string;
  time: string;
  durationMinutes: number;
  isAvailable: boolean;
}

export interface DailyAgenda {
  date: string;
  clinicId: string;
  clinicName: string;
  appointments: Appointment[];
  availableSlots: string[];
}

export interface Appointment {
  id: string;
  patientId: string;
  patientName: string;
  clinicId: string;
  clinicName: string;
  scheduledDate: string;
  scheduledTime: string;
  durationMinutes: number;
  type: string;
  status: AppointmentStatus;
  notes?: string;
  checkInTime?: string;
  checkOutTime?: string;
  createdAt: string;
}

export type AppointmentStatus = 'Agendado' | 'Confirmado' | 'Atendido' | 'Cancelado';
