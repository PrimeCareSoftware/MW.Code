import { Procedure, ProcedureCategory, AppointmentProcedure, AppointmentBillingSummary } from '../models/procedure.model';

export const MOCK_PROCEDURES: Procedure[] = [
  {
    id: '1',
    name: 'Consulta Clínica Geral',
    code: 'CONS-001',
    description: 'Consulta médica com clínico geral',
    category: ProcedureCategory.Consultation,
    price: 150.00,
    durationMinutes: 30,
    requiresMaterials: false,
    isActive: true,
    allowInMedicalAttendance: true,
    allowInExclusiveProcedureAttendance: false,
    createdAt: '2024-01-01T10:00:00Z'
  },
  {
    id: '2',
    name: 'Hemograma Completo',
    code: 'EXAM-001',
    description: 'Exame laboratorial de sangue completo',
    category: ProcedureCategory.Exam,
    price: 80.00,
    durationMinutes: 15,
    requiresMaterials: true,
    isActive: true,
    allowInMedicalAttendance: true,
    allowInExclusiveProcedureAttendance: true,
    createdAt: '2024-01-01T10:00:00Z'
  },
  {
    id: '3',
    name: 'Eletrocardiograma',
    code: 'EXAM-002',
    description: 'Exame de eletrocardiograma (ECG)',
    category: ProcedureCategory.Diagnostic,
    price: 100.00,
    durationMinutes: 20,
    requiresMaterials: true,
    isActive: true,
    allowInMedicalAttendance: true,
    allowInExclusiveProcedureAttendance: true,
    createdAt: '2024-01-01T10:00:00Z'
  },
  {
    id: '4',
    name: 'Vacinação Gripe',
    code: 'VAC-001',
    description: 'Vacina contra gripe',
    category: ProcedureCategory.Vaccination,
    price: 50.00,
    durationMinutes: 10,
    requiresMaterials: true,
    isActive: true,
    allowInMedicalAttendance: true,
    allowInExclusiveProcedureAttendance: false,
    createdAt: '2024-01-01T10:00:00Z'
  }
];

export const MOCK_APPOINTMENT_PROCEDURES: AppointmentProcedure[] = [
  {
    id: '1',
    appointmentId: '1',
    procedureId: '1',
    patientId: '1',
    procedureName: 'Consulta Clínica Geral',
    procedureCode: 'CONS-001',
    priceCharged: 150.00,
    notes: 'Consulta de rotina',
    performedAt: '2024-11-08T09:00:00Z'
  }
];

export const MOCK_BILLING_SUMMARY: AppointmentBillingSummary = {
  appointmentId: '1',
  patientId: '1',
  patientName: 'João Silva',
  appointmentDate: '2024-11-08',
  procedures: MOCK_APPOINTMENT_PROCEDURES,
  subTotal: 150.00,
  taxAmount: 0.00,
  total: 150.00,
  paymentStatus: 'Pendente'
};
