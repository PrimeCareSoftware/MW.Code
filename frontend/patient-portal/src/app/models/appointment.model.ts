export enum AppointmentStatus {
  Scheduled = 'Scheduled',
  Confirmed = 'Confirmed',
  InProgress = 'InProgress',
  Completed = 'Completed',
  Cancelled = 'Cancelled',
  NoShow = 'NoShow'
}

export interface Appointment {
  id: string;
  doctorId: string;
  doctorName: string;
  doctorSpecialty: string;
  clinicId: string;
  clinicName: string;
  appointmentDate: Date;
  startTime: string;
  endTime: string;
  status: string;
  appointmentType: string;
  isTelehealth: boolean;
  telehealthLink?: string;
  notes?: string;
  canReschedule: boolean;
  canCancel: boolean;
}

export interface Specialty {
  id: string;
  name: string;
  description?: string;
}

export interface Doctor {
  id: string;
  name: string;
  specialty: string;
  crm: string;
  photoUrl?: string;
  rating?: number;
  availableForOnlineBooking: boolean;
}

export interface TimeSlot {
  startTime: string;
  endTime: string;
  isAvailable: boolean;
}

export interface AvailableSlotsResponse {
  date: string;
  slots: TimeSlot[];
}

export interface BookAppointmentRequest {
  doctorId: string;
  clinicId: string;
  scheduledDate: string;
  startTime: string;
  durationMinutes?: number;
  reason?: string;
  appointmentType?: number;
  appointmentMode?: number;
  isTelehealth?: boolean;
}

export interface BookAppointmentResponse {
  id: string;
  doctorName: string;
  scheduledDate: string;
  startTime: string;
  status: string;
  message: string;
}

export interface CancelAppointmentRequest {
  reason: string;
}

export interface RescheduleAppointmentRequest {
  newDate: string;
  newTime: string;
  reason?: string;
}
