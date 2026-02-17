import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ModuleConfigService } from '../../../../services/module-config.service';
import { ModuleConfig } from '../../../../models/module-config.model';

/**
 * Dialog component for configuring module settings
 */
@Component({
  selector: 'app-module-config-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule
  ],
  template: `
    <h2 mat-dialog-title>
      <mat-icon>{{ data.module.icon }}</mat-icon>
      Configurar {{ data.module.displayName }}
    </h2>
    
    <mat-dialog-content>
      <p class="description">{{ data.module.description }}</p>
      
      <!-- Configuration Help -->
      <div class="help-section" *ngIf="data.module.configurationHelp">
        <mat-icon>help_outline</mat-icon>
        <p>{{ data.module.configurationHelp }}</p>
      </div>
      
      <!-- Configuration Example -->
      <div class="example-section" *ngIf="data.module.configurationExample">
        <h4>
          <mat-icon>code</mat-icon>
          Exemplo de Configuração
        </h4>
        <pre>{{ data.module.configurationExample }}</pre>
      </div>
      
      <form [formGroup]="configForm">
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Configurações (JSON)</mat-label>
          <textarea matInput 
                    formControlName="configuration" 
                    rows="10"
                    [placeholder]="getPlaceholder()"></textarea>
          <mat-hint>Configure opções específicas do módulo em formato JSON</mat-hint>
          <mat-error *ngIf="configForm.get('configuration')?.hasError('invalidJson')">
            JSON inválido. Por favor, corrija a sintaxe.
          </mat-error>
        </mat-form-field>
      </form>

      <div class="info-section">
        <mat-icon>info</mat-icon>
        <p>As configurações devem estar em formato JSON válido. Deixe vazio para usar configurações padrão.</p>
      </div>
    </mat-dialog-content>

    <mat-dialog-actions align="end">
      <button mat-button (click)="onCancel()">
        Cancelar
      </button>
      <button mat-raised-button color="primary" 
              (click)="onSave()"
              [disabled]="!configForm.valid || saving">
        <mat-icon *ngIf="!saving">save</mat-icon>
        {{ saving ? 'Salvando...' : 'Salvar' }}
      </button>
    </mat-dialog-actions>
  `,
  styles: [`
    h2 {
      display: flex;
      align-items: center;
      gap: 8px;
      
      mat-icon {
        color: #1976d2;
      }
    }

    .description {
      margin-bottom: 24px;
      color: rgba(0, 0, 0, 0.7);
      font-size: 14px;
      line-height: 1.5;
    }

    .help-section {
      display: flex;
      align-items: flex-start;
      gap: 8px;
      padding: 12px;
      background-color: #fff3e0;
      border-radius: 4px;
      margin-bottom: 16px;

      mat-icon {
        color: #f57c00;
        font-size: 20px;
        width: 20px;
        height: 20px;
        flex-shrink: 0;
      }

      p {
        margin: 0;
        font-size: 13px;
        color: rgba(0, 0, 0, 0.8);
        line-height: 1.5;
      }
    }

    .example-section {
      padding: 12px;
      background-color: #f5f5f5;
      border-radius: 4px;
      margin-bottom: 16px;

      h4 {
        display: flex;
        align-items: center;
        gap: 8px;
        margin: 0 0 8px 0;
        font-size: 14px;
        font-weight: 500;
        color: rgba(0, 0, 0, 0.87);

        mat-icon {
          font-size: 18px;
          width: 18px;
          height: 18px;
        }
      }

      pre {
        margin: 0;
        padding: 8px;
        background-color: #ffffff;
        border: 1px solid #e0e0e0;
        border-radius: 4px;
        font-size: 12px;
        line-height: 1.4;
        overflow-x: auto;
        white-space: pre-wrap;
        word-wrap: break-word;
      }
    }

    .full-width {
      width: 100%;
    }

    .info-section {
      display: flex;
      align-items: flex-start;
      gap: 8px;
      padding: 12px;
      background-color: #e3f2fd;
      border-radius: 4px;
      margin-top: 16px;

      mat-icon {
        color: #1976d2;
        font-size: 20px;
        width: 20px;
        height: 20px;
        flex-shrink: 0;
      }

      p {
        margin: 0;
        font-size: 13px;
        color: rgba(0, 0, 0, 0.7);
      }
    }

    mat-dialog-actions {
      padding: 16px 24px;
      gap: 8px;

      button {
        mat-icon {
          margin-right: 4px;
        }
      }
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
      configuration: [
        data.module.configuration || '{}',
        [this.jsonValidator]
      ]
    });
  }

  /**
   * Get placeholder text for configuration textarea
   */
  getPlaceholder(): string {
    return this.data.module.configurationExample || '{"option1": "value1", "option2": "value2"}';
  }

  /**
   * Custom validator for JSON format
   */
  jsonValidator(control: AbstractControl): ValidationErrors | null {
    if (!control.value) {
      return null;
    }

    try {
      JSON.parse(control.value);
      return null;
    } catch (e) {
      return { invalidJson: true };
    }
  }

  /**
   * Cancel and close dialog
   */
  onCancel(): void {
    this.dialogRef.close(false);
  }

  /**
   * Save configuration
   */
  onSave(): void {
    if (!this.configForm.valid) {
      return;
    }

    this.saving = true;
    const config = this.configForm.value.configuration;

    this.moduleService.updateModuleConfig(this.data.module.moduleName, config)
      .subscribe({
        next: () => {
          this.snackBar.open('Configuração salva com sucesso', 'Fechar', { 
            duration: 3000,
            panelClass: ['success-snackbar']
          });
          this.dialogRef.close(true);
        },
        error: (error) => {
          console.error('Erro ao salvar configuração:', error);
          this.snackBar.open(
            error.error?.message || 'Erro ao salvar configuração', 
            'Fechar', 
            { duration: 3000 }
          );
          this.saving = false;
        }
      });
  }
}
