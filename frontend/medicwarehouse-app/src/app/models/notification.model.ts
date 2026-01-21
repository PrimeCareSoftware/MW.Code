export interface Notification {
  id: string;
  type: NotificationType;
  title: string;
  message: string;
  data?: any;
  isRead: boolean;
  createdAt: Date;
}

export enum NotificationType {
  AppointmentCompleted = 'AppointmentCompleted',
  PatientReady = 'PatientReady',
  CallNextPatient = 'CallNextPatient',
  AppointmentReminder = 'AppointmentReminder',
  General = 'General'
}

export interface AppointmentCompletedNotification {
  appointmentId: string;
  doctorName: string;
  patientName: string;
  completedAt: Date;
  nextPatientId?: string;
  nextPatientName?: string;
}

export interface CallNextPatientNotification {
  appointmentId: string;
  patientName: string;
  doctorName: string;
  roomNumber?: string;
}
