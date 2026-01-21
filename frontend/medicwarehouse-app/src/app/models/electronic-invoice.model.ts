export interface CreateElectronicInvoice {
  type: string;  // NFSe, NFe, NFCe
  clientCpfCnpj: string;
  clientName: string;
  clientEmail?: string;
  clientPhone?: string;
  clientAddress?: string;
  clientCity?: string;
  clientState?: string;
  clientZipCode?: string;
  serviceDescription: string;
  serviceCode?: string;
  serviceAmount: number;
  paymentId?: string;
  appointmentId?: string;
  autoIssue?: boolean;
}

export interface ElectronicInvoice {
  id: string;
  type: string;
  number: string;
  series: string;
  issueDate: Date;
  status: string;

  // Provider
  providerCnpj: string;
  providerName: string;
  providerMunicipalRegistration?: string;

  // Client
  clientCpfCnpj: string;
  clientName: string;
  clientEmail?: string;
  clientPhone?: string;

  // Service
  serviceDescription: string;
  serviceCode?: string;
  serviceAmount: number;

  // Taxes
  issRate: number;
  issAmount: number;
  pisAmount: number;
  cofinsAmount: number;
  csllAmount: number;
  inssAmount: number;
  irAmount: number;
  totalTaxes: number;
  netAmount: number;

  // SEFAZ
  authorizationCode?: string;
  accessKey?: string;
  verificationCode?: string;
  protocol?: string;
  authorizationDate?: Date;

  // Documents
  pdfUrl?: string;
  hasXml: boolean;

  // Cancellation
  cancellationDate?: Date;
  cancellationReason?: string;

  // References
  paymentId?: string;
  appointmentId?: string;

  // Error
  errorMessage?: string;
  errorCode?: string;

  // Metadata
  createdAt: Date;
  updatedAt?: Date;
}

export interface ElectronicInvoiceListItem {
  id: string;
  type: string;
  number: string;
  issueDate: Date;
  status: string;
  clientName: string;
  serviceAmount: number;
  netAmount: number;
  accessKey?: string;
  pdfUrl?: string;
}

export interface ElectronicInvoiceStatistics {
  totalInvoices: number;
  authorizedInvoices: number;
  cancelledInvoices: number;
  pendingInvoices: number;
  errorInvoices: number;
  totalAmount: number;
  totalTaxes: number;
  netAmount: number;
}

export interface CreateInvoiceConfiguration {
  cnpj: string;
  companyName: string;
  municipalRegistration?: string;
  stateRegistration?: string;
  tradeName?: string;
  address?: string;
  addressNumber?: string;
  addressComplement?: string;
  neighborhood?: string;
  city?: string;
  state?: string;
  zipCode?: string;
  cityCode?: string;
  phone?: string;
  email?: string;
  serviceCode?: string;
  defaultIssRate: number;
  issRetainedByDefault: boolean;
  isSimplifiedTaxRegime: boolean;
  simplifiedTaxRegimeCode?: string;
  gateway: string;  // FocusNFe, ENotas, NFeCidades, Direct
  gatewayApiKey?: string;
  gatewayEnvironment?: string;
  autoIssueAfterPayment: boolean;
  sendEmailAfterIssuance: boolean;
}

export interface UpdateInvoiceConfiguration {
  municipalRegistration?: string;
  stateRegistration?: string;
  tradeName?: string;
  address?: string;
  addressNumber?: string;
  addressComplement?: string;
  neighborhood?: string;
  city?: string;
  state?: string;
  zipCode?: string;
  cityCode?: string;
  phone?: string;
  email?: string;
  serviceCode?: string;
  defaultIssRate?: number;
  issRetainedByDefault?: boolean;
  isSimplifiedTaxRegime?: boolean;
  simplifiedTaxRegimeCode?: string;
  gateway?: string;
  gatewayApiKey?: string;
  gatewayEnvironment?: string;
  autoIssueAfterPayment?: boolean;
  sendEmailAfterIssuance?: boolean;
}

export interface InvoiceConfiguration {
  id: string;
  cnpj: string;
  companyName: string;
  municipalRegistration?: string;
  stateRegistration?: string;
  tradeName?: string;

  // Address
  address?: string;
  addressNumber?: string;
  addressComplement?: string;
  neighborhood?: string;
  city?: string;
  state?: string;
  zipCode?: string;
  cityCode?: string;

  // Contact
  phone?: string;
  email?: string;

  // Tax
  serviceCode?: string;
  defaultIssRate: number;
  issRetainedByDefault: boolean;
  isSimplifiedTaxRegime: boolean;
  simplifiedTaxRegimeCode?: string;

  // Certificate
  hasCertificate: boolean;
  certificateExpirationDate?: Date;
  isCertificateExpired: boolean;

  // Numbering
  currentInvoiceNumber: number;
  defaultInvoiceSeries: string;
  currentRpsNumber: number;

  // Gateway
  gateway: string;
  hasGatewayApiKey: boolean;
  gatewayEnvironment?: string;
  isActive: boolean;

  // Automation
  autoIssueAfterPayment: boolean;
  sendEmailAfterIssuance: boolean;

  // Metadata
  createdAt: Date;
  updatedAt?: Date;
}

export interface CancelInvoiceRequest {
  reason: string;
}

export interface SendInvoiceEmailRequest {
  email: string;
}

export interface UploadCertificateRequest {
  certificate: string;  // Base64 encoded
  password: string;
}
