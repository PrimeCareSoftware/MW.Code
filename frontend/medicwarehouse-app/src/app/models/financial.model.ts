// Financial Module Models

// Enums
export enum ReceivableStatus {
  Pending = 1,
  PartiallyPaid = 2,
  Paid = 3,
  Overdue = 4,
  Cancelled = 5,
  InNegotiation = 6
}

export enum ReceivableType {
  Consultation = 1,
  Procedure = 2,
  Exam = 3,
  HealthInsurance = 4,
  Other = 5
}

export enum PayableStatus {
  Pending = 1,
  PartiallyPaid = 2,
  Paid = 3,
  Overdue = 4,
  Cancelled = 5
}

export enum PayableCategory {
  Rent = 1,
  Salaries = 2,
  Supplies = 3,
  Equipment = 4,
  Maintenance = 5,
  Utilities = 6,
  Marketing = 7,
  Insurance = 8,
  Taxes = 9,
  ProfessionalServices = 10,
  Laboratory = 11,
  Pharmacy = 12,
  Other = 99
}

export enum CashFlowType {
  Income = 1,
  Expense = 2
}

export enum CashFlowCategory {
  ConsultationPayment = 1,
  ProcedurePayment = 2,
  ExamPayment = 3,
  HealthInsurancePayment = 4,
  OtherIncome = 5,
  Rent = 101,
  Salaries = 102,
  Supplies = 103,
  Equipment = 104,
  Maintenance = 105,
  Utilities = 106,
  Marketing = 107,
  Insurance = 108,
  Taxes = 109,
  ProfessionalServices = 110,
  Laboratory = 111,
  Pharmacy = 112,
  OtherExpense = 199
}

export enum FinancialClosureStatus {
  Open = 1,
  PendingPayment = 2,
  PartiallyPaid = 3,
  Closed = 4,
  Cancelled = 5
}

export enum ClosurePaymentType {
  OutOfPocket = 1,
  HealthInsurance = 2,
  Mixed = 3
}

// Accounts Receivable
export interface AccountsReceivable {
  id: string;
  appointmentId?: string;
  patientId?: string;
  healthInsuranceOperatorId?: string;
  documentNumber: string;
  type: string;
  status: string;
  issueDate: Date;
  dueDate: Date;
  totalAmount: number;
  paidAmount: number;
  outstandingAmount: number;
  description?: string;
  notes?: string;
  settlementDate?: Date;
  cancellationReason?: string;
  installmentNumber?: number;
  totalInstallments?: number;
  interestRate?: number;
  fineRate?: number;
  discountRate?: number;
  daysOverdue: number;
  isOverdue: boolean;
  payments: ReceivablePayment[];
  createdAt: Date;
  updatedAt?: Date;
}

export interface ReceivablePayment {
  id: string;
  receivableId: string;
  amount: number;
  paymentDate: Date;
  transactionId?: string;
  notes?: string;
  createdAt: Date;
}

export interface CreateAccountsReceivable {
  appointmentId?: string;
  patientId?: string;
  healthInsuranceOperatorId?: string;
  documentNumber: string;
  type: number;
  dueDate: string;
  totalAmount: number;
  description?: string;
  installmentNumber?: number;
  totalInstallments?: number;
}

export interface AddReceivablePayment {
  receivableId: string;
  amount: number;
  paymentDate: string;
  transactionId?: string;
  notes?: string;
}

// Accounts Payable
export interface AccountsPayable {
  id: string;
  documentNumber: string;
  supplierId?: string;
  supplier?: Supplier;
  category: string;
  status: string;
  issueDate: Date;
  dueDate: Date;
  totalAmount: number;
  paidAmount: number;
  outstandingAmount: number;
  description: string;
  notes?: string;
  paymentDate?: Date;
  cancellationReason?: string;
  installmentNumber?: number;
  totalInstallments?: number;
  bankName?: string;
  bankAccount?: string;
  pixKey?: string;
  daysOverdue: number;
  isOverdue: boolean;
  payments: PayablePayment[];
  createdAt: Date;
  updatedAt?: Date;
}

