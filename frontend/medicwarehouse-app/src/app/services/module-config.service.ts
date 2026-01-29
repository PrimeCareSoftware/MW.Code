import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { 
  ModuleConfig, 
  ModuleInfo, 
  ValidationResponse, 
  ModuleHistoryEntry,
  ModuleEnableRequest,
  ModuleConfigUpdateRequest 
} from '../models/module-config.model';

/**
 * Service for managing clinic module configurations
 */
@Injectable({
  providedIn: 'root'
})
export class ModuleConfigService {
  private apiUrl = `${environment.apiUrl}/api/module-config`;

  constructor(private http: HttpClient) {}

  /**
   * Get all modules configured for the current clinic
   */
  getModules(): Observable<ModuleConfig[]> {
    return this.http.get<ModuleConfig[]>(this.apiUrl);
  }

  /**
   * Get information about all available modules
   */
  getModulesInfo(): Observable<ModuleInfo[]> {
    return this.http.get<ModuleInfo[]>(`${this.apiUrl}/info`);
  }

  /**
   * Enable a module for the clinic
   * @param moduleName - Name of the module to enable
   * @param reason - Optional reason for enabling
   */
  enableModule(moduleName: string, reason?: string): Observable<any> {
    const body: ModuleEnableRequest = reason ? { reason } : {};
    return this.http.post(`${this.apiUrl}/${moduleName}/enable`, body);
  }

  /**
   * Disable a module for the clinic
   * @param moduleName - Name of the module to disable
   */
  disableModule(moduleName: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/${moduleName}/disable`, {});
  }

  /**
   * Update module configuration
   * @param moduleName - Name of the module
   * @param configuration - Configuration JSON string
   */
  updateModuleConfig(moduleName: string, configuration: string): Observable<any> {
    const body: ModuleConfigUpdateRequest = { configuration };
    return this.http.put(`${this.apiUrl}/${moduleName}/config`, body);
  }

  /**
   * Validate if a module can be enabled
   * @param moduleName - Name of the module to validate
   */
  validateModule(moduleName: string): Observable<ValidationResponse> {
    return this.http.post<ValidationResponse>(`${this.apiUrl}/validate`, { moduleName });
  }

  /**
   * Get history of changes for a specific module
   * @param moduleName - Name of the module
   */
  getModuleHistory(moduleName: string): Observable<ModuleHistoryEntry[]> {
    return this.http.get<ModuleHistoryEntry[]>(`${this.apiUrl}/${moduleName}/history`);
  }
}
