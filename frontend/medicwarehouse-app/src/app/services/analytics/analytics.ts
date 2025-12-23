import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface FinancialSummary {
  periodStart: string;
  periodEnd: string;
  totalRevenue: number;
  totalExpenses: number;
  netProfit: number;
  totalAppointments: number;
  totalPatients: number;
  averageAppointmentValue: number;
  revenueByPaymentMethod: RevenueByMethod[];
  expensesByCategory: ExpenseByCategory[];
}

export interface RevenueByMethod {
  paymentMethod: string;
  amount: number;
  count: number;
  percentage: number;
}

export interface ExpenseByCategory {
  category: string;
  amount: number;
  count: number;
  percentage: number;
}

export interface RevenueReport {
  periodStart: string;
  periodEnd: string;
  totalRevenue: number;
  totalTransactions: number;
  dailyBreakdown: DailyRevenue[];
}

export interface DailyRevenue {
  date: string;
  revenue: number;
  transactions: number;
}

export interface AppointmentsReport {
  periodStart: string;
  periodEnd: string;
  totalAppointments: number;
  completedAppointments: number;
  cancelledAppointments: number;
  noShowAppointments: number;
  completionRate: number;
  cancellationRate: number;
  appointmentsByStatus: AppointmentsByStatus[];
  appointmentsByType: AppointmentsByType[];
}

export interface AppointmentsByStatus {
  status: string;
  count: number;
  percentage: number;
}

export interface AppointmentsByType {
  type: string;
  count: number;
  percentage: number;
}

export interface PatientsReport {
  periodStart: string;
  periodEnd: string;
  totalPatients: number;
  newPatients: number;
  activePatients: number;
  monthlyBreakdown: MonthlyPatients[];
}

export interface MonthlyPatients {
  year: number;
  month: number;
  newPatients: number;
  totalPatients: number;
}

@Injectable({
  providedIn: 'root'
})
export class Analytics {
  private apiUrl = `${environment.apiUrl}/reports`;

  constructor(private http: HttpClient) {}

  getFinancialSummary(clinicId: string, startDate: string, endDate: string): Observable<FinancialSummary> {
    const params = new HttpParams()
      .set('clinicId', clinicId)
      .set('startDate', startDate)
      .set('endDate', endDate);
    
    return this.http.get<FinancialSummary>(`${this.apiUrl}/financial-summary`, { params });
  }

  getRevenueReport(clinicId: string, startDate: string, endDate: string): Observable<RevenueReport> {
    const params = new HttpParams()
      .set('clinicId', clinicId)
      .set('startDate', startDate)
      .set('endDate', endDate);
    
    return this.http.get<RevenueReport>(`${this.apiUrl}/revenue`, { params });
  }

  getAppointmentsReport(clinicId: string, startDate: string, endDate: string): Observable<AppointmentsReport> {
    const params = new HttpParams()
      .set('clinicId', clinicId)
      .set('startDate', startDate)
      .set('endDate', endDate);
    
    return this.http.get<AppointmentsReport>(`${this.apiUrl}/appointments`, { params });
  }

  getPatientsReport(clinicId: string, startDate: string, endDate: string): Observable<PatientsReport> {
    const params = new HttpParams()
      .set('clinicId', clinicId)
      .set('startDate', startDate)
      .set('endDate', endDate);
    
    return this.http.get<PatientsReport>(`${this.apiUrl}/patients`, { params });
  }
}
