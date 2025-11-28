import { Component, OnInit, OnDestroy, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { interval, Subscription } from 'rxjs';
import { Navbar } from '../../shared/navbar/navbar';
import { RichTextEditor } from '../../shared/rich-text-editor/rich-text-editor';
import { AppointmentService } from '../../services/appointment';
import { MedicalRecordService } from '../../services/medical-record';
import { PatientService } from '../../services/patient';
import { ProcedureService } from '../../services/procedure';
import { ExamRequestService } from '../../services/exam-request';
import { Appointment } from '../../models/appointment.model';
import { MedicalRecord } from '../../models/medical-record.model';
import { Patient } from '../../models/patient.model';
import { Procedure, AppointmentProcedure, ProcedureCategory, ProcedureCategoryLabels } from '../../models/procedure.model';
import { ExamRequest, ExamType, ExamUrgency, ExamTypeLabels, ExamUrgencyLabels } from '../../models/exam-request.model';
import { MedicationAutocomplete } from '../../models/medication.model';
import { ExamAutocomplete } from '../../models/exam-catalog.model';

@Component({
  selector: 'app-attendance',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, Navbar, RichTextEditor],
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

  // Procedures
  availableProcedures = signal<Procedure[]>([]);
  appointmentProcedures = signal<AppointmentProcedure[]>([]);
  showAddProcedure = signal<boolean>(false);
  procedureForm: FormGroup;
  
  // Exam Requests
  examRequests = signal<ExamRequest[]>([]);
  showAddExam = signal<boolean>(false);
  examForm: FormGroup;
  
  // Enum helpers
  procedureCategories = Object.values(ProcedureCategory).filter(v => typeof v === 'number') as ProcedureCategory[];
  procedureCategoryLabels = ProcedureCategoryLabels;
  examTypes = Object.values(ExamType).filter(v => typeof v === 'number') as ExamType[];
  examTypeLabels = ExamTypeLabels;
  examUrgencies = Object.values(ExamUrgency).filter(v => typeof v === 'number') as ExamUrgency[];
  examUrgencyLabels = ExamUrgencyLabels;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private appointmentService: AppointmentService,
    private medicalRecordService: MedicalRecordService,
    private patientService: PatientService,
    private procedureService: ProcedureService,
    private examRequestService: ExamRequestService
  ) {
    this.attendanceForm = this.fb.group({
      diagnosis: [''],
      prescription: [''],
      notes: ['']
    });

    this.procedureForm = this.fb.group({
      procedureId: ['', Validators.required],
      customPrice: [''],
      notes: ['']
    });

    this.examForm = this.fb.group({
      examType: [ExamType.Laboratory, Validators.required],
      examName: ['', Validators.required],
      description: ['', Validators.required],
      urgency: [ExamUrgency.Routine, Validators.required],
      notes: ['']
    });
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('appointmentId');
    if (id) {
      this.appointmentId.set(id);
      this.loadAppointment(id);
      this.loadAvailableProcedures();
      this.loadAppointmentProcedures(id);
      this.loadExamRequests(id);
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

  // Procedure Management
  loadAvailableProcedures(): void {
    this.procedureService.getAll(true).subscribe({
      next: (procedures) => {
        this.availableProcedures.set(procedures);
      },
      error: (error) => {
        console.error('Error loading procedures:', error);
      }
    });
  }

  loadAppointmentProcedures(appointmentId: string): void {
    this.procedureService.getAppointmentProcedures(appointmentId).subscribe({
      next: (procedures) => {
        this.appointmentProcedures.set(procedures);
      },
      error: (error) => {
        console.error('Error loading appointment procedures:', error);
      }
    });
  }

  toggleAddProcedure(): void {
    this.showAddProcedure.set(!this.showAddProcedure());
    if (!this.showAddProcedure()) {
      this.procedureForm.reset();
    }
  }

  onAddProcedure(): void {
    if (!this.procedureForm.valid || !this.appointmentId()) return;

    const formValue = this.procedureForm.value;
    this.procedureService.addProcedureToAppointment(this.appointmentId()!, {
      procedureId: formValue.procedureId,
      customPrice: formValue.customPrice || undefined,
      notes: formValue.notes || undefined
    }).subscribe({
      next: (procedure) => {
        this.appointmentProcedures.update(procs => [...procs, procedure]);
        this.successMessage.set('Procedimento adicionado com sucesso!');
        this.procedureForm.reset();
        this.showAddProcedure.set(false);
        setTimeout(() => this.successMessage.set(''), 3000);
      },
      error: (error) => {
        console.error('Error adding procedure:', error);
        this.errorMessage.set('Erro ao adicionar procedimento');
        setTimeout(() => this.errorMessage.set(''), 3000);
      }
    });
  }

  getProcedureName(procedureId: string): string {
    const procedure = this.availableProcedures().find(p => p.id === procedureId);
    return procedure ? procedure.name : '';
  }

  getProcedurePrice(procedureId: string): number {
    const procedure = this.availableProcedures().find(p => p.id === procedureId);
    return procedure ? procedure.price : 0;
  }

  // Exam Request Management
  toggleAddExam(): void {
    this.showAddExam.set(!this.showAddExam());
    if (!this.showAddExam()) {
      this.examForm.reset({
        examType: ExamType.Laboratory,
        urgency: ExamUrgency.Routine
      });
    }
  }

  loadExamRequests(appointmentId: string): void {
    this.examRequestService.getByAppointment(appointmentId).subscribe({
      next: (examRequests) => {
        this.examRequests.set(examRequests);
      },
      error: (error) => {
        console.error('Error loading exam requests:', error);
      }
    });
  }

  onAddExamRequest(): void {
    if (!this.examForm.valid || !this.appointmentId() || !this.patient()) return;

    const formValue = this.examForm.value;
    
    this.examRequestService.create({
      appointmentId: this.appointmentId()!,
      patientId: this.patient()!.id,
      examType: formValue.examType,
      examName: formValue.examName,
      description: formValue.description,
      urgency: formValue.urgency,
      notes: formValue.notes || undefined
    }).subscribe({
      next: (examRequest) => {
        this.examRequests.update(exams => [...exams, examRequest]);
        this.successMessage.set('Pedido de exame adicionado com sucesso!');
        this.examForm.reset({
          examType: ExamType.Laboratory,
          urgency: ExamUrgency.Routine
        });
        this.showAddExam.set(false);
        setTimeout(() => this.successMessage.set(''), 3000);
      },
      error: (error) => {
        console.error('Error adding exam request:', error);
        this.errorMessage.set('Erro ao adicionar pedido de exame');
        setTimeout(() => this.errorMessage.set(''), 3000);
      }
    });
  }

  removeExamRequest(examRequest: ExamRequest): void {
    if (!confirm('Tem certeza que deseja remover este pedido de exame?')) return;

    this.examRequestService.cancel(examRequest.id).subscribe({
      next: () => {
        this.examRequests.update(exams => exams.filter(e => e.id !== examRequest.id));
        this.successMessage.set('Pedido de exame removido com sucesso!');
        setTimeout(() => this.successMessage.set(''), 3000);
      },
      error: (error) => {
        console.error('Error removing exam request:', error);
        this.errorMessage.set('Erro ao remover pedido de exame');
        setTimeout(() => this.errorMessage.set(''), 3000);
      }
    });
  }

  getTotalProceduresCost(): number {
    return this.appointmentProcedures().reduce((sum, proc) => sum + proc.priceCharged, 0);
  }

  // Event handlers for medication and exam autocomplete
  onMedicationSelected(medication: MedicationAutocomplete): void {
    console.log('Medicação selecionada:', medication);
    // The medication text is already inserted by the RichTextEditor component
    // Additional logic can be added here if needed (e.g., tracking selected medications)
  }

  onExamSelected(exam: ExamAutocomplete): void {
    console.log('Exame selecionado:', exam);
    // The exam text is already inserted by the RichTextEditor component
    // Additional logic can be added here if needed (e.g., auto-filling exam request form)
  }
}
