# 🎨 PROMPT: Frontend System Admin - Configuração Global de Módulos

> **Fase:** 2 de 5  
> **Duração Estimada:** 2-3 semanas  
> **Desenvolvedores:** 1-2  
> **Prioridade:** 🔥🔥🔥 ALTA  
> **Dependências:** 01-PROMPT-BACKEND.md (concluído)

---

## 📋 Contexto

### Situação Atual

O **mw-system-admin** é uma aplicação Angular 20 separada para administração do sistema.

**Localização:**
```
/frontend/mw-system-admin/
  ├── src/
  │   ├── app/
  │   │   ├── components/
  │   │   ├── services/
  │   │   ├── models/
  │   │   └── pages/
  │   └── assets/
```

**Tecnologias:**
- Angular 20 (standalone components)
- Angular Material
- RxJS
- TypeScript 5.0+
- ApexCharts (para gráficos)

### O Que Precisa Ser Desenvolvido

Criar interface web completa para o **System Admin** gerenciar módulos:

1. **Dashboard de Módulos**
   - Visão geral de todos os módulos
   - Métricas de adoção e uso
   - Gráficos de analytics

2. **Gestão de Módulos por Plano**
   - Vincular módulos aos planos de assinatura
   - Definir módulos premium vs. básicos
   - Visualizar estrutura de planos

3. **Gestão de Módulos por Clínica**
   - Ver módulos habilitados por clínica
   - Habilitar/desabilitar em lote
   - Filtros e buscas avançadas

---

## 🎯 Objetivos da Tarefa

### Objetivos Principais

1. Criar páginas de gestão de módulos no System Admin
2. Implementar dashboard com métricas e gráficos
3. Criar interfaces de configuração de planos
4. Implementar ações em lote e filtros
5. Garantir UX intuitiva e responsiva

### Benefícios Esperados

- 📊 **Visibilidade Total:** Métricas de uso de módulos
- 🎯 **Gestão Eficiente:** Configuração rápida e intuitiva
- 💰 **Monetização:** Controle fino de features por plano
- 🔍 **Analytics:** Insights sobre adoção de funcionalidades

---

## 📝 Tarefas Detalhadas

### 1. Criar Models e Services (2 dias)

#### 1.1. Models TypeScript

**Criar:** `/frontend/mw-system-admin/src/app/models/module-config.model.ts`

```typescript
export interface ModuleInfo {
  name: string;
  displayName: string;
  description: string;
  category: 'Core' | 'Advanced' | 'Premium' | 'Analytics';
  icon: string;
  isCore: boolean;
  requiredModules: string[];
  minimumPlan: string;
}

export interface ModuleConfig {
  moduleName: string;
  displayName: string;
  description: string;
  category: string;
  icon: string;
  isEnabled: boolean;
  isAvailableInPlan: boolean;
  isCore: boolean;
  requiredModules: string[];
  configuration?: string;
  updatedAt?: Date;
}

export interface ModuleUsage {
  moduleName: string;
  displayName: string;
  totalClinics: number;
  clinicsWithModuleEnabled: number;
  adoptionRate: number;
  category: string;
}

export interface ModuleAdoption {
  moduleName: string;
  displayName: string;
  adoptionRate: number;
  enabledCount: number;
}

export interface ModuleUsageByPlan {
  planName: string;
  moduleName: string;
  clinicsCount: number;
  usagePercentage: number;
}

export interface ClinicModule {
  clinicId: string;
  clinicName: string;
  isEnabled: boolean;
  configuration?: string;
  updatedAt?: Date;
}

export interface ModuleConfigHistory {
  id: string;
  moduleName: string;
  action: 'Enabled' | 'Disabled' | 'ConfigUpdated';
  changedBy: string;
  changedAt: Date;
  reason?: string;
  previousConfiguration?: string;
  newConfiguration?: string;
}

export interface ValidationResponse {
  isValid: boolean;
  errorMessage?: string;
}
```

#### 1.2. Service para API

**Criar:** `/frontend/mw-system-admin/src/app/services/module-config.service.ts`

