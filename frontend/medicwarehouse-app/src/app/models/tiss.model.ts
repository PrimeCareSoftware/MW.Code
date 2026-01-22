export interface Address {
  street: string;
  number: string;
  complement?: string;
  neighborhood: string;
  city: string;
  state: string;
  zipCode: string;
  country: string;
}

export interface HealthInsuranceOperator {
  id: string;
  ansCode: string;
  tradeName: string;
  legalName: string;
  cnpj: string;
  email: string;
  phoneCountryCode: string;
  phoneNumber: string;
  address: Address;
  isActive: boolean;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateHealthInsuranceOperator {
  ansCode: string;
  tradeName: string;
  legalName: string;
  cnpj: string;
  email: string;
  phoneCountryCode: string;
  phoneNumber: string;
  address: Address;
}

export interface UpdateHealthInsuranceOperator {
  tradeName: string;
  legalName: string;
  cnpj: string;
  email: string;
  phoneCountryCode: string;
  phoneNumber: string;
  address: Address;
  isActive: boolean;
}

export enum TissGuideType {
  ConsultationGuide = 'ConsultationGuide',
  ServiceProvisionGuide = 'ServiceProvisionGuide',
  HospitalizationGuide = 'HospitalizationGuide',
  SadtGuide = 'SadtGuide'
}

export enum GuideStatus {
  Draft = 'Draft',
  Pending = 'Pending',
  Approved = 'Approved',
  Rejected = 'Rejected',
  Billed = 'Billed',
  Paid = 'Paid',
  Cancelled = 'Cancelled'
}

export enum AuthorizationStatus {
  Pending = 'Pending',
  Approved = 'Approved',
  PartiallyApproved = 'PartiallyApproved',
  Denied = 'Denied',
  Cancelled = 'Cancelled'
}

export enum BatchStatus {
  Draft = 'Draft',
  Generated = 'Generated',
  Sent = 'Sent',
  Processing = 'Processing',
  Approved = 'Approved',
  PartiallyApproved = 'PartiallyApproved',
  Rejected = 'Rejected',
  Paid = 'Paid'
}

export interface PatientHealthInsurance {
  id: string;
  patientId: string;
  patientName?: string;
  healthInsurancePlanId: string;
  planName?: string;
  operatorName?: string;
  cardNumber: string;
  validityDate: string;
  isActive: boolean;
  createdAt: string;
  updatedAt?: string;
}

export interface CreatePatientHealthInsurance {
  patientId: string;
  healthInsurancePlanId: string;
  cardNumber: string;
  validityDate: string;
}

export interface UpdatePatientHealthInsurance {
  cardNumber: string;
  validityDate: string;
  isActive: boolean;
}

export interface AuthorizationRequest {
  id: string;
  patientHealthInsuranceId: string;
  patientName?: string;
  planName?: string;
  requestDate: string;
  requestedBy: string;
  requestedByName?: string;
  procedureCode: string;
  procedureName?: string;
  quantity: number;
  justification: string;
  status: AuthorizationStatus;
  authorizationNumber?: string;
  approvedQuantity?: number;
  denialReason?: string;
  responseDate?: string;
  expiryDate?: string;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateAuthorizationRequest {
  patientHealthInsuranceId: string;
  procedureCode: string;
  quantity: number;
  justification: string;
}

export interface UpdateAuthorizationRequest {
  status: AuthorizationStatus;
  authorizationNumber?: string;
  approvedQuantity?: number;
  denialReason?: string;
  responseDate?: string;
  expiryDate?: string;
}

export interface TissGuide {
  id: string;
  guideNumber: string;
  guideType: TissGuideType;
  patientHealthInsuranceId: string;
  patientName?: string;
  planName?: string;
  operatorName?: string;
  authorizationRequestId?: string;
  authorizationNumber?: string;
  serviceDate: string;
  providerId: string;
  providerName?: string;
  status: GuideStatus;
  totalAmount: number;
  approvedAmount?: number;
  procedures: TissGuideProcedure[];
  denialReason?: string;
  batchId?: string;
  createdAt: string;
  updatedAt?: string;
}

export interface TissGuideProcedure {
  id?: string;
  tissGuideId?: string;
  procedureCode: string;
  procedureName?: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
  sequenceNumber: number;
}

export interface CreateTissGuide {
  guideType: TissGuideType;
  patientHealthInsuranceId: string;
  authorizationRequestId?: string;
  serviceDate: string;
  procedures: CreateTissGuideProcedure[];
}

export interface CreateTissGuideProcedure {
  procedureCode: string;
  quantity: number;
  unitPrice: number;
}

export interface UpdateTissGuide {
  serviceDate: string;
  status: GuideStatus;
  denialReason?: string;
}

export interface TissBatch {
  id: string;
  batchNumber: string;
  healthInsuranceOperatorId: string;
  operatorName?: string;
  competence: string;
  status: BatchStatus;
  totalAmount: number;
  guideCount: number;
  xmlFilePath?: string;
  guides?: TissGuide[];
  generatedAt?: string;
  sentAt?: string;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateTissBatch {
  healthInsuranceOperatorId: string;
  competence: string;
  guideIds: string[];
}

export interface UpdateTissBatch {
  status: BatchStatus;
}

export interface TussCategory {
  id: string;
  code: string;
  name: string;
  description?: string;
}

export interface TussProcedure {
  id: string;
  code: string;
  name: string;
  categoryId?: string;
  categoryName?: string;
  description?: string;
  isActive: boolean;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateTussProcedure {
  code: string;
  name: string;
  categoryId?: string;
  description?: string;
}

export interface UpdateTussProcedure {
  name: string;
  categoryId?: string;
  description?: string;
  isActive: boolean;
}

// Analytics DTOs
export interface GlosasSummary {
  totalBilled: number;
  totalApproved: number;
  totalGlosed: number;
  glosaPercentage: number;
  totalBatches: number;
  totalGuides: number;
  glosedGuides: number;
}

export interface GlosasByOperator {
  operatorId: string;
  operatorName: string;
  totalBilled: number;
  totalGlosed: number;
  glosaPercentage: number;
  totalGuides: number;
  glosedGuides: number;
}

export interface GlosasTrend {
  year: number;
  month: number;
  monthName: string;
  totalBilled: number;
  totalGlosed: number;
  glosaPercentage: number;
  totalGuides: number;
}

export interface ProcedureGlosas {
  procedureId: string;
  procedureCode: string;
  procedureName: string;
  totalBilled: number;
  totalGlosed: number;
  glosaPercentage: number;
  totalOccurrences: number;
  glosedOccurrences: number;
}

export interface AuthorizationRate {
  operatorId: string;
  operatorName: string;
  totalRequests: number;
  approvedRequests: number;
  rejectedRequests: number;
  pendingRequests: number;
  approvalRate: number;
}

export interface ApprovalTime {
  operatorId: string;
  operatorName: string;
  averageApprovalDays: number;
  totalProcessed: number;
  minApprovalDays: number;
  maxApprovalDays: number;
}

export interface MonthlyPerformance {
  year: number;
  month: number;
  monthName: string;
  totalBilled: number;
  totalApproved: number;
  glosaPercentage: number;
  averageApprovalDays: number;
  totalGuides: number;
}

export interface GlosaAlert {
  alertType: string;
  message: string;
  severity: 'high' | 'medium' | 'low';
  value: number;
  threshold: number;
}
