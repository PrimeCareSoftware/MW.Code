import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { interval, Subscription } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { WaitingQueueService } from '../../../services/waiting-queue.service';
import { PatientService } from '../../../services/patient';
import { Patient } from '../../../models/patient.model';
import { Navbar } from '../../../shared/navbar/navbar';
import {
  WaitingQueueSummary,
  WaitingQueueEntry,
  TriagePriority,
  QueueStatus,
  UpdateQueueTriage,
  CreateWaitingQueueEntry,
  TRIAGE_PRIORITY_LABELS,
  QUEUE_STATUS_LABELS
} from '../../../models/waiting-queue.model';

@Component({
  selector: 'app-queue-management',
  standalone: true,
  imports: [CommonModule, FormsModule, Navbar],
  templateUrl: './queue-management.html',
  styleUrls: ['./queue-management.scss']
})
export class QueueManagementComponent implements OnInit, OnDestroy {
  // Constants
  private readonly MIN_SEARCH_LENGTH = 3;
  private readonly AD_HOC_APPOINTMENT_ID = '00000000-0000-0000-0000-000000000000'; // Special ID for ad-hoc patients
  private readonly AUTO_REFRESH_INTERVAL = 30000; // 30 seconds

  summary: WaitingQueueSummary | null = null;
  loading = false;
  error: string | null = null;
  clinicId: string = ''; // Should be set from auth service
  
  selectedEntry: WaitingQueueEntry | null = null;
  editingTriage = false;
  triageForm = {
    priority: TriagePriority.Normal,
    notes: ''
  };

  // Patient search properties
  patientSearchTerm = '';
  searchResults: Patient[] = [];
  searchingPatients = false;
  addingToQueue = false;

  autoRefreshSubscription?: Subscription;

  // Expose enums and labels to template
  TriagePriority = TriagePriority;
  QueueStatus = QueueStatus;
  TRIAGE_PRIORITY_LABELS = TRIAGE_PRIORITY_LABELS;
  QUEUE_STATUS_LABELS = QUEUE_STATUS_LABELS;

  constructor(
    private waitingQueueService: WaitingQueueService,
    private patientService: PatientService
  ) {}

  ngOnInit(): void {
    // In a real app, get clinicId from auth service
    this.clinicId = localStorage.getItem('clinicId') || '';
    
    if (this.clinicId) {
      this.loadQueueSummary();
      this.startAutoRefresh();
    }
  }

  ngOnDestroy(): void {
    this.stopAutoRefresh();
  }

  loadQueueSummary(): void {
    this.loading = true;
    this.error = null;

    this.waitingQueueService.getQueueSummary(this.clinicId).subscribe({
      next: (summary) => {
        this.summary = summary;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Erro ao carregar fila de espera';
        console.error(err);
        this.loading = false;
      }
    });
  }

  startAutoRefresh(): void {
    this.autoRefreshSubscription = interval(this.AUTO_REFRESH_INTERVAL)
      .pipe(
        switchMap(() => this.waitingQueueService.getQueueSummary(this.clinicId))
      )
      .subscribe({
        next: (summary) => {
          this.summary = summary;
        },
        error: (err) => {
          console.error('Auto-refresh error:', err);
        }
      });
  }

  stopAutoRefresh(): void {
    if (this.autoRefreshSubscription) {
      this.autoRefreshSubscription.unsubscribe();
    }
  }

  openTriageDialog(entry: WaitingQueueEntry): void {
    this.selectedEntry = entry;
    this.triageForm = {
      priority: entry.priority,
      notes: entry.triageNotes || ''
    };
    this.editingTriage = true;
  }

  closeTriageDialog(): void {
    this.selectedEntry = null;
    this.editingTriage = false;
  }

  saveTriage(): void {
    if (!this.selectedEntry) return;

    const triage: UpdateQueueTriage = {
      priority: this.triageForm.priority,
      triageNotes: this.triageForm.notes
    };

    this.waitingQueueService.updateTriage(this.selectedEntry.id, triage).subscribe({
      next: () => {
        this.closeTriageDialog();
        this.loadQueueSummary();
      },
      error: (err) => {
        alert('Erro ao atualizar triagem');
        console.error(err);
      }
    });
  }

  callPatient(entry: WaitingQueueEntry): void {
    if (!confirm(`Chamar paciente ${entry.patientName}?`)) return;

    this.waitingQueueService.callPatient(entry.id).subscribe({
      next: () => {
        this.loadQueueSummary();
        this.playNotificationSound();
      },
      error: (err) => {
        alert('Erro ao chamar paciente');
        console.error(err);
      }
    });
  }

  startService(entry: WaitingQueueEntry): void {
    this.waitingQueueService.startService(entry.id).subscribe({
      next: () => {
        this.loadQueueSummary();
      },
      error: (err) => {
        alert('Erro ao iniciar atendimento');
        console.error(err);
      }
    });
  }

