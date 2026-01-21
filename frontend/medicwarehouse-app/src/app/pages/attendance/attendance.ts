import { Component, OnInit, OnDestroy, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { interval, Subscription, forkJoin } from 'rxjs';
import { Navbar } from '../../shared/navbar/navbar';
import { RichTextEditor } from '../../shared/rich-text-editor/rich-text-editor';
import { InformedConsentFormComponent } from './components/informed-consent-form.component';
import { AppointmentService } from '../../services/appointment';
import { MedicalRecordService } from '../../services/medical-record';
import { PatientService } from '../../services/patient';
import { ProcedureService } from '../../services/procedure';
import { ExamRequestService } from '../../services/exam-request';
import { NotificationService } from '../../services/notification.service';
import { ClinicalExaminationService } from '../../services/clinical-examination.service';
import { DiagnosticHypothesisService } from '../../services/diagnostic-hypothesis.service';
import { TherapeuticPlanService } from '../../services/therapeutic-plan.service';
import { InformedConsentService } from '../../services/informed-consent.service';
import { Appointment } from '../../models/appointment.model';
import { MedicalRecord, ClinicalExamination, DiagnosticHypothesis, TherapeuticPlan, InformedConsent, DiagnosisType, DiagnosisTypeLabels } from '../../models/medical-record.model';
import { Patient } from '../../models/patient.model';
import { Procedure, AppointmentProcedure, ProcedureCategory, ProcedureCategoryLabels } from '../../models/procedure.model';
import { ExamRequest, ExamType, ExamUrgency, ExamTypeLabels, ExamUrgencyLabels } from '../../models/exam-request.model';
import { MedicationAutocomplete } from '../../models/medication.model';
import { ExamAutocomplete } from '../../models/exam-catalog.model';

// Constants
const ICD10_PATTERN = /^[A-Z]\d{2}(\.\d{1,2})?$/;

@Component({
  selector: 'app-attendance',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, Navbar, RichTextEditor, InformedConsentFormComponent],
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
  clinicalExaminationForm: FormGroup;
  diagnosticForm: FormGroup;
  therapeuticPlanForm: FormGroup;
  
  // CFM 1.821 Entities
  clinicalExaminations = signal<ClinicalExamination[]>([]);
  diagnosticHypotheses = signal<DiagnosticHypothesis[]>([]);
  therapeuticPlans = signal<TherapeuticPlan[]>([]);
  informedConsents = signal<InformedConsent[]>([]);
  
  showAddClinicalExamination = signal<boolean>(false);
  showAddDiagnosis = signal<boolean>(false);
  showAddTherapeuticPlan = signal<boolean>(false);
  
  // Enum helpers
  diagnosisTypes = Object.values(DiagnosisType).filter(v => typeof v === 'number') as DiagnosisType[];
  diagnosisTypeLabels = DiagnosisTypeLabels;
  
  // Cronômetro
  elapsedSeconds = signal<number>(0);
  timerSubscription?: Subscription;
  autosaveSubscription?: Subscription;
  startTime?: Date;
  lastSaveTime?: Date;

  // Procedures
  availableProcedures = signal<Procedure[]>([]);
  appointmentProcedures = signal<AppointmentProcedure[]>([]);
  showAddProcedure = signal<boolean>(false);
  procedureForm: FormGroup;
  
  // Exam Requests
  examRequests = signal<ExamRequest[]>([]);
  showAddExam = signal<boolean>(false);
  examForm: FormGroup;
  
  // Payment tracking
  showPaymentDialog = signal<boolean>(false);
  registerPaymentOnComplete = signal<boolean>(false);
  
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
    private examRequestService: ExamRequestService,
    private notificationService: NotificationService,
    private clinicalExaminationService: ClinicalExaminationService,
    private diagnosticHypothesisService: DiagnosticHypothesisService,
    private therapeuticPlanService: TherapeuticPlanService,
    private informedConsentService: InformedConsentService
  ) {
    // CFM 1.821 Required Fields
    this.attendanceForm = this.fb.group({
      chiefComplaint: ['', [Validators.required, Validators.minLength(10)]],
      historyOfPresentIllness: ['', [Validators.required, Validators.minLength(50)]],
      pastMedicalHistory: [''],
      familyHistory: [''],
      lifestyleHabits: [''],
      currentMedications: [''],
      // Legacy fields (optional)
      diagnosis: [''],
      prescription: [''],
      notes: ['']
    });

    // Clinical Examination Form
    this.clinicalExaminationForm = this.fb.group({
      systematicExamination: ['', [Validators.required, Validators.minLength(20)]],
      bloodPressureSystolic: ['', [Validators.min(50), Validators.max(300)]],
      bloodPressureDiastolic: ['', [Validators.min(30), Validators.max(200)]],
      heartRate: ['', [Validators.min(30), Validators.max(220)]],
      respiratoryRate: ['', [Validators.min(8), Validators.max(60)]],
      temperature: ['', [Validators.min(32), Validators.max(45)]],
      oxygenSaturation: ['', [Validators.min(0), Validators.max(100)]],
      generalState: ['']
    });

    // Diagnostic Hypothesis Form
    this.diagnosticForm = this.fb.group({
      description: ['', Validators.required],
      icd10Code: ['', [Validators.required, Validators.pattern(ICD10_PATTERN)]],
      type: [DiagnosisType.Principal, Validators.required]
    });

    // Therapeutic Plan Form
    this.therapeuticPlanForm = this.fb.group({
      treatment: ['', [Validators.required, Validators.minLength(20)]],
      medicationPrescription: [''],
      examRequests: [''],
      referrals: [''],
      patientGuidance: [''],
      returnDate: ['']
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
          chiefComplaint: record.chiefComplaint,
          historyOfPresentIllness: record.historyOfPresentIllness,
          pastMedicalHistory: record.pastMedicalHistory,
          familyHistory: record.familyHistory,
          lifestyleHabits: record.lifestyleHabits,
          currentMedications: record.currentMedications,
          diagnosis: record.diagnosis,
          prescription: record.prescription,
          notes: record.notes
        });
        
        // Load CFM 1.821 entities
        this.loadCFMEntities(record.id);
        
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

  loadCFMEntities(medicalRecordId: string): void {
    // Load all CFM entities in parallel using forkJoin
    forkJoin({
      examinations: this.clinicalExaminationService.getByMedicalRecord(medicalRecordId),
      hypotheses: this.diagnosticHypothesisService.getByMedicalRecord(medicalRecordId),
      plans: this.therapeuticPlanService.getByMedicalRecord(medicalRecordId),
      consents: this.informedConsentService.getByMedicalRecord(medicalRecordId)
    }).subscribe({
      next: (results) => {
        this.clinicalExaminations.set(results.examinations);
        this.diagnosticHypotheses.set(results.hypotheses);
        this.therapeuticPlans.set(results.plans);
        this.informedConsents.set(results.consents);
      },
      error: (error) => {
        console.error('Error loading CFM entities:', error);
      }
    });
  }

  createMedicalRecord(appointmentId: string, patientId: string): void {
    const now = new Date().toISOString();
    // Initialize with empty required fields - will be filled by user
    this.medicalRecordService.create({
      appointmentId,
      patientId,
      consultationStartTime: now,
      chiefComplaint: '',
      historyOfPresentIllness: ''
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

    // Start autosave every 30 seconds
    this.startAutosave();
  }

  stopTimer(): void {
    if (this.timerSubscription) {
      this.timerSubscription.unsubscribe();
      this.timerSubscription = undefined;
    }
    this.stopAutosave();
  }

  startAutosave(): void {
    if (this.autosaveSubscription) {
      return; // Autosave já está ativo
    }

    // Autosave every 30 seconds
    this.autosaveSubscription = interval(30000).subscribe(() => {
      this.autoSave();
    });
  }

  stopAutosave(): void {
    if (this.autosaveSubscription) {
      this.autosaveSubscription.unsubscribe();
      this.autosaveSubscription = undefined;
    }
  }

  autoSave(): void {
    if (!this.medicalRecord() || !this.attendanceForm.dirty) return;

    const now = new Date();
    // Don't autosave if we just saved manually (within last 5 seconds)
    if (this.lastSaveTime && (now.getTime() - this.lastSaveTime.getTime()) < 5000) {
      return;
    }

    const formValue = this.attendanceForm.value;
    
    this.medicalRecordService.update(this.medicalRecord()!.id, {
      chiefComplaint: formValue.chiefComplaint,
      historyOfPresentIllness: formValue.historyOfPresentIllness,
      pastMedicalHistory: formValue.pastMedicalHistory,
      familyHistory: formValue.familyHistory,
      lifestyleHabits: formValue.lifestyleHabits,
      currentMedications: formValue.currentMedications,
      diagnosis: formValue.diagnosis,
      prescription: formValue.prescription,
      notes: formValue.notes,
      consultationDurationMinutes: Math.floor(this.elapsedSeconds() / 60)
    }).subscribe({
      next: (record) => {
        this.medicalRecord.set(record);
        this.lastSaveTime = new Date();
        this.attendanceForm.markAsPristine();
        // Silent autosave - no success message
        console.log('Autosave completed');
      },
      error: (error) => {
        console.error('Autosave error:', error);
        // Don't show error to user for autosave failures
      }
    });
  }

  formatTime(seconds: number): string {
    const hours = Math.floor(seconds / 3600);
    const minutes = Math.floor((seconds % 3600) / 60);
    const secs = seconds % 60;
    return `${hours.toString().padStart(2, '0')}:${minutes.toString().padStart(2, '0')}:${secs.toString().padStart(2, '0')}`;
  }

  onSave(): void {
    if (!this.medicalRecord()) return;

    if (this.attendanceForm.invalid) {
      this.errorMessage.set('Por favor, preencha todos os campos obrigatórios corretamente.');
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set('');
    this.successMessage.set('');

    const formValue = this.attendanceForm.value;
    
    this.medicalRecordService.update(this.medicalRecord()!.id, {
      chiefComplaint: formValue.chiefComplaint,
      historyOfPresentIllness: formValue.historyOfPresentIllness,
      pastMedicalHistory: formValue.pastMedicalHistory,
      familyHistory: formValue.familyHistory,
      lifestyleHabits: formValue.lifestyleHabits,
      currentMedications: formValue.currentMedications,
      diagnosis: formValue.diagnosis,
      prescription: formValue.prescription,
      notes: formValue.notes,
      consultationDurationMinutes: Math.floor(this.elapsedSeconds() / 60)
    }).subscribe({
      next: (record) => {
        this.medicalRecord.set(record);
        this.lastSaveTime = new Date();
        this.attendanceForm.markAsPristine();
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
    if (!this.medicalRecord() || !this.appointment()) return;

    this.isLoading.set(true);
    this.errorMessage.set('');
    this.successMessage.set('');

    const formValue = this.attendanceForm.value;
    
    // First complete the medical record
    this.medicalRecordService.complete(this.medicalRecord()!.id, {
      diagnosis: formValue.diagnosis,
      prescription: formValue.prescription,
      notes: formValue.notes
    }).subscribe({
      next: () => {
        // Then complete the appointment (check-out)
        this.appointmentService.complete(
          this.appointment()!.id,
          formValue.notes,
          this.registerPaymentOnComplete()
        ).subscribe({
          next: () => {
            this.stopTimer();
            this.successMessage.set('Atendimento finalizado com sucesso!');
            this.isLoading.set(false);
            
            // Notify secretary about completion
            this.notifySecretaryConsultationCompleted();
            
            // Redireciona após 2 segundos
            setTimeout(() => {
              this.router.navigate(['/appointments']);
            }, 2000);
          },
          error: (error) => {
            console.error('Error completing appointment:', error);
            this.errorMessage.set('Erro ao finalizar atendimento');
            this.isLoading.set(false);
          }
        });
      },
      error: (error) => {
        console.error('Error completing consultation:', error);
        this.errorMessage.set('Erro ao finalizar atendimento');
        this.isLoading.set(false);
      }
    });
  }

  togglePaymentRegistration(): void {
    this.registerPaymentOnComplete.update(v => !v);
  }

  markAppointmentAsPaid(paymentReceiverType: string): void {
    if (!this.appointment()) return;

    this.isLoading.set(true);
    this.appointmentService.markAsPaid(this.appointment()!.id, paymentReceiverType).subscribe({
      next: () => {
        // Reload appointment to get updated payment status
        this.appointmentService.getById(this.appointment()!.id).subscribe({
          next: (appointment) => {
            this.appointment.set(appointment);
            this.successMessage.set('Pagamento registrado com sucesso!');
            this.isLoading.set(false);
            this.showPaymentDialog.set(false);
          },
          error: (error) => {
            console.error('Error reloading appointment:', error);
            this.isLoading.set(false);
          }
        });
      },
      error: (error) => {
        console.error('Error marking payment:', error);
        this.errorMessage.set('Erro ao registrar pagamento');
        this.isLoading.set(false);
      }
    });
  }

  notifySecretaryConsultationCompleted(): void {
    const appointment = this.appointment();
    const patient = this.patient();
    
    if (!appointment || !patient) return;

    // Send notification to secretary
    this.notificationService.notifyAppointmentCompleted({
      appointmentId: appointment.id,
      doctorName: 'Dr. Sistema', // In real app, this would come from authenticated user
      patientName: patient.name,
      completedAt: new Date(),
      // nextPatientId and nextPatientName would be determined by checking the next appointment
    }).subscribe({
      next: () => {
        // Secretary notified successfully
      },
      error: (error) => {
        console.error('Error notifying secretary:', error);
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
    // The medication text is already inserted by the RichTextEditor component
    // Additional logic can be added here if needed (e.g., tracking selected medications)
  }

  onExamSelected(exam: ExamAutocomplete): void {
    // The exam text is already inserted by the RichTextEditor component
    // Additional logic can be added here if needed (e.g., auto-filling exam request form)
  }

  // CFM 1.821 - Clinical Examination Management
  toggleAddClinicalExamination(): void {
    this.showAddClinicalExamination.set(!this.showAddClinicalExamination());
    if (!this.showAddClinicalExamination()) {
      this.clinicalExaminationForm.reset();
    }
  }

  onAddClinicalExamination(): void {
    if (!this.clinicalExaminationForm.valid || !this.medicalRecord()) return;

    const formValue = this.clinicalExaminationForm.value;
    this.clinicalExaminationService.create({
      medicalRecordId: this.medicalRecord()!.id,
      systematicExamination: formValue.systematicExamination,
      bloodPressureSystolic: formValue.bloodPressureSystolic || undefined,
      bloodPressureDiastolic: formValue.bloodPressureDiastolic || undefined,
      heartRate: formValue.heartRate || undefined,
      respiratoryRate: formValue.respiratoryRate || undefined,
      temperature: formValue.temperature || undefined,
      oxygenSaturation: formValue.oxygenSaturation || undefined,
      generalState: formValue.generalState || undefined
    }).subscribe({
      next: (examination) => {
        this.clinicalExaminations.update(exams => [...exams, examination]);
        this.successMessage.set('Exame clínico adicionado com sucesso!');
        this.clinicalExaminationForm.reset();
        this.showAddClinicalExamination.set(false);
        setTimeout(() => this.successMessage.set(''), 3000);
      },
      error: (error) => {
        console.error('Error adding clinical examination:', error);
        this.errorMessage.set('Erro ao adicionar exame clínico');
        setTimeout(() => this.errorMessage.set(''), 3000);
      }
    });
  }

  // CFM 1.821 - Diagnostic Hypothesis Management
  toggleAddDiagnosis(): void {
    this.showAddDiagnosis.set(!this.showAddDiagnosis());
    if (!this.showAddDiagnosis()) {
      this.diagnosticForm.reset({ type: DiagnosisType.Principal });
    }
  }

  onAddDiagnosis(): void {
    if (!this.diagnosticForm.valid || !this.medicalRecord()) return;

    const formValue = this.diagnosticForm.value;
    this.diagnosticHypothesisService.create({
      medicalRecordId: this.medicalRecord()!.id,
      description: formValue.description,
      icd10Code: formValue.icd10Code.toUpperCase(),
      type: formValue.type
    }).subscribe({
      next: (hypothesis) => {
        this.diagnosticHypotheses.update(hyps => [...hyps, hypothesis]);
        this.successMessage.set('Hipótese diagnóstica adicionada com sucesso!');
        this.diagnosticForm.reset({ type: DiagnosisType.Principal });
        this.showAddDiagnosis.set(false);
        setTimeout(() => this.successMessage.set(''), 3000);
      },
      error: (error) => {
        console.error('Error adding diagnostic hypothesis:', error);
        this.errorMessage.set('Erro ao adicionar hipótese diagnóstica');
        setTimeout(() => this.errorMessage.set(''), 3000);
      }
    });
  }

  removeDiagnosis(hypothesis: DiagnosticHypothesis): void {
    if (!confirm('Tem certeza que deseja remover esta hipótese diagnóstica?')) return;

    this.diagnosticHypothesisService.delete(hypothesis.id).subscribe({
      next: () => {
        this.diagnosticHypotheses.update(hyps => hyps.filter(h => h.id !== hypothesis.id));
        this.successMessage.set('Hipótese diagnóstica removida com sucesso!');
        setTimeout(() => this.successMessage.set(''), 3000);
      },
      error: (error) => {
        console.error('Error removing diagnostic hypothesis:', error);
        this.errorMessage.set('Erro ao remover hipótese diagnóstica');
        setTimeout(() => this.errorMessage.set(''), 3000);
      }
    });
  }

  // CFM 1.821 - Therapeutic Plan Management
  toggleAddTherapeuticPlan(): void {
    this.showAddTherapeuticPlan.set(!this.showAddTherapeuticPlan());
    if (!this.showAddTherapeuticPlan()) {
      this.therapeuticPlanForm.reset();
    }
  }

  onAddTherapeuticPlan(): void {
    if (!this.therapeuticPlanForm.valid || !this.medicalRecord()) return;

    const formValue = this.therapeuticPlanForm.value;
    this.therapeuticPlanService.create({
      medicalRecordId: this.medicalRecord()!.id,
      treatment: formValue.treatment,
      medicationPrescription: formValue.medicationPrescription || undefined,
      examRequests: formValue.examRequests || undefined,
      referrals: formValue.referrals || undefined,
      patientGuidance: formValue.patientGuidance || undefined,
      returnDate: formValue.returnDate || undefined
    }).subscribe({
      next: (plan) => {
        this.therapeuticPlans.update(plans => [...plans, plan]);
        this.successMessage.set('Plano terapêutico adicionado com sucesso!');
        this.therapeuticPlanForm.reset();
        this.showAddTherapeuticPlan.set(false);
        setTimeout(() => this.successMessage.set(''), 3000);
      },
      error: (error) => {
        console.error('Error adding therapeutic plan:', error);
        this.errorMessage.set('Erro ao adicionar plano terapêutico');
        setTimeout(() => this.errorMessage.set(''), 3000);
      }
    });
  }

  // CFM 1.821 - Informed Consent Management
  onConsentCreated(consent: InformedConsent): void {
    this.informedConsents.update(consents => [...consents, consent]);
  }
}
