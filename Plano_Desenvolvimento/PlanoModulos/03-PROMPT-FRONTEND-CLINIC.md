# 🏥 PROMPT: Frontend Clínica - Configuração de Módulos por Clínica

> **Fase:** 3 de 5  
> **Duração Estimada:** 2-3 semanas  
> **Desenvolvedores:** 1-2  
> **Prioridade:** 🔥🔥🔥 ALTA  
> **Dependências:** 01-PROMPT-BACKEND.md, 02-PROMPT-FRONTEND-SYSTEM-ADMIN.md (concluídos)

---

## 📋 Contexto

### Situação Atual

O **medicwarehouse-app** é a aplicação Angular 20 usada pelas clínicas.

**Localização:**
```
/frontend/medicwarehouse-app/
  ├── src/
  │   ├── app/
  │   │   ├── pages/
  │   │   │   └── clinic-admin/  (área administrativa da clínica)
  │   │   ├── services/
  │   │   └── models/
```

### O Que Precisa Ser Desenvolvido

Criar interface para que **administradores da clínica** possam:

1. **Ver Módulos Disponíveis**
   - Módulos habilitados no plano
   - Status de cada módulo (habilitado/desabilitado)
   - Descrição e funcionalidades

2. **Gerenciar Módulos**
   - Habilitar/desabilitar módulos disponíveis
   - Configurar ajustes avançados por módulo
   - Ver histórico de mudanças

3. **Validações e Feedback**
   - Ver restrições do plano atual
   - Mensagens de upgrade quando necessário
   - Validação de módulos dependentes

---

## 🎯 Objetivos da Tarefa

### Objetivos Principais

1. Criar aba "Módulos" na área administrativa da clínica
2. Implementar interface de habilitar/desabilitar módulos
3. Adicionar validações de plano e dependências
4. Criar interface de configurações avançadas
5. Implementar feedback visual claro

### Benefícios Esperados

- 🎯 **Autonomia:** Clínica gerencia seus próprios módulos
- 🔒 **Segurança:** Validações de plano e permissões
- 🎨 **UX Intuitiva:** Interface simples e clara
- 📊 **Visibilidade:** Status claro de cada módulo

---

## 📝 Tarefas Detalhadas

### 1. Criar Models e Services (1-2 dias)

#### 1.1. Reutilizar Models do System Admin

**Copiar/adaptar:** `/frontend/medicwarehouse-app/src/app/models/module-config.model.ts`

```typescript
// Mesmos models do System Admin (ModuleInfo, ModuleConfig, etc.)
// Ver 02-PROMPT-FRONTEND-SYSTEM-ADMIN.md
```

#### 1.2. Service para API (perspectiva da clínica)

**Criar:** `/frontend/medicwarehouse-app/src/app/services/module-config.service.ts`