```typescript
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
  private apiUrl = `${environment.apiUrl}/api/system-admin/modules`;

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
  getModulesInfo(): Observable<ModuleInfo[]> {
    return this.http.get<ModuleInfo[]>(`${environment.apiUrl}/api/module-config/info`);
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
      `${environment.apiUrl}/api/module-config/validate`,
      { moduleName }
    );
  }
}
```

---

### 2. Dashboard de Módulos (3-4 dias)

#### 2.1. Componente Principal

**Criar:** `/frontend/mw-system-admin/src/app/pages/modules-dashboard/modules-dashboard.component.ts`

```typescript
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ModuleConfigService } from '../../services/module-config.service';
import { ModuleUsage, ModuleAdoption } from '../../models/module-config.model';

@Component({
  selector: 'app-modules-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatGridListModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './modules-dashboard.component.html',
  styleUrls: ['./modules-dashboard.component.scss']
})
export class ModulesDashboardComponent implements OnInit {
  loading = false;
  moduleUsage: ModuleUsage[] = [];
  moduleAdoption: ModuleAdoption[] = [];
  totalModules = 0;
  averageAdoption = 0;

  constructor(private moduleService: ModuleConfigService) {}

  ngOnInit(): void {
    this.loadDashboardData();
  }

  loadDashboardData(): void {
    this.loading = true;

    // Carregar dados em paralelo
    Promise.all([
      this.moduleService.getGlobalModuleUsage().toPromise(),
      this.moduleService.getModuleAdoption().toPromise()
    ]).then(([usage, adoption]) => {
      this.moduleUsage = usage || [];
      this.moduleAdoption = adoption || [];
      this.totalModules = this.moduleUsage.length;
      this.averageAdoption = this.calculateAverageAdoption();
      this.loading = false;
    }).catch(error => {
      console.error('Erro ao carregar dados:', error);
      this.loading = false;
    });
  }

  calculateAverageAdoption(): number {
    if (this.moduleUsage.length === 0) return 0;
    const sum = this.moduleUsage.reduce((acc, m) => acc + m.adoptionRate, 0);
    return sum / this.moduleUsage.length;
  }

  getCategoryColor(category: string): string {
    const colors: { [key: string]: string } = {
      'Core': '#4CAF50',
      'Advanced': '#2196F3',
      'Premium': '#FF9800',
      'Analytics': '#9C27B0'
    };
    return colors[category] || '#757575';
  }

  getAdoptionClass(rate: number): string {
    if (rate >= 75) return 'high-adoption';
    if (rate >= 50) return 'medium-adoption';
    return 'low-adoption';
  }
}
```

**Template:** `/frontend/mw-system-admin/src/app/pages/modules-dashboard/modules-dashboard.component.html`

