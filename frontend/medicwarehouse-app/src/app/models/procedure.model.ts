export interface Procedure {
  id: string;
  name: string;
  code: string;
  description: string;
  category: ProcedureCategory;
  price: number;
  durationMinutes: number;
  requiresMaterials: boolean;
  isActive: boolean;
  clinicId?: string;
  acceptedHealthInsurances?: string;
  allowInMedicalAttendance: boolean;
  allowInExclusiveProcedureAttendance: boolean;
  createdAt: string;
  updatedAt?: string;
}

export enum ProcedureCategory {
  Consultation = 0,
  Exam = 1,
  Surgery = 2,
  Therapy = 3,
  Vaccination = 4,
  Diagnostic = 5,
  Treatment = 6,
  Emergency = 7,
  Prevention = 8,
  Aesthetic = 9,
  FollowUp = 10,
  Other = 11
}

export interface CreateProcedure {
  name: string;
  code: string;
  description: string;
  category: ProcedureCategory;
  price: number;
  durationMinutes: number;
  requiresMaterials: boolean;
  clinicId?: string;
  acceptedHealthInsurances?: string;
  allowInMedicalAttendance: boolean;
  allowInExclusiveProcedureAttendance: boolean;
}

export interface UpdateProcedure {
  name: string;
  description: string;
  category: ProcedureCategory;
  price: number;
  durationMinutes: number;
  requiresMaterials: boolean;
  clinicId?: string;
  acceptedHealthInsurances?: string;
  allowInMedicalAttendance: boolean;
  allowInExclusiveProcedureAttendance: boolean;
}

export interface AppointmentProcedure {
  id: string;
  appointmentId: string;
  procedureId: string;
  patientId: string;
  procedureName: string;
  procedureCode: string;
  priceCharged: number;
  notes?: string;
  performedAt: string;
}

export interface AddProcedureToAppointment {
  procedureId: string;
  customPrice?: number;
  notes?: string;
}

export interface AppointmentBillingSummary {
  appointmentId: string;
  patientId: string;
  patientName: string;
  appointmentDate: string;
  procedures: AppointmentProcedure[];
  subTotal: number;
  taxAmount: number;
  total: number;
  paymentStatus?: string;
}

export const ProcedureCategoryLabels: Record<ProcedureCategory, string> = {
  [ProcedureCategory.Consultation]: 'Consulta',
  [ProcedureCategory.Exam]: 'Exame',
  [ProcedureCategory.Surgery]: 'Cirurgia',
  [ProcedureCategory.Therapy]: 'Terapia',
  [ProcedureCategory.Vaccination]: 'Vacinação',
  [ProcedureCategory.Diagnostic]: 'Diagnóstico',
  [ProcedureCategory.Treatment]: 'Tratamento',
  [ProcedureCategory.Emergency]: 'Emergência',
  [ProcedureCategory.Prevention]: 'Prevenção',
  [ProcedureCategory.Aesthetic]: 'Estética',
  [ProcedureCategory.FollowUp]: 'Retorno',
  [ProcedureCategory.Other]: 'Outros'
};
