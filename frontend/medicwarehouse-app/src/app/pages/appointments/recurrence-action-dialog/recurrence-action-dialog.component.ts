import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatRadioModule } from '@angular/material/radio';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

export interface RecurrenceActionDialogData {
  action: 'update' | 'delete';
  blockDate: Date;
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
    MatRadioModule
  ],
  template: `
    <h2 mat-dialog-title>
      <mat-icon>{{ data.action === 'delete' ? 'delete' : 'edit' }}</mat-icon>
      {{ data.action === 'delete' ? 'Excluir Bloqueio' : 'Editar Bloqueio' }}
    </h2>
    
    <mat-dialog-content>
      <p class="info-text">
        Este bloqueio faz parte de uma série recorrente.
      </p>
      
      <form [formGroup]="actionForm">
        <mat-radio-group formControlName="action" class="action-options">
          <mat-radio-button value="single">
            <div class="option-content">
              <strong>Apenas este bloqueio</strong>
              <span class="option-description">
                {{ data.action === 'delete' ? 'Remove' : 'Altera' }} somente o bloqueio do dia 
                {{ formatDate(data.blockDate) }}
              </span>
            </div>
          </mat-radio-button>
          
          <mat-radio-button value="series">
            <div class="option-content">
              <strong>Toda a série</strong>
              <span class="option-description">
                {{ data.action === 'delete' ? 'Remove' : 'Altera' }} todos os bloqueios desta série recorrente
              </span>
            </div>
          </mat-radio-button>
        </mat-radio-group>
      </form>
    </mat-dialog-content>

    <mat-dialog-actions align="end">
      <button mat-button (click)="onCancel()">
        Cancelar
      </button>
      <button 
        mat-raised-button 
        [color]="data.action === 'delete' ? 'warn' : 'primary'"
        (click)="onConfirm()"
        [disabled]="!actionForm.valid">
        <mat-icon>{{ data.action === 'delete' ? 'delete' : 'check' }}</mat-icon>
        {{ data.action === 'delete' ? 'Excluir' : 'Confirmar' }}
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

    .info-text {
      margin-bottom: 24px;
      color: rgba(0, 0, 0, 0.6);
      font-size: 14px;
    }

    .action-options {
      display: flex;
      flex-direction: column;
      gap: 16px;
    }

    .option-content {
      display: flex;
      flex-direction: column;
      gap: 4px;
      margin-left: 8px;
    }

    .option-description {
      font-size: 13px;
      color: rgba(0, 0, 0, 0.54);
    }

    mat-dialog-content {
      min-width: 450px;
      padding: 20px 24px;
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
  actionForm: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<RecurrenceActionDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: RecurrenceActionDialogData,
    private fb: FormBuilder
  ) {
    this.actionForm = this.fb.group({
      action: ['single', Validators.required]
    });
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
      const action = this.actionForm.get('action')?.value as RecurrenceAction;
      this.dialogRef.close(action);
    }
  }
}
