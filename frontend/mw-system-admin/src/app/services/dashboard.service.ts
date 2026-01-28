import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  CustomDashboard,
  DashboardWidget,
  CreateDashboardDto,
  UpdateDashboardDto,
  CreateWidgetDto,
  WidgetPositionDto,
  WidgetTemplate
} from '../models/system-admin.model';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private apiUrl = `${environment.apiUrl}/system-admin/dashboards`;

  constructor(private http: HttpClient) {}

  getAllDashboards(): Observable<CustomDashboard[]> {
    return this.http.get<CustomDashboard[]>(this.apiUrl);
  }

  getDashboard(id: number): Observable<CustomDashboard> {
    return this.http.get<CustomDashboard>(`${this.apiUrl}/${id}`);
  }

  createDashboard(dto: CreateDashboardDto): Observable<CustomDashboard> {
    return this.http.post<CustomDashboard>(this.apiUrl, dto);
  }

  updateDashboard(id: number, dto: UpdateDashboardDto): Observable<CustomDashboard> {
    return this.http.put<CustomDashboard>(`${this.apiUrl}/${id}`, dto);
  }

  deleteDashboard(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  addWidget(dashboardId: number, dto: CreateWidgetDto): Observable<DashboardWidget> {
    return this.http.post<DashboardWidget>(`${this.apiUrl}/${dashboardId}/widgets`, dto);
  }

  updateWidgetPosition(widgetId: number, position: WidgetPositionDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/widgets/${widgetId}/position`, position);
  }

  getWidgetData(widgetId: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/widgets/${widgetId}/data`);
  }

  getTemplates(): Observable<WidgetTemplate[]> {
    return this.http.get<WidgetTemplate[]>(`${this.apiUrl}/templates`);
  }

  exportDashboard(id: number, format: 'pdf' | 'excel' = 'pdf'): Observable<Blob> {
    return this.http.post(`${this.apiUrl}/${id}/export?format=${format}`, {}, {
      responseType: 'blob'
    });
  }
}
