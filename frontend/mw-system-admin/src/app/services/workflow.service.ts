import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  WorkflowDto,
  CreateWorkflowDto,
  UpdateWorkflowDto,
  WorkflowExecution
} from '../models/workflow.model';

@Injectable({
  providedIn: 'root'
})
export class WorkflowService {
  private apiUrl = `${environment.apiUrl}/system-admin/workflows`;

  constructor(private http: HttpClient) {}

  /**
   * Get all workflows
   */
  getAll(): Observable<WorkflowDto[]> {
    return this.http.get<WorkflowDto[]>(this.apiUrl);
  }

  /**
   * Get workflow by ID
   */
  getById(id: number): Observable<WorkflowDto> {
    return this.http.get<WorkflowDto>(`${this.apiUrl}/${id}`);
  }

  /**
   * Create a new workflow
   */
  create(dto: CreateWorkflowDto): Observable<WorkflowDto> {
    return this.http.post<WorkflowDto>(this.apiUrl, dto);
  }

  /**
   * Update workflow
   */
  update(id: number, dto: UpdateWorkflowDto): Observable<WorkflowDto> {
    return this.http.put<WorkflowDto>(`${this.apiUrl}/${id}`, dto);
  }

  /**
   * Delete workflow
   */
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  /**
   * Toggle workflow enabled status
   */
  toggle(id: number): Observable<{ isEnabled: boolean }> {
    return this.http.post<{ isEnabled: boolean }>(`${this.apiUrl}/${id}/toggle`, {});
  }

  /**
   * Test workflow with sample data
   */
  test(id: number, testData: any): Observable<WorkflowExecution> {
    return this.http.post<WorkflowExecution>(`${this.apiUrl}/${id}/test`, testData);
  }

  /**
   * Get workflow executions
   */
  getExecutions(id: number, page: number = 1, pageSize: number = 20): Observable<WorkflowExecution[]> {
    return this.http.get<WorkflowExecution[]>(`${this.apiUrl}/${id}/executions`, {
      params: { page: page.toString(), pageSize: pageSize.toString() }
    });
  }

  /**
   * Get single execution details
   */
  getExecution(executionId: number): Observable<WorkflowExecution> {
    return this.http.get<WorkflowExecution>(`${this.apiUrl}/executions/${executionId}`);
  }
}
