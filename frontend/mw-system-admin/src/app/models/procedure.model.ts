export enum ProcedureCategory {
  Consultation = 'Consultation',
  Exam = 'Exam',
  Diagnostic = 'Diagnostic',
  Procedure = 'Procedure',
  Surgery = 'Surgery',
  Vaccination = 'Vaccination'
}

export interface Procedure {
  id: string;
  name: string;
  code: string;
  description: string;
  category?: ProcedureCategory | string;
  price: number;
  durationMinutes?: number;
  requiresMaterials?: boolean;
  isActive?: boolean;
  createdAt?: string;
}

export interface AppointmentProcedure {
  id: string;
  appointmentId: string;
  procedureId: string;
  patientId?: string;
  procedureName: string;
  procedureCode?: string;
  quantity?: number;
  priceCharged?: number;
  price?: number;
  totalPrice?: number;
  notes?: string;
  performedAt?: string;
}

export interface AppointmentBillingSummary {
  appointmentId: string;
  patientId?: string;
  patientName?: string;
  appointmentDate?: string;
  procedures: AppointmentProcedure[];
  subTotal?: number;
  taxAmount?: number;
  total?: number;
  totalPrice?: number;
  paymentStatus?: string;
  status?: string;
}
