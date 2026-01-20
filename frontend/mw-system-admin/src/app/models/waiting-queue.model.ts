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
  Attending = 'Attending',
  Completed = 'Completed',
  NoShow = 'NoShow',
  Cancelled = 'Cancelled'
}

export enum QueueDisplayMode {
  List = 'LIST',
  Grid = 'GRID',
  Table = 'TABLE',
  Both = 'Both'
}

export interface WaitingQueueEntry {
  id: string;
  appointmentId: string;
  clinicId: string;
  patientId: string;
  patientName: string;
  position: number;
  priority: TriagePriority;
  status: QueueStatus;
  checkInTime: Date | string;
  calledTime?: Date | string;
  completedTime?: Date | string;
  triageNotes?: string;
  estimatedWaitTimeMinutes?: number;
  actualWaitTimeMinutes?: number;
  createdAt: Date | string;
  updatedAt?: Date | string;
}

export interface WaitingQueueSummary {
  clinicId: string;
  totalInQueue?: number;
  totalWaiting?: number;
  totalCalled?: number;
  totalInProgress?: number;
  totalServed?: number;
  averageWaitTimeMinutes: number;
  entries?: WaitingQueueEntry[];
}

export interface WaitingQueueConfiguration {
  id: string;
  clinicId: string;
  displayMode: QueueDisplayMode;
  showEstimatedWaitTime?: boolean;
  showPatientNames?: boolean;
  showPriority?: boolean;
  showPosition?: boolean;
  autoRefreshSeconds?: number;
  refreshIntervalSeconds?: number;
  enableSoundNotifications?: boolean;
  maxDisplayEntries?: number;
  isActive?: boolean;
}

export interface PublicQueueDisplay {
  position?: number;
  patientIdentifier?: string;
  status?: string;
  estimatedWaitTimeMinutes?: number;
  id?: string;
  displayName?: string;
  displayMode?: QueueDisplayMode;
  isActive?: boolean;
}

export interface WaitingQueueItem {
  id: string;
  patientName: string;
  arrivalTime: string;
  status: string;
}
