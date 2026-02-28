import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { TelemedicineService } from '../../../services/telemedicine.service';
import { TelemedicineSession, SessionStatus } from '../../../models/telemedicine.model';

@Component({
  selector: 'app-session-details',
  imports: [CommonModule, RouterLink],
  templateUrl: './session-details.html',
  styleUrl: './session-details.scss'
})
export class SessionDetails implements OnInit {
  session = signal<TelemedicineSession | null>(null);
  isLoading = signal<boolean>(true);
  errorMessage = signal<string>('');
  sessionId: string = '';

  readonly SessionStatus = SessionStatus;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private telemedicineService: TelemedicineService
  ) {}

  ngOnInit(): void {
    this.sessionId = this.route.snapshot.paramMap.get('id') || '';
    
    if (!this.sessionId) {
      this.errorMessage.set('ID da sessão inválido');
      this.isLoading.set(false);
      return;
    }
    
    this.loadSession();
  }

  loadSession(): void {
    this.telemedicineService.getSessionById(this.sessionId).subscribe({
      next: (data) => {
        this.session.set(data);
        this.isLoading.set(false);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao carregar detalhes da sessão');
        this.isLoading.set(false);
        console.error('Error loading session:', error);
      }
    });
  }

  startSession(): void {
    this.telemedicineService.startSession(this.sessionId).subscribe({
      next: () => {
        this.loadSession();
      },
      error: (error) => {
        this.errorMessage.set('Erro ao iniciar sessão');
        console.error('Error starting session:', error);
      }
    });
  }

  cancelSession(): void {
    if (confirm('Tem certeza que deseja cancelar esta sessão?')) {
      this.telemedicineService.cancelSession(this.sessionId, 'Cancelado pelo usuário').subscribe({
        next: () => {
          this.router.navigate(['/telemedicine']);
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

  formatDateTime(dateString: string | undefined): string {
    if (!dateString) return '';
    return new Date(dateString).toLocaleString('pt-BR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  formatDate(dateString: string | undefined): string {
    if (!dateString) return '';
    return new Date(dateString).toLocaleDateString('pt-BR');
  }

  formatTime(dateString: string | undefined): string {
    if (!dateString) return '';
    return new Date(dateString).toLocaleTimeString('pt-BR', {
      hour: '2-digit',
      minute: '2-digit'
    });
  }
}
