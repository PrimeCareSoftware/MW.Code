import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  AccountsReceivable,
  CreateAccountsReceivable,
  AddReceivablePayment,
  AccountsPayable,
  CreateAccountsPayable,
  AddPayablePayment,
  Supplier,
  CreateSupplier,
  CashFlowEntry,
  CreateCashFlowEntry,
  CashFlowSummary,
  FinancialClosure,
  CreateFinancialClosure,
  AddClosureItem,
  ApplyClosureDiscount,
  RecordClosurePayment
} from '../models/financial.model';

@Injectable({
  providedIn: 'root'
})
export class FinancialService {
  private readonly apiUrl = `${environment.apiUrl}`;

  constructor(private http: HttpClient) {}

  // ===== ACCOUNTS RECEIVABLE =====
  
  getAllReceivables(): Observable<AccountsReceivable[]> {
    return this.http.get<AccountsReceivable[]>(`${this.apiUrl}/accounts-receivable`);
  }

  getReceivableById(id: string): Observable<AccountsReceivable> {
    return this.http.get<AccountsReceivable>(`${this.apiUrl}/accounts-receivable/${id}`);
  }

  getReceivablesByPatient(patientId: string): Observable<AccountsReceivable[]> {
    return this.http.get<AccountsReceivable[]>(
      `${this.apiUrl}/accounts-receivable/by-patient/${patientId}`
    );
  }

  getReceivablesByAppointment(appointmentId: string): Observable<AccountsReceivable[]> {
    return this.http.get<AccountsReceivable[]>(
      `${this.apiUrl}/accounts-receivable/by-appointment/${appointmentId}`
    );
  }

  getOverdueReceivables(): Observable<AccountsReceivable[]> {
    return this.http.get<AccountsReceivable[]>(`${this.apiUrl}/accounts-receivable/overdue`);
  }

  getReceivablesByStatus(status: number): Observable<AccountsReceivable[]> {
    return this.http.get<AccountsReceivable[]>(
      `${this.apiUrl}/accounts-receivable/by-status/${status}`
    );
  }

  getTotalOutstandingReceivables(): Observable<number> {
    return this.http.get<number>(`${this.apiUrl}/accounts-receivable/total-outstanding`);
  }

  getTotalOverdueReceivables(): Observable<number> {
    return this.http.get<number>(`${this.apiUrl}/accounts-receivable/total-overdue`);
  }

  createReceivable(data: CreateAccountsReceivable): Observable<AccountsReceivable> {
    return this.http.post<AccountsReceivable>(`${this.apiUrl}/accounts-receivable`, data);
  }

  addReceivablePayment(id: string, payment: AddReceivablePayment): Observable<AccountsReceivable> {
    return this.http.post<AccountsReceivable>(
      `${this.apiUrl}/accounts-receivable/${id}/payments`,
      payment
    );
  }

