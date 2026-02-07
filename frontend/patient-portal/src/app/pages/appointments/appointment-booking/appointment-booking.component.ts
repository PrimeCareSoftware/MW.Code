import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatStepperModule, MatStepper } from '@angular/material/stepper';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { MatDividerModule } from '@angular/material/divider';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatRadioModule } from '@angular/material/radio';
import { AppointmentService } from '../../../services/appointment.service';
import { NotificationService } from '../../../services/notification.service';
import { AuthService } from '../../../services/auth.service';
import { Specialty, Doctor, TimeSlot, BookAppointmentRequest } from '../../../models/appointment.model';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-appointment-booking',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    MatStepperModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatProgressSpinnerModule,
    MatChipsModule,
    MatDividerModule,
    MatTooltipModule,
    MatRadioModule
  ],
  templateUrl: './appointment-booking.component.html',
  styleUrls: ['./appointment-booking.component.scss']
})
export class AppointmentBookingComponent implements OnInit {
  @ViewChild('stepper') stepper!: MatStepper;

  // Forms
  specialtyFormGroup!: FormGroup;
  doctorFormGroup!: FormGroup;
  dateTimeFormGroup!: FormGroup;
  detailsFormGroup!: FormGroup;

  // Data
  specialties: Specialty[] = [];
  doctors: Doctor[] = [];
  availableSlots: TimeSlot[] = [];
  selectedSpecialty: Specialty | null = null;
  selectedDoctor: Doctor | null = null;
  selectedDate: Date | null = null;
  selectedSlot: TimeSlot | null = null;

  // Loading states
  loadingSpecialties = false;
  loadingDoctors = false;
  loadingSlots = false;
  submitting = false;

  // Error states
  specialtiesError = false;
  doctorsError = false;
  slotsError = false;

  // Date filter
  minDate: Date;
  maxDate: Date;

  // Clinic ID - dynamically retrieved from authenticated user
  private readonly clinicId: string;

  constructor(
    private formBuilder: FormBuilder,
    private appointmentService: AppointmentService,
    private notificationService: NotificationService,
    private authService: AuthService,
    private router: Router
  ) {
    this.minDate = new Date();
    this.maxDate = new Date();
    this.maxDate.setMonth(this.maxDate.getMonth() + 3);
    // Get clinic ID from authenticated user, with fallback to environment default
    this.clinicId = this.authService.getUserClinicId();
  }

  ngOnInit(): void {
    this.initializeForms();
    this.loadSpecialties();
  }

