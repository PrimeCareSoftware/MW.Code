export interface WorkflowDto {
  id: number;
  name: string;
  description?: string;
  isEnabled: boolean;
  triggerType: string;
  triggerConfig?: string;
  stopOnError: boolean;
  createdAt: Date;
  createdBy: string;
  updatedAt?: Date;
  updatedBy?: string;
  actions?: WorkflowActionDto[];
}

export interface WorkflowActionDto {
  id: number;
  order: number;
  actionType: string;
  config: string;
  condition?: string;
  delaySeconds?: number;
}

export interface CreateWorkflowDto {
  name: string;
  description?: string;
  isEnabled: boolean;
  triggerType: string;
  triggerConfig?: string;
  stopOnError: boolean;
  actions?: CreateWorkflowActionDto[];
}

export interface CreateWorkflowActionDto {
  order: number;
  actionType: string;
  config: string;
  condition?: string;
  delaySeconds?: number;
}

export interface UpdateWorkflowDto {
  name: string;
  description?: string;
  isEnabled: boolean;
  triggerType: string;
  triggerConfig?: string;
  stopOnError: boolean;
  actions?: CreateWorkflowActionDto[];
}

export interface WorkflowExecution {
  id: number;
  workflowId: number;
  workflow?: WorkflowDto;
  status: 'pending' | 'running' | 'completed' | 'failed';
  triggerData?: string;
  startedAt: Date;
  completedAt?: Date;
  error?: string;
  actionExecutions?: WorkflowActionExecution[];
}

export interface WorkflowActionExecution {
  id: number;
  workflowActionId: number;
  workflowAction?: WorkflowActionDto;
  startedAt: Date;
  completedAt?: Date;
  status: 'pending' | 'running' | 'completed' | 'failed';
  error?: string;
}

export interface WorkflowExecutionResponse {
  data: WorkflowExecution[];
  page: number;
  pageSize: number;
  totalCount: number;
}

// Trigger types
export const TriggerTypes = {
  TIME: 'time',
  EVENT: 'event',
  MANUAL: 'manual'
} as const;

// Action types
export const ActionTypes = {
  EMAIL: 'email',
  SMS: 'sms',
  WEBHOOK: 'webhook',
  TAG: 'tag',
  TICKET: 'ticket',
  SLACK: 'slack',
  UPDATE_FIELD: 'update_field'
} as const;

// Event types for workflows
export const EventTypes = {
  CLINIC_CREATED: 'clinic.created',
  CLINIC_SUSPENDED: 'clinic.suspended',
  SUBSCRIPTION_EXPIRED: 'subscription.expired',
  PAYMENT_FAILED: 'payment.failed',
  TRIAL_ENDING: 'trial.ending',
  HEALTH_SCORE_LOW: 'health_score.low'
} as const;