```html
<div class="dashboard-container">
  <div class="header">
    <h1>Dashboard de Módulos</h1>
    <p class="subtitle">Visão geral do uso e adoção de módulos no sistema</p>
  </div>

  <!-- Loading -->
  <div *ngIf="loading" class="loading-container">
    <mat-spinner></mat-spinner>
  </div>

  <!-- Dashboard Content -->
  <div *ngIf="!loading" class="dashboard-content">
    
    <!-- KPI Cards -->
    <mat-grid-list cols="4" rowHeight="120px" gutterSize="16px" class="kpi-grid">
      
      <!-- Total de Módulos -->
      <mat-grid-tile>
        <mat-card class="kpi-card">
          <mat-icon class="kpi-icon">extension</mat-icon>
          <div class="kpi-content">
            <div class="kpi-value">{{ totalModules }}</div>
            <div class="kpi-label">Total de Módulos</div>
          </div>
        </mat-card>
      </mat-grid-tile>

      <!-- Taxa Média de Adoção -->
      <mat-grid-tile>
        <mat-card class="kpi-card">
          <mat-icon class="kpi-icon">trending_up</mat-icon>
          <div class="kpi-content">
            <div class="kpi-value">{{ averageAdoption | number:'1.1-1' }}%</div>
            <div class="kpi-label">Taxa Média de Adoção</div>
          </div>
        </mat-card>
      </mat-grid-tile>

      <!-- Módulos Mais Usados -->
      <mat-grid-tile>
        <mat-card class="kpi-card">
          <mat-icon class="kpi-icon">star</mat-icon>
          <div class="kpi-content">
            <div class="kpi-value">{{ getMostUsedModule() }}</div>
            <div class="kpi-label">Mais Usado</div>
          </div>
        </mat-card>
      </mat-grid-tile>

      <!-- Módulos Menos Usados -->
      <mat-grid-tile>
        <mat-card class="kpi-card">
          <mat-icon class="kpi-icon">warning</mat-icon>
          <div class="kpi-content">
            <div class="kpi-value">{{ getLeastUsedModule() }}</div>
            <div class="kpi-label">Menos Usado</div>
          </div>
        </mat-card>
      </mat-grid-tile>
    </mat-grid-list>

    <!-- Module Usage Table -->
    <mat-card class="modules-table-card">
      <mat-card-header>
        <mat-card-title>Uso de Módulos</mat-card-title>
      </mat-card-header>
      <mat-card-content>
        <table class="modules-table">
          <thead>
            <tr>
              <th>Módulo</th>
              <th>Categoria</th>
              <th>Clínicas</th>
              <th>Taxa de Adoção</th>
              <th>Ações</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let module of moduleUsage">
              <td>
                <div class="module-cell">
                  <mat-icon [style.color]="getCategoryColor(module.category)">
                    extension
                  </mat-icon>
                  <span class="module-name">{{ module.displayName }}</span>
                </div>
              </td>
              <td>
                <span class="category-badge" 
                      [style.background-color]="getCategoryColor(module.category)">
                  {{ module.category }}
                </span>
              </td>
              <td>{{ module.clinicsWithModuleEnabled }} / {{ module.totalClinics }}</td>
              <td>
                <div class="adoption-cell">
                  <div class="adoption-bar">
                    <div class="adoption-fill" 
                         [style.width.%]="module.adoptionRate"
                         [ngClass]="getAdoptionClass(module.adoptionRate)">
                    </div>
                  </div>
                  <span class="adoption-text">{{ module.adoptionRate | number:'1.1-1' }}%</span>
                </div>
              </td>
              <td>
                <button mat-icon-button 
                        [routerLink]="['/modules', module.moduleName]"
                        matTooltip="Ver detalhes">
                  <mat-icon>visibility</mat-icon>
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </mat-card-content>
    </mat-card>

    <!-- Chart: Adoção por Módulo -->
    <mat-card class="chart-card">
      <mat-card-header>
        <mat-card-title>Adoção por Módulo</mat-card-title>
      </mat-card-header>
      <mat-card-content>
        <div id="adoptionChart"></div>
      </mat-card-content>
    </mat-card>
  </div>
</div>
```

**Styles:** `/frontend/mw-system-admin/src/app/pages/modules-dashboard/modules-dashboard.component.scss`

