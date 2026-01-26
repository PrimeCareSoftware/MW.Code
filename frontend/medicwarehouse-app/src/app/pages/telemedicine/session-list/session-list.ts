import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Navbar } from '../../../shared/navbar/navbar';
import { HelpButtonComponent } from '../../../shared/help-button/help-button';
import { TelemedicineService } from '../../../services/telemedicine.service';
import { TelemedicineSession, SessionStatus } from '../../../models/telemedicine.model';
import { Auth } from '../../../services/auth';

@Component({
  selector: 'app-session-list',
  imports: [HelpButtonComponent, CommonModule, RouterLink, Navbar],
  templateUrl: './session-list.html',
  styleUrl: './session-list.scss'
})
export class SessionList implements OnInit {
  sessions = signal<TelemedicineSession[]>([]);
  filteredSessions = signal<TelemedicineSession[]>([]);
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');
  selectedStatus = signal<string>('All');
  clinicId: string | null = null;

  readonly SessionStatus = SessionStatus;

  constructor(
    private telemedicineService: TelemedicineService,
    private auth: Auth
  ) {}

  ngOnInit(): void {
    this.clinicId = this.auth.getClinicId();
    
    if (!this.clinicId) {
      this.errorMessage.set('Para acessar as sessões de telemedicina, você precisa estar associado a uma clínica.');
      return;
    }
    
    this.loadSessions();
  }

  loadSessions(): void {
    if (!this.clinicId) return;
    
    this.isLoading.set(true);
    this.errorMessage.set('');
    
    this.telemedicineService.getClinicSessions(this.clinicId).subscribe({
      next: (data) => {
        this.sessions.set(data);
        this.filterSessions();
        this.isLoading.set(false);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao carregar sessões de telemedicina');
        this.isLoading.set(false);
        console.error('Error loading sessions:', error);
      }
    });
  }

  filterSessions(): void {
    const status = this.selectedStatus();
    const allSessions = this.sessions();
    
    if (status === 'All') {
      this.filteredSessions.set(allSessions);
    } else {
      this.filteredSessions.set(allSessions.filter(s => s.status === status));
    }
  }

  onStatusChange(status: string): void {
    this.selectedStatus.set(status);
    this.filterSessions();
  }

  startSession(sessionId: string): void {
    this.telemedicineService.startSession(sessionId).subscribe({
      next: () => {
        this.loadSessions();
      },
      error: (error) => {
        this.errorMessage.set('Erro ao iniciar sessão');
        console.error('Error starting session:', error);
      }
    });
  }

  cancelSession(sessionId: string): void {
    if (confirm('Tem certeza que deseja cancelar esta sessão?')) {
      this.telemedicineService.cancelSession(sessionId, 'Cancelado pelo usuário').subscribe({
        next: () => {
          this.loadSessions();
        },
        error: (error) => {
          this.errorMessage.set('Erro ao cancelar sessão');
          console.error('Error canceling session:', error);
        }
      });
    }
  }

  getStatusBadgeClass(status: SessionStatus): string {
    switch (status) {
      case SessionStatus.Scheduled:
        return 'badge-scheduled';
      case SessionStatus.InProgress:
        return 'badge-in-progress';
      case SessionStatus.Completed:
        return 'badge-completed';
      case SessionStatus.Cancelled:
        return 'badge-cancelled';
      case SessionStatus.Failed:
        return 'badge-failed';
      default:
        return '';
    }
  }

  getStatusLabel(status: SessionStatus): string {
    switch (status) {
      case SessionStatus.Scheduled:
        return 'Agendada';
      case SessionStatus.InProgress:
        return 'Em Andamento';
      case SessionStatus.Completed:
        return 'Concluída';
      case SessionStatus.Cancelled:
        return 'Cancelada';
      case SessionStatus.Failed:
        return 'Falhou';
      default:
        return status;
    }
  }

  canStartSession(session: TelemedicineSession): boolean {
    return session.status === SessionStatus.Scheduled;
  }

  canJoinSession(session: TelemedicineSession): boolean {
    return session.status === SessionStatus.InProgress;
  }

  canCancelSession(session: TelemedicineSession): boolean {
    return session.status === SessionStatus.Scheduled || session.status === SessionStatus.InProgress;
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleString('pt-BR');
  }
}
