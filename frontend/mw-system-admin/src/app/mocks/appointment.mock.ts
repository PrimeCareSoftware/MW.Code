import { Appointment, DailyAgenda, AvailableSlot } from '../models/appointment.model';

export const MOCK_APPOINTMENTS: Appointment[] = [
  {
    id: '1',
    patientId: '1',
    patientName: 'João Silva',
    clinicId: 'clinic1',
    clinicName: 'Clínica Saúde Total',
    scheduledDate: '2024-11-08',
    scheduledTime: '09:00',
    durationMinutes: 30,
    type: 'Consulta',
    status: 'Agendado',
    notes: 'Consulta de rotina',
    createdAt: '2024-11-01T10:00:00Z'
  },
  {
    id: '2',
    patientId: '2',
    patientName: 'Maria Santos',
    clinicId: 'clinic1',
    clinicName: 'Clínica Saúde Total',
    scheduledDate: '2024-11-08',
    scheduledTime: '10:00',
    durationMinutes: 30,
    type: 'Retorno',
    status: 'Confirmado',
    notes: 'Revisão de exames',
    createdAt: '2024-11-02T14:30:00Z'
  },
  {
    id: '3',
    patientId: '3',
    patientName: 'Pedro Oliveira',
    clinicId: 'clinic1',
    clinicName: 'Clínica Saúde Total',
    scheduledDate: '2024-11-08',
    scheduledTime: '14:00',
    durationMinutes: 30,
    type: 'Consulta',
    status: 'Atendido',
    notes: 'Consulta pediátrica',
    checkInTime: '13:55',
    checkOutTime: '14:25',
    createdAt: '2024-11-03T09:15:00Z'
  }
];

export const MOCK_DAILY_AGENDA: DailyAgenda = {
  date: '2024-11-08',
  clinicId: 'clinic1',
  clinicName: 'Clínica Saúde Total',
  appointments: MOCK_APPOINTMENTS,
  availableSlots: ['11:00', '11:30', '15:00', '15:30', '16:00']
};

export const MOCK_AVAILABLE_SLOTS: AvailableSlot[] = [
  {
    date: '2024-11-08',
    time: '11:00',
    durationMinutes: 30,
    isAvailable: true
  },
  {
    date: '2024-11-08',
    time: '11:30',
    durationMinutes: 30,
    isAvailable: true
  },
  {
    date: '2024-11-08',
    time: '15:00',
    durationMinutes: 30,
    isAvailable: true
  },
  {
    date: '2024-11-08',
    time: '15:30',
    durationMinutes: 30,
    isAvailable: true
  },
  {
    date: '2024-11-08',
    time: '16:00',
    durationMinutes: 30,
    isAvailable: true
  }
];
