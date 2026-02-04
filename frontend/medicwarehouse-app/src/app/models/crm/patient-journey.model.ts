import { JourneyStageEnum, ChurnRiskLevel, TouchpointType, TouchpointDirection } from './crm-enums';

export interface PatientJourney {
  id: string;
  pacienteId: string;
  pacienteNome: string;
  currentStage: JourneyStageEnum;
  currentStageName: string;
  totalTouchpoints: number;
  lifetimeValue: number;
  npsScore: number;
  satisfactionScore: number;
  churnRisk: ChurnRiskLevel;
  churnRiskName: string;
  tags: string[];
  engagementScore: number;
  stages: JourneyStage[];
  createdAt: Date;
  updatedAt: Date;
}

export interface JourneyStage {
  id: string;
  stage: JourneyStageEnum;
  stageName: string;
  enteredAt: Date;
  exitedAt?: Date;
  durationDays: number;
  exitTrigger?: string;
  touchpoints: PatientTouchpoint[];
}

export interface PatientTouchpoint {
  id: string;
  timestamp: Date;
  type: TouchpointType;
  typeName: string;
  channel: string;
  description: string;
  direction: TouchpointDirection;
  directionName: string;
  sentimentAnalysisId?: string;
}

export interface CreatePatientTouchpoint {
  type: TouchpointType;
  channel: string;
  description: string;
  direction: TouchpointDirection;
}

export interface UpdatePatientJourneyMetrics {
  lifetimeValue?: number;
  npsScore?: number;
  satisfactionScore?: number;
  churnRisk?: ChurnRiskLevel;
}

export interface AdvanceJourneyStage {
  newStage: JourneyStageEnum;
  trigger: string;
}

export interface PatientJourneyMetrics {
  pacienteId: string;
  pacienteNome: string;
  lifetimeValue: number;
  npsScore: number;
  satisfactionScore: number;
  churnRisk: ChurnRiskLevel;
  engagementScore: number;
  currentStage: JourneyStageEnum;
  totalTouchpoints: number;
  daysInCurrentStage: number;
  totalDaysInJourney: number;
  touchpointsByChannel: Record<string, number>;
  touchpointsByType: Record<string, number>;
}
