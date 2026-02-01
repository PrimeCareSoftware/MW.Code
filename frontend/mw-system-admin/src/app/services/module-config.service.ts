import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  ModuleInfo,
  ModuleUsage,
  ModuleAdoption,
  ModuleUsageByPlan,
  ClinicModule,
  ValidationResponse
} from '../models/module-config.model';

@Injectable({
  providedIn: 'root'
})
export class ModuleConfigService {
  private apiUrl = `${environment.apiUrl}/system-admin/modules`;

  constructor(private http: HttpClient) {}

  // Dashboard e Métricas
  getGlobalModuleUsage(): Observable<ModuleUsage[]> {
    return this.http.get<ModuleUsage[]>(`${this.apiUrl}/usage`);
  }

  getModuleAdoption(): Observable<ModuleAdoption[]> {
    return this.http.get<ModuleAdoption[]>(`${this.apiUrl}/adoption`);
  }

  getUsageByPlan(): Observable<ModuleUsageByPlan[]> {
    return this.http.get<ModuleUsageByPlan[]>(`${this.apiUrl}/usage-by-plan`);
  }

  // Informações de Módulos
  // Note: This endpoint uses /module-config instead of /system-admin/modules
  // because it provides general module information that may be used by both
  // system-admin and clinic-specific contexts
  getModulesInfo(): Observable<ModuleInfo[]> {
    return this.http.get<ModuleInfo[]>(`${environment.apiUrl}/module-config/info`);
  }

  // Ações Globais
  enableModuleGlobally(moduleName: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/${moduleName}/enable-globally`, {});
  }

  disableModuleGlobally(moduleName: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/${moduleName}/disable-globally`, {});
  }

  // Clínicas com Módulo
  getClinicsWithModule(moduleName: string): Observable<ClinicModule[]> {
    return this.http.get<ClinicModule[]>(`${this.apiUrl}/${moduleName}/clinics`);
  }

  // Validação
  validateModule(clinicId: string, moduleName: string): Observable<ValidationResponse> {
    return this.http.post<ValidationResponse>(
      `${environment.apiUrl}/module-config/validate`,
      { moduleName }
    );
  }

  // Estatísticas de Módulo
  getModuleStats(moduleName: string): Observable<any> {
    return this.http.get(`${this.apiUrl}/${moduleName}/stats`);
  }
}
