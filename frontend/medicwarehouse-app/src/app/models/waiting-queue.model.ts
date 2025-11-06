export interface WaitingQueueEntry {
  id: string;
  appointmentId: string;
  clinicId: string;
  patientId: string;
  patientName: string;
  position: number;
  priority: TriagePriority;
  status: QueueStatus;
  checkInTime: Date;
  calledTime?: Date;
  completedTime?: Date;
  triageNotes?: string;
  estimatedWaitTimeMinutes: number;
  actualWaitTimeMinutes: number;
  createdAt: Date;
  updatedAt?: Date;
}

export interface CreateWaitingQueueEntry {
  appointmentId: string;
  clinicId: string;
  patientId: string;
  priority?: TriagePriority;
  triageNotes?: string;
}

export interface UpdateQueueTriage {
  priority: TriagePriority;
  triageNotes?: string;
}

export interface WaitingQueueSummary {
  clinicId: string;
  totalWaiting: number;
  totalCalled: number;
  totalInProgress: number;
  averageWaitTimeMinutes: number;
  entries: WaitingQueueEntry[];
}

export interface PublicQueueDisplay {
  position: number;
  patientIdentifier: string;
  status: string;
  estimatedWaitTimeMinutes?: number;
}

export interface WaitingQueueConfiguration {
  id: string;
  clinicId: string;
  displayMode: QueueDisplayMode;
  showEstimatedWaitTime: boolean;
  showPatientNames: boolean;
  showPriority: boolean;
  showPosition: boolean;
  autoRefreshSeconds: number;
  enableSoundNotifications: boolean;
}

export interface UpdateWaitingQueueConfiguration {
  displayMode: QueueDisplayMode;
  showEstimatedWaitTime: boolean;
  showPatientNames: boolean;
  showPriority: boolean;
  showPosition: boolean;
  autoRefreshSeconds: number;
  enableSoundNotifications: boolean;
}

export enum TriagePriority {
  Low = 'Low',
  Normal = 'Normal',
  High = 'High',
  Urgent = 'Urgent',
  Emergency = 'Emergency'
}

export enum QueueStatus {
  Waiting = 'Waiting',
  Called = 'Called',
  InProgress = 'InProgress',
  Completed = 'Completed',
  Cancelled = 'Cancelled'
}

export enum QueueDisplayMode {
  InternalOnly = 'InternalOnly',
  PublicDisplay = 'PublicDisplay',
  Both = 'Both'
}

export const TRIAGE_PRIORITY_LABELS: Record<TriagePriority, string> = {
  [TriagePriority.Low]: 'Baixa',
  [TriagePriority.Normal]: 'Normal',
  [TriagePriority.High]: 'Alta',
  [TriagePriority.Urgent]: 'Urgente',
  [TriagePriority.Emergency]: 'Emergência'
};

export const QUEUE_STATUS_LABELS: Record<QueueStatus, string> = {
  [QueueStatus.Waiting]: 'Aguardando',
  [QueueStatus.Called]: 'Chamado',
  [QueueStatus.InProgress]: 'Em Atendimento',
  [QueueStatus.Completed]: 'Concluído',
  [QueueStatus.Cancelled]: 'Cancelado'
};

export const DISPLAY_MODE_LABELS: Record<QueueDisplayMode, string> = {
  [QueueDisplayMode.InternalOnly]: 'Apenas Interno',
  [QueueDisplayMode.PublicDisplay]: 'Exibição Pública',
  [QueueDisplayMode.Both]: 'Ambos'
};
