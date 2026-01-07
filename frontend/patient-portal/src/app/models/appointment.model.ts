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
  doctorName: string;
  doctorSpecialty: string;
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