  cancelReceivable(id: string, reason: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/accounts-receivable/${id}/cancel`, {
      receivableId: id,
      reason
    });
  }

  deleteReceivable(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/accounts-receivable/${id}`);
  }

  // ===== ACCOUNTS PAYABLE =====

  getAllPayables(): Observable<AccountsPayable[]> {
    return this.http.get<AccountsPayable[]>(`${this.apiUrl}/accounts-payable`);
  }

  getPayableById(id: string): Observable<AccountsPayable> {
    return this.http.get<AccountsPayable>(`${this.apiUrl}/accounts-payable/${id}`);
  }

  getPayablesBySupplier(supplierId: string): Observable<AccountsPayable[]> {
    return this.http.get<AccountsPayable[]>(
      `${this.apiUrl}/accounts-payable/by-supplier/${supplierId}`
    );
  }

  getOverduePayables(): Observable<AccountsPayable[]> {
    return this.http.get<AccountsPayable[]>(`${this.apiUrl}/accounts-payable/overdue`);
  }

  getPayablesByStatus(status: number): Observable<AccountsPayable[]> {
    return this.http.get<AccountsPayable[]>(`${this.apiUrl}/accounts-payable/by-status/${status}`);
  }

  getPayablesByCategory(category: number): Observable<AccountsPayable[]> {
    return this.http.get<AccountsPayable[]>(
      `${this.apiUrl}/accounts-payable/by-category/${category}`
    );
  }

  getTotalOutstandingPayables(): Observable<number> {
    return this.http.get<number>(`${this.apiUrl}/accounts-payable/total-outstanding`);
  }

  getTotalOverduePayables(): Observable<number> {
    return this.http.get<number>(`${this.apiUrl}/accounts-payable/total-overdue`);
  }

  createPayable(data: CreateAccountsPayable): Observable<AccountsPayable> {
    return this.http.post<AccountsPayable>(`${this.apiUrl}/accounts-payable`, data);
  }

  addPayablePayment(id: string, payment: AddPayablePayment): Observable<AccountsPayable> {
    return this.http.post<AccountsPayable>(
      `${this.apiUrl}/accounts-payable/${id}/payments`,
      payment
    );
  }

  cancelPayable(id: string, reason: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/accounts-payable/${id}/cancel`, {
      payableId: id,
      reason
    });
  }

  deletePayable(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/accounts-payable/${id}`);
  }

  // ===== SUPPLIERS =====

  getAllSuppliers(): Observable<Supplier[]> {
    return this.http.get<Supplier[]>(`${this.apiUrl}/suppliers`);
  }

  getActiveSuppliers(): Observable<Supplier[]> {
    return this.http.get<Supplier[]>(`${this.apiUrl}/suppliers/active`);
  }

  getSupplierById(id: string): Observable<Supplier> {
    return this.http.get<Supplier>(`${this.apiUrl}/suppliers/${id}`);
  }

  searchSuppliers(name: string): Observable<Supplier[]> {
    const params = new HttpParams().set('name', name);
    return this.http.get<Supplier[]>(`${this.apiUrl}/suppliers/search`, { params });
  }

  createSupplier(data: CreateSupplier): Observable<Supplier> {
    return this.http.post<Supplier>(`${this.apiUrl}/suppliers`, data);
  }

  updateSupplier(id: string, data: Partial<CreateSupplier>): Observable<Supplier> {
    return this.http.put<Supplier>(`${this.apiUrl}/suppliers/${id}`, data);
  }

  activateSupplier(id: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/suppliers/${id}/activate`, {});
  }

  deactivateSupplier(id: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/suppliers/${id}/deactivate`, {});
  }

  deleteSupplier(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/suppliers/${id}`);
  }

  // ===== CASH FLOW =====

  getAllCashFlowEntries(): Observable<CashFlowEntry[]> {
    return this.http.get<CashFlowEntry[]>(`${this.apiUrl}/cash-flow`);
  }

  getCashFlowEntryById(id: string): Observable<CashFlowEntry> {
    return this.http.get<CashFlowEntry>(`${this.apiUrl}/cash-flow/${id}`);
  }

  getCashFlowByDateRange(startDate: string, endDate: string): Observable<CashFlowEntry[]> {
    const params = new HttpParams().set('startDate', startDate).set('endDate', endDate);
    return this.http.get<CashFlowEntry[]>(`${this.apiUrl}/cash-flow/by-date-range`, { params });
  }

  getCashFlowByType(type: number): Observable<CashFlowEntry[]> {
    return this.http.get<CashFlowEntry[]>(`${this.apiUrl}/cash-flow/by-type/${type}`);
  }

  getCashFlowByCategory(category: number): Observable<CashFlowEntry[]> {
    return this.http.get<CashFlowEntry[]>(`${this.apiUrl}/cash-flow/by-category/${category}`);
  }

  getCashFlowSummary(startDate: string, endDate: string): Observable<CashFlowSummary> {
    const params = new HttpParams().set('startDate', startDate).set('endDate', endDate);
    return this.http.get<CashFlowSummary>(`${this.apiUrl}/cash-flow/summary`, { params });
  }

  getTotalIncome(startDate: string, endDate: string): Observable<number> {
    const params = new HttpParams().set('startDate', startDate).set('endDate', endDate);
    return this.http.get<number>(`${this.apiUrl}/cash-flow/total-income`, { params });
  }

  getTotalExpense(startDate: string, endDate: string): Observable<number> {
    const params = new HttpParams().set('startDate', startDate).set('endDate', endDate);
    return this.http.get<number>(`${this.apiUrl}/cash-flow/total-expense`, { params });
  }

  createCashFlowEntry(data: CreateCashFlowEntry): Observable<CashFlowEntry> {
    return this.http.post<CashFlowEntry>(`${this.apiUrl}/cash-flow`, data);
  }

  updateCashFlowEntry(id: string, data: Partial<CreateCashFlowEntry>): Observable<CashFlowEntry> {
    return this.http.put<CashFlowEntry>(`${this.apiUrl}/cash-flow/${id}`, data);
  }

  deleteCashFlowEntry(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/cash-flow/${id}`);
  }

  // ===== FINANCIAL CLOSURES =====

  getAllClosures(): Observable<FinancialClosure[]> {
    return this.http.get<FinancialClosure[]>(`${this.apiUrl}/financial-closures`);
  }

  getClosureById(id: string): Observable<FinancialClosure> {
    return this.http.get<FinancialClosure>(`${this.apiUrl}/financial-closures/${id}`);
  }

  getClosureByAppointment(appointmentId: string): Observable<FinancialClosure> {
    return this.http.get<FinancialClosure>(
      `${this.apiUrl}/financial-closures/by-appointment/${appointmentId}`
    );
  }

  getClosuresByPatient(patientId: string): Observable<FinancialClosure[]> {
    return this.http.get<FinancialClosure[]>(
      `${this.apiUrl}/financial-closures/by-patient/${patientId}`
    );
  }

  getClosuresByStatus(status: number): Observable<FinancialClosure[]> {
    return this.http.get<FinancialClosure[]>(
      `${this.apiUrl}/financial-closures/by-status/${status}`
    );
  }

  getClosureByNumber(closureNumber: string): Observable<FinancialClosure> {
    return this.http.get<FinancialClosure>(
      `${this.apiUrl}/financial-closures/by-number/${closureNumber}`
    );
  }

  createClosure(data: CreateFinancialClosure): Observable<FinancialClosure> {
    return this.http.post<FinancialClosure>(`${this.apiUrl}/financial-closures`, data);
  }

  addClosureItem(id: string, item: AddClosureItem): Observable<FinancialClosure> {
    return this.http.post<FinancialClosure>(`${this.apiUrl}/financial-closures/${id}/items`, item);
  }

  removeClosureItem(closureId: string, itemId: string): Observable<FinancialClosure> {
    return this.http.delete<FinancialClosure>(
      `${this.apiUrl}/financial-closures/${closureId}/items/${itemId}`
    );
  }

  applyClosureDiscount(id: string, discount: ApplyClosureDiscount): Observable<FinancialClosure> {
    return this.http.post<FinancialClosure>(
      `${this.apiUrl}/financial-closures/${id}/apply-discount`,
      discount
    );
  }

  recordClosurePayment(
    id: string,
    payment: RecordClosurePayment
  ): Observable<FinancialClosure> {
    return this.http.post<FinancialClosure>(
      `${this.apiUrl}/financial-closures/${id}/record-payment`,
      payment
    );
  }

  markClosureAsPending(id: string): Observable<FinancialClosure> {
    return this.http.post<FinancialClosure>(
      `${this.apiUrl}/financial-closures/${id}/mark-pending`,
      {}
    );
  }

  cancelClosure(id: string, reason: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/financial-closures/${id}/cancel`, {
      closureId: id,
      reason
    });
  }

  deleteClosure(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/financial-closures/${id}`);
  }
}
