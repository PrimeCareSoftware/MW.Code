// CRM Enumerations
export enum SurveyType {
  NPS = 'NPS',
  CSAT = 'CSAT',
  CES = 'CES',
  Custom = 'Custom'
}

export enum QuestionType {
  Text = 'Text',
  NumericScale = 'NumericScale',
  MultipleChoice = 'MultipleChoice',
  Boolean = 'Boolean'
}

export enum JourneyStageEnum {
  Descoberta = 'Descoberta',
  Consideracao = 'Consideracao',
  PrimeiraConsulta = 'PrimeiraConsulta',
  Tratamento = 'Tratamento',
  Retorno = 'Retorno',
  Fidelizacao = 'Fidelizacao',
  Advocacia = 'Advocacia'
}

export enum ComplaintCategory {
  WaitTime = 'WaitTime',
  ServiceQuality = 'ServiceQuality',
  MedicalCare = 'MedicalCare',
  Billing = 'Billing',
  Facilities = 'Facilities',
  Staff = 'Staff',
  Other = 'Other'
}

export enum ComplaintPriority {
  Low = 'Low',
  Medium = 'Medium',
  High = 'High',
  Critical = 'Critical'
}

export enum ComplaintStatus {
  Received = 'Received',
  InProgress = 'InProgress',
  Resolved = 'Resolved',
  Closed = 'Closed'
}

export enum TouchpointType {
  EmailInteraction = 'EmailInteraction',
  PhoneCall = 'PhoneCall',
  InPersonVisit = 'InPersonVisit',
  WhatsAppMessage = 'WhatsAppMessage',
  SMSMessage = 'SMSMessage',
  AppointmentScheduled = 'AppointmentScheduled',
  AppointmentCompleted = 'AppointmentCompleted',
  AppointmentCancelled = 'AppointmentCancelled',
  AppointmentNoShow = 'AppointmentNoShow',
  SurveyResponse = 'SurveyResponse',
  ComplaintFiled = 'ComplaintFiled',
  PaymentReceived = 'PaymentReceived',
  PaymentOverdue = 'PaymentOverdue',
  Other = 'Other'
}

export enum TouchpointDirection {
  Inbound = 'Inbound',
  Outbound = 'Outbound'
}

export enum ChurnRiskLevel {
  Low = 'Low',
  Medium = 'Medium',
  High = 'High',
  Critical = 'Critical'
}

export enum ActionType {
  SendEmail = 'SendEmail',
  SendSMS = 'SendSMS',
  SendWhatsApp = 'SendWhatsApp',
  AddTag = 'AddTag',
  RemoveTag = 'RemoveTag',
  ChangeScore = 'ChangeScore',
  AssignToUser = 'AssignToUser',
  CreateTask = 'CreateTask',
  TriggerWebhook = 'TriggerWebhook'
}

export enum AutomationTriggerType {
  StageChange = 'StageChange',
  Event = 'Event',
  Schedule = 'Schedule',
  Behavior = 'Behavior',
  Date = 'Date'
}
