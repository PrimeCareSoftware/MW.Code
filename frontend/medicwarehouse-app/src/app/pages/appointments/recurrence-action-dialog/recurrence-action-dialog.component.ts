import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatRadioModule } from '@angular/material/radio';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RecurringDeleteScope, RecurringDeleteScopeLabels, RecurringDeleteScopeDescriptions } from '../../../models/appointment.model';

export interface RecurrenceActionDialogData {
  action: 'update' | 'delete';
  blockDate: Date;
  totalOccurrences?: number;
}

export interface RecurrenceActionDialogResult {
  scope: RecurringDeleteScope;
  reason?: string;
}

export type RecurrenceAction = 'single' | 'series';

@Component({
  selector: 'app-recurrence-action-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatIconModule,
    MatRadioModule,
    MatFormFieldModule,
    MatInputModule
  ],
  template: `
    <h2 mat-dialog-title>
      <mat-icon>{{ data.action === 'delete' ? 'delete' : 'edit' }}</mat-icon>
      {{ data.action === 'delete' ? 'Excluir Bloqueio Recorrente' : 'Editar Bloqueio' }}
    </h2>
    
    <mat-dialog-content>
      <div class="info-message">
        <mat-icon>info</mat-icon>
        <span>
          Este bloqueio faz parte de uma série recorrente.
          Como deseja proceder?
        </span>
      </div>
      
      <form [formGroup]="actionForm">
        <div class="scope-options">
          <mat-radio-group formControlName="scope">
            <mat-radio-button [value]="RecurringDeleteScope.ThisOccurrence">
              <div class="option-content">
                <strong>{{ getScopeLabel(RecurringDeleteScope.ThisOccurrence) }}</strong>
                <span class="description">
                  {{ getScopeDescription(RecurringDeleteScope.ThisOccurrence) }}
                </span>
              </div>
            </mat-radio-button>
            
            <mat-radio-button [value]="RecurringDeleteScope.ThisAndFuture">
              <div class="option-content">
                <strong>{{ getScopeLabel(RecurringDeleteScope.ThisAndFuture) }}</strong>
                <span class="description">
                  {{ getScopeDescription(RecurringDeleteScope.ThisAndFuture) }}
                </span>
              </div>
            </mat-radio-button>
            
            <mat-radio-button [value]="RecurringDeleteScope.AllInSeries">
              <div class="option-content">
                <strong>{{ getScopeLabel(RecurringDeleteScope.AllInSeries) }}</strong>
                <span class="description">
                  {{ getScopeDescription(RecurringDeleteScope.AllInSeries) }}
                </span>
              </div>
            </mat-radio-button>
          </mat-radio-group>
        </div>

        <mat-form-field appearance="outline" class="full-width reason-field">
          <mat-label>Motivo (opcional)</mat-label>
          <textarea 
            matInput 
            formControlName="reason"
            rows="3"
            placeholder="Informe o motivo para auditoria..."
            maxlength="500">
          </textarea>
          <mat-hint align="end">{{ actionForm.get('reason')?.value?.length || 0 }} / 500</mat-hint>
        </mat-form-field>
      </form>
    </mat-dialog-content>

    <mat-dialog-actions align="end">
      <button mat-button (click)="onCancel()">
        Cancelar
      </button>
      <button 
        mat-raised-button 
        color="warn"
        (click)="onConfirm()"
        [disabled]="!actionForm.valid">
        <mat-icon>delete</mat-icon>
        Excluir
      </button>
    </mat-dialog-actions>
  `,
  styles: [`
    h2 {
      display: flex;
      align-items: center;
      gap: 8px;
      
      mat-icon {
        color: #f44336;
      }
    }

    .info-message {
      display: flex;
      align-items: center;
      gap: 12px;
      padding: 12px;
      margin-bottom: 24px;
      background-color: #e3f2fd;
      border-left: 4px solid #2196f3;
      border-radius: 4px;
      
      mat-icon {
        color: #2196f3;
      }
      
      span {
        font-size: 14px;
        color: rgba(0, 0, 0, 0.87);
      }
    }

    .scope-options {
      display: flex;
      flex-direction: column;
      gap: 16px;
      margin-bottom: 24px;
      
      mat-radio-button {
        display: flex;
        align-items: flex-start;
      }
    }

    .option-content {
      display: flex;
      flex-direction: column;
      gap: 4px;
      margin-left: 8px;
    }

    .description {
      font-size: 13px;
      color: rgba(0, 0, 0, 0.54);
      line-height: 1.4;
    }

    .full-width {
      width: 100%;
    }

    .reason-field {
      margin-top: 8px;
    }

    mat-dialog-content {
      min-width: 550px;
      padding: 20px 24px;
      overflow-y: auto;
      max-height: 70vh;
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
export class RecurrenceActionDialogComponent {
  RecurringDeleteScope = RecurringDeleteScope;
  actionForm: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<RecurrenceActionDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: RecurrenceActionDialogData,
    private fb: FormBuilder
  ) {
    this.actionForm = this.fb.group({
      scope: [RecurringDeleteScope.ThisOccurrence, Validators.required],
      reason: ['']
    });
  }

  getScopeLabel(scope: RecurringDeleteScope): string {
    return RecurringDeleteScopeLabels[scope];
  }

  getScopeDescription(scope: RecurringDeleteScope): string {
    return RecurringDeleteScopeDescriptions[scope];
  }

  formatDate(date: Date): string {
    return new Date(date).toLocaleDateString('pt-BR', {
      day: '2-digit',
      month: 'long',
      year: 'numeric'
    });
  }

  onCancel(): void {
    this.dialogRef.close(null);
  }

  onConfirm(): void {
    if (this.actionForm.valid) {
      const result: RecurrenceActionDialogResult = {
        scope: this.actionForm.get('scope')?.value,
        reason: this.actionForm.get('reason')?.value?.trim() || undefined
      };
      this.dialogRef.close(result);
    }
  }
}