export interface PayablePayment {
  id: string;
  payableId: string;
  amount: number;
  paymentDate: Date;
  transactionId?: string;
  notes?: string;
  createdAt: Date;
}

export interface CreateAccountsPayable {
  documentNumber: string;
  supplierId?: string;
  category: number;
  dueDate: string;
  totalAmount: number;
  description: string;
  installmentNumber?: number;
  totalInstallments?: number;
  bankName?: string;
  bankAccount?: string;
  pixKey?: string;
}

export interface AddPayablePayment {
  payableId: string;
  amount: number;
  paymentDate: string;
  transactionId?: string;
  notes?: string;
}

// Supplier
export interface Supplier {
  id: string;
  name: string;
  tradeName?: string;
  documentNumber?: string;
  email?: string;
  phone?: string;
  address?: string;
  city?: string;
  state?: string;
  zipCode?: string;
  bankName?: string;
  bankAccount?: string;
  pixKey?: string;
  notes?: string;
  isActive: boolean;
  createdAt: Date;
  updatedAt?: Date;
}

export interface CreateSupplier {
  name: string;
  tradeName?: string;
  documentNumber?: string;
  email?: string;
  phone?: string;
  address?: string;
  city?: string;
  state?: string;
  zipCode?: string;
  bankName?: string;
  bankAccount?: string;
  pixKey?: string;
  notes?: string;
}

// Cash Flow
export interface CashFlowEntry {
  id: string;
  type: string;
  category: string;
  transactionDate: Date;
  amount: number;
  description: string;
  reference?: string;
  notes?: string;
  paymentId?: string;
  receivableId?: string;
  payableId?: string;
  appointmentId?: string;
  bankAccount?: string;
  paymentMethod?: string;
  createdAt: Date;
  updatedAt?: Date;
}

export interface CreateCashFlowEntry {
  type: number;
  category: number;
  transactionDate: string;
  amount: number;
  description: string;
  reference?: string;
  paymentId?: string;
  receivableId?: string;
  payableId?: string;
  appointmentId?: string;
  bankAccount?: string;
  paymentMethod?: string;
}

export interface CashFlowSummary {
  startDate: Date;
  endDate: Date;
  totalIncome: number;
  totalExpense: number;
  netCashFlow: number;
  openingBalance: number;
  closingBalance: number;
}

// Financial Closure
export interface FinancialClosure {
  id: string;
  appointmentId: string;
  patientId: string;
  healthInsuranceOperatorId?: string;
  closureNumber: string;
  status: string;
  paymentType: string;
  closureDate: Date;
  totalAmount: number;
  patientAmount: number;
  insuranceAmount: number;
  paidAmount: number;
  outstandingAmount: number;
  notes?: string;
  settlementDate?: Date;
  cancellationReason?: string;
  discountAmount?: number;
  discountReason?: string;
  items: FinancialClosureItem[];
  createdAt: Date;
  updatedAt?: Date;
}

export interface FinancialClosureItem {
  id: string;
  closureId: string;
  description: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
  coverByInsurance: boolean;
  createdAt: Date;
}

export interface CreateFinancialClosure {
  appointmentId: string;
  patientId: string;
  healthInsuranceOperatorId?: string;
  closureNumber: string;
  paymentType: number;
}

export interface AddClosureItem {
  closureId: string;
  description: string;
  quantity: number;
  unitPrice: number;
  coverByInsurance: boolean;
}

export interface ApplyClosureDiscount {
  closureId: string;
  discountAmount: number;
  reason: string;
}

export interface RecordClosurePayment {
  closureId: string;
  amount: number;
}

// Financial Reports Models (PR 309 implementation)

