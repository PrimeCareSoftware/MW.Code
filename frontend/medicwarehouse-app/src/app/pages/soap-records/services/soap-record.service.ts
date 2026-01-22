import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { environment } from '../../../../environments/environment';
import {
  SoapRecord,
  UpdateSubjectiveCommand,
  UpdateObjectiveCommand,
  UpdateAssessmentCommand,
  UpdatePlanCommand,
  SoapRecordValidation
} from '../models/soap-record.model';

@Injectable({
  providedIn: 'root'
})
export class SoapRecordService {
  private apiUrl = `${environment.apiUrl}/SoapRecords`;

  constructor(private http: HttpClient) {}

  createSoapRecord(attendanceId: string): Observable<SoapRecord> {
    return this.http.post<SoapRecord>(`${this.apiUrl}/attendance/${attendanceId}`, {})
      .pipe(catchError(this.handleError));
  }

  updateSubjective(soapId: string, data: UpdateSubjectiveCommand): Observable<SoapRecord> {
    return this.http.put<SoapRecord>(`${this.apiUrl}/${soapId}/subjective`, data)
      .pipe(catchError(this.handleError));
  }

  updateObjective(soapId: string, data: UpdateObjectiveCommand): Observable<SoapRecord> {
    return this.http.put<SoapRecord>(`${this.apiUrl}/${soapId}/objective`, data)
      .pipe(catchError(this.handleError));
  }

  updateAssessment(soapId: string, data: UpdateAssessmentCommand): Observable<SoapRecord> {
    return this.http.put<SoapRecord>(`${this.apiUrl}/${soapId}/assessment`, data)
      .pipe(catchError(this.handleError));
  }

  updatePlan(soapId: string, data: UpdatePlanCommand): Observable<SoapRecord> {
    return this.http.put<SoapRecord>(`${this.apiUrl}/${soapId}/plan`, data)
      .pipe(catchError(this.handleError));
  }

  completeSoapRecord(soapId: string): Observable<SoapRecord> {
    return this.http.post<SoapRecord>(`${this.apiUrl}/${soapId}/complete`, {})
      .pipe(catchError(this.handleError));
  }

  getSoapRecord(soapId: string): Observable<SoapRecord> {
    return this.http.get<SoapRecord>(`${this.apiUrl}/${soapId}`)
      .pipe(catchError(this.handleError));
  }

  getPatientSoapRecords(patientId: string): Observable<SoapRecord[]> {
    return this.http.get<SoapRecord[]>(`${this.apiUrl}/patient/${patientId}`)
      .pipe(catchError(this.handleError));
  }

  validateSoapRecord(soapId: string): Observable<SoapRecordValidation> {
    return this.http.get<SoapRecordValidation>(`${this.apiUrl}/${soapId}/validate`)
      .pipe(catchError(this.handleError));
  }

  private handleError(error: HttpErrorResponse) {
    let errorMessage = 'Ocorreu um erro ao processar a solicitação.';
    
    if (error.error instanceof ErrorEvent) {
      errorMessage = `Erro: ${error.error.message}`;
    } else {
      if (error.status === 0) {
        errorMessage = 'Não foi possível conectar ao servidor. Verifique sua conexão.';
      } else if (error.status === 400) {
        errorMessage = error.error?.message || 'Dados inválidos.';
      } else if (error.status === 401) {
        errorMessage = 'Não autorizado. Faça login novamente.';
      } else if (error.status === 404) {
        errorMessage = 'Registro não encontrado.';
      } else if (error.status === 500) {
        errorMessage = 'Erro interno do servidor.';
      }
    }
    
    return throwError(() => new Error(errorMessage));
  }
}