```typescript
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { ModuleConfig, ModuleInfo, ValidationResponse } from '../models/module-config.model';

@Injectable({
  providedIn: 'root'
})
export class ModuleConfigService {
  private apiUrl = `${environment.apiUrl}/api/module-config`;

  constructor(private http: HttpClient) {}

  // Listar módulos da clínica
  getModules(): Observable<ModuleConfig[]> {
    return this.http.get<ModuleConfig[]>(this.apiUrl);
  }

  // Informações de todos os módulos disponíveis
  getModulesInfo(): Observable<ModuleInfo[]> {
    return this.http.get<ModuleInfo[]>(`${this.apiUrl}/info`);
  }

  // Habilitar módulo
  enableModule(moduleName: string, reason?: string): Observable<any> {
    const body = reason ? { reason } : {};
    return this.http.post(`${this.apiUrl}/${moduleName}/enable`, body);
  }

  // Desabilitar módulo
  disableModule(moduleName: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/${moduleName}/disable`, {});
  }

  // Atualizar configuração do módulo
  updateModuleConfig(moduleName: string, configuration: string): Observable<any> {
    return this.http.put(`${this.apiUrl}/${moduleName}/config`, { configuration });
  }

  // Validar se módulo pode ser habilitado
  validateModule(moduleName: string): Observable<ValidationResponse> {
    return this.http.post<ValidationResponse>(`${this.apiUrl}/validate`, { moduleName });
  }

  // Histórico de mudanças do módulo
  getModuleHistory(moduleName: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/${moduleName}/history`);
  }
}
```

---

### 2. Página de Módulos da Clínica (3-4 dias)

#### 2.1. Componente Principal

**Criar:** `/frontend/medicwarehouse-app/src/app/pages/clinic-admin/modules/clinic-modules.component.ts`

```typescript
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { ModuleConfigService } from '../../../services/module-config.service';
import { ModuleConfig } from '../../../models/module-config.model';
import { ModuleConfigDialogComponent } from './module-config-dialog/module-config-dialog.component';

@Component({
  selector: 'app-clinic-modules',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatSlideToggleModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatDialogModule
  ],
  templateUrl: './clinic-modules.component.html',
  styleUrls: ['./clinic-modules.component.scss']
})
export class ClinicModulesComponent implements OnInit {
  modules: ModuleConfig[] = [];
  loading = false;

  constructor(
    private moduleService: ModuleConfigService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.loadModules();
  }

  loadModules(): void {
    this.loading = true;
    this.moduleService.getModules().subscribe({
      next: (modules) => {
        this.modules = modules;
        this.loading = false;
      },
      error: (error) => {
        console.error('Erro ao carregar módulos:', error);
        this.snackBar.open('Erro ao carregar módulos', 'Fechar', { duration: 3000 });
        this.loading = false;
      }
    });
  }

  toggleModule(module: ModuleConfig, event: any): void {
    const isEnabled = event.checked;

    // Validar antes de habilitar
    if (isEnabled) {
      this.moduleService.validateModule(module.moduleName).subscribe({
        next: (validation) => {
          if (validation.isValid) {
            this.enableModule(module);
          } else {
            // Reverter toggle
            event.source.checked = false;
            this.snackBar.open(validation.errorMessage || 'Não é possível habilitar este módulo', 
                              'Fechar', { duration: 5000 });
          }
        },
        error: (error) => {
          event.source.checked = false;
          this.snackBar.open('Erro ao validar módulo', 'Fechar', { duration: 3000 });
        }
      });
    } else {
      this.disableModule(module);
    }
  }

  enableModule(module: ModuleConfig): void {
    this.moduleService.enableModule(module.moduleName).subscribe({
      next: () => {
        module.isEnabled = true;
        this.snackBar.open(`${module.displayName} habilitado com sucesso`, 'Fechar', 
                          { duration: 3000 });
      },
      error: (error) => {
        console.error('Erro ao habilitar módulo:', error);
        this.snackBar.open('Erro ao habilitar módulo', 'Fechar', { duration: 3000 });
      }
    });
  }

  disableModule(module: ModuleConfig): void {
    this.moduleService.disableModule(module.moduleName).subscribe({
      next: () => {
        module.isEnabled = false;
        this.snackBar.open(`${module.displayName} desabilitado`, 'Fechar', 
                          { duration: 3000 });
      },
      error: (error) => {
        console.error('Erro ao desabilitar módulo:', error);
        this.snackBar.open('Erro ao desabilitar módulo', 'Fechar', { duration: 3000 });
      }
    });
  }

  openConfigDialog(module: ModuleConfig): void {
    const dialogRef = this.dialog.open(ModuleConfigDialogComponent, {
      width: '600px',
      data: { module }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadModules(); // Recarregar módulos
      }
    });
  }

  getCategoryModules(category: string): ModuleConfig[] {
    return this.modules.filter(m => m.category === category);
  }

  getCategories(): string[] {
    return [...new Set(this.modules.map(m => m.category))];
  }

  getCategoryIcon(category: string): string {
    const icons: { [key: string]: string } = {
      'Core': 'star',
      'Advanced': 'build',
      'Premium': 'workspace_premium',
      'Analytics': 'analytics'
    };
    return icons[category] || 'extension';
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
}
```

**Template:** `/frontend/medicwarehouse-app/src/app/pages/clinic-admin/modules/clinic-modules.component.html`

```html
<div class="clinic-modules-container">
  <div class="header">
    <h1>Módulos do Sistema</h1>
    <p class="subtitle">Gerencie os módulos disponíveis para sua clínica</p>
  </div>

  <div *ngIf="loading" class="loading">Carregando módulos...</div>

  <div *ngIf="!loading" class="modules-content">
    <div *ngFor="let category of getCategories()" class="category-section">
      
      <!-- Category Header -->
      <div class="category-header">
        <mat-icon [style.color]="getCategoryColor(category)">
          {{ getCategoryIcon(category) }}
        </mat-icon>
        <h2>{{ category }}</h2>
      </div>

      <!-- Modules Grid -->
      <div class="modules-grid">
        <mat-card *ngFor="let module of getCategoryModules(category)" 
                  class="module-card"
                  [class.disabled]="!module.isAvailableInPlan"
                  [class.enabled]="module.isEnabled">
          
          <mat-card-header>
            <div class="module-header">
              <div class="module-title">
                <mat-icon>{{ module.icon }}</mat-icon>
                <h3>{{ module.displayName }}</h3>
              </div>
              
              <!-- Toggle -->
              <mat-slide-toggle 
                *ngIf="module.isAvailableInPlan && !module.isCore"
                [checked]="module.isEnabled"
                (change)="toggleModule(module, $event)"
                color="primary">
              </mat-slide-toggle>

              <!-- Core Badge -->
              <mat-chip *ngIf="module.isCore" class="core-badge">
                ESSENCIAL
              </mat-chip>

              <!-- Plan Upgrade Badge -->
              <mat-chip *ngIf="!module.isAvailableInPlan" class="upgrade-badge">
                UPGRADE NECESSÁRIO
              </mat-chip>
            </div>
          </mat-card-header>

          <mat-card-content>
            <p class="module-description">{{ module.description }}</p>

            <!-- Required Modules -->
            <div *ngIf="module.requiredModules.length > 0" class="required-modules">
              <mat-icon class="info-icon">info</mat-icon>
              <span>Requer: {{ module.requiredModules.join(', ') }}</span>
            </div>

            <!-- Status Indicators -->
            <div class="status-indicators">
              <mat-chip *ngIf="module.isEnabled" class="status-chip enabled">
                <mat-icon>check_circle</mat-icon>
                Habilitado
              </mat-chip>
              <mat-chip *ngIf="!module.isEnabled && module.isAvailableInPlan" 
                        class="status-chip disabled">
                <mat-icon>cancel</mat-icon>
                Desabilitado
              </mat-chip>
            </div>
          </mat-card-content>

          <mat-card-actions>
            <button mat-button 
                    *ngIf="module.isEnabled && !module.isCore"
                    (click)="openConfigDialog(module)">
              <mat-icon>settings</mat-icon>
              Configurar
            </button>
            
            <button mat-button 
                    *ngIf="!module.isAvailableInPlan"
                    color="primary">
              <mat-icon>upgrade</mat-icon>
              Fazer Upgrade
            </button>
          </mat-card-actions>
        </mat-card>
      </div>
    </div>
  </div>
</div>
```

**Styles:** `/frontend/medicwarehouse-app/src/app/pages/clinic-admin/modules/clinic-modules.component.scss`

```scss
.clinic-modules-container {
  padding: 24px;
  max-width: 1400px;
  margin: 0 auto;

  .header {
    margin-bottom: 32px;

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

  .loading {
    text-align: center;
    padding: 48px;
    font-size: 18px;
    color: rgba(0, 0, 0, 0.6);
  }

  .modules-content {
    .category-section {
      margin-bottom: 48px;

      .category-header {
        display: flex;
        align-items: center;
        gap: 12px;
        margin-bottom: 24px;
        padding-bottom: 12px;
        border-bottom: 2px solid #e0e0e0;

        mat-icon {
          font-size: 32px;
          width: 32px;
          height: 32px;
        }

        h2 {
          margin: 0;
          font-size: 24px;
          font-weight: 500;
        }
      }

      .modules-grid {
        display: grid;
        grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
        gap: 24px;

        .module-card {
          transition: all 0.3s ease;

          &.disabled {
            opacity: 0.6;
            background-color: #fafafa;
          }

          &.enabled {
            border-left: 4px solid #4CAF50;
          }

          &:hover:not(.disabled) {
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
            transform: translateY(-2px);
          }

          .module-header {
            width: 100%;
            display: flex;
            justify-content: space-between;
            align-items: center;

            .module-title {
              display: flex;
              align-items: center;
              gap: 8px;

              mat-icon {
                color: #1976d2;
              }

              h3 {
                margin: 0;
                font-size: 18px;
                font-weight: 500;
              }
            }
          }

          .module-description {
            color: rgba(0, 0, 0, 0.7);
            margin: 16px 0;
            min-height: 40px;
          }

          .required-modules {
            display: flex;
            align-items: center;
            gap: 8px;
            margin-bottom: 12px;
            padding: 8px;
            background-color: #e3f2fd;
            border-radius: 4px;
            font-size: 13px;

            .info-icon {
              font-size: 18px;
              width: 18px;
              height: 18px;
              color: #1976d2;
            }
          }

          .status-indicators {
            margin-top: 12px;

            .status-chip {
              font-size: 12px;
              height: 28px;

              &.enabled {
                background-color: #e8f5e9;
                color: #2e7d32;

                mat-icon {
                  color: #2e7d32;
                  font-size: 16px;
                  width: 16px;
                  height: 16px;
                }
              }

              &.disabled {
                background-color: #ffebee;
                color: #c62828;

                mat-icon {
                  color: #c62828;
                  font-size: 16px;
                  width: 16px;
                  height: 16px;
                }
              }
            }
          }

          .core-badge {
            background-color: #4CAF50;
            color: white;
            font-weight: 500;
          }

          .upgrade-badge {
            background-color: #FF9800;
            color: white;
            font-weight: 500;
          }

          mat-card-actions {
            padding: 16px;
            display: flex;
            gap: 8px;
          }
        }
      }
    }
  }
}

// Responsive
@media (max-width: 768px) {
  .clinic-modules-container {
    padding: 16px;

    .modules-content .category-section .modules-grid {
      grid-template-columns: 1fr;
    }
  }
}
```

---

### 3. Dialog de Configuração Avançada (2 dias)

**Criar:** `/frontend/medicwarehouse-app/src/app/pages/clinic-admin/modules/module-config-dialog/module-config-dialog.component.ts`

```typescript
import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ModuleConfigService } from '../../../../services/module-config.service';
import { ModuleConfig } from '../../../../models/module-config.model';

@Component({
  selector: 'app-module-config-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule
  ],
  template: `
    <h2 mat-dialog-title>Configurar {{ data.module.displayName }}</h2>
    
    <mat-dialog-content>
      <p>{{ data.module.description }}</p>
      
      <form [formGroup]="configForm">
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Configurações (JSON)</mat-label>
          <textarea matInput 
                    formControlName="configuration" 
                    rows="10"
                    placeholder='{"option1": "value1"}'></textarea>
          <mat-hint>Configure opções específicas do módulo em formato JSON</mat-hint>
        </mat-form-field>
      </form>
    </mat-dialog-content>

    <mat-dialog-actions align="end">
      <button mat-button (click)="onCancel()">Cancelar</button>
      <button mat-raised-button color="primary" 
              (click)="onSave()"
              [disabled]="!configForm.valid || saving">
        {{ saving ? 'Salvando...' : 'Salvar' }}
      </button>
    </mat-dialog-actions>
  `,
  styles: [`
    .full-width {
      width: 100%;
    }
  `]
})
export class ModuleConfigDialogComponent {
  configForm: FormGroup;
  saving = false;

  constructor(
    public dialogRef: MatDialogRef<ModuleConfigDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { module: ModuleConfig },
    private fb: FormBuilder,
    private moduleService: ModuleConfigService,
    private snackBar: MatSnackBar
  ) {
    this.configForm = this.fb.group({
      configuration: [data.module.configuration || '{}']
    });
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }

  onSave(): void {
    this.saving = true;
    const config = this.configForm.value.configuration;

    this.moduleService.updateModuleConfig(this.data.module.moduleName, config)
      .subscribe({
        next: () => {
          this.snackBar.open('Configuração salva com sucesso', 'Fechar', { duration: 3000 });
          this.dialogRef.close(true);
        },
        error: (error) => {
          console.error('Erro ao salvar:', error);
          this.snackBar.open('Erro ao salvar configuração', 'Fechar', { duration: 3000 });
          this.saving = false;
        }
      });
  }
}
```

---

### 4. Adicionar à Navegação (30 min)

**Atualizar:** `/frontend/medicwarehouse-app/src/app/pages/clinic-admin/clinic-admin.routes.ts`

```typescript
export const clinicAdminRoutes: Routes = [
  // ... rotas existentes
  
  {
    path: 'modules',
    component: ClinicModulesComponent,
    title: 'Módulos'
  }
];
```

**Adicionar ao menu:**

```html
<mat-list-item [routerLink]="['modules']" routerLinkActive="active">
  <mat-icon matListItemIcon>extension</mat-icon>
  <span matListItemTitle>Módulos</span>
</mat-list-item>
```

---

## ✅ Critérios de Sucesso

### Funcional
- ✅ Clínica vê apenas módulos do seu plano
- ✅ Toggle habilitar/desabilitar funciona
- ✅ Validações de plano funcionam
- ✅ Configurações avançadas salvam corretamente
- ✅ Feedback visual claro

### Técnico
- ✅ Componentes standalone Angular 20
- ✅ Responsivo e acessível
- ✅ Performance adequada
- ✅ Código limpo e documentado

### UX/UI
- ✅ Interface intuitiva
- ✅ Mensagens claras de erro/sucesso
- ✅ Visual consistente com o sistema
- ✅ Fácil de usar

---

## ⏭️ Próximos Passos

Após completar este prompt:
1. Testar fluxo completo de habilitar/desabilitar
2. Validar mensagens de erro
3. Testar responsividade
4. Prosseguir para **04-PROMPT-TESTES.md**

---

> **Status:** ✅ **IMPLEMENTADO** (29 de Janeiro de 2026)  
> **Última Atualização:** 29 de Janeiro de 2026