```scss
.dashboard-container {
  padding: 24px;
  
  .header {
    margin-bottom: 24px;
    
    h1 {
      font-size: 28px;
      font-weight: 500;
      margin: 0 0 8px 0;
    }
    
    .subtitle {
      color: rgba(0, 0, 0, 0.6);
      margin: 0;
    }
  }

  .loading-container {
    display: flex;
    justify-content: center;
    padding: 48px;
  }

  .dashboard-content {
    display: flex;
    flex-direction: column;
    gap: 24px;
  }

  .kpi-grid {
    margin-bottom: 16px;
  }

  .kpi-card {
    width: 100%;
    height: 100%;
    display: flex;
    align-items: center;
    padding: 16px;
    
    .kpi-icon {
      font-size: 48px;
      width: 48px;
      height: 48px;
      color: #1976d2;
      margin-right: 16px;
    }
    
    .kpi-content {
      flex: 1;
      
      .kpi-value {
        font-size: 32px;
        font-weight: 600;
        line-height: 1;
        margin-bottom: 4px;
      }
      
      .kpi-label {
        font-size: 14px;
        color: rgba(0, 0, 0, 0.6);
      }
    }
  }

  .modules-table-card {
    .modules-table {
      width: 100%;
      border-collapse: collapse;
      
      thead {
        background-color: #f5f5f5;
        
        th {
          text-align: left;
          padding: 12px 16px;
          font-weight: 600;
          border-bottom: 2px solid #e0e0e0;
        }
      }
      
      tbody {
        tr {
          &:hover {
            background-color: #fafafa;
          }
          
          td {
            padding: 12px 16px;
            border-bottom: 1px solid #e0e0e0;
          }
        }
      }
      
      .module-cell {
        display: flex;
        align-items: center;
        gap: 8px;
        
        mat-icon {
          font-size: 20px;
          width: 20px;
          height: 20px;
        }
        
        .module-name {
          font-weight: 500;
        }
      }
      
      .category-badge {
        display: inline-block;
        padding: 4px 12px;
        border-radius: 12px;
        font-size: 12px;
        font-weight: 500;
        color: white;
      }
      
      .adoption-cell {
        display: flex;
        align-items: center;
        gap: 12px;
        
        .adoption-bar {
          flex: 1;
          height: 8px;
          background-color: #e0e0e0;
          border-radius: 4px;
          overflow: hidden;
          
          .adoption-fill {
            height: 100%;
            transition: width 0.3s ease;
            
            &.high-adoption {
              background-color: #4CAF50;
            }
            
            &.medium-adoption {
              background-color: #FF9800;
            }
            
            &.low-adoption {
              background-color: #F44336;
            }
          }
        }
        
        .adoption-text {
          font-weight: 500;
          min-width: 50px;
        }
      }
    }
  }

  .chart-card {
    min-height: 400px;
  }
}
```

---

### 3. Gestão de Módulos por Plano (3-4 dias)

#### 3.1. Componente de Configuração de Planos

**Criar:** `/frontend/mw-system-admin/src/app/pages/plan-modules/plan-modules.component.ts`

```typescript
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { SubscriptionPlanService } from '../../services/subscription-plan.service';
import { ModuleConfigService } from '../../services/module-config.service';
import { SubscriptionPlan } from '../../models/subscription-plan.model';
import { ModuleInfo } from '../../models/module-config.model';

@Component({
  selector: 'app-plan-modules',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatSelectModule,
    MatCheckboxModule,
    MatButtonModule,
    MatSnackBarModule
  ],
  templateUrl: './plan-modules.component.html',
  styleUrls: ['./plan-modules.component.scss']
})
export class PlanModulesComponent implements OnInit {
  plans: SubscriptionPlan[] = [];
  modules: ModuleInfo[] = [];
  selectedPlan?: SubscriptionPlan;
  moduleForm!: FormGroup;
  loading = false;

  constructor(
    private fb: FormBuilder,
    private planService: SubscriptionPlanService,
    private moduleService: ModuleConfigService,
    private snackBar: MatSnackBar
  ) {
    this.moduleForm = this.fb.group({});
  }

  ngOnInit(): void {
    this.loadPlans();
    this.loadModules();
  }

  loadPlans(): void {
    this.planService.getAllPlans().subscribe({
      next: (plans) => {
        this.plans = plans;
      },
      error: (error) => {
        console.error('Erro ao carregar planos:', error);
        this.snackBar.open('Erro ao carregar planos', 'Fechar', { duration: 3000 });
      }
    });
  }

  loadModules(): void {
    this.moduleService.getModulesInfo().subscribe({
      next: (modules) => {
        this.modules = modules;
        this.buildForm();
      },
      error: (error) => {
        console.error('Erro ao carregar módulos:', error);
      }
    });
  }

  buildForm(): void {
    const controls: { [key: string]: any } = {};
    this.modules.forEach(module => {
      controls[module.name] = [false];
    });
    this.moduleForm = this.fb.group(controls);
  }

  onPlanSelected(plan: SubscriptionPlan): void {
    this.selectedPlan = plan;
    this.updateFormValues();
  }

  updateFormValues(): void {
    if (!this.selectedPlan) return;

    const enabledModules = this.selectedPlan.enabledModules || [];
    
    this.modules.forEach(module => {
      const isEnabled = enabledModules.includes(module.name) || 
                       this.isModuleEnabledInPlan(module.name);
      this.moduleForm.patchValue({
        [module.name]: isEnabled
      });
      
      // Desabilitar módulos core
      if (module.isCore) {
        this.moduleForm.get(module.name)?.disable();
      }
    });
  }

  isModuleEnabledInPlan(moduleName: string): boolean {
    if (!this.selectedPlan) return false;
    
    // Verificar propriedades antigas
    switch (moduleName) {
      case 'Reports':
        return this.selectedPlan.hasReports;
      case 'WhatsAppIntegration':
        return this.selectedPlan.hasWhatsAppIntegration;
      case 'SMSNotifications':
        return this.selectedPlan.hasSMSNotifications;
      case 'TissExport':
        return this.selectedPlan.hasTissExport;
      default:
        return false;
    }
  }

  saveModules(): void {
    if (!this.selectedPlan) return;

    this.loading = true;
    const enabledModules = Object.keys(this.moduleForm.value)
      .filter(key => this.moduleForm.value[key]);

    this.planService.updatePlanModules(this.selectedPlan.id, enabledModules)
      .subscribe({
        next: () => {
          this.snackBar.open('Módulos atualizados com sucesso', 'Fechar', { 
            duration: 3000 
          });
          this.loading = false;
        },
        error: (error) => {
          console.error('Erro ao salvar:', error);
          this.snackBar.open('Erro ao atualizar módulos', 'Fechar', { 
            duration: 3000 
          });
          this.loading = false;
        }
      });
  }

  getModulesByCategory(category: string): ModuleInfo[] {
    return this.modules.filter(m => m.category === category);
  }

  getCategories(): string[] {
    return [...new Set(this.modules.map(m => m.category))];
  }
}
```

