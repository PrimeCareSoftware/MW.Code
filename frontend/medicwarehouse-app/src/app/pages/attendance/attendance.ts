import { Component, OnInit, OnDestroy, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { interval, Subscription } from 'rxjs';
import { Navbar } from '../../shared/navbar/navbar';
import { AppointmentService } from '../../services/appointment';
import { MedicalRecordService } from '../../services/medical-record';
import { PatientService } from '../../services/patient';
import { Appointment } from '../../models/appointment.model';
import { MedicalRecord } from '../../models/medical-record.model';
import { Patient } from '../../models/patient.model';

@Component({
  selector: 'app-attendance',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, Navbar],
  templateUrl: './attendance.html',
  styleUrl: './attendance.scss'
})
export class Attendance implements OnInit, OnDestroy {
  appointmentId = signal<string | null>(null);
  appointment = signal<Appointment | null>(null);
  patient = signal<Patient | null>(null);
  medicalRecord = signal<MedicalRecord | null>(null);
  patientHistory = signal<MedicalRecord[]>([]);
  
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');
  successMessage = signal<string>('');
  
  attendanceForm: FormGroup;
  
  // Cronômetro
  elapsedSeconds = signal<number>(0);
  timerSubscription?: Subscription;
  startTime?: Date;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private appointmentService: AppointmentService,
    private medicalRecordService: MedicalRecordService,
    private patientService: PatientService
  ) {
    this.attendanceForm = this.fb.group({
      diagnosis: [''],
      prescription: [''],
      notes: ['']
    });
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('appointmentId');
    if (id) {
      this.appointmentId.set(id);
      this.loadAppointment(id);
    }
  }

  ngOnDestroy(): void {
    this.stopTimer();
  }

  loadAppointment(id: string): void {
    this.isLoading.set(true);
    
    this.appointmentService.getById(id).subscribe({
      next: (appointment) => {
        this.appointment.set(appointment);
        this.loadPatient(appointment.patientId);
        this.loadOrCreateMedicalRecord(appointment.id, appointment.patientId);
        this.isLoading.set(false);
      },
      error: (error) => {
        console.error('Error loading appointment:', error);
        this.errorMessage.set('Erro ao carregar agendamento');
        this.isLoading.set(false);
      }
    });
  }

  loadPatient(patientId: string): void {
    this.patientService.getById(patientId).subscribe({
      next: (patient) => {
        this.patient.set(patient);
        this.loadPatientHistory(patientId);
      },
      error: (error) => {
        console.error('Error loading patient:', error);
        this.errorMessage.set('Erro ao carregar dados do paciente');
      }
    });
  }

  loadPatientHistory(patientId: string): void {
    this.medicalRecordService.getPatientRecords(patientId).subscribe({
      next: (records) => {
        this.patientHistory.set(records);
      },
      error: (error) => {
        console.error('Error loading patient history:', error);
      }
    });
  }

  loadOrCreateMedicalRecord(appointmentId: string, patientId: string): void {
    this.medicalRecordService.getByAppointment(appointmentId).subscribe({
      next: (record) => {
        this.medicalRecord.set(record);
        this.attendanceForm.patchValue({
          diagnosis: record.diagnosis,
          prescription: record.prescription,
          notes: record.notes
        });
        
        // Calcula o tempo decorrido se a consulta ainda estiver em andamento
        if (record.consultationStartTime && !record.consultationEndTime) {
          this.startTime = new Date(record.consultationStartTime);
          this.startTimer();
        }
      },
      error: (error) => {
        // Se não encontrado, cria um novo prontuário
        if (error.status === 404) {
          this.createMedicalRecord(appointmentId, patientId);
        } else {
          console.error('Error loading medical record:', error);
          this.errorMessage.set('Erro ao carregar prontuário');
        }
      }
    });
  }

  createMedicalRecord(appointmentId: string, patientId: string): void {
    const now = new Date().toISOString();
    this.medicalRecordService.create({
      appointmentId,
      patientId,
      consultationStartTime: now
    }).subscribe({
      next: (record) => {
        this.medicalRecord.set(record);
        this.startTime = new Date(now);
        this.startTimer();
      },
      error: (error) => {
        console.error('Error creating medical record:', error);
        this.errorMessage.set('Erro ao iniciar atendimento');
      }
    });
  }

  startTimer(): void {
    if (this.timerSubscription) {
      return; // Cronômetro já está em execução
    }

    this.timerSubscription = interval(1000).subscribe(() => {
      if (this.startTime) {
        const now = new Date();
        const diff = Math.floor((now.getTime() - this.startTime.getTime()) / 1000);
        this.elapsedSeconds.set(diff);
      }
    });
  }

  stopTimer(): void {
    if (this.timerSubscription) {
      this.timerSubscription.unsubscribe();
      this.timerSubscription = undefined;
    }
  }

  formatTime(seconds: number): string {
    const hours = Math.floor(seconds / 3600);
    const minutes = Math.floor((seconds % 3600) / 60);
    const secs = seconds % 60;
    return `${hours.toString().padStart(2, '0')}:${minutes.toString().padStart(2, '0')}:${secs.toString().padStart(2, '0')}`;
  }

  onSave(): void {
    if (!this.medicalRecord()) return;

    this.isLoading.set(true);
    this.errorMessage.set('');
    this.successMessage.set('');

    const formValue = this.attendanceForm.value;
    
    this.medicalRecordService.update(this.medicalRecord()!.id, {
      diagnosis: formValue.diagnosis,
      prescription: formValue.prescription,
      notes: formValue.notes,
      consultationDurationMinutes: Math.floor(this.elapsedSeconds() / 60)
    }).subscribe({
      next: (record) => {
        this.medicalRecord.set(record);
        this.successMessage.set('Prontuário salvo com sucesso!');
        this.isLoading.set(false);
      },
      error: (error) => {
        console.error('Error saving medical record:', error);
        this.errorMessage.set('Erro ao salvar prontuário');
        this.isLoading.set(false);
      }
    });
  }

  onComplete(): void {
    if (!this.medicalRecord()) return;

    this.isLoading.set(true);
    this.errorMessage.set('');
    this.successMessage.set('');

    const formValue = this.attendanceForm.value;
    
    this.medicalRecordService.complete(this.medicalRecord()!.id, {
      diagnosis: formValue.diagnosis,
      prescription: formValue.prescription,
      notes: formValue.notes
    }).subscribe({
      next: () => {
        this.stopTimer();
        this.successMessage.set('Atendimento finalizado com sucesso!');
        this.isLoading.set(false);
        
        // Redireciona após 2 segundos
        setTimeout(() => {
          this.router.navigate(['/appointments']);
        }, 2000);
      },
      error: (error) => {
        console.error('Error completing consultation:', error);
        this.errorMessage.set('Erro ao finalizar atendimento');
        this.isLoading.set(false);
      }
    });
  }

  onPrint(): void {
    window.print();
  }
}
