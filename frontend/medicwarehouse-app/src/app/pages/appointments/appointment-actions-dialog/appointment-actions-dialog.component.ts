import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { Appointment } from '../../../models/appointment.model';

export interface AppointmentActionsDialogData {
  appointment: Appointment;
}

export interface AppointmentActionsDialogResult {
  action: 'reschedule' | 'cancel' | 'start' | 'details';
}

@Component({
  selector: 'app-appointment-actions-dialog',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MatButtonModule,
    MatIconModule,
    MatDividerModule
  ],
  template: `
    <h2 mat-dialog-title>
      <mat-icon>event</mat-icon>
      Ações do Agendamento
    </h2>
    
    <mat-dialog-content>
      <!-- Appointment Summary -->
      <div class="appointment-summary">
        <div class="summary-row">
          <mat-icon>person</mat-icon>
          <span class="label">Paciente:</span>
          <span class="value">{{ data.appointment.patientName }}</span>
        </div>
        
        <div class="summary-row">
          <mat-icon>medical_services</mat-icon>
          <span class="label">Profissional:</span>
          <span class="value">{{ data.appointment.professionalName || data.appointment.doctorName || 'Não informado' }}</span>
        </div>
        
        <div class="summary-row">
          <mat-icon>event</mat-icon>
          <span class="label">Data:</span>
          <span class="value">{{ formatDate(data.appointment.scheduledDate) }}</span>
        </div>
        
        <div class="summary-row">
          <mat-icon>schedule</mat-icon>
          <span class="label">Horário:</span>
          <span class="value">{{ data.appointment.scheduledTime }} ({{ data.appointment.durationMinutes }}min)</span>
        </div>
        
        <div class="summary-row">
          <mat-icon>category</mat-icon>
          <span class="label">Tipo:</span>
          <span class="value">{{ data.appointment.type }}</span>
        </div>
        
        <div class="summary-row">
          <mat-icon>info</mat-icon>
          <span class="label">Status:</span>
          <span class="value status" [class]="getStatusClass(data.appointment.status)">
            {{ getStatusLabel(data.appointment.status) }}
          </span>
        </div>
      </div>

      <mat-divider></mat-divider>

      <!-- Action Buttons -->
      <div class="action-buttons">
        <button 
          mat-raised-button 
          color="primary"
          class="action-btn"
          (click)="onStartAttendance()"
          [disabled]="!canStartAttendance()">
          <mat-icon>play_arrow</mat-icon>
          <div class="btn-content">
            <span class="btn-title">Iniciar Atendimento</span>
            <span class="btn-description">Começar a consulta agora</span>
          </div>
        </button>

        <button 
          mat-stroked-button
          class="action-btn"
          (click)="onReschedule()"
          [disabled]="!canReschedule()">
          <mat-icon>event_repeat</mat-icon>
          <div class="btn-content">
            <span class="btn-title">Remarcar Agendamento</span>
            <span class="btn-description">Alterar data e horário</span>
          </div>
        </button>

        <button 
          mat-stroked-button
          class="action-btn"
          (click)="onViewDetails()">
          <mat-icon>visibility</mat-icon>
          <div class="btn-content">
            <span class="btn-title">Ver Detalhes</span>
            <span class="btn-description">Visualizar informações completas</span>
          </div>
        </button>

        <button 
          mat-stroked-button
          color="warn"
          class="action-btn"
          (click)="onCancel()"
          [disabled]="!canCancel()">
          <mat-icon>cancel</mat-icon>
          <div class="btn-content">
            <span class="btn-title">Cancelar Agendamento</span>
            <span class="btn-description">Cancelar esta consulta</span>
          </div>
        </button>
      </div>
    </mat-dialog-content>

    <mat-dialog-actions align="end">
      <button mat-button (click)="onClose()">
        Fechar
      </button>
    </mat-dialog-actions>
  `,
  styles: [`
    h2 {
      display: flex;
      align-items: center;
      gap: 8px;
      margin: 0;
      
      mat-icon {
        color: #1976d2;
      }
    }

    mat-dialog-content {
      min-width: 500px;
      padding: 20px 24px;
      overflow-y: auto;
      max-height: 70vh;
    }

    .appointment-summary {
      display: flex;
      flex-direction: column;
      gap: 12px;
      margin-bottom: 20px;
    }

    .summary-row {
      display: flex;
      align-items: center;
      gap: 12px;
      
      mat-icon {
        color: rgba(0, 0, 0, 0.54);
        font-size: 20px;
        width: 20px;
        height: 20px;
      }
      
      .label {
        font-weight: 500;
        color: rgba(0, 0, 0, 0.87);
        min-width: 100px;
      }
      
      .value {
        color: rgba(0, 0, 0, 0.87);
        flex: 1;
        
        &.status {
          padding: 4px 12px;
          border-radius: 12px;
          font-size: 12px;
          font-weight: 600;
          text-transform: uppercase;
          display: inline-block;
          
          &.status-scheduled {
            background-color: #e3f2fd;
            color: #1976d2;
          }
          
          &.status-confirmed {
            background-color: #e8f5e9;
            color: #388e3c;
          }
          
          &.status-cancelled {
            background-color: #ffebee;
            color: #d32f2f;
          }
          
          &.status-completed {
            background-color: #f3e5f5;
            color: #7b1fa2;
          }
        }
      }
    }

    mat-divider {
      margin: 20px 0;
    }

    .action-buttons {
      display: flex;
      flex-direction: column;
      gap: 12px;
      margin-top: 20px;
    }

    .action-btn {
      display: flex;
      align-items: center;
      gap: 12px;
      padding: 16px;
      text-align: left;
      justify-content: flex-start;
      height: auto;
      
      mat-icon {
        font-size: 24px;
        width: 24px;
        height: 24px;
      }
      
      .btn-content {
        display: flex;
        flex-direction: column;
        gap: 2px;
        align-items: flex-start;
      }
      
      .btn-title {
        font-size: 14px;
        font-weight: 500;
      }
      
      .btn-description {
        font-size: 12px;
        opacity: 0.7;
      }
      
      &:disabled {
        opacity: 0.5;
      }
    }

    mat-dialog-actions {
      padding: 16px 24px;
      gap: 8px;
    }
  `]
})
export class AppointmentActionsDialogComponent {
  constructor(
    public dialogRef: MatDialogRef<AppointmentActionsDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: AppointmentActionsDialogData,
    private router: Router
  ) {}

