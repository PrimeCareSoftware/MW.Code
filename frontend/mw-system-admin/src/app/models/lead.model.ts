export interface Lead {
  id: string;
  sessionId: string;
  companyName?: string;
  contactName?: string;
  email?: string;
  phone?: string;
  city?: string;
  state?: string;
  planId?: string;
  planName?: string;
  lastStepReached: number;
  leadSource: string;
  status: LeadStatus;
  referrer?: string;
  utmCampaign?: string;
  utmSource?: string;
  utmMedium?: string;
  capturedAt: Date;
  lastActivityAt?: Date;
  assignedToUserId?: string;
  assignedAt?: Date;
  nextFollowUpDate?: Date;
  score: number;
  tags?: string;
  notes?: string;
  metadata?: string;
  createdAt: Date;
  updatedAt?: Date;
  activities?: LeadActivity[];
}

export interface LeadActivity {
  id: string;
  leadId: string;
  type: ActivityType;
  title: string;
  description?: string;
  performedByUserId?: string;
  performedByUserName?: string;
  activityDate: Date;
  durationMinutes?: number;
  outcome?: string;
  createdAt: Date;
}

export enum LeadStatus {
  New = 0,
  Contacted = 1,
  Qualified = 2,
  Converted = 3,
  Lost = 4,
  Nurturing = 5
}

export enum ActivityType {
  PhoneCall = 0,
  Email = 1,
  Meeting = 2,
  Note = 3,
  StatusChange = 4,
  Assignment = 5,
  FollowUpScheduled = 6,
  Other = 99
}

export interface LeadStatistics {
  totalLeads: number;
  newLeads: number;
  contactedLeads: number;
  qualifiedLeads: number;
  convertedLeads: number;
  lostLeads: number;
  nurturingLeads: number;
  unassignedLeads: number;
  needingFollowUp: number;
  conversionRate: number;
  averageScore: number;
}

export interface UserLeadStatistics {
  userId: string;
  userName?: string;
  assignedLeads: number;
  convertedLeads: number;
  needingFollowUp: number;
  conversionRate: number;
}

export interface AssignLeadRequest {
  userId: string;
}

export interface UpdateLeadStatusRequest {
  status: LeadStatus;
  notes?: string;
}

export interface ScheduleFollowUpRequest {
  followUpDate: Date;
}

export interface AddActivityRequest {
  type: ActivityType;
  title: string;
  description?: string;
  durationMinutes?: number;
  outcome?: string;
}

export interface UpdateContactInfoRequest {
  contactName?: string;
  email?: string;
  phone?: string;
}

export interface AddNotesRequest {
  notes: string;
}

export interface SetTagsRequest {
  tags?: string;
}

// Helper functions
export function getLeadStatusLabel(status: LeadStatus): string {
  switch (status) {
    case LeadStatus.New:
      return 'Novo';
    case LeadStatus.Contacted:
      return 'Contactado';
    case LeadStatus.Qualified:
      return 'Qualificado';
    case LeadStatus.Converted:
      return 'Convertido';
    case LeadStatus.Lost:
      return 'Perdido';
    case LeadStatus.Nurturing:
      return 'Nutrição';
    default:
      return 'Desconhecido';
  }
}

export function getActivityTypeLabel(type: ActivityType): string {
  switch (type) {
    case ActivityType.PhoneCall:
      return 'Ligação';
    case ActivityType.Email:
      return 'Email';
    case ActivityType.Meeting:
      return 'Reunião';
    case ActivityType.Note:
      return 'Nota';
    case ActivityType.StatusChange:
      return 'Mudança de Status';
    case ActivityType.Assignment:
      return 'Atribuição';
    case ActivityType.FollowUpScheduled:
      return 'Follow-up Agendado';
    case ActivityType.Other:
      return 'Outro';
    default:
      return 'Desconhecido';
  }
}

export function getLeadScoreColor(score: number): string {
  if (score >= 80) return 'success';
  if (score >= 60) return 'primary';
  if (score >= 40) return 'warning';
  return 'danger';
}

export function getLeadStatusColor(status: LeadStatus): string {
  switch (status) {
    case LeadStatus.New:
      return 'primary';
    case LeadStatus.Contacted:
      return 'info';
    case LeadStatus.Qualified:
      return 'warning';
    case LeadStatus.Converted:
      return 'success';
    case LeadStatus.Lost:
      return 'danger';
    case LeadStatus.Nurturing:
      return 'secondary';
    default:
      return 'light';
  }
}
