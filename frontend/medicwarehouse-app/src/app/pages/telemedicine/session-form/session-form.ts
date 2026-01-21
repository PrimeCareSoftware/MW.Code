import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { Navbar } from '../../../shared/navbar/navbar';
import { TelemedicineService } from '../../../services/telemedicine.service';
import { AppointmentService } from '../../../services/appointment';
import { CreateSessionRequest } from '../../../models/telemedicine.model';
import { Appointment } from '../../../models/appointment.model';
import { Auth } from '../../../services/auth';

@Component({
  selector: 'app-session-form',
  imports: [CommonModule, ReactiveFormsModule, Navbar],
  templateUrl: './session-form.html',
  styleUrl: './session-form.scss'
})
export class SessionForm implements OnInit {
  sessionForm: FormGroup;
  appointments = signal<Appointment[]>([]);
  isLoading = signal<boolean>(false);
  isSaving = signal<boolean>(false);
  errorMessage = signal<string>('');
  successMessage = signal<string>('');
  clinicId: string | null = null;
  appointmentId: string | null = null;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private route: ActivatedRoute,
    private telemedicineService: TelemedicineService,
    private appointmentService: AppointmentService,
    private auth: Auth
  ) {
    this.sessionForm = this.fb.group({
      appointmentId: ['', Validators.required],
      notes: ['']
    });
  }

  ngOnInit(): void {
    this.clinicId = this.auth.getClinicId();
    
    if (!this.clinicId) {
      this.errorMessage.set('Para criar uma sessão, você precisa estar associado a uma clínica.');
      return;
    }

    // Check if appointmentId is provided in query params
    this.route.queryParams.subscribe(params => {
      this.appointmentId = params['appointmentId'];
      if (this.appointmentId) {
        this.sessionForm.patchValue({ appointmentId: this.appointmentId });
      }
    });

    this.loadTodayAppointments();
  }

  loadTodayAppointments(): void {
    if (!this.clinicId) return;
    
    this.isLoading.set(true);
    const today = new Date().toISOString().split('T')[0];
    
    this.appointmentService.getDailyAgenda(this.clinicId, today).subscribe({
      next: (agenda) => {
        // Filter appointments that don't have a session yet
        this.appointments.set(agenda.appointments || []);
        this.isLoading.set(false);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao carregar agendamentos');
        this.isLoading.set(false);
        console.error('Error loading appointments:', error);
      }
    });
  }

  onAppointmentChange(event: Event): void {
    const appointmentId = (event.target as HTMLSelectElement).value;
    if (!appointmentId) return;

    const appointment = this.appointments().find(a => a.id === appointmentId);
    if (appointment) {
      // Pre-fill notes if needed
      this.sessionForm.patchValue({
        notes: `Sessão de telemedicina para consulta de ${appointment.patientName}`
      });
    }
  }

  onSubmit(): void {
    if (this.sessionForm.invalid) {
      this.errorMessage.set('Por favor, preencha todos os campos obrigatórios');
      return;
    }

    if (!this.clinicId) {
      this.errorMessage.set('ID da clínica não disponível');
      return;
    }

    const selectedAppointmentId = this.sessionForm.value.appointmentId;
    const selectedAppointment = this.appointments().find(a => a.id === selectedAppointmentId);

    if (!selectedAppointment) {
      this.errorMessage.set('Agendamento selecionado não encontrado');
      return;
    }

    const user = this.auth.currentUser();
    if (!user) {
      this.errorMessage.set('Usuário não autenticado');
      return;
    }

    const request: CreateSessionRequest = {
      appointmentId: selectedAppointmentId,
      clinicId: this.clinicId,
      providerId: selectedAppointment.doctorId || user.username,
      patientId: selectedAppointment.patientId
    };

    this.isSaving.set(true);
    this.errorMessage.set('');

    this.telemedicineService.createSession(request).subscribe({
      next: (session) => {
        this.successMessage.set('Sessão criada com sucesso!');
        this.isSaving.set(false);
        
        setTimeout(() => {
          this.router.navigate(['/telemedicine']);
        }, 1500);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao criar sessão. Verifique se já não existe uma sessão para este agendamento.');
        this.isSaving.set(false);
        console.error('Error creating session:', error);
      }
    });
  }

  cancel(): void {
    this.router.navigate(['/telemedicine']);
  }

  getAppointmentDisplay(appointment: Appointment): string {
    const time = new Date(appointment.scheduledDate).toLocaleTimeString('pt-BR', {
      hour: '2-digit',
      minute: '2-digit'
    });
    return `${time} - ${appointment.patientName}`;
  }
}
