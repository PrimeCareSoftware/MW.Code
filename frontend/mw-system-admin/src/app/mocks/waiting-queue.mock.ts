import { WaitingQueueEntry, WaitingQueueSummary, WaitingQueueConfiguration, TriagePriority, QueueStatus, QueueDisplayMode, PublicQueueDisplay } from '../models/waiting-queue.model';

export const MOCK_QUEUE_ENTRIES: WaitingQueueEntry[] = [
  {
    id: '1',
    appointmentId: '1',
    clinicId: 'clinic1',
    patientId: '1',
    patientName: 'João Silva',
    position: 1,
    priority: TriagePriority.Normal,
    status: QueueStatus.Waiting,
    checkInTime: new Date('2024-11-08T08:55:00Z'),
    triageNotes: 'Consulta de rotina',
    estimatedWaitTimeMinutes: 15,
    actualWaitTimeMinutes: 0,
    createdAt: new Date('2024-11-08T08:55:00Z')
  },
  {
    id: '2',
    appointmentId: '2',
    clinicId: 'clinic1',
    patientId: '2',
    patientName: 'Maria Santos',
    position: 2,
    priority: TriagePriority.High,
    status: QueueStatus.Called,
    checkInTime: new Date('2024-11-08T09:50:00Z'),
    calledTime: new Date('2024-11-08T09:58:00Z'),
    triageNotes: 'Paciente com sintomas de hiperglicemia',
    estimatedWaitTimeMinutes: 30,
    actualWaitTimeMinutes: 8,
    createdAt: new Date('2024-11-08T09:50:00Z'),
    updatedAt: new Date('2024-11-08T09:58:00Z')
  },
  {
    id: '3',
    appointmentId: '3',
    clinicId: 'clinic1',
    patientId: '3',
    patientName: 'Pedro Oliveira',
    position: 3,
    priority: TriagePriority.Normal,
    status: QueueStatus.Completed,
    checkInTime: new Date('2024-11-08T13:55:00Z'),
    calledTime: new Date('2024-11-08T14:00:00Z'),
    completedTime: new Date('2024-11-08T14:25:00Z'),
    triageNotes: 'Pediatria - resfriado',
    estimatedWaitTimeMinutes: 5,
    actualWaitTimeMinutes: 30,
    createdAt: new Date('2024-11-08T13:55:00Z'),
    updatedAt: new Date('2024-11-08T14:25:00Z')
  }
];

export const MOCK_QUEUE_SUMMARY: WaitingQueueSummary = {
  clinicId: 'clinic1',
  totalWaiting: 1,
  totalCalled: 1,
  totalInProgress: 0,
  averageWaitTimeMinutes: 14,
  entries: MOCK_QUEUE_ENTRIES
};

export const MOCK_QUEUE_CONFIGURATION: WaitingQueueConfiguration = {
  id: '1',
  clinicId: 'clinic1',
  displayMode: QueueDisplayMode.Both,
  showEstimatedWaitTime: true,
  showPatientNames: true,
  showPriority: true,
  showPosition: true,
  autoRefreshSeconds: 30,
  enableSoundNotifications: true
};

export const MOCK_PUBLIC_QUEUE_DISPLAY: PublicQueueDisplay[] = [
  {
    position: 1,
    patientIdentifier: 'João S.',
    status: 'Aguardando',
    estimatedWaitTimeMinutes: 15
  },
  {
    position: 2,
    patientIdentifier: 'Maria S.',
    status: 'Chamado',
    estimatedWaitTimeMinutes: 0
  }
];
