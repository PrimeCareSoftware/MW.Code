import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AppointmentService } from '../../../services/appointment.service';
import { AuthService } from '../../../services/auth.service';
import { Appointment, TimeSlot } from '../../../models/appointment.model';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-reschedule-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatSelectModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './reschedule-dialog.component.html',
  styleUrls: ['./reschedule-dialog.component.scss']
})
export class RescheduleDialogComponent implements OnInit {
  rescheduleForm: FormGroup;
  availableSlots: TimeSlot[] = [];
  loadingSlots = false;
  slotsError = false;
  slotsErrorMessage = '';
  minDate: Date;
  maxDate: Date;
  private readonly clinicId: string;

  constructor(
    public dialogRef: MatDialogRef<RescheduleDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { appointment: Appointment },
    private formBuilder: FormBuilder,
    private appointmentService: AppointmentService,
    private authService: AuthService
  ) {
    this.minDate = new Date();
    this.maxDate = new Date();
    this.maxDate.setMonth(this.maxDate.getMonth() + 3);
    // Use clinic ID from appointment (required field)
    this.clinicId = this.data.appointment.clinicId;

    this.rescheduleForm = this.formBuilder.group({
      newDate: ['', Validators.required],
      newTime: ['', Validators.required],
      reason: ['', [Validators.minLength(10), Validators.maxLength(500)]]
    });
  }

  ngOnInit(): void {
    // Setup date change listener
    this.rescheduleForm.get('newDate')?.valueChanges.subscribe((date) => {
      if (date) {
        this.loadAvailableSlots(date);
      }
    });
  }

  loadAvailableSlots(date: Date): void {
    this.loadingSlots = true;
    this.availableSlots = [];
    this.slotsError = false;
    this.rescheduleForm.patchValue({ newTime: '' });

    const dateStr = this.formatDate(date);
    const doctorId = this.extractDoctorId();

    if (!doctorId) {
      console.error('Doctor ID is missing from appointment data');
      this.loadingSlots = false;
      this.slotsError = true;
      this.slotsErrorMessage = 'Erro: Identificação do médico não encontrada.';
      return;
    }

    this.appointmentService.getAvailableSlots(this.clinicId, doctorId, dateStr).subscribe({
      next: (response) => {
        this.availableSlots = response.slots.filter(slot => slot.isAvailable);
        this.loadingSlots = false;
        this.slotsError = false;
      },
      error: (error) => {
        console.error('Error loading available slots:', error);
        this.loadingSlots = false;
        this.slotsError = true;
        this.slotsErrorMessage = 'Não foi possível carregar os horários disponíveis. Tente novamente.';
      }
    });
  }

  retryLoadSlots(): void {
    const selectedDate = this.rescheduleForm.get('newDate')?.value;
    if (selectedDate) {
      this.loadAvailableSlots(selectedDate);
    }
  }

  extractDoctorId(): string {
    // Extract doctor ID from the appointment
    // Return empty string if not available to trigger error handling
    return this.data.appointment.doctorId || '';
  }

  formatDate(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  formatTime(time: string): string {
    return time.substring(0, 5);
  }

  dateFilter = (date: Date | null): boolean => {
    if (!date) return false;
    const day = date.getDay();
    return day !== 0 && day !== 6;
  };

  onCancel(): void {
    this.dialogRef.close();
  }

  onConfirm(): void {
    if (this.rescheduleForm.valid) {
      const formValue = this.rescheduleForm.value;
      this.dialogRef.close({
        confirmed: true,
        newDate: this.formatDate(formValue.newDate),
        newTime: formValue.newTime,
        reason: formValue.reason || undefined
      });
    }
  }
}