**Template:** `/frontend/mw-system-admin/src/app/pages/plan-modules/plan-modules.component.html`

```html
<div class="plan-modules-container">
  <div class="header">
    <h1>Configuração de Módulos por Plano</h1>
    <p class="subtitle">Defina quais módulos estão disponíveis em cada plano de assinatura</p>
  </div>

  <div class="content">
    <!-- Seleção de Plano -->
    <mat-card class="plan-selector-card">
      <mat-card-header>
        <mat-card-title>Selecione um Plano</mat-card-title>
      </mat-card-header>
      <mat-card-content>
        <mat-form-field appearance="outline" class="plan-select">
          <mat-label>Plano de Assinatura</mat-label>
          <mat-select [(value)]="selectedPlan" (selectionChange)="onPlanSelected($event.value)">
            <mat-option *ngFor="let plan of plans" [value]="plan">
              {{ plan.name }} - R$ {{ plan.monthlyPrice | number:'1.2-2' }}/mês
            </mat-option>
          </mat-select>
        </mat-form-field>

        <div *ngIf="selectedPlan" class="plan-info">
          <p><strong>Descrição:</strong> {{ selectedPlan.description }}</p>
          <p><strong>Tipo:</strong> {{ selectedPlan.type }}</p>
          <p><strong>Usuários:</strong> até {{ selectedPlan.maxUsers }}</p>
          <p><strong>Pacientes:</strong> até {{ selectedPlan.maxPatients }}</p>
        </div>
      </mat-card-content>
    </mat-card>

    <!-- Configuração de Módulos -->
    <mat-card *ngIf="selectedPlan" class="modules-config-card">
      <mat-card-header>
        <mat-card-title>Módulos Disponíveis</mat-card-title>
        <mat-card-subtitle>
          Selecione os módulos que estarão disponíveis no plano {{ selectedPlan.name }}
        </mat-card-subtitle>
      </mat-card-header>
      
      <mat-card-content>
        <form [formGroup]="moduleForm">
          <div *ngFor="let category of getCategories()" class="category-section">
            <h3 class="category-title">{{ category }}</h3>
            
            <div class="modules-grid">
              <div *ngFor="let module of getModulesByCategory(category)" 
                   class="module-item"
                   [class.disabled]="module.isCore">
                <mat-checkbox 
                  [formControlName]="module.name"
                  [disabled]="module.isCore">
                  <div class="module-info">
                    <mat-icon>{{ module.icon }}</mat-icon>
                    <div class="module-text">
                      <span class="module-name">{{ module.displayName }}</span>
                      <span class="module-description">{{ module.description }}</span>
                      <span *ngIf="module.isCore" class="core-badge">CORE</span>
                      <span *ngIf="module.requiredModules.length > 0" class="required-info">
                        Requer: {{ module.requiredModules.join(', ') }}
                      </span>
                    </div>
                  </div>
                </mat-checkbox>
              </div>
            </div>
          </div>
        </form>
      </mat-card-content>

      <mat-card-actions>
        <button mat-raised-button color="primary" 
                (click)="saveModules()"
                [disabled]="loading || !moduleForm.valid">
          {{ loading ? 'Salvando...' : 'Salvar Configurações' }}
        </button>
        <button mat-button (click)="updateFormValues()">
          Cancelar
        </button>
      </mat-card-actions>
    </mat-card>
  </div>
</div>
```

