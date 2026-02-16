export interface Appointment {
  id: string;
  patientId: string;
  patientName: string;
  professionalId?: string; // Backend uses ProfessionalId
  professionalName?: string;
  professionalSpecialty?: string; // Legacy: Professional's specialty as string
  professionalSpecialtyEnum?: ProfessionalSpecialty; // NEW: Strongly-typed specialty
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

// Professional Specialty Enum - matches backend ProfessionalSpecialty
export enum ProfessionalSpecialty {
  Medico = 1,
  Psicologo = 2,
  Nutricionista = 3,
  Fisioterapeuta = 4,
  Dentista = 5,
  Enfermeiro = 6,
  TerapeutaOcupacional = 7,
  Fonoaudiologo = 8,
  Veterinario = 9,
  Outro = 99
}

export const ProfessionalSpecialtyLabels: { [key: number]: string } = {
  [ProfessionalSpecialty.Medico]: 'Médico',
  [ProfessionalSpecialty.Psicologo]: 'Psicólogo',
  [ProfessionalSpecialty.Nutricionista]: 'Nutricionista',
  [ProfessionalSpecialty.Fisioterapeuta]: 'Fisioterapeuta',
  [ProfessionalSpecialty.Dentista]: 'Dentista',
  [ProfessionalSpecialty.Enfermeiro]: 'Enfermeiro',
  [ProfessionalSpecialty.TerapeutaOcupacional]: 'Terapeuta Ocupacional',
  [ProfessionalSpecialty.Fonoaudiologo]: 'Fonoaudiólogo',
  [ProfessionalSpecialty.Veterinario]: 'Veterinário',
  [ProfessionalSpecialty.Outro]: 'Outro'
};

// Mapping for API compatibility - converts enum to string used by terminology service
export const ProfessionalSpecialtyApiStrings: { [key: number]: string } = {
  [ProfessionalSpecialty.Medico]: 'Medico',
  [ProfessionalSpecialty.Psicologo]: 'Psicologo',
  [ProfessionalSpecialty.Nutricionista]: 'Nutricionista',
  [ProfessionalSpecialty.Fisioterapeuta]: 'Fisioterapeuta',
  [ProfessionalSpecialty.Dentista]: 'Dentista',
  [ProfessionalSpecialty.Enfermeiro]: 'Enfermeiro',
  [ProfessionalSpecialty.TerapeutaOcupacional]: 'TerapeutaOcupacional',
  [ProfessionalSpecialty.Fonoaudiologo]: 'Fonoaudiologo',
  [ProfessionalSpecialty.Veterinario]: 'Veterinario',
  [ProfessionalSpecialty.Outro]: 'Outro'
};

// Helper function to convert enum to API string
export function professionalSpecialtyToString(specialty: ProfessionalSpecialty | number): string {
  return ProfessionalSpecialtyApiStrings[specialty] || 'Medico';
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
  specialty?: string; // Legacy string specialty
  professionalSpecialty?: ProfessionalSpecialty; // NEW: Typed specialty
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
  recurringSeriesId?: string;  // 🆕 NEW: Unique identifier for this specific series
  isException?: boolean;        // 🆕 NEW: Flag indicating this is an exception
  createdAt: string;
  updatedAt?: string;
  clinicName?: string;
  professionalName?: string;
}

// Recurrence Delete Scope - Inspired by Google Calendar, Outlook, and RFC 5545
export enum RecurringDeleteScope {
  ThisOccurrence = 1,      // Apenas esta ocorrência
  ThisAndFuture = 2,       // Esta e futuras ocorrências
  AllInSeries = 3          // Toda a série
}

export const RecurringDeleteScopeLabels: { [key: number]: string } = {
  [RecurringDeleteScope.ThisOccurrence]: 'Apenas esta ocorrência',
  [RecurringDeleteScope.ThisAndFuture]: 'Esta e futuras ocorrências',
  [RecurringDeleteScope.AllInSeries]: 'Toda a série'
};

export const RecurringDeleteScopeDescriptions: { [key: number]: string } = {
  [RecurringDeleteScope.ThisOccurrence]: 'Remove apenas este bloqueio específico. Outras ocorrências da série permanecerão intactas.',
  [RecurringDeleteScope.ThisAndFuture]: 'Remove este bloqueio e todas as ocorrências futuras. Bloqueios passados serão mantidos.',
  [RecurringDeleteScope.AllInSeries]: 'Remove todos os bloqueios desta série recorrente (passados, presentes e futuros).'
};

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
