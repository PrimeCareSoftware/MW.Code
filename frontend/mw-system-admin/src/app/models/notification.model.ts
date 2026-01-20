export interface Notification {
  id: string;
  title: string;
  message: string;
  type: NotificationType;
  timestamp: string;
  createdAt?: string;
  isRead?: boolean;
  data?: {
    nextPatientName?: string;
    [key: string]: any;
  };
}

export enum NotificationType {
  Success = 'SUCCESS',
  Error = 'ERROR',
  Warning = 'WARNING',
  Info = 'INFO',
  AppointmentCompleted = 'APPOINTMENT_COMPLETED',
  PatientReady = 'PATIENT_READY',
  AppointmentReminder = 'APPOINTMENT_REMINDER'
}
