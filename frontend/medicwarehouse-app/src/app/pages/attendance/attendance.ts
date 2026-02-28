import { Component, OnInit, OnDestroy, signal, ViewChild, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { interval, Subscription, forkJoin } from 'rxjs';
import { HelpButtonComponent } from '../../shared/help-button/help-button';
import { RichTextEditor } from '../../shared/rich-text-editor/rich-text-editor';
import { InformedConsentFormComponent } from './components/informed-consent-form.component';
import { ClinicalExaminationFormComponent } from './components/clinical-examination-form.component';
import { DiagnosticHypothesisFormComponent } from './components/diagnostic-hypothesis-form.component';
import { TherapeuticPlanFormComponent } from './components/therapeutic-plan-form.component';
import { CustomFieldsRendererComponent } from './components/custom-fields-renderer.component';
import { NotificationModalComponent } from '../../shared/notification-modal/notification-modal';
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
import { ConsultationFormConfigurationService } from '../../services/consultation-form-configuration.service';
import { TerminologyService, TerminologyMap } from '../../services/terminology.service';
import { Appointment, ProfessionalSpecialty, professionalSpecialtyToString } from '../../models/appointment.model';
import { Auth } from '../../services/auth';
import { MedicalRecord, ClinicalExamination, DiagnosticHypothesis, TherapeuticPlan, InformedConsent, DiagnosisType, DiagnosisTypeLabels } from '../../models/medical-record.model';
import { Patient } from '../../models/patient.model';
import { Procedure, AppointmentProcedure, ProcedureCategory, ProcedureCategoryLabels } from '../../models/procedure.model';
import { ExamRequest, ExamType, ExamUrgency, ExamTypeLabels, ExamUrgencyLabels } from '../../models/exam-request.model';
import { MedicationAutocomplete } from '../../models/medication.model';
import { ExamAutocomplete } from '../../models/exam-catalog.model';
import type { CallNextPatientNotification } from '../../models/notification.model';
import { NotificationType } from '../../models/notification.model';
import { ConsultationFormConfigurationDto } from '../../models/consultation-form-configuration.model';
import { RolePermissionService } from '../../services/role-permission.service';

// Constants
const ICD10_PATTERN = /^[A-Z]\d{2}(\.\d{1,2})?$/;

@Component({
  selector: 'app-attendance',
  standalone: true,
  imports: [
    HelpButtonComponent, 
    CommonModule, 
    ReactiveFormsModule, 
    RouterLink, 
    
    RichTextEditor, 
    InformedConsentFormComponent, 
    ClinicalExaminationFormComponent, 
    DiagnosticHypothesisFormComponent, 
    TherapeuticPlanFormComponent,
    CustomFieldsRendererComponent,
    NotificationModalComponent
  ],
  templateUrl: './attendance.html',
  styleUrl: './attendance.scss'
})
export class Attendance implements OnInit, OnDestroy {
  @ViewChild(NotificationModalComponent) notificationModal?: NotificationModalComponent;
  
  appointmentId = signal<string | null>(null);
  appointment = signal<Appointment | null>(null);
  patient = signal<Patient | null>(null);
  medicalRecord = signal<MedicalRecord | null>(null);
  patientHistory = signal<MedicalRecord[]>([]);
  nextPatient = signal<Appointment | null>(null);
  formConfig = signal<ConsultationFormConfigurationDto | null>(null);
  terminology = signal<TerminologyMap | null>(null);
  
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');
  successMessage = signal<string>('');
  warningMessage = signal<string>('');
  
  attendanceForm: FormGroup;
  
  // CFM 1.821 Entities
  clinicalExaminations = signal<ClinicalExamination[]>([]);
  diagnosticHypotheses = signal<DiagnosticHypothesis[]>([]);
  therapeuticPlans = signal<TherapeuticPlan[]>([]);
  informedConsents = signal<InformedConsent[]>([]);
  
  // CFM 1.821 Compliance Status
  cfm1821Status = signal<any | null>(null);
  cfm1821IsCompliant = signal<boolean>(false);
  cfm1821MissingRequirements = signal<string[]>([]);
  cfm1821CompletenessPercentage = signal<number>(0);
  
  // Enum helpers
  diagnosisTypes = Object.values(DiagnosisType).filter(v => typeof v === 'number') as DiagnosisType[];
  diagnosisTypeLabels = DiagnosisTypeLabels;
  
  // Cronômetro
  elapsedSeconds = signal<number>(0);
  timerSubscription?: Subscription;
  autosaveSubscription?: Subscription;
  lastSaveTime?: Date;
  
  // Autosave configuration
  private readonly AUTOSAVE_INTERVAL_MS = 30000; // 30 seconds
  private readonly MIN_TIME_BETWEEN_SAVES_MS = 5000; // 5 seconds

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

  // Services
  private consultationFormConfigService = inject(ConsultationFormConfigurationService);
  private terminologyService = inject(TerminologyService);
  private authService = inject(Auth);
  private rolePermissionService = inject(RolePermissionService);

  readonly professionalProfile = signal<'doctor' | 'nutritionist' | 'psychologist' | 'unknown'>('unknown');
  readonly canAccessAttendance = signal<boolean>(true);

  isDoctorProfile(): boolean {
    return this.professionalProfile() === 'doctor';
  }

  isNutritionistProfile(): boolean {
    return this.professionalProfile() === 'nutritionist';
  }

  isPsychologistProfile(): boolean {
    return this.professionalProfile() === 'psychologist';
  }

  private resolveProfessionalProfile(role?: string | null): 'doctor' | 'nutritionist' | 'psychologist' | 'unknown' {
    const normalizedRole = (role ?? '')
      .toLowerCase()
      .normalize('NFD')
      .replace(/[̀-ͯ]/g, '')
      .trim();

    if (['doctor', 'medico', 'medica', 'medico(a)'].includes(normalizedRole)) return 'doctor';
    if (['nutritionist', 'nutricionista'].includes(normalizedRole)) return 'nutritionist';
    if (['psychologist', 'psicologo', 'psicologa'].includes(normalizedRole)) return 'psychologist';

    return 'unknown';
  }

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
      nutritionalPlan: [''],
      nutritionalEvolution: [''],
      therapeuticEvolution: [''],
      // Legacy fields (optional)
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
    const currentUser = this.authService.currentUser();
    const profile = this.resolveProfessionalProfile(currentUser?.role);
    this.professionalProfile.set(profile);

    const canAccess = this.rolePermissionService.canAccessCareFeatures(currentUser?.role, !!currentUser?.isSystemOwner)
      && profile !== 'unknown';

    this.canAccessAttendance.set(canAccess);
    if (!canAccess) {
      this.errorMessage.set('Perfil sem permissão para visualizar ou registrar atendimentos.');
      this.router.navigate(['/403']);
      return;
    }

    const id = this.route.snapshot.paramMap.get('appointmentId');
    if (id) {
      this.appointmentId.set(id);
      this.loadAppointment(id);
      this.loadAvailableProcedures();
      this.loadAppointmentProcedures(id);
      this.loadExamRequests(id);
    }

    // Subscribe to notifications to show modal
    this.notificationService.onNotification$.subscribe(notification => {
      if (notification.type === NotificationType.CallNextPatient && !notification.isRead) {
        // Show modal for call next patient notifications
        // Delay ensures ViewChild is initialized after component render
        const MODAL_DISPLAY_DELAY = 100; // milliseconds
        setTimeout(() => {
          if (this.notificationModal) {
            this.notificationModal.show(notification);
          }
        }, MODAL_DISPLAY_DELAY);
      }
    });
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
        this.loadFormConfiguration(appointment.clinicId);
        // Use strongly-typed enum if available, fallback to string specialty
        const specialtyToLoad = appointment.professionalSpecialtyEnum 
          ? professionalSpecialtyToString(appointment.professionalSpecialtyEnum)
          : appointment.professionalSpecialty;
        this.loadTerminology(specialtyToLoad);
        this.isLoading.set(false);
      },
      error: (error) => {
        console.error('Error loading appointment:', error);
        this.errorMessage.set('Erro ao carregar agendamento');
        this.isLoading.set(false);
      }
    });
  }

  loadTerminology(specialty?: string): void {
    this.terminologyService.getTerminologyBySpecialty(specialty).subscribe({
      next: (terminology) => {
        this.terminology.set(terminology);
      },
      error: (error) => {
        console.error('Error loading terminology:', error);
        // Fallback to default terminology
        this.terminology.set({
          appointment: 'Consulta',
          professional: 'Profissional',
          registration: 'Registro',
          client: 'Paciente',
          mainDocument: 'Prontuário',
          exitDocument: 'Documento'
        });
      }
    });
  }

  loadFormConfiguration(clinicId: string): void {
    this.consultationFormConfigService.getActiveConfigurationByClinicId(clinicId).subscribe({
      next: (config) => {
        this.formConfig.set(config);
        this.applyFormValidators(config);
      },
      error: (error) => {
        // Configuration not found (404) or other error - use default behavior (all fields required)
        if (error.status === 404) {
          console.log('No custom form configuration found for clinic, using default validators');
        } else {
          console.error('Error loading form configuration:', error);
        }
        this.formConfig.set(null);
      }
    });
  }

  private applyFormValidators(config: ConsultationFormConfigurationDto): void {
    // Update chiefComplaint validators
    const chiefComplaintControl = this.attendanceForm.get('chiefComplaint');
    if (chiefComplaintControl) {
      const validators = config.requireChiefComplaint 
        ? [Validators.required, Validators.minLength(10)] 
        : [Validators.minLength(10)];
      chiefComplaintControl.setValidators(validators);
      chiefComplaintControl.updateValueAndValidity();
    }

    // Update historyOfPresentIllness validators
    const hpiControl = this.attendanceForm.get('historyOfPresentIllness');
    if (hpiControl) {
      const validators = config.requireHistoryOfPresentIllness 
        ? [Validators.required, Validators.minLength(50)] 
        : [Validators.minLength(50)];
      hpiControl.setValidators(validators);
      hpiControl.updateValueAndValidity();
    }

    // Update pastMedicalHistory validators
    const pmhControl = this.attendanceForm.get('pastMedicalHistory');
    if (pmhControl) {
      const validators = config.requirePastMedicalHistory 
        ? [Validators.required] 
        : [];
      pmhControl.setValidators(validators);
      pmhControl.updateValueAndValidity();
    }

    // Update familyHistory validators
    const familyControl = this.attendanceForm.get('familyHistory');
    if (familyControl) {
      const validators = config.requireFamilyHistory 
        ? [Validators.required] 
        : [];
      familyControl.setValidators(validators);
      familyControl.updateValueAndValidity();
    }

    // Update lifestyleHabits validators
    const lifestyleControl = this.attendanceForm.get('lifestyleHabits');
    if (lifestyleControl) {
      const validators = config.requireLifestyleHabits 
        ? [Validators.required] 
        : [];
      lifestyleControl.setValidators(validators);
      lifestyleControl.updateValueAndValidity();
    }

    // Update currentMedications validators
    const medicationsControl = this.attendanceForm.get('currentMedications');
    if (medicationsControl) {
      const validators = config.requireCurrentMedications 
        ? [Validators.required] 
        : [];
      medicationsControl.setValidators(validators);
      medicationsControl.updateValueAndValidity();
    }
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
          nutritionalPlan: record.nutritionalPlan,
          nutritionalEvolution: record.nutritionalEvolution,
          therapeuticEvolution: record.therapeuticEvolution,
          diagnosis: record.diagnosis,
          prescription: record.prescription,
          notes: record.notes
        });
        
        // Load CFM 1.821 entities
        this.loadCFMEntities(record.id);
        
        // Resume timer from saved elapsed time if consultation is still in progress
        if (record.consultationStartTime && !record.consultationEndTime) {
          // Initialize elapsed seconds from saved duration (convert minutes to seconds)
          // Handle null/undefined with fallback to 0
          const savedSeconds = (record.consultationDurationMinutes || 0) * 60;
          this.elapsedSeconds.set(savedSeconds);
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
        // Start timer at 00:00 for new consultations
        this.elapsedSeconds.set(0);
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

    // Timer increments from current elapsed seconds
    // This ensures proper behavior when:
    // 1. Starting a new consultation (elapsedSeconds = 0)
    // 2. Resuming after leaving page (elapsedSeconds = saved value)
    // 3. Starting appointments early (no dependency on scheduled time)
    
    this.timerSubscription = interval(1000).subscribe(() => {
      // Simply increment elapsed seconds by 1 each second
      this.elapsedSeconds.update(val => val + 1);
    });

    // Start autosave every 30 seconds (cleanup first to prevent duplicates)
    this.stopAutosave();
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

    // Autosave at configured interval
    this.autosaveSubscription = interval(this.AUTOSAVE_INTERVAL_MS).subscribe(() => {
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
    // Don't autosave if we just saved manually
    if (this.lastSaveTime && (now.getTime() - this.lastSaveTime.getTime()) < this.MIN_TIME_BETWEEN_SAVES_MS) {
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
      nutritionalPlan: formValue.nutritionalPlan,
      nutritionalEvolution: formValue.nutritionalEvolution,
      therapeuticEvolution: formValue.therapeuticEvolution,
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

  /**
   * Get the title for the custom fields section based on professional terminology
   */
  getCustomFieldsSectionTitle(): string {
    const professionalTerm = this.terminology()?.professional || 'Profissional';
    return `Campos Específicos - ${professionalTerm}`;
  }

  onSave(): void {
    if (!this.medicalRecord()) return;

    this.isLoading.set(true);
    this.errorMessage.set('');
    this.successMessage.set('');
    this.warningMessage.set('');

    // Show warning if form is invalid but continue with save
    if (this.attendanceForm.invalid) {
      this.warningMessage.set('⚠️ Atenção: Alguns campos obrigatórios não foram preenchidos. Recomenda-se completá-los para conformidade com CFM 1.821/2007.');
    }

    const formValue = this.attendanceForm.value;
    
    this.medicalRecordService.update(this.medicalRecord()!.id, {
      chiefComplaint: formValue.chiefComplaint,
      historyOfPresentIllness: formValue.historyOfPresentIllness,
      pastMedicalHistory: formValue.pastMedicalHistory,
      familyHistory: formValue.familyHistory,
      lifestyleHabits: formValue.lifestyleHabits,
      currentMedications: formValue.currentMedications,
      nutritionalPlan: formValue.nutritionalPlan,
      nutritionalEvolution: formValue.nutritionalEvolution,
      therapeuticEvolution: formValue.therapeuticEvolution,
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

    // Check CFM 1.821 compliance and show warning if not compliant
    this.checkCfm1821Compliance(() => {
      // Show warning but allow completion
      if (!this.cfm1821IsCompliant()) {
        const missingFieldsList = this.cfm1821MissingRequirements().map(field => `• ${field}`).join('\n');
        this.warningMessage.set(`⚠️ ATENÇÃO: Atendimento não está em conformidade com CFM 1.821/2007.\n\nCampos faltando:\n${missingFieldsList}\n\nO atendimento será finalizado mesmo assim.`);
      }

      this.proceedWithCompletion();
    });
  }

  private proceedWithCompletion(): void {
    this.isLoading.set(true);
    this.errorMessage.set('');
    this.successMessage.set('');
    // Keep warning message visible during completion

    const formValue = this.attendanceForm.value;
    
    // First complete the medical record
    this.medicalRecordService.complete(this.medicalRecord()!.id, {
      diagnosis: formValue.diagnosis,
      prescription: formValue.prescription,
      notes: formValue.notes,
      nutritionalPlan: formValue.nutritionalPlan,
      nutritionalEvolution: formValue.nutritionalEvolution,
      therapeuticEvolution: formValue.therapeuticEvolution
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
        const errorMessage = error.error?.message || error.error || 'Erro ao finalizar atendimento';
        this.errorMessage.set(errorMessage);
        this.isLoading.set(false);
      }
    });
  }

  checkCfm1821Compliance(onComplete?: () => void): void {
    if (!this.medicalRecord()) return;

    this.medicalRecordService.getCfm1821Status(this.medicalRecord()!.id).subscribe({
      next: (status) => {
        this.cfm1821Status.set(status);
        this.cfm1821IsCompliant.set(status.isCompliant);
        this.cfm1821MissingRequirements.set(status.missingRequirements);
        this.cfm1821CompletenessPercentage.set(status.completenessPercentage);
        
        if (onComplete) {
          onComplete();
        }
      },
      error: (error) => {
        console.error('Error checking CFM 1.821 compliance:', error);
        this.errorMessage.set('Erro ao verificar conformidade CFM 1.821');
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

  callNextPatient(): void {
    const appointment = this.appointment();
    const patient = this.patient();
    
    if (!appointment || !patient) {
      this.errorMessage.set('Nenhum paciente selecionado');
      setTimeout(() => this.errorMessage.set(''), 3000);
      return;
    }

    // Get next patient from agenda
    this.loadNextPatient(appointment.clinicId, appointment.scheduledDate);
  }

  private loadNextPatient(clinicId: string, date: string): void {
    const currentTime = this.appointment()?.scheduledTime;
    
    this.appointmentService.getDailyAgenda(clinicId, date).subscribe({
      next: (agenda) => {
        // Find next appointment after current one
        const upcomingAppointments = agenda.appointments
          .filter(apt => apt.scheduledTime > (currentTime || '') && apt.status !== 'Cancelled' && apt.status !== 'Completed')
          .sort((a, b) => a.scheduledTime.localeCompare(b.scheduledTime));
        
        const next = upcomingAppointments.length > 0 ? upcomingAppointments[0] : null;
        
        if (next) {
          this.nextPatient.set(next);
          this.triggerCallNextPatientNotification(next);
        } else {
          this.notificationService.info('Não há próximo paciente agendado');
        }
      },
      error: (error) => {
        console.error('Error loading next patient:', error);
        this.errorMessage.set('Erro ao buscar próximo paciente');
        setTimeout(() => this.errorMessage.set(''), 3000);
      }
    });
  }

  private triggerCallNextPatientNotification(nextAppointment: Appointment): void {
    const notification: CallNextPatientNotification = {
      appointmentId: nextAppointment.id,
      patientName: nextAppointment.patientName,
      doctorName: nextAppointment.doctorName || 'Dr. Sistema',
      roomNumber: undefined // TODO: Room number could be determined from clinic configuration
    };

    this.notificationService.callNextPatient(notification).subscribe({
      next: () => {
        this.successMessage.set(`Chamando próximo paciente: ${nextAppointment.patientName}`);
        setTimeout(() => this.successMessage.set(''), 5000);
        this.notificationService.playNotificationSound();
      },
      error: (error) => {
        console.error('Error calling next patient:', error);
        this.errorMessage.set('Erro ao chamar próximo paciente');
        setTimeout(() => this.errorMessage.set(''), 3000);
      }
    });
  }

  onNotificationConfirmed(notificationId: string): void {
    // Mark notification as read
    this.notificationService.markAsRead(notificationId).subscribe({
      next: () => {
        console.log('Notification marked as read');
      },
      error: (error) => {
        console.error('Error marking notification as read:', error);
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
  onClinicalExaminationSaved(examination: ClinicalExamination): void {
    // Reload all CFM entities to get the latest data
    if (this.medicalRecord()?.id) {
      this.loadCFMEntities(this.medicalRecord()!.id);
    }
    this.successMessage.set('Exame clínico salvo com sucesso!');
    setTimeout(() => this.successMessage.set(''), 3000);
  }

  // CFM 1.821 - Diagnostic Hypothesis Management
  onDiagnosticHypothesisSaved(hypothesis: DiagnosticHypothesis): void {
    // Reload all CFM entities to get the latest data
    if (this.medicalRecord()?.id) {
      this.loadCFMEntities(this.medicalRecord()!.id);
    }
    this.successMessage.set('Hipótese diagnóstica salva com sucesso!');
    setTimeout(() => this.successMessage.set(''), 3000);
  }

  onDiagnosticHypothesisDeleted(id: string): void {
    // Reload all CFM entities to get the latest data
    if (this.medicalRecord()?.id) {
      this.loadCFMEntities(this.medicalRecord()!.id);
    }
    this.successMessage.set('Hipótese diagnóstica removida com sucesso!');
    setTimeout(() => this.successMessage.set(''), 3000);
  }

  // CFM 1.821 - Therapeutic Plan Management
  onTherapeuticPlanSaved(plan: TherapeuticPlan): void {
    // Reload all CFM entities to get the latest data
    if (this.medicalRecord()?.id) {
      this.loadCFMEntities(this.medicalRecord()!.id);
    }
    this.successMessage.set('Plano terapêutico salvo com sucesso!');
    setTimeout(() => this.successMessage.set(''), 3000);
  }

  // CFM 1.821 - Informed Consent Management
  onConsentCreated(consent: InformedConsent): void {
    this.informedConsents.update(consents => [...consents, consent]);
  }
}