// DRE - Demonstrativo de Resultados do Exercício (Income Statement)
export interface DREReport {
  periodStart: Date;
  periodEnd: Date;
  grossRevenue: number;
  deductions: number;
  netRevenue: number;
  operationalCosts: number;
  administrativeExpenses: number;
  salesExpenses: number;
  financialExpenses: number;
  totalExpenses: number;
  operationalProfit: number;
  netProfit: number;
  profitMargin: number;
  revenueDetails: RevenueDetail[];
  expenseDetails: ExpenseDetail[];
}

export interface RevenueDetail {
  category: string;
  amount: number;
  percentage: number;
}

export interface ExpenseDetail {
  category: string;
  amount: number;
  percentage: number;
}

// Cash Flow Forecast
export interface CashFlowForecast {
  startDate: Date;
  endDate: Date;
  currentBalance: number;
  projectedIncome: number;
  projectedExpenses: number;
  projectedBalance: number;
  monthlyForecast: MonthlyForecast[];
  pendingReceivables: ReceivableForecast[];
  pendingPayables: PayableForecast[];
}

export interface MonthlyForecast {
  year: number;
  month: number;
  expectedIncome: number;
  expectedExpenses: number;
  expectedBalance: number;
  cumulativeBalance: number;
}

export interface ReceivableForecast {
  id: string;
  documentNumber: string;
  dueDate: Date;
  amount: number;
  status: string;
  patientName: string;
}

export interface PayableForecast {
  id: string;
  documentNumber: string;
  dueDate: Date;
  amount: number;
  category: string;
  supplierName?: string;
}

// Profitability Analysis
export interface ProfitabilityAnalysis {
  periodStart: Date;
  periodEnd: Date;
  totalRevenue: number;
  totalCosts: number;
  totalProfit: number;
  profitMargin: number;
  byProcedure: ProfitabilityByProcedure[];
  byDoctor: ProfitabilityByDoctor[];
  byInsurance: ProfitabilityByInsurance[];
}

export interface ProfitabilityByProcedure {
  procedureName: string;
  count: number;
  revenue: number;
  averageValue: number;
  percentage: number;
}

export interface ProfitabilityByDoctor {
  doctorId: string;
  doctorName: string;
  appointmentsCount: number;
  revenue: number;
  averageAppointmentValue: number;
  percentage: number;
}

export interface ProfitabilityByInsurance {
  insuranceId?: string;
  insuranceName?: string;
  appointmentsCount: number;
  revenue: number;
  averageValue: number;
  percentage: number;
}

// ===== PRICING CONFIGURATION MODELS (PR 752) =====

export enum ProcedureConsultationPolicy {
  ChargeConsultation = 1,
  DiscountOnConsultation = 2,
  NoCharge = 3
}

export interface ClinicPricingConfiguration {
  id: string;
  clinicId: string;
  defaultConsultationPrice: number;
  followUpConsultationPrice?: number;
  telemedicineConsultationPrice?: number;
  defaultProcedurePolicy: ProcedureConsultationPolicy;
  consultationDiscountPercentage?: number;
  consultationDiscountFixedAmount?: number;
  createdAt: Date;
  updatedAt?: Date;
}

export interface CreateClinicPricingConfiguration {
  clinicId: string;
  defaultConsultationPrice: number;
  followUpConsultationPrice?: number;
  telemedicineConsultationPrice?: number;
  defaultProcedurePolicy: ProcedureConsultationPolicy;
  consultationDiscountPercentage?: number;
  consultationDiscountFixedAmount?: number;
}

export interface ProcedurePricingConfiguration {
  id: string;
  procedureId: string;
  clinicId: string;
  consultationPolicy?: ProcedureConsultationPolicy;
  consultationDiscountPercentage?: number;
  consultationDiscountFixedAmount?: number;
  customPrice?: number;
  createdAt: Date;
  updatedAt?: Date;
}

export interface CreateProcedurePricingConfiguration {
  procedureId: string;
  clinicId: string;
  consultationPolicy?: ProcedureConsultationPolicy;
  consultationDiscountPercentage?: number;
  consultationDiscountFixedAmount?: number;
  customPrice?: number;
}
