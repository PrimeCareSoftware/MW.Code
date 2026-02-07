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
  calendarColor?: string; // Hex color for calendar display
}

export interface DailyAgenda {
  date: string;
  clinicId: string;
  clinicName: string;
  appointments: Appointment[];
  availableSlots: string[];
}

export interface WeekAgenda {
  startDate: string;
  endDate: string;
  clinicId: string;
  clinicName: string;
  appointments: Appointment[];
}

export interface AvailableSlot {
  date: string;
  time: string;
  durationMinutes: number;
  isAvailable: boolean;
}

// Blocked Time Slot Types
export enum BlockedTimeSlotType {
  Break = 1,           // Intervalo (almoço, descanso)
  Unavailable = 2,     // Indisponível (férias, compromisso pessoal)
  Maintenance = 3,     // Manutenção (limpeza, equipamento)
  Training = 4,        // Treinamento
  Meeting = 5,         // Reunião
  Other = 6           // Outro motivo
}

export const BlockedTimeSlotTypeLabels: { [key: number]: string } = {
  [BlockedTimeSlotType.Break]: 'Intervalo',
  [BlockedTimeSlotType.Unavailable]: 'Indisponível',
  [BlockedTimeSlotType.Maintenance]: 'Manutenção',
  [BlockedTimeSlotType.Training]: 'Treinamento',
  [BlockedTimeSlotType.Meeting]: 'Reunião',
  [BlockedTimeSlotType.Other]: 'Outro'
};

// Recurrence Frequency
export enum RecurrenceFrequency {
  Daily = 1,
  Weekly = 2,
  Biweekly = 3,
  Monthly = 4,
  Custom = 5
}

export const RecurrenceFrequencyLabels: { [key: number]: string } = {
  [RecurrenceFrequency.Daily]: 'Diário',
  [RecurrenceFrequency.Weekly]: 'Semanal',
  [RecurrenceFrequency.Biweekly]: 'Quinzenal',
  [RecurrenceFrequency.Monthly]: 'Mensal',
  [RecurrenceFrequency.Custom]: 'Personalizado'
};

// Recurrence Days (flags)
export enum RecurrenceDays {
  None = 0,
  Sunday = 1,
  Monday = 2,
  Tuesday = 4,
  Wednesday = 8,
  Thursday = 16,
  Friday = 32,
  Saturday = 64
}

export const RecurrenceDaysLabels: { [key: number]: string } = {
  [RecurrenceDays.Sunday]: 'Domingo',
  [RecurrenceDays.Monday]: 'Segunda',
  [RecurrenceDays.Tuesday]: 'Terça',
  [RecurrenceDays.Wednesday]: 'Quarta',
  [RecurrenceDays.Thursday]: 'Quinta',
  [RecurrenceDays.Friday]: 'Sexta',
  [RecurrenceDays.Saturday]: 'Sábado'
};

export interface BlockedTimeSlot {
  id: string;
  clinicId: string;
  professionalId?: string;
  date: string;
  startTime: string;
  endTime: string;
  type: BlockedTimeSlotType;
  reason?: string;
  isRecurring: boolean;
  recurringPatternId?: string;
  createdAt: string;
  updatedAt?: string;
  clinicName?: string;
  professionalName?: string;
}

export interface CreateBlockedTimeSlot {
  clinicId: string;
  professionalId?: string;
  date: string;
  startTime: string;
  endTime: string;
  type: BlockedTimeSlotType;
  reason?: string;
}

export interface UpdateBlockedTimeSlot {
  startTime: string;
  endTime: string;
  type: BlockedTimeSlotType;
  reason?: string;
}

export interface RecurringAppointmentPattern {
  id: string;
  clinicId: string;
  professionalId?: string;
  patientId?: string;
  frequency: RecurrenceFrequency;
  interval: number;
  daysOfWeek?: number; // RecurrenceDays flags
  dayOfMonth?: number;
  startDate: string;
  endDate?: string;
  occurrencesCount?: number;
  startTime: string;
  endTime: string;
  durationMinutes?: number;
  appointmentType?: string;
  blockedSlotType?: BlockedTimeSlotType;
  notes?: string;
  isActive: boolean;
  createdAt: string;
  clinicName?: string;
  professionalName?: string;
  patientName?: string;
}

export interface CreateRecurringBlockedSlots {
  clinicId: string;
  professionalId?: string;
  frequency: RecurrenceFrequency;
  interval: number;
  daysOfWeek?: number;
  dayOfMonth?: number;
  startDate: string;
  endDate?: string;
  occurrencesCount?: number;
  startTime: string;
  endTime: string;
  blockedSlotType: BlockedTimeSlotType;
  notes?: string;
}