  completeService(entry: WaitingQueueEntry): void {
    if (!confirm(`Finalizar atendimento de ${entry.patientName}?`)) return;

    this.waitingQueueService.completeService(entry.id).subscribe({
      next: () => {
        this.loadQueueSummary();
      },
      error: (err) => {
        alert('Erro ao finalizar atendimento');
        console.error(err);
      }
    });
  }

  cancelEntry(entry: WaitingQueueEntry): void {
    if (!confirm(`Cancelar entrada de ${entry.patientName}?`)) return;

    this.waitingQueueService.cancelEntry(entry.id).subscribe({
      next: () => {
        this.loadQueueSummary();
      },
      error: (err) => {
        alert('Erro ao cancelar entrada');
        console.error(err);
      }
    });
  }

  getPriorityClass(priority: TriagePriority): string {
    switch (priority) {
      case TriagePriority.Emergency:
        return 'priority-emergency';
      case TriagePriority.Urgent:
        return 'priority-urgent';
      case TriagePriority.High:
        return 'priority-high';
      case TriagePriority.Normal:
        return 'priority-normal';
      case TriagePriority.Low:
        return 'priority-low';
      default:
        return '';
    }
  }

  getStatusClass(status: QueueStatus): string {
    switch (status) {
      case QueueStatus.Waiting:
        return 'status-waiting';
      case QueueStatus.Called:
        return 'status-called';
      case QueueStatus.InProgress:
        return 'status-inprogress';
      case QueueStatus.Completed:
        return 'status-completed';
      case QueueStatus.Cancelled:
        return 'status-cancelled';
      default:
        return '';
    }
  }

  formatWaitTime(minutes: number): string {
    if (minutes < 60) {
      return `${minutes} min`;
    }
    const hours = Math.floor(minutes / 60);
    const mins = minutes % 60;
    return `${hours}h ${mins}min`;
  }

  playNotificationSound(): void {
    // Simple beep sound
    const audioContext = new (window.AudioContext || (window as any).webkitAudioContext)();
    const oscillator = audioContext.createOscillator();
    const gainNode = audioContext.createGain();

    oscillator.connect(gainNode);
    gainNode.connect(audioContext.destination);

    oscillator.frequency.value = 800;
    oscillator.type = 'sine';

    gainNode.gain.setValueAtTime(0.3, audioContext.currentTime);
    gainNode.gain.exponentialRampToValueAtTime(0.01, audioContext.currentTime + 0.5);

    oscillator.start(audioContext.currentTime);
    oscillator.stop(audioContext.currentTime + 0.5);
  }

  searchPatients(): void {
    if (!this.patientSearchTerm || this.patientSearchTerm.trim().length < this.MIN_SEARCH_LENGTH) {
      this.searchResults = [];
      return;
    }

    this.searchingPatients = true;
    this.patientService.search(this.patientSearchTerm.trim()).subscribe({
      next: (patients) => {
        this.searchResults = patients;
        this.searchingPatients = false;
      },
      error: (err) => {
        console.error('Error searching patients:', err);
        this.searchingPatients = false;
        this.searchResults = [];
        this.showErrorMessage('Erro ao buscar pacientes. Por favor, tente novamente.');
      }
    });
  }

  addPatientToQueue(patient: Patient): void {
    if (!this.clinicId) {
      this.showErrorMessage('Clínica não configurada. Por favor, configure a clínica antes de adicionar pacientes à fila.');
      return;
    }

    this.addingToQueue = true;

    // Create a waiting queue entry for the patient
    const queueEntry: CreateWaitingQueueEntry = {
      appointmentId: this.AD_HOC_APPOINTMENT_ID, // Special ID for ad-hoc patients without appointment
      clinicId: this.clinicId,
      patientId: patient.id,
      priority: TriagePriority.Normal,
      triageNotes: 'Paciente avulso adicionado à fila'
    };

    this.waitingQueueService.addToQueue(queueEntry).subscribe({
      next: () => {
        this.addingToQueue = false;
        this.patientSearchTerm = '';
        this.searchResults = [];
        this.loadQueueSummary();
        this.showSuccessMessage(`Paciente ${patient.name} adicionado à fila com sucesso!`);
      },
      error: (err) => {
        console.error('Error adding patient to queue:', err);
        this.addingToQueue = false;
        this.showErrorMessage('Erro ao adicionar paciente à fila. Por favor, tente novamente.');
      }
    });
  }

  // Helper methods for user feedback
  private showSuccessMessage(message: string): void {
    // TODO: Replace with toast notification service when available
    alert(message);
  }

  private showErrorMessage(message: string): void {
    // TODO: Replace with toast notification service when available
    alert(message);
  }
}
