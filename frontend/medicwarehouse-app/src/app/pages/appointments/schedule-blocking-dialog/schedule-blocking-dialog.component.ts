import { Component, Inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatRadioModule } from '@angular/material/radio';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AppointmentService } from '../../../services/appointment';
import { 
  BlockedTimeSlot, Professional, 
  BlockedTimeSlotType, BlockedTimeSlotTypeLabels,
  RecurrenceFrequency, RecurrenceFrequencyLabels,
  RecurrenceDays, RecurrenceDaysLabels
} from '../../../models/appointment.model';

export interface ScheduleBlockingDialogData {
  clinicId: string;
  date?: Date;
  timeSlot?: { hour: number; minute: number };
  professionals: Professional[];
  blockedSlot?: BlockedTimeSlot; // For editing existing block
}

@Component({
  selector: 'app-schedule-blocking-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatSelectModule,
    MatCheckboxModule,
    MatRadioModule,
    MatDatepickerModule,
    MatNativeDateModule
  ],
  template: `
    <h2 mat-dialog-title>
      <mat-icon>block</mat-icon>
      {{ isEditMode() ? 'Editar Bloqueio' : 'Bloquear Agenda' }}
    </h2>
    
    <mat-dialog-content>
      <form [formGroup]="blockForm">
        <!-- Professional Selection -->
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Profissional</mat-label>
          <mat-select formControlName="professionalId">
            <mat-option [value]="null">Toda a clínica</mat-option>
            <mat-option *ngFor="let prof of data.professionals" [value]="prof.id">
              {{ prof.fullName }}{{ prof.specialty ? ' - ' + prof.specialty : '' }}
            </mat-option>
          </mat-select>
          <mat-hint>Deixe em branco para bloquear toda a clínica</mat-hint>
        </mat-form-field>

        <!-- Block Type Selection -->
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Tipo de Bloqueio</mat-label>
          <mat-select formControlName="type" required>
            <mat-option *ngFor="let type of blockTypes" [value]="type.value">
              {{ type.label }}
            </mat-option>
          </mat-select>
        </mat-form-field>

        <!-- Reason -->
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Motivo (opcional)</mat-label>
          <textarea matInput 
                    formControlName="reason" 
                    rows="2"
                    placeholder="Descreva o motivo do bloqueio"></textarea>
        </mat-form-field>

        <!-- Recurrence Options -->
        <div class="recurrence-section">
          <label class="section-label">Tipo de Bloqueio</label>
          <mat-radio-group formControlName="blockMode" class="radio-group">
            <mat-radio-button value="single">Bloqueio Único</mat-radio-button>
            <mat-radio-button value="recurring">Bloqueio Recorrente</mat-radio-button>
          </mat-radio-group>
        </div>

        <!-- Single Block Fields -->
        <div *ngIf="blockForm.get('blockMode')?.value === 'single'" class="single-block-section">
          <mat-form-field appearance="outline" class="full-width">
            <mat-label>Data</mat-label>
            <input matInput 
                   [matDatepicker]="datePicker" 
                   formControlName="date"
                   required>
            <mat-datepicker-toggle matSuffix [for]="datePicker"></mat-datepicker-toggle>
            <mat-datepicker #datePicker></mat-datepicker>
          </mat-form-field>

          <div class="time-row">
            <mat-form-field appearance="outline" class="half-width">
              <mat-label>Hora Inicial</mat-label>
              <input matInput 
                     type="time" 
                     formControlName="startTime"
                     required>
            </mat-form-field>

            <mat-form-field appearance="outline" class="half-width">
              <mat-label>Hora Final</mat-label>
              <input matInput 
                     type="time" 
                     formControlName="endTime"
                     required>
            </mat-form-field>
          </div>
        </div>

        <!-- Recurring Block Fields -->
        <div *ngIf="blockForm.get('blockMode')?.value === 'recurring'" class="recurring-block-section">
          <mat-form-field appearance="outline" class="full-width">
            <mat-label>Frequência</mat-label>
            <mat-select formControlName="frequency" required>
              <mat-option *ngFor="let freq of frequencies" [value]="freq.value">
                {{ freq.label }}
              </mat-option>
            </mat-select>
          </mat-form-field>

          <!-- Days of Week for Weekly recurrence -->
          <div *ngIf="blockForm.get('frequency')?.value === 2" class="days-selection">
            <label class="section-label">Dias da Semana</label>
            <div class="days-checkboxes">
              <mat-checkbox *ngFor="let day of daysOfWeek" 
                          [checked]="isDaySelected(day.value)"
                          (change)="toggleDay(day.value)">
                {{ day.label }}
              </mat-checkbox>
            </div>
          </div>

          <!-- Day of Month for Monthly recurrence -->
          <mat-form-field *ngIf="blockForm.get('frequency')?.value === 4" 
                          appearance="outline" 
                          class="full-width">
            <mat-label>Dia do Mês</mat-label>
            <input matInput 
                   type="number" 
                   formControlName="dayOfMonth"
                   min="1"
                   max="31"
                   required>
            <mat-hint>1-31</mat-hint>
          </mat-form-field>

          <div class="date-row">
            <mat-form-field appearance="outline" class="half-width">
              <mat-label>Data Inicial</mat-label>
              <input matInput 
                     [matDatepicker]="startDatePicker" 
                     formControlName="startDate"
                     required>
              <mat-datepicker-toggle matSuffix [for]="startDatePicker"></mat-datepicker-toggle>
              <mat-datepicker #startDatePicker></mat-datepicker>
            </mat-form-field>

            <mat-form-field appearance="outline" class="half-width">
              <mat-label>Data Final</mat-label>
              <input matInput 
                     [matDatepicker]="endDatePicker" 
                     formControlName="endDate">
              <mat-datepicker-toggle matSuffix [for]="endDatePicker"></mat-datepicker-toggle>
              <mat-datepicker #endDatePicker></mat-datepicker>
              <mat-hint>Opcional</mat-hint>
            </mat-form-field>
          </div>

          <div class="time-row">
            <mat-form-field appearance="outline" class="half-width">
              <mat-label>Hora Inicial</mat-label>
              <input matInput 
                     type="time" 
                     formControlName="startTime"
                     required>
            </mat-form-field>

            <mat-form-field appearance="outline" class="half-width">
              <mat-label>Hora Final</mat-label>
              <input matInput 
                     type="time" 
                     formControlName="endTime"
                     required>
            </mat-form-field>
          </div>

          <mat-form-field appearance="outline" class="full-width">
            <mat-label>Número de Ocorrências</mat-label>
            <input matInput 
                   type="number" 
                   formControlName="occurrencesCount"
                   min="1"
                   placeholder="Deixe em branco para usar data final">
            <mat-hint>Alternativa à data final</mat-hint>
          </mat-form-field>
        </div>

        @if (errorMessage()) {
          <div class="error-message">
            <mat-icon>error</mat-icon>
            {{ errorMessage() }}
          </div>
        }
      </form>

      <div class="info-section">
        <mat-icon>info</mat-icon>
        <p>O bloqueio impedirá que novos agendamentos sejam criados no horário especificado.</p>
      </div>
    </mat-dialog-content>

    <mat-dialog-actions align="end">
      <button mat-button (click)="onCancel()">
        Cancelar
      </button>
      <button mat-raised-button color="primary" 
              (click)="onSave()"
              [disabled]="!blockForm.valid || saving()">
        <mat-icon *ngIf="!saving()">{{ isEditMode() ? 'save' : 'block' }}</mat-icon>
        {{ saving() ? 'Salvando...' : (isEditMode() ? 'Atualizar' : 'Bloquear') }}
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

    .full-width {
      width: 100%;
      margin-bottom: 16px;
    }

    .half-width {
      width: calc(50% - 8px);
    }

    .time-row,
    .date-row {
      display: flex;
      gap: 16px;
      margin-bottom: 16px;
    }

    .recurrence-section {
      margin-bottom: 24px;
    }

    .section-label {
      display: block;
      font-weight: 500;
      margin-bottom: 8px;
      font-size: 14px;
      color: rgba(0, 0, 0, 0.87);
    }

    .radio-group {
      display: flex;
      flex-direction: column;
      gap: 8px;
    }

    .single-block-section,
    .recurring-block-section {
      padding: 16px;
      background-color: #f5f5f5;
      border-radius: 4px;
      margin-bottom: 16px;
    }

    .days-selection {
      margin-bottom: 16px;
    }

    .days-checkboxes {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(120px, 1fr));
      gap: 8px;
      padding: 8px 0;
    }

    .info-section {
      display: flex;
      align-items: flex-start;
      gap: 8px;
      padding: 12px;
      background-color: #fff3cd;
      border-radius: 4px;
      margin-top: 16px;

      mat-icon {
        color: #856404;
        font-size: 20px;
        width: 20px;
        height: 20px;
        flex-shrink: 0;
      }

      p {
        margin: 0;
        font-size: 13px;
        color: #856404;
      }
    }

    .error-message {
      display: flex;
      align-items: center;
      gap: 8px;
      padding: 12px;
      background-color: #ffebee;
      border-radius: 4px;
      margin-top: 16px;
      color: #c62828;

      mat-icon {
        font-size: 20px;
        width: 20px;
        height: 20px;
      }
    }

    mat-dialog-content {
      min-width: 500px;
      max-height: 80vh;
      overflow-y: auto;
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
export class ScheduleBlockingDialogComponent implements OnInit {
  blockForm: FormGroup;
  saving = signal(false);
  errorMessage = signal('');
  isEditMode = signal(false);
  
  selectedDays = signal(0); // Bitmask for selected days

  blockTypes = Object.entries(BlockedTimeSlotTypeLabels).map(([value, label]) => ({
    value: parseInt(value),
    label
  }));

  frequencies = Object.entries(RecurrenceFrequencyLabels).map(([value, label]) => ({
    value: parseInt(value),
    label
  })).filter(f => f.value !== RecurrenceFrequency.Custom); // Exclude custom for now

  daysOfWeek = [
    { value: RecurrenceDays.Sunday, label: RecurrenceDaysLabels[RecurrenceDays.Sunday] },
    { value: RecurrenceDays.Monday, label: RecurrenceDaysLabels[RecurrenceDays.Monday] },
    { value: RecurrenceDays.Tuesday, label: RecurrenceDaysLabels[RecurrenceDays.Tuesday] },
    { value: RecurrenceDays.Wednesday, label: RecurrenceDaysLabels[RecurrenceDays.Wednesday] },
    { value: RecurrenceDays.Thursday, label: RecurrenceDaysLabels[RecurrenceDays.Thursday] },
    { value: RecurrenceDays.Friday, label: RecurrenceDaysLabels[RecurrenceDays.Friday] },
    { value: RecurrenceDays.Saturday, label: RecurrenceDaysLabels[RecurrenceDays.Saturday] }
  ];

  constructor(
    public dialogRef: MatDialogRef<ScheduleBlockingDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ScheduleBlockingDialogData,
    private fb: FormBuilder,
    private appointmentService: AppointmentService,
    private snackBar: MatSnackBar
  ) {
    const initialDate = data.date || new Date();
    const initialTime = data.timeSlot 
      ? `${data.timeSlot.hour.toString().padStart(2, '0')}:${data.timeSlot.minute.toString().padStart(2, '0')}`
      : '08:00';
    const endTime = data.timeSlot 
      ? `${(data.timeSlot.hour + 1).toString().padStart(2, '0')}:${data.timeSlot.minute.toString().padStart(2, '0')}`
      : '09:00';

    this.blockForm = this.fb.group({
      professionalId: [null],
      type: [BlockedTimeSlotType.Unavailable, Validators.required],
      reason: [''],
      blockMode: ['single', Validators.required],
      // Single block fields
      date: [initialDate],
      // Recurring block fields
      frequency: [RecurrenceFrequency.Weekly],
      dayOfMonth: [1],
      startDate: [initialDate],
      endDate: [null],
      occurrencesCount: [null],
      // Common fields
      startTime: [initialTime, Validators.required],
      endTime: [endTime, Validators.required]
    });

    // If editing, populate form
    if (data.blockedSlot) {
      this.isEditMode.set(true);
      this.populateFormForEdit(data.blockedSlot);
    }
  }

  ngOnInit(): void {
    // Watch for block mode changes to update validators
    this.blockForm.get('blockMode')?.valueChanges.subscribe(mode => {
      this.updateValidators(mode);
    });

    this.updateValidators(this.blockForm.get('blockMode')?.value);
  }

  private updateValidators(mode: string): void {
    const dateControl = this.blockForm.get('date');
    const startDateControl = this.blockForm.get('startDate');
    const frequencyControl = this.blockForm.get('frequency');

    if (mode === 'single') {
      dateControl?.setValidators([Validators.required]);
      startDateControl?.clearValidators();
      frequencyControl?.clearValidators();
    } else {
      dateControl?.clearValidators();
      startDateControl?.setValidators([Validators.required]);
      frequencyControl?.setValidators([Validators.required]);
    }

    dateControl?.updateValueAndValidity();
    startDateControl?.updateValueAndValidity();
    frequencyControl?.updateValueAndValidity();
  }

  private populateFormForEdit(blockedSlot: BlockedTimeSlot): void {
    this.blockForm.patchValue({
      professionalId: blockedSlot.professionalId || null,
      type: blockedSlot.type,
      reason: blockedSlot.reason || '',
      blockMode: 'single', // Edit only supports single blocks for now
      date: new Date(blockedSlot.date),
      startTime: blockedSlot.startTime,
      endTime: blockedSlot.endTime
    });
  }

  isDaySelected(day: RecurrenceDays): boolean {
    return (this.selectedDays() & day) !== 0;
  }

  toggleDay(day: RecurrenceDays): void {
    const current = this.selectedDays();
    if (this.isDaySelected(day)) {
      this.selectedDays.set(current & ~day);
    } else {
      this.selectedDays.set(current | day);
    }
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }

  onSave(): void {
    if (!this.blockForm.valid) {
      this.errorMessage.set('Por favor, preencha todos os campos obrigatórios');
      return;
    }

    this.saving.set(true);
    this.errorMessage.set('');

    const formValue = this.blockForm.value;
    const isRecurring = formValue.blockMode === 'recurring';

    if (this.isEditMode()) {
      // Update existing block (only single blocks can be edited)
      this.updateBlock(formValue);
    } else if (isRecurring) {
      // Create recurring blocks
      this.createRecurringBlocks(formValue);
    } else {
      // Create single block
      this.createSingleBlock(formValue);
    }
  }

  private createSingleBlock(formValue: any): void {
    const blockData = {
      clinicId: this.data.clinicId,
      professionalId: formValue.professionalId || undefined,
      date: this.formatDate(formValue.date),
      startTime: formValue.startTime,
      endTime: formValue.endTime,
      type: formValue.type,
      reason: formValue.reason || undefined
    };

    this.appointmentService.createBlockedTimeSlot(blockData).subscribe({
      next: () => {
        this.snackBar.open('Bloqueio criado com sucesso', 'Fechar', { 
          duration: 3000,
          panelClass: ['success-snackbar']
        });
        this.dialogRef.close(true);
      },
      error: (error) => {
        console.error('Erro ao criar bloqueio:', error);
        this.errorMessage.set(error.error?.message || 'Erro ao criar bloqueio');
        this.saving.set(false);
      }
    });
  }

  private createRecurringBlocks(formValue: any): void {
    const frequency = formValue.frequency;
    const daysOfWeek = frequency === RecurrenceFrequency.Weekly ? this.selectedDays() : undefined;

    if (frequency === RecurrenceFrequency.Weekly && !daysOfWeek) {
      this.errorMessage.set('Selecione pelo menos um dia da semana');
      this.saving.set(false);
      return;
    }

    const patternData = {
      clinicId: this.data.clinicId,
      professionalId: formValue.professionalId || undefined,
      frequency: frequency,
      interval: 1,
      daysOfWeek: daysOfWeek,
      dayOfMonth: frequency === RecurrenceFrequency.Monthly ? formValue.dayOfMonth : undefined,
      startDate: this.formatDate(formValue.startDate),
      endDate: formValue.endDate ? this.formatDate(formValue.endDate) : undefined,
      occurrencesCount: formValue.occurrencesCount || undefined,
      startTime: formValue.startTime,
      endTime: formValue.endTime,
      blockedSlotType: formValue.type,
      notes: formValue.reason || undefined
    };

    this.appointmentService.createRecurringBlockedSlots(patternData).subscribe({
      next: () => {
        this.snackBar.open('Bloqueios recorrentes criados com sucesso', 'Fechar', { 
          duration: 3000,
          panelClass: ['success-snackbar']
        });
        this.dialogRef.close(true);
      },
      error: (error) => {
        console.error('Erro ao criar bloqueios recorrentes:', error);
        this.errorMessage.set(error.error?.message || 'Erro ao criar bloqueios recorrentes');
        this.saving.set(false);
      }
    });
  }

  private updateBlock(formValue: any): void {
    if (!this.data.blockedSlot) return;

    const updateData = {
      startTime: formValue.startTime,
      endTime: formValue.endTime,
      type: formValue.type,
      reason: formValue.reason || undefined
    };

    this.appointmentService.updateBlockedTimeSlot(this.data.blockedSlot.id, updateData).subscribe({
      next: () => {
        this.snackBar.open('Bloqueio atualizado com sucesso', 'Fechar', { 
          duration: 3000,
          panelClass: ['success-snackbar']
        });
        this.dialogRef.close(true);
      },
      error: (error) => {
        console.error('Erro ao atualizar bloqueio:', error);
        this.errorMessage.set(error.error?.message || 'Erro ao atualizar bloqueio');
        this.saving.set(false);
      }
    });
  }

  private formatDate(date: Date): string {
    return date.toISOString().split('T')[0];
  }
}
