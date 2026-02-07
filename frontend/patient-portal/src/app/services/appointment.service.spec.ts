import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { AppointmentService } from './appointment.service';
import { Appointment } from '../models/appointment.model';

describe('AppointmentService', () => {
  let service: AppointmentService;
  let httpMock: HttpTestingController;

  const mockAppointments: Appointment[] = [
    {
      id: '1',
      doctorId: 'doc-123',
      doctorName: 'Dr. Smith',
      doctorSpecialty: 'General Practice',
      clinicId: 'clinic-001',
      clinicName: 'Health Clinic',
      appointmentDate: new Date('2026-02-01T10:00:00Z'),
      startTime: '10:00',
      endTime: '11:00',
      status: 'Scheduled',
      appointmentType: 'Consultation',
      isTelehealth: false,
      notes: 'Regular checkup',
      canReschedule: true,
      canCancel: true
    },
    {
      id: '2',
      doctorId: 'doc-456',
      doctorName: 'Dr. Johnson',
      doctorSpecialty: 'Cardiology',
      clinicId: 'clinic-001',
      clinicName: 'Health Clinic',
      appointmentDate: new Date('2026-01-20T14:00:00Z'),
      startTime: '14:00',
      endTime: '15:00',
      status: 'Completed',
      appointmentType: 'Follow-up',
      isTelehealth: false,
      notes: 'Follow-up appointment',
      canReschedule: false,
      canCancel: false
    }
  ];

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [AppointmentService]
    });

    service = TestBed.inject(AppointmentService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('getMyAppointments', () => {
    it('should retrieve appointments with default pagination', (done) => {
      service.getMyAppointments().subscribe(appointments => {
        expect(appointments).toEqual(mockAppointments);
        expect(appointments.length).toBe(2);
        done();
      });

      const req = httpMock.expectOne('http://localhost:5000/api/appointments?skip=0&take=50');
      expect(req.request.method).toBe('GET');
      req.flush(mockAppointments);
    });

    it('should retrieve appointments with custom pagination', (done) => {
      service.getMyAppointments(10, 20).subscribe(appointments => {
        expect(appointments).toEqual(mockAppointments);
        done();
      });

      const req = httpMock.expectOne('http://localhost:5000/api/appointments?skip=10&take=20');
      expect(req.request.method).toBe('GET');
      req.flush(mockAppointments);
    });
  });

  describe('getUpcomingAppointments', () => {
    it('should retrieve upcoming appointments with default limit', (done) => {
      const upcomingAppointments = [mockAppointments[0]];

      service.getUpcomingAppointments().subscribe(appointments => {
        expect(appointments).toEqual(upcomingAppointments);
        expect(appointments.length).toBe(1);
        done();
      });

      const req = httpMock.expectOne('http://localhost:5000/api/appointments/upcoming?take=10');
      expect(req.request.method).toBe('GET');
      req.flush(upcomingAppointments);
    });

    it('should retrieve upcoming appointments with custom limit', (done) => {
      const upcomingAppointments = [mockAppointments[0]];

      service.getUpcomingAppointments(5).subscribe(appointments => {
        expect(appointments).toEqual(upcomingAppointments);
        done();
      });

      const req = httpMock.expectOne('http://localhost:5000/api/appointments/upcoming?take=5');
      expect(req.request.method).toBe('GET');
      req.flush(upcomingAppointments);
    });
  });

  describe('getAppointmentById', () => {
    it('should retrieve a single appointment by id', (done) => {
      const appointment = mockAppointments[0];

      service.getAppointmentById('1').subscribe(result => {
        expect(result).toEqual(appointment);
        done();
      });

      const req = httpMock.expectOne('http://localhost:5000/api/appointments/1');
      expect(req.request.method).toBe('GET');
      req.flush(appointment);
    });

    it('should handle 404 when appointment not found', (done) => {
      service.getAppointmentById('999').subscribe({
        next: () => fail('should have failed'),
        error: (error) => {
          expect(error.status).toBe(404);
          done();
        }
      });

      const req = httpMock.expectOne('http://localhost:5000/api/appointments/999');
      req.flush({ message: 'Appointment not found' }, { status: 404, statusText: 'Not Found' });
    });
  });

  describe('getAppointmentsByStatus', () => {
    it('should retrieve appointments by status with default pagination', (done) => {
      const scheduledAppointments = [mockAppointments[0]];

      service.getAppointmentsByStatus('Scheduled').subscribe(appointments => {
        expect(appointments).toEqual(scheduledAppointments);
        done();
      });

      const req = httpMock.expectOne('http://localhost:5000/api/appointments/status/Scheduled?skip=0&take=50');
      expect(req.request.method).toBe('GET');
      req.flush(scheduledAppointments);
    });

    it('should retrieve appointments by status with custom pagination', (done) => {
      const completedAppointments = [mockAppointments[1]];

      service.getAppointmentsByStatus('Completed', 5, 10).subscribe(appointments => {
        expect(appointments).toEqual(completedAppointments);
        done();
      });

      const req = httpMock.expectOne('http://localhost:5000/api/appointments/status/Completed?skip=5&take=10');
      expect(req.request.method).toBe('GET');
      req.flush(completedAppointments);
    });
  });

  describe('getAppointmentsCount', () => {
    it('should retrieve appointments count', (done) => {
      const countResponse = { count: 15 };

      service.getAppointmentsCount().subscribe(result => {
        expect(result.count).toBe(15);
        done();
      });

      const req = httpMock.expectOne('http://localhost:5000/api/appointments/count');
      expect(req.request.method).toBe('GET');
      req.flush(countResponse);
    });
  });

  describe('error handling', () => {
    it('should handle network errors', (done) => {
      service.getMyAppointments().subscribe({
        next: () => fail('should have failed'),
        error: (error) => {
          expect(error.status).toBe(0);
          done();
        }
      });

      const req = httpMock.expectOne('http://localhost:5000/api/appointments?skip=0&take=50');
      req.error(new ProgressEvent('error'), { status: 0, statusText: 'Network error' });
    });

    it('should handle server errors', (done) => {
      service.getMyAppointments().subscribe({
        next: () => fail('should have failed'),
        error: (error) => {
          expect(error.status).toBe(500);
          done();
        }
      });

      const req = httpMock.expectOne('http://localhost:5000/api/appointments?skip=0&take=50');
      req.flush({ message: 'Internal server error' }, { status: 500, statusText: 'Internal Server Error' });
    });
  });

  describe('Booking endpoints', () => {
    const clinicId = 'clinic-123';

    it('should get specialties', (done) => {
      const mockSpecialties = [
        { id: '1', name: 'Cardiology' },
        { id: '2', name: 'Dermatology' }
      ];

      service.getSpecialties(clinicId).subscribe(specialties => {
        expect(specialties).toEqual(mockSpecialties);
        done();
      });

      const req = httpMock.expectOne(`http://localhost:5000/api/appointments/specialties?clinicId=${clinicId}`);
      expect(req.request.method).toBe('GET');
      req.flush(mockSpecialties);
    });

    it('should get doctors with specialty filter', (done) => {
      const mockDoctors = [
        { id: '1', name: 'Dr. Smith', specialty: 'Cardiology', crm: '12345', availableForOnlineBooking: true }
      ];

      service.getDoctors(clinicId, 'Cardiology').subscribe(doctors => {
        expect(doctors).toEqual(mockDoctors);
        done();
      });

      const req = httpMock.expectOne(`http://localhost:5000/api/appointments/doctors?clinicId=${clinicId}&specialty=Cardiology`);
      expect(req.request.method).toBe('GET');
      req.flush(mockDoctors);
    });

    it('should get doctors without specialty filter', (done) => {
      const mockDoctors = [
        { id: '1', name: 'Dr. Smith', specialty: 'Cardiology', crm: '12345', availableForOnlineBooking: true }
      ];

      service.getDoctors(clinicId).subscribe(doctors => {
        expect(doctors).toEqual(mockDoctors);
        done();
      });

      const req = httpMock.expectOne(`http://localhost:5000/api/appointments/doctors?clinicId=${clinicId}`);
      expect(req.request.method).toBe('GET');
      req.flush(mockDoctors);
    });

    it('should get available slots', (done) => {
      const mockSlots = {
        date: '2025-02-15',
        slots: [
          { startTime: '09:00:00', endTime: '09:30:00', isAvailable: true }
        ]
      };

      service.getAvailableSlots(clinicId, 'doctor-1', '2025-02-15').subscribe(response => {
        expect(response).toEqual(mockSlots);
        done();
      });

      const req = httpMock.expectOne(`http://localhost:5000/api/appointments/available-slots?clinicId=${clinicId}&doctorId=doctor-1&date=2025-02-15`);
      expect(req.request.method).toBe('GET');
      req.flush(mockSlots);
    });

    it('should book appointment', (done) => {
      const bookingRequest = {
        doctorId: 'doctor-1',
        clinicId: 'clinic-123',
        scheduledDate: '2025-02-15',
        startTime: '09:00:00',
        reason: 'Regular checkup'
      };

      const mockResponse = {
        id: 'appointment-123',
        doctorName: 'Dr. Smith',
        scheduledDate: '2025-02-15',
        startTime: '09:00:00',
        status: 'Scheduled',
        message: 'Success'
      };

      service.bookAppointment(bookingRequest).subscribe(response => {
        expect(response).toEqual(mockResponse);
        done();
      });

      const req = httpMock.expectOne('http://localhost:5000/api/appointments/book');
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(bookingRequest);
      req.flush(mockResponse);
    });

    it('should confirm appointment', (done) => {
      service.confirmAppointment('appointment-1').subscribe(() => {
        done();
      });

      const req = httpMock.expectOne('http://localhost:5000/api/appointments/appointment-1/confirm');
      expect(req.request.method).toBe('POST');
      req.flush(null);
    });

    it('should cancel appointment', (done) => {
      const cancelRequest = { reason: 'Personal reasons' };

      service.cancelAppointment('appointment-1', cancelRequest).subscribe(() => {
        done();
      });

      const req = httpMock.expectOne('http://localhost:5000/api/appointments/appointment-1/cancel');
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(cancelRequest);
      req.flush(null);
    });

    it('should reschedule appointment', (done) => {
      const rescheduleRequest = {
        newDate: '2025-02-20',
        newTime: '10:00:00',
        reason: 'Schedule conflict'
      };

      service.rescheduleAppointment('appointment-1', rescheduleRequest).subscribe(() => {
        done();
      });

      const req = httpMock.expectOne('http://localhost:5000/api/appointments/appointment-1/reschedule');
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(rescheduleRequest);
      req.flush(null);
    });
  });
});
