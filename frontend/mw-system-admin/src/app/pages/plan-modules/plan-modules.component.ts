import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { SystemAdminService } from '../../services/system-admin';
import { ModuleConfigService } from '../../services/module-config.service';
import { SubscriptionPlan } from '../../models/system-admin.model';
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
    MatSnackBarModule,
    MatIconModule,
    MatProgressSpinnerModule
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
  loadingData = false;

  constructor(
    private fb: FormBuilder,
    private systemAdminService: SystemAdminService,
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
    this.loadingData = true;
    this.systemAdminService.getSubscriptionPlans().subscribe({
      next: (plans) => {
        this.plans = plans;
        this.loadingData = false;
      },
      error: (error) => {
        console.error('Erro ao carregar planos:', error);
        this.snackBar.open('Erro ao carregar planos', 'Fechar', { duration: 3000 });
        this.loadingData = false;
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
        this.snackBar.open('Erro ao carregar módulos', 'Fechar', { duration: 3000 });
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
      } else {
        this.moduleForm.get(module.name)?.enable();
      }
    });
  }

  isModuleEnabledInPlan(moduleName: string): boolean {
    if (!this.selectedPlan) return false;
    
    // Verificar propriedades antigas
    switch (moduleName) {
      case 'Reports':
        return this.selectedPlan.hasReports || false;
      case 'WhatsAppIntegration':
        return this.selectedPlan.hasWhatsAppIntegration || false;
      case 'SMSNotifications':
        return this.selectedPlan.hasSMSNotifications || false;
      case 'TissExport':
        return this.selectedPlan.hasTissExport || false;
      default:
        return false;
    }
  }

  saveModules(): void {
    if (!this.selectedPlan) return;

    this.loading = true;
    const formValues = this.moduleForm.getRawValue();
    const enabledModules = Object.keys(formValues)
      .filter(key => formValues[key] === true);

    this.systemAdminService.updatePlanModules(this.selectedPlan.id, enabledModules)
      .subscribe({
        next: () => {
          this.snackBar.open('Módulos atualizados com sucesso', 'Fechar', { 
            duration: 3000 
          });
          this.loading = false;
          this.loadPlans(); // Reload to get updated data
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

  cancelChanges(): void {
    this.updateFormValues();
  }
}
