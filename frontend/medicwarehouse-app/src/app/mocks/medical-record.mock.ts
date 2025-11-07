import { MedicalRecord } from '../models/medical-record.model';

export const MOCK_MEDICAL_RECORDS: MedicalRecord[] = [
  {
    id: '1',
    appointmentId: '1',
    patientId: '1',
    patientName: 'João Silva',
    diagnosis: 'Hipertensão arterial controlada',
    prescription: 'Losartana 50mg - 1 comprimido pela manhã\nHidrocloriatiazida 25mg - 1 comprimido pela manhã',
    notes: 'Paciente apresenta bom controle da pressão arterial. Orientado a manter dieta hipossódica e prática regular de exercícios.',
    consultationDurationMinutes: 30,
    consultationStartTime: '2024-11-08T09:00:00Z',
    consultationEndTime: '2024-11-08T09:30:00Z',
    createdAt: '2024-11-08T09:30:00Z'
  },
  {
    id: '2',
    appointmentId: '2',
    patientId: '2',
    patientName: 'Maria Santos',
    diagnosis: 'Diabetes Mellitus tipo 2 em tratamento',
    prescription: 'Metformina 850mg - 1 comprimido 2x ao dia\nGlibenclamida 5mg - 1 comprimido pela manhã',
    notes: 'Glicemia em níveis adequados. Paciente aderente ao tratamento. Solicitado hemoglobina glicada para controle.',
    consultationDurationMinutes: 35,
    consultationStartTime: '2024-11-08T10:00:00Z',
    consultationEndTime: '2024-11-08T10:35:00Z',
    createdAt: '2024-11-08T10:35:00Z'
  },
  {
    id: '3',
    appointmentId: '3',
    patientId: '3',
    patientName: 'Pedro Oliveira',
    diagnosis: 'Resfriado comum',
    prescription: 'Paracetamol 500mg - 1 comprimido de 6/6h se febre\nLoratadina 10mg - 1 comprimido à noite se necessário',
    notes: 'Criança com quadro de resfriado leve. Orientações sobre hidratação e repouso. Retornar se sintomas piorarem.',
    consultationDurationMinutes: 25,
    consultationStartTime: '2024-11-08T14:00:00Z',
    consultationEndTime: '2024-11-08T14:25:00Z',
    createdAt: '2024-11-08T14:25:00Z'
  }
];