---

### 4. Detalhes de Módulo (2 dias)

**Criar página de detalhes que mostra:**
- Informações completas do módulo
- Lista de clínicas usando o módulo
- Histórico de mudanças
- Gráfico de adoção ao longo do tempo
- Ações: enable/disable global

---

### 5. Rotas e Navegação (1 dia)

**Atualizar:** `/frontend/mw-system-admin/src/app/app.routes.ts`

```typescript
export const routes: Routes = [
  // ... rotas existentes
  
  {
    path: 'modules',
    children: [
      {
        path: '',
        component: ModulesDashboardComponent,
        title: 'Dashboard de Módulos'
      },
      {
        path: 'plans',
        component: PlanModulesComponent,
        title: 'Módulos por Plano'
      },
      {
        path: ':moduleName',
        component: ModuleDetailsComponent,
        title: 'Detalhes do Módulo'
      }
    ]
  }
];
```

**Atualizar menu de navegação:**

```html
<mat-nav-list>
  <!-- ... itens existentes -->
  
  <mat-list-item [routerLink]="['/modules']" routerLinkActive="active">
    <mat-icon matListItemIcon>extension</mat-icon>
    <span matListItemTitle>Módulos</span>
  </mat-list-item>
  
  <mat-list-item [routerLink]="['/modules/plans']" routerLinkActive="active">
    <mat-icon matListItemIcon>assignment</mat-icon>
    <span matListItemTitle>Módulos por Plano</span>
  </mat-list-item>
</mat-nav-list>
```

---

## ✅ Critérios de Sucesso

### Funcional
- ✅ Dashboard mostra métricas corretas
- ✅ Configuração de planos funciona perfeitamente
- ✅ Filtros e buscas funcionam
- ✅ Feedback visual claro (loading, success, errors)

### Técnico
- ✅ Componentes standalone Angular 20
- ✅ TypeScript strict mode
- ✅ Responsivo (desktop, tablet, mobile)
- ✅ Acessível (WCAG 2.1)

### UX/UI
- ✅ Interface intuitiva
- ✅ Feedback visual adequado
- ✅ Performance: load < 2s
- ✅ Design consistente com o sistema

---

## 📊 Estrutura de Arquivos Final

```
/frontend/mw-system-admin/src/app/
├── models/
│   └── module-config.model.ts
├── services/
│   └── module-config.service.ts
└── pages/
    ├── modules-dashboard/
    │   ├── modules-dashboard.component.ts
    │   ├── modules-dashboard.component.html
    │   └── modules-dashboard.component.scss
    ├── plan-modules/
    │   ├── plan-modules.component.ts
    │   ├── plan-modules.component.html
    │   └── plan-modules.component.scss
    └── module-details/
        ├── module-details.component.ts
        ├── module-details.component.html
        └── module-details.component.scss
```

---

## ⏭️ Próximos Passos

Após completar este prompt:
1. Testar todas as telas no navegador
2. Validar responsividade
3. Verificar acessibilidade
4. Prosseguir para **03-PROMPT-FRONTEND-CLINIC.md**

---

> **Status:** 📝 Pronto para desenvolvimento  
> **Última Atualização:** 29 de Janeiro de 2026
