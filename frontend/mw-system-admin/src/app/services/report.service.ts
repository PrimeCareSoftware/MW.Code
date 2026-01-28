import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  ReportTemplate,
  ReportResult,
  ReportParameters,
  ScheduledReport,
  ScheduleReportDto
} from '../models/system-admin.model';

@Injectable({
  providedIn: 'root'
})
export class ReportService {
  private apiUrl = `${environment.apiUrl}/system-admin/reports`;

  constructor(private http: HttpClient) {}

  getAvailableReports(): Observable<ReportTemplate[]> {
    return this.http.get<ReportTemplate[]>(`${this.apiUrl}/templates`);
  }

  generateReport(templateId: number, parameters: ReportParameters): Observable<ReportResult> {
    return this.http.post<ReportResult>(`${this.apiUrl}/generate/${templateId}`, parameters);
  }

  exportReport(reportId: string, format: 'pdf' | 'excel'): Observable<Blob> {
    return this.http.post(`${this.apiUrl}/${reportId}/export?format=${format}`, {}, {
      responseType: 'blob'
    });
  }

  scheduleReport(dto: ScheduleReportDto): Observable<ScheduledReport> {
    return this.http.post<ScheduledReport>(`${this.apiUrl}/schedule`, dto);
  }

  getScheduledReports(): Observable<ScheduledReport[]> {
    return this.http.get<ScheduledReport[]>(`${this.apiUrl}/scheduled`);
  }

  deleteScheduledReport(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/scheduled/${id}`);
  }

  updateScheduledReport(id: number, dto: ScheduleReportDto): Observable<ScheduledReport> {
    return this.http.put<ScheduledReport>(`${this.apiUrl}/scheduled/${id}`, dto);
  }
}
