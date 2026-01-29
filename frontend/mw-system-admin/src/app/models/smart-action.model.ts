export interface ImpersonateRequest {
  clinicId: string;
}

export interface ImpersonationResult {
  token: string;
  expiresIn: number;
}

export interface GrantCreditRequest {
  clinicId: string;
  days: number;
  reason: string;
}

export interface ApplyDiscountRequest {
  clinicId: string;
  percentage: number;
  months: number;
}

export interface SuspendRequest {
  clinicId: string;
  reactivationDate?: Date;
  reason: string;
}

export interface ExportDataRequest {
  clinicId: string;
}

export interface MigratePlanRequest {
  clinicId: string;
  newPlanId: string;
  proRata: boolean;
}

export interface SendCustomEmailRequest {
  clinicId: string;
  subject: string;
  body: string;
}

export interface SmartActionResponse {
  success: boolean;
  message: string;
}
