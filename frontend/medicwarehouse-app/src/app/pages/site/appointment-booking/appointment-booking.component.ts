import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { PublicClinicService, PublicClinicDto, AvailableSlotDto, PublicAppointmentRequest } from '../../../services/public-clinic.service';
import { HeaderComponent } from '../../../components/site/header/header';
import { FooterComponent } from '../../../components/site/footer/footer';

@Component({
  selector: 'app-appointment-booking',
  standalone: true,
  imports: [CommonModule, RouterModule, ReactiveFormsModule, HeaderComponent, FooterComponent],
  templateUrl: './appointment-booking.component.html',
  styleUrls: ['./appointment-booking.component.scss']
})
export class AppointmentBookingComponent implements OnInit {
  clinicId: string = '';
  clinic: PublicClinicDto | null = null;
  availableSlots: AvailableSlotDto[] = [];
  
  // Form
  bookingForm!: FormGroup;
  
  // Loading states
  loadingClinic = false;
  loadingSlots = false;
  submitting = false;
  
  // Success state
  appointmentCreated = false;
  appointmentDetails: any = null;
  
  // Error states
  clinicError = '';
  slotsError = '';
  bookingError = '';
  
  // Date selection
  selectedDate: Date | null = null;
  minDate: Date;
  maxDate: Date;
  availableDates: Date[] = [];
  
  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder,
    private publicClinicService: PublicClinicService
  ) {
    this.minDate = new Date();
    this.maxDate = new Date();
    this.maxDate.setMonth(this.maxDate.getMonth() + 2); // Can book up to 2 months ahead
  }

  ngOnInit(): void {
    this.clinicId = this.route.snapshot.params['id'];
    this.initializeForm();
    this.loadClinic();
  }

  initializeForm(): void {
    this.bookingForm = this.formBuilder.group({
      date: ['', Validators.required],
      time: ['', Validators.required],
      patientName: ['', [Validators.required, Validators.minLength(3)]],
      patientCpf: ['', [Validators.required, Validators.pattern(/^\d{11}$/)]],
      patientBirthDate: ['', Validators.required],
      patientPhone: ['', [Validators.required, Validators.pattern(/^\d{10,11}$/)]],
      patientEmail: ['', [Validators.required, Validators.email]],
      notes: ['', Validators.maxLength(500)]
    });
  }

  loadClinic(): void {
    this.loadingClinic = true;
    this.clinicError = '';

    this.publicClinicService.getClinicById(this.clinicId).subscribe({
      next: (clinic) => {
        this.clinic = clinic;
        this.loadingClinic = false;
        
        if (!clinic.isAcceptingNewPatients) {
          this.clinicError = 'Esta clínica não está aceitando novos pacientes no momento.';
        }
      },
      error: (error) => {
        console.error('Error loading clinic:', error);
        this.clinicError = 'Não foi possível carregar os dados da clínica. Tente novamente.';
        this.loadingClinic = false;
      }
    });
  }

  onDateChange(event: any): void {
    const date = event.target.value;
    if (!date || !this.clinic) return;

    this.selectedDate = new Date(date);
    this.bookingForm.patchValue({ time: '' });
    this.loadAvailableSlots(date);
  }

  loadAvailableSlots(date: string): void {
    if (!this.clinic) return;

    this.loadingSlots = true;
    this.slotsError = '';
    this.availableSlots = [];

    this.publicClinicService.getAvailableSlots(
      this.clinicId,
      date,
      this.clinic.appointmentDurationMinutes
    ).subscribe({
      next: (slots) => {
        this.availableSlots = slots.filter(s => s.isAvailable);
        this.loadingSlots = false;
        
        if (this.availableSlots.length === 0) {
          this.slotsError = 'Não há horários disponíveis para esta data. Por favor, selecione outra data.';
        }
      },
      error: (error) => {
        console.error('Error loading slots:', error);
        this.slotsError = 'Erro ao carregar horários disponíveis. Tente novamente.';
        this.loadingSlots = false;
      }
    });
  }

  onSubmit(): void {
    if (this.bookingForm.invalid || !this.clinic) {
      this.markFormGroupTouched(this.bookingForm);
      return;
    }

    this.submitting = true;
    this.bookingError = '';

    const formValue = this.bookingForm.value;
    const request: PublicAppointmentRequest = {
      clinicId: this.clinicId,
      scheduledDate: formValue.date,
      scheduledTime: formValue.time,
      durationMinutes: this.clinic.appointmentDurationMinutes,
      patientName: formValue.patientName,
      patientCpf: formValue.patientCpf,
      patientBirthDate: formValue.patientBirthDate,
      patientPhone: formValue.patientPhone,
      patientEmail: formValue.patientEmail,
      notes: formValue.notes || ''
    };

    this.publicClinicService.createPublicAppointment(request).subscribe({
      next: (response) => {
        this.appointmentCreated = true;
        this.appointmentDetails = response;
        this.submitting = false;
        
        // Scroll to success message
        window.scrollTo({ top: 0, behavior: 'smooth' });
      },
      error: (error) => {
        console.error('Error creating appointment:', error);
        this.bookingError = error.error?.message || 'Erro ao agendar consulta. Verifique os dados e tente novamente.';
        this.submitting = false;
      }
    });
  }

  private markFormGroupTouched(formGroup: FormGroup): void {
    Object.keys(formGroup.controls).forEach(key => {
      const control = formGroup.get(key);
      control?.markAsTouched();

      if (control instanceof FormGroup) {
        this.markFormGroupTouched(control);
      }
    });
  }

  formatCpf(event: any): void {
    let value = event.target.value.replace(/\D/g, '');
    this.bookingForm.patchValue({ patientCpf: value });
  }

  formatPhone(event: any): void {
    let value = event.target.value.replace(/\D/g, '');
    this.bookingForm.patchValue({ patientPhone: value });
  }

  goBack(): void {
    this.router.navigate(['/site/clinics']);
  }

  bookAnother(): void {
    this.appointmentCreated = false;
    this.appointmentDetails = null;
    this.bookingForm.reset();
    this.availableSlots = [];
    this.selectedDate = null;
  }
}
