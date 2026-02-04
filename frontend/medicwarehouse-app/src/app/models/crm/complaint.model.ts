import { ComplaintCategory, ComplaintPriority, ComplaintStatus } from './crm-enums';

export interface Complaint {
  id: string;
  protocolNumber: string;
  patientId: string;
  patientName: string;
  subject: string;
  description: string;
  category: ComplaintCategory;
  categoryName: string;
  priority: ComplaintPriority;
  priorityName: string;
  status: ComplaintStatus;
  statusName: string;
  assignedToUserId?: string;
  assignedToUserName?: string;
  receivedAt: Date;
  firstResponseAt?: Date;
  resolvedAt?: Date;
  closedAt?: Date;
  responseTimeMinutes?: number;
  resolutionTimeMinutes?: number;
  satisfactionRating?: number;
  satisfactionFeedback?: string;
  interactions: ComplaintInteraction[];
  createdAt: Date;
  updatedAt: Date;
}

export interface ComplaintInteraction {
  id: string;
  complaintId: string;
  userId: string;
  userName: string;
  message: string;
  isInternal: boolean;
  interactionDate: Date;
}

export interface CreateComplaint {
  patientId: string;
  subject: string;
  description: string;
  category: ComplaintCategory;
  priority?: ComplaintPriority;
}

export interface UpdateComplaint {
  subject?: string;
  description?: string;
  category?: ComplaintCategory;
  priority?: ComplaintPriority;
}

export interface AddComplaintInteraction {
  message: string;
  isInternal: boolean;
}

export interface UpdateComplaintStatus {
  status: ComplaintStatus;
}

export interface AssignComplaint {
  userId: string;
  userName: string;
}

export interface ComplaintDashboard {
  totalComplaints: number;
  openComplaints: number;
  inProgressComplaints: number;
  resolvedComplaints: number;
  closedComplaints: number;
  complaintsByCategory: Record<ComplaintCategory, number>;
  complaintsByPriority: Record<ComplaintPriority, number>;
  complaintsByStatus: Record<ComplaintStatus, number>;
  averageResponseTimeMinutes: number;
  averageResolutionTimeMinutes: number;
  complaintsWithinSLA: number;
  complaintsOutsideSLA: number;
  slaComplianceRate: number;
  averageSatisfactionRating: number;
  totalRatings: number;
  recentComplaints: Complaint[];
  urgentComplaints: Complaint[];
}
