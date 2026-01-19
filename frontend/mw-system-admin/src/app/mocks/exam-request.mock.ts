import { ExamRequest, ExamType, ExamUrgency, ExamRequestStatus } from '../models/exam-request.model';

export const MOCK_EXAM_REQUESTS: ExamRequest[] = [
  {
    id: '1',
    appointmentId: '1',
    patientId: '1',
    patientName: 'João Silva',
    examType: ExamType.Laboratory,
    examName: 'Hemograma Completo',
    description: 'Exame de sangue completo para check-up',
    urgency: ExamUrgency.Routine,
    status: ExamRequestStatus.Pending,
    requestedDate: '2024-11-08',
    notes: 'Paciente deve estar em jejum de 8 horas',
    createdAt: '2024-11-08T09:30:00Z'
  },
  {
    id: '2',
    appointmentId: '2',
    patientId: '2',
    patientName: 'Maria Santos',
    examType: ExamType.Imaging,
    examName: 'Raio-X de Tórax',
    description: 'Radiografia do tórax',
    urgency: ExamUrgency.Urgent,
    status: ExamRequestStatus.Scheduled,
    requestedDate: '2024-11-08',
    scheduledDate: '2024-11-09',
    notes: 'Suspeita de pneumonia',
    createdAt: '2024-11-08T10:30:00Z'
  },
  {
    id: '3',
    appointmentId: '3',
    patientId: '3',
    patientName: 'Pedro Oliveira',
    examType: ExamType.Cardiac,
    examName: 'Eletrocardiograma',
    description: 'ECG de rotina',
    urgency: ExamUrgency.Routine,
    status: ExamRequestStatus.Completed,
    requestedDate: '2024-11-07',
    scheduledDate: '2024-11-08',
    completedDate: '2024-11-08',
    results: 'ECG normal, sem alterações',
    notes: 'Exame preventivo',
    createdAt: '2024-11-07T14:00:00Z',
    updatedAt: '2024-11-08T14:30:00Z'
  }
];

export const MOCK_PENDING_EXAMS = MOCK_EXAM_REQUESTS.filter(
  exam => exam.status === ExamRequestStatus.Pending
);

export const MOCK_URGENT_EXAMS = MOCK_EXAM_REQUESTS.filter(
  exam => exam.urgency === ExamUrgency.Urgent || exam.urgency === ExamUrgency.Emergency
);