  formatDate(dateString: string): string {
    const parts = dateString.split('-');
    if (parts.length !== 3) return dateString;
    
    const [year, month, day] = parts.map(Number);
    const date = new Date(year, month - 1, day);
    
    return date.toLocaleDateString('pt-BR', {
      day: '2-digit',
      month: 'long',
      year: 'numeric'
    });
  }

  getStatusClass(status: string): string {
    return `status-${status.toLowerCase()}`;
  }

  getStatusLabel(status: string): string {
    const labels: { [key: string]: string } = {
      'Scheduled': 'Agendado',
      'Confirmed': 'Confirmado',
      'Cancelled': 'Cancelado',
      'Completed': 'Concluído',
      'NoShow': 'Faltou',
      'InProgress': 'Em Andamento'
    };
    return labels[status] || status;
  }

  canStartAttendance(): boolean {
    const status = this.data.appointment.status;
    return status === 'Scheduled' || status === 'Confirmed';
  }

  canReschedule(): boolean {
    const status = this.data.appointment.status;
    return status === 'Scheduled' || status === 'Confirmed';
  }

  canCancel(): boolean {
    const status = this.data.appointment.status;
    return status === 'Scheduled' || status === 'Confirmed';
  }

  onStartAttendance(): void {
    this.dialogRef.close({ action: 'start' } as AppointmentActionsDialogResult);
    this.router.navigate(['/appointments', this.data.appointment.id, 'attendance']);
  }

  onReschedule(): void {
    this.dialogRef.close({ action: 'reschedule' } as AppointmentActionsDialogResult);
    // Navigate to appointment edit page or open reschedule dialog
    this.router.navigate(['/appointments', this.data.appointment.id, 'edit']);
  }

  onViewDetails(): void {
    this.dialogRef.close({ action: 'details' } as AppointmentActionsDialogResult);
    // Navigate to appointment details page
    this.router.navigate(['/appointments', this.data.appointment.id]);
  }

  onCancel(): void {
    this.dialogRef.close({ action: 'cancel' } as AppointmentActionsDialogResult);
    // Open cancel confirmation dialog or navigate to cancel page
    this.router.navigate(['/appointments', this.data.appointment.id, 'cancel']);
  }

  onClose(): void {
    this.dialogRef.close(null);
  }
}
