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
import { Appointment, TimeSlot } from '../../../models/appointment.model';

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
  minDate: Date;
  maxDate: Date;

  constructor(
    public dialogRef: MatDialogRef<RescheduleDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { appointment: Appointment },
    private formBuilder: FormBuilder,
    private appointmentService: AppointmentService
  ) {
    this.minDate = new Date();
    this.maxDate = new Date();
    this.maxDate.setMonth(this.maxDate.getMonth() + 3);

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
    this.rescheduleForm.patchValue({ newTime: '' });

    const dateStr = this.formatDate(date);
    const doctorId = this.extractDoctorId();

    if (!doctorId) {
      console.error('Cannot extract doctor ID from appointment');
      this.loadingSlots = false;
      return;
    }

    this.appointmentService.getAvailableSlots(doctorId, dateStr).subscribe({
      next: (response) => {
        this.availableSlots = response.slots.filter(slot => slot.isAvailable);
        this.loadingSlots = false;
      },
      error: (error) => {
        console.error('Error loading slots:', error);
        this.loadingSlots = false;
      }
    });
  }

  extractDoctorId(): string {
    // In a real scenario, the appointment would have a doctorId field
    // For now, we'll return a placeholder or handle it differently
    return (this.data.appointment as any).doctorId || '';
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
