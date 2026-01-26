/**
 * Mock data for Fiscal Dashboard
 * Used only when environment.useMockData is true
 */

export interface InvoiceTypeSummary {
  type: string; // NFSe, NFe, NFCe
  quantity: number;
  totalValue: number;
}

export interface TaxBreakdown {
  taxType: string; // ISS, PIS, COFINS, CSLL, INSS, IR
  amount: number;
  percentage: number;
}

export interface InvoiceStatusSummary {
  authorized: number;
  cancelled: number;
  error: number;
  pending: number;
}

export interface TopClient {
  clientId: string;
  clientName: string;
  totalBilled: number;
  invoiceCount: number;
}

export interface MonthlyTrend {
  month: string;
  year: number;
  totalIssued: number;
  invoiceCount: number;
}

export interface Alert {
  id: string;
  type: 'warning' | 'error' | 'info';
  message: string;
  date: Date;
}

export interface FiscalDashboardData {
  totalIssuedMonth: number;
  totalTaxesPaid: number;
  invoicesByType: InvoiceTypeSummary[];
  taxBreakdown: TaxBreakdown[];
  invoiceStatus: InvoiceStatusSummary;
  topClients: TopClient[];
  monthlyTrend: MonthlyTrend[];
  alerts: Alert[];
}

export const MOCK_FISCAL_DASHBOARD_DATA: FiscalDashboardData = {
  totalIssuedMonth: 287450.75,
  totalTaxesPaid: 35789.25,
  invoicesByType: [
    { type: 'NFS-e', quantity: 145, totalValue: 198750.50 },
    { type: 'NF-e', quantity: 78, totalValue: 75320.25 },
    { type: 'NFC-e', quantity: 32, totalValue: 13380.00 }
  ],
  taxBreakdown: [
    { taxType: 'ISS', amount: 14372.54, percentage: 5.0 },
    { taxType: 'PIS', amount: 1873.43, percentage: 0.65 },
    { taxType: 'COFINS', amount: 8623.52, percentage: 3.0 },
    { taxType: 'CSLL', amount: 2587.05, percentage: 0.9 },
    { taxType: 'INSS', amount: 5748.01, percentage: 2.0 },
    { taxType: 'IR', amount: 2584.70, percentage: 0.9 }
  ],
  invoiceStatus: {
    authorized: 238,
    cancelled: 12,
    error: 5,
    pending: 0
  },
  topClients: [
    { clientId: '1', clientName: 'Hospital Santa Casa', totalBilled: 45780.90, invoiceCount: 23 },
    { clientId: '2', clientName: 'Clínica Médica Centro', totalBilled: 38940.50, invoiceCount: 19 },
    { clientId: '3', clientName: 'Laboratório Analisa', totalBilled: 32150.00, invoiceCount: 15 },
    { clientId: '4', clientName: 'Unimed Regional', totalBilled: 28470.35, invoiceCount: 12 },
    { clientId: '5', clientName: 'Prefeitura Municipal', totalBilled: 24890.75, invoiceCount: 10 }
  ],
  monthlyTrend: [
    { month: 'Jan', year: 2024, totalIssued: 245870.50, invoiceCount: 231 },
    { month: 'Fev', year: 2024, totalIssued: 268940.75, invoiceCount: 248 },
    { month: 'Mar', year: 2024, totalIssued: 297850.25, invoiceCount: 267 },
    { month: 'Abr', year: 2024, totalIssued: 283470.80, invoiceCount: 255 },
    { month: 'Mai', year: 2024, totalIssued: 287450.75, invoiceCount: 255 }
  ],
  alerts: [
    {
      id: '1',
      type: 'warning',
      message: 'Certificado digital vence em 45 dias (30/07/2024)',
      date: new Date()
    },
    {
      id: '2',
      type: 'error',
      message: '5 notas fiscais com erro de transmissão - requer atenção',
      date: new Date()
    },
    {
      id: '3',
      type: 'info',
      message: 'Limite mensal de emissões: 75% utilizado',
      date: new Date()
    }
  ]
};
