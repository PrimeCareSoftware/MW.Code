import { SurveyType, QuestionType, JourneyStageEnum } from './crm-enums';

export interface Survey {
  id: string;
  name: string;
  description: string;
  type: SurveyType;
  typeName: string;
  isActive: boolean;
  triggerStage?: JourneyStageEnum;
  triggerEvent?: string;
  delayHours?: number;
  averageScore: number;
  totalResponses: number;
  responseRate: number;
  questions: SurveyQuestion[];
  createdAt: Date;
  updatedAt: Date;
}

export interface SurveyQuestion {
  id: string;
  order: number;
  text: string;
  type: QuestionType;
  typeName: string;
  isRequired: boolean;
  options: string[];
}

export interface SurveyResponse {
  id: string;
  surveyId: string;
  surveyName: string;
  patientId: string;
  patientName: string;
  startedAt: Date;
  completedAt?: Date;
  isCompleted: boolean;
  npsScore?: number;
  csatScore?: number;
  questionResponses: SurveyQuestionResponse[];
}

export interface SurveyQuestionResponse {
  id: string;
  questionId: string;
  questionText: string;
  textAnswer?: string;
  numericAnswer?: number;
  booleanAnswer?: boolean;
}

export interface CreateSurvey {
  name: string;
  description: string;
  type: SurveyType;
  triggerStage?: JourneyStageEnum;
  triggerEvent?: string;
  delayHours?: number;
  questions: CreateSurveyQuestion[];
}

export interface CreateSurveyQuestion {
  order: number;
  text: string;
  type: QuestionType;
  isRequired: boolean;
  options: string[];
}

export interface UpdateSurvey {
  name?: string;
  description?: string;
  triggerStage?: JourneyStageEnum;
  triggerEvent?: string;
  delayHours?: number;
  questions?: CreateSurveyQuestion[];
}

export interface SubmitSurveyResponse {
  surveyId: string;
  patientId: string;
  responses: SubmitQuestionResponse[];
}

export interface SubmitQuestionResponse {
  questionId: string;
  textAnswer?: string;
  numericAnswer?: number;
  booleanAnswer?: boolean;
}

export interface SurveyAnalytics {
  surveyId: string;
  surveyName: string;
  type: SurveyType;
  totalSent: number;
  totalResponses: number;
  responseRate: number;
  averageScore: number;
  promoters?: number;
  passives?: number;
  detractors?: number;
  npsScore?: number;
  verySatisfied?: number;
  satisfied?: number;
  neutral?: number;
  unsatisfied?: number;
  veryUnsatisfied?: number;
  recentResponses: SurveyResponse[];
}
