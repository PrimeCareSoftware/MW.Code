import { MedicalRecord } from '../models/medical-record.model';

export const MOCK_MEDICAL_RECORDS: MedicalRecord[] = [
  {
    id: '1',
    appointmentId: '1',
    patientId: '1',
    patientName: 'João Silva',
    // CFM 1.821 Required Fields
    chiefComplaint: 'Dor de cabeça e pressão alta',
    historyOfPresentIllness: 'Paciente relata cefaleia há 3 dias, principalmente pela manhã. Verificou pressão arterial em casa e estava elevada (150/95 mmHg). Nega outros sintomas.',
    isClosed: true,
    // Legacy fields
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
    // CFM 1.821 Required Fields
    chiefComplaint: 'Retorno para controle de diabetes',
    historyOfPresentIllness: 'Paciente em acompanhamento de Diabetes Mellitus tipo 2. Relata adesão ao tratamento medicamentoso e dieta. Glicemias capilares em jejum variando entre 110-130 mg/dL.',
    isClosed: true,
    // Legacy fields
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
    // CFM 1.821 Required Fields
    chiefComplaint: 'Tosse e coriza há 2 dias',
    historyOfPresentIllness: 'Criança de 7 anos com quadro de coriza hialina e tosse seca há 2 dias. Sem febre. Mantém boa aceitação alimentar e hidratação. Outros membros da família com sintomas gripais.',
    isClosed: true,
    // Legacy fields
    diagnosis: 'Resfriado comum',
    prescription: 'Paracetamol 500mg - 1 comprimido de 6/6h se febre\nLoratadina 10mg - 1 comprimido à noite se necessário',
    notes: 'Criança com quadro de resfriado leve. Orientações sobre hidratação e repouso. Retornar se sintomas piorarem.',
    consultationDurationMinutes: 25,
    consultationStartTime: '2024-11-08T14:00:00Z',
    consultationEndTime: '2024-11-08T14:25:00Z',
    createdAt: '2024-11-08T14:25:00Z'
  }
];
