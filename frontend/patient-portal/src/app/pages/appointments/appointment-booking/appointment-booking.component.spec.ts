import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { of, throwError } from 'rxjs';
import { AppointmentBookingComponent } from './appointment-booking.component';
import { AppointmentService } from '../../../services/appointment.service';
import { NotificationService } from '../../../services/notification.service';
import { Specialty, Doctor, AvailableSlotsResponse, BookAppointmentResponse } from '../../../models/appointment.model';

describe('AppointmentBookingComponent', () => {
  let component: AppointmentBookingComponent;
  let fixture: ComponentFixture<AppointmentBookingComponent>;
  let appointmentService: jasmine.SpyObj<AppointmentService>;
  let notificationService: jasmine.SpyObj<NotificationService>;
  let router: jasmine.SpyObj<Router>;

  const mockSpecialties: Specialty[] = [
    { id: '1', name: 'Cardiologia', description: 'Heart specialist' },
    { id: '2', name: 'Dermatologia', description: 'Skin specialist' }
  ];

  const mockDoctors: Doctor[] = [
    {
      id: '1',
      name: 'Dr. João Silva',
      specialty: 'Cardiologia',
      crm: '12345',
      rating: 4.5,
      availableForOnlineBooking: true
    },
    {
      id: '2',
      name: 'Dr. Maria Santos',
      specialty: 'Cardiologia',
      crm: '67890',
      rating: 4.8,
      availableForOnlineBooking: true
    }
  ];

  const mockSlots: AvailableSlotsResponse = {
    date: '2025-02-15',
    slots: [
      { startTime: '09:00:00', endTime: '09:30:00', isAvailable: true },
      { startTime: '10:00:00', endTime: '10:30:00', isAvailable: true },
      { startTime: '14:00:00', endTime: '14:30:00', isAvailable: false }
    ]
  };

  const mockBookingResponse: BookAppointmentResponse = {
    id: 'appointment-123',
    doctorName: 'Dr. João Silva',
    scheduledDate: '2025-02-15',
    startTime: '09:00:00',
    status: 'Scheduled',
    message: 'Appointment booked successfully'
  };

  beforeEach(async () => {
    const appointmentServiceSpy = jasmine.createSpyObj('AppointmentService', [
      'getSpecialties',
      'getDoctors',
      'getAvailableSlots',
      'bookAppointment'
    ]);
    const notificationServiceSpy = jasmine.createSpyObj('NotificationService', [
      'success',
      'error',
      'warning'
    ]);
    const routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [
        AppointmentBookingComponent,
        ReactiveFormsModule,
        BrowserAnimationsModule
      ],
      providers: [
        { provide: AppointmentService, useValue: appointmentServiceSpy },
        { provide: NotificationService, useValue: notificationServiceSpy },
        { provide: Router, useValue: routerSpy }
      ]
    }).compileComponents();

    appointmentService = TestBed.inject(AppointmentService) as jasmine.SpyObj<AppointmentService>;
    notificationService = TestBed.inject(NotificationService) as jasmine.SpyObj<NotificationService>;
    router = TestBed.inject(Router) as jasmine.SpyObj<Router>;

    appointmentService.getSpecialties.and.returnValue(of(mockSpecialties));

    fixture = TestBed.createComponent(AppointmentBookingComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize forms on init', () => {
    fixture.detectChanges();
    expect(component.specialtyFormGroup).toBeDefined();
    expect(component.doctorFormGroup).toBeDefined();
    expect(component.dateTimeFormGroup).toBeDefined();
    expect(component.detailsFormGroup).toBeDefined();
  });

  it('should load specialties on init', () => {
    fixture.detectChanges();
    expect(appointmentService.getSpecialties).toHaveBeenCalled();
    expect(component.specialties).toEqual(mockSpecialties);
    expect(component.loadingSpecialties).toBe(false);
  });

  it('should handle specialty loading error', () => {
    appointmentService.getSpecialties.and.returnValue(
      throwError(() => new Error('Network error'))
    );
    fixture.detectChanges();
    expect(component.specialtiesError).toBe(true);
    expect(notificationService.error).toHaveBeenCalledWith('Erro ao carregar especialidades');
  });

  it('should load doctors when specialty is selected', () => {
    appointmentService.getDoctors.and.returnValue(of(mockDoctors));
    fixture.detectChanges();

    component.specialtyFormGroup.patchValue({ specialty: '1' });
    component.onSpecialtySelected();

    expect(appointmentService.getDoctors).toHaveBeenCalledWith('Cardiologia');
    expect(component.doctors).toEqual(mockDoctors);
  });

  it('should filter out doctors not available for online booking', () => {
    const doctorsWithUnavailable = [
      ...mockDoctors,
      { ...mockDoctors[0], id: '3', availableForOnlineBooking: false }
    ];
    appointmentService.getDoctors.and.returnValue(of(doctorsWithUnavailable));
    fixture.detectChanges();

    component.specialtyFormGroup.patchValue({ specialty: '1' });
    component.onSpecialtySelected();

    expect(component.doctors.length).toBe(2);
    expect(component.doctors.every(d => d.availableForOnlineBooking)).toBe(true);
  });

  it('should load available slots when date is selected', () => {
    appointmentService.getAvailableSlots.and.returnValue(of(mockSlots));
    fixture.detectChanges();

    component.selectedDoctor = mockDoctors[0];
    component.dateTimeFormGroup.patchValue({ date: new Date('2025-02-15') });
    component.onDateSelected();

    expect(appointmentService.getAvailableSlots).toHaveBeenCalledWith(
      '1',
      '2025-02-15'
    );
    expect(component.availableSlots.length).toBe(2); // Only available slots
  });

  it('should validate all forms before submission', () => {
    fixture.detectChanges();
    expect(component.isFormValid()).toBe(false);

    component.specialtyFormGroup.patchValue({ specialty: '1' });
    component.doctorFormGroup.patchValue({ doctor: '1' });
    component.dateTimeFormGroup.patchValue({
      date: new Date('2025-02-15'),
      timeSlot: '09:00:00'
    });
    component.detailsFormGroup.patchValue({
      reason: 'Regular checkup appointment',
      appointmentType: 'Consulta'
    });

    expect(component.isFormValid()).toBe(true);
  });

  it('should submit booking request with correct data', () => {
    appointmentService.bookAppointment.and.returnValue(of(mockBookingResponse));
    fixture.detectChanges();

    component.selectedDoctor = mockDoctors[0];
    component.selectedDate = new Date('2025-02-15');
    component.selectedSlot = mockSlots.slots[0];

    component.specialtyFormGroup.patchValue({ specialty: '1' });
    component.doctorFormGroup.patchValue({ doctor: '1' });
    component.dateTimeFormGroup.patchValue({
      date: new Date('2025-02-15'),
      timeSlot: '09:00:00'
    });
    component.detailsFormGroup.patchValue({
      reason: 'Regular checkup appointment',
      appointmentType: 'Consulta'
    });

    component.onSubmit();

    expect(appointmentService.bookAppointment).toHaveBeenCalledWith({
      doctorId: '1',
      scheduledDate: '2025-02-15',
      startTime: '09:00:00',
      reason: 'Regular checkup appointment',
      appointmentType: 'Consulta'
    });
    expect(notificationService.success).toHaveBeenCalledWith('Consulta agendada com sucesso!');
    expect(router.navigate).toHaveBeenCalledWith(['/appointments']);
  });

  it('should handle booking error', () => {
    appointmentService.bookAppointment.and.returnValue(
      throwError(() => ({ error: { message: 'Slot no longer available' } }))
    );
    fixture.detectChanges();

    component.selectedDoctor = mockDoctors[0];
    component.selectedDate = new Date('2025-02-15');
    component.selectedSlot = mockSlots.slots[0];

    component.specialtyFormGroup.patchValue({ specialty: '1' });
    component.doctorFormGroup.patchValue({ doctor: '1' });
    component.dateTimeFormGroup.patchValue({
      date: new Date('2025-02-15'),
      timeSlot: '09:00:00'
    });
    component.detailsFormGroup.patchValue({
      reason: 'Regular checkup appointment',
      appointmentType: 'Consulta'
    });

    component.onSubmit();

    expect(notificationService.error).toHaveBeenCalledWith('Slot no longer available');
    expect(router.navigate).not.toHaveBeenCalled();
  });

  it('should format date correctly', () => {
    const date = new Date('2025-02-15');
    const formatted = component.formatDate(date);
    expect(formatted).toBe('2025-02-15');
  });

  it('should format time correctly', () => {
    const time = '09:00:00';
    const formatted = component.formatTime(time);
    expect(formatted).toBe('09:00');
  });

  it('should disable weekends in date filter', () => {
    const saturday = new Date('2025-02-15'); // Saturday
    const sunday = new Date('2025-02-16'); // Sunday
    const monday = new Date('2025-02-17'); // Monday

    expect(component.dateFilter(saturday)).toBe(false);
    expect(component.dateFilter(sunday)).toBe(false);
    expect(component.dateFilter(monday)).toBe(true);
  });

  it('should reset date and time when doctor changes', () => {
    fixture.detectChanges();
    component.dateTimeFormGroup.patchValue({
      date: new Date('2025-02-15'),
      timeSlot: '09:00:00'
    });
    component.availableSlots = mockSlots.slots;

    component.doctorFormGroup.patchValue({ doctor: '2' });
    component.onDoctorSelected();

    expect(component.dateTimeFormGroup.get('date')?.value).toBeNull();
    expect(component.dateTimeFormGroup.get('timeSlot')?.value).toBeNull();
    expect(component.availableSlots).toEqual([]);
  });

  it('should navigate back to appointments list', () => {
    component.goBack();
    expect(router.navigate).toHaveBeenCalledWith(['/appointments']);
  });

  it('should validate reason field length', () => {
    fixture.detectChanges();
    const reasonControl = component.detailsFormGroup.get('reason');

    reasonControl?.setValue('short');
    expect(reasonControl?.hasError('minlength')).toBe(true);

    reasonControl?.setValue('This is a valid reason for the appointment');
    expect(reasonControl?.hasError('minlength')).toBe(false);

    const longReason = 'a'.repeat(501);
    reasonControl?.setValue(longReason);
    expect(reasonControl?.hasError('maxlength')).toBe(true);
  });
});