  initializeForms(): void {
    this.specialtyFormGroup = this.formBuilder.group({
      specialty: ['', Validators.required]
    });

    this.doctorFormGroup = this.formBuilder.group({
      doctor: ['', Validators.required]
    });

    this.dateTimeFormGroup = this.formBuilder.group({
      date: ['', Validators.required],
      timeSlot: ['', Validators.required]
    });

    this.detailsFormGroup = this.formBuilder.group({
      reason: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(500)]],
      appointmentType: ['Consulta', Validators.required]
    });
  }

  loadSpecialties(): void {
    this.loadingSpecialties = true;
    this.specialtiesError = false;

    this.appointmentService.getSpecialties(this.clinicId).subscribe({
      next: (specialties) => {
        this.specialties = specialties;
        this.loadingSpecialties = false;
      },
      error: (error) => {
        console.error('Error loading specialties:', error);
        this.specialtiesError = true;
        this.loadingSpecialties = false;
        this.notificationService.error('Erro ao carregar especialidades');
      }
    });
  }

  onSpecialtySelected(): void {
    const specialtyId = this.specialtyFormGroup.get('specialty')?.value;
    this.selectedSpecialty = this.specialties.find(s => s.id === specialtyId) || null;
    
    if (this.selectedSpecialty) {
      this.loadDoctors(this.selectedSpecialty.name);
    }
  }

  loadDoctors(specialty: string): void {
    this.loadingDoctors = true;
    this.doctorsError = false;
    this.doctors = [];
    this.doctorFormGroup.reset();

    this.appointmentService.getDoctors(this.clinicId, specialty).subscribe({
      next: (doctors) => {
        this.doctors = doctors.filter(d => d.availableForOnlineBooking);
        this.loadingDoctors = false;
        
        if (this.doctors.length === 0) {
          this.notificationService.warning('Nenhum médico disponível para esta especialidade no momento');
        }
      },
      error: (error) => {
        console.error('Error loading doctors:', error);
        this.doctorsError = true;
        this.loadingDoctors = false;
        this.notificationService.error('Erro ao carregar médicos');
      }
    });
  }

  onDoctorSelected(): void {
    const doctorId = this.doctorFormGroup.get('doctor')?.value;
    this.selectedDoctor = this.doctors.find(d => d.id === doctorId) || null;
    
    // Reset date and time selection
    this.dateTimeFormGroup.patchValue({ date: null, timeSlot: null });
    this.availableSlots = [];
    this.selectedDate = null;
    this.selectedSlot = null;
  }

  onDateSelected(): void {
    const date = this.dateTimeFormGroup.get('date')?.value;
    if (!date || !this.selectedDoctor) return;

    this.selectedDate = date;
    this.dateTimeFormGroup.patchValue({ timeSlot: null });
    this.loadAvailableSlots();
  }

  loadAvailableSlots(): void {
    if (!this.selectedDoctor || !this.selectedDate) return;

    this.loadingSlots = true;
    this.slotsError = false;
    this.availableSlots = [];

    const dateStr = this.formatDate(this.selectedDate);

    this.appointmentService.getAvailableSlots(this.clinicId, this.selectedDoctor.id, dateStr).subscribe({
      next: (response) => {
        this.availableSlots = response.slots.filter(slot => slot.isAvailable);
        this.loadingSlots = false;

        if (this.availableSlots.length === 0) {
          this.notificationService.warning('Nenhum horário disponível para esta data');
        }
      },
      error: (error) => {
        console.error('Error loading slots:', error);
        this.slotsError = true;
        this.loadingSlots = false;
        this.notificationService.error('Erro ao carregar horários disponíveis');
      }
    });
  }

  onTimeSlotSelected(): void {
    const slotTime = this.dateTimeFormGroup.get('timeSlot')?.value;
    this.selectedSlot = this.availableSlots.find(s => s.startTime === slotTime) || null;
  }

  formatDate(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  formatDisplayDate(date: Date): string {
    return new Date(date).toLocaleDateString('pt-BR', {
      weekday: 'long',
      day: '2-digit',
      month: 'long',
      year: 'numeric'
    });
  }

  formatTime(time: string): string {
    return time.substring(0, 5);
  }

  dateFilter = (date: Date | null): boolean => {
    if (!date) return false;
    const day = date.getDay();
    // Disable weekends (0 = Sunday, 6 = Saturday)
    return day !== 0 && day !== 6;
  };

  onSubmit(): void {
    if (!this.isFormValid()) {
      this.notificationService.error('Por favor, preencha todos os campos obrigatórios');
      return;
    }

    this.submitting = true;

    const request: BookAppointmentRequest = {
      doctorId: this.selectedDoctor!.id,
      clinicId: this.clinicId,
      scheduledDate: this.formatDate(this.selectedDate!),
      startTime: this.selectedSlot!.startTime,
      durationMinutes: 30,
      reason: this.detailsFormGroup.get('reason')?.value,
      appointmentType: 1, // Regular consultation
      appointmentMode: 1  // In-person
    };

    this.appointmentService.bookAppointment(request).subscribe({
      next: (response) => {
        this.submitting = false;
        this.notificationService.success('Consulta agendada com sucesso!');
        this.router.navigate(['/appointments']);
      },
      error: (error) => {
        console.error('Error booking appointment:', error);
        this.submitting = false;
        
        const errorMessage = error.error?.message || 'Erro ao agendar consulta. Tente novamente.';
        this.notificationService.error(errorMessage);
      }
    });
  }

  isFormValid(): boolean {
    return this.specialtyFormGroup.valid &&
           this.doctorFormGroup.valid &&
           this.dateTimeFormGroup.valid &&
           this.detailsFormGroup.valid;
  }

  retryLoadSpecialties(): void {
    this.loadSpecialties();
  }

  retryLoadDoctors(): void {
    if (this.selectedSpecialty) {
      this.loadDoctors(this.selectedSpecialty.name);
    }
  }

  retryLoadSlots(): void {
    this.loadAvailableSlots();
  }

  goBack(): void {
    this.router.navigate(['/appointments']);
  }
}
