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
      doctorName: 'Dr. Smith',
      doctorSpecialty: 'General Practice',
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
      doctorName: 'Dr. Johnson',
      doctorSpecialty: 'Cardiology',
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
});
