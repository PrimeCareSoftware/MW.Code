export enum TicketStatus {
  Open = 0,
  InAnalysis = 1,
  InProgress = 2,
  Blocked = 3,
  Completed = 4,
  Cancelled = 5
}

export enum TicketType {
  BugReport = 0,
  FeatureRequest = 1,
  SystemAdjustment = 2,
  FinancialIssue = 3,
  TechnicalSupport = 4,
  UserSupport = 5,
  Other = 6
}

export enum TicketPriority {
  Low = 0,
  Medium = 1,
  High = 2,
  Critical = 3
}

export interface Ticket {
  id: string;
  title: string;
  description: string;
  type: TicketType;
  status: TicketStatus;
  priority: TicketPriority;
  userId: string;
  userName: string;
  userEmail: string;
  clinicId?: string;
  clinicName?: string;
  tenantId: string;
  assignedToId?: string;
  assignedToName?: string;
  attachments: TicketAttachment[];
  comments: TicketComment[];
  unreadUpdates: number;
  createdAt: string;
  updatedAt: string;
  lastStatusChangeAt?: string;
}

export interface TicketSummary {
  id: string;
  title: string;
  type: TicketType;
  status: TicketStatus;
  priority: TicketPriority;
  userName: string;
  clinicName?: string;
  unreadUpdates: number;
  createdAt: string;
  updatedAt: string;
}

export interface CreateTicketRequest {
  title: string;
  description: string;
  type: TicketType;
  priority?: TicketPriority;
}

export interface UpdateTicketRequest {
  title?: string;
  description?: string;
  type?: TicketType;
  priority?: TicketPriority;
}

export interface UpdateTicketStatusRequest {
  status: TicketStatus;
  comment?: string;
}

export interface AddTicketCommentRequest {
  comment: string;
  isInternal?: boolean;
}

export interface TicketComment {
  id: string;
  ticketId: string;
  comment: string;
  authorName: string;
  isInternal: boolean;
  isSystemOwner: boolean;
  createdAt: string;
}

export interface TicketAttachment {
  id: string;
  ticketId: string;
  fileName: string;
  fileUrl: string;
  contentType: string;
  fileSize: number;
  uploadedAt: string;
}

export interface UploadAttachmentRequest {
  fileName: string;
  base64Data: string;
  contentType: string;
}

export interface TicketStatistics {
  totalTickets: number;
  openTickets: number;
  inAnalysisTickets: number;
  inProgressTickets: number;
  blockedTickets: number;
  completedTickets: number;
  cancelledTickets: number;
  ticketsByType: { [key: string]: number };
  ticketsByPriority: { [key: string]: number };
  ticketsByClinic: { [key: string]: number };
  averageResolutionTimeHours: number;
}

export interface AssignTicketRequest {
  assignedToId?: string;
}

// Helper functions
export function getTicketStatusLabel(status: TicketStatus): string {
  const labels: { [key in TicketStatus]: string } = {
    [TicketStatus.Open]: 'Aberto',
    [TicketStatus.InAnalysis]: 'Em Análise',
    [TicketStatus.InProgress]: 'Em Atendimento',
    [TicketStatus.Blocked]: 'Com Impedimento',
    [TicketStatus.Completed]: 'Concluído',
    [TicketStatus.Cancelled]: 'Cancelado'
  };
  return labels[status];
}

export function getTicketTypeLabel(type: TicketType): string {
  const labels: { [key in TicketType]: string } = {
    [TicketType.BugReport]: 'Reporte de Bug',
    [TicketType.FeatureRequest]: 'Solicitação de Funcionalidade',
    [TicketType.SystemAdjustment]: 'Ajuste no Sistema',
    [TicketType.FinancialIssue]: 'Questão Financeira',
    [TicketType.TechnicalSupport]: 'Suporte Técnico',
    [TicketType.UserSupport]: 'Suporte ao Usuário',
    [TicketType.Other]: 'Outro'
  };
  return labels[type];
}

export function getTicketPriorityLabel(priority: TicketPriority): string {
  const labels: { [key in TicketPriority]: string } = {
    [TicketPriority.Low]: 'Baixa',
    [TicketPriority.Medium]: 'Média',
    [TicketPriority.High]: 'Alta',
    [TicketPriority.Critical]: 'Crítica'
  };
  return labels[priority];
}

export function getTicketStatusColor(status: TicketStatus): string {
  const colors: { [key in TicketStatus]: string } = {
    [TicketStatus.Open]: '#3b82f6',
    [TicketStatus.InAnalysis]: '#f59e0b',
    [TicketStatus.InProgress]: '#8b5cf6',
    [TicketStatus.Blocked]: '#ef4444',
    [TicketStatus.Completed]: '#10b981',
    [TicketStatus.Cancelled]: '#6b7280'
  };
  return colors[status];
}

export function getTicketPriorityColor(priority: TicketPriority): string {
  const colors: { [key in TicketPriority]: string } = {
    [TicketPriority.Low]: '#10b981',
    [TicketPriority.Medium]: '#f59e0b',
    [TicketPriority.High]: '#f97316',
    [TicketPriority.Critical]: '#ef4444'
  };
  return colors[priority];
}
