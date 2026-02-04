import { AutomationTriggerType, JourneyStageEnum, ActionType } from './crm-enums';

export interface MarketingAutomation {
  id: string;
  name: string;
  description: string;
  isActive: boolean;
  triggerType: AutomationTriggerType;
  triggerStage?: JourneyStageEnum;
  triggerEvent?: string;
  delayMinutes?: number;
  segmentFilter?: string;
  tags: string[];
  actions: AutomationAction[];
  timesExecuted: number;
  lastExecutedAt?: Date;
  successRate: number;
  createdAt: Date;
  updatedAt: Date;
}

export interface AutomationAction {
  id: string;
  order: number;
  type: ActionType;
  emailTemplateId?: string;
  emailTemplateName?: string;
  messageTemplate?: string;
  channel?: string;
  tagToAdd?: string;
  tagToRemove?: string;
  scoreChange?: number;
  condition?: string;
}

export interface CreateMarketingAutomation {
  name: string;
  description: string;
  triggerType: AutomationTriggerType;
  triggerStage?: JourneyStageEnum;
  triggerEvent?: string;
  delayMinutes?: number;
  segmentFilter?: string;
  tags: string[];
  actions: CreateAutomationAction[];
}

export interface CreateAutomationAction {
  order: number;
  type: ActionType;
  emailTemplateId?: string;
  messageTemplate?: string;
  channel?: string;
  tagToAdd?: string;
  tagToRemove?: string;
  scoreChange?: number;
  condition?: string;
}

export interface UpdateMarketingAutomation {
  name?: string;
  description?: string;
  triggerType?: AutomationTriggerType;
  triggerStage?: JourneyStageEnum;
  triggerEvent?: string;
  delayMinutes?: number;
  segmentFilter?: string;
  tags?: string[];
  actions?: CreateAutomationAction[];
}

export interface MarketingAutomationMetrics {
  automationId: string;
  name: string;
  timesExecuted: number;
  successfulExecutions: number;
  failedExecutions: number;
  successRate: number;
  lastExecutedAt?: Date;
  firstExecutedAt?: Date;
  totalPatientsReached: number;
}
