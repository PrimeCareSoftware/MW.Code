import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { interval, Subscription } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { WaitingQueueService } from '../../../services/waiting-queue.service';
import {
  WaitingQueueSummary,
  WaitingQueueEntry,
  TriagePriority,
  QueueStatus,
  UpdateQueueTriage,
  TRIAGE_PRIORITY_LABELS,
  QUEUE_STATUS_LABELS
} from '../../../models/waiting-queue.model';

@Component({
  selector: 'app-queue-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './queue-management.html',
  styleUrls: ['./queue-management.scss']
})
export class QueueManagementComponent implements OnInit, OnDestroy {
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

  autoRefreshSubscription?: Subscription;
  autoRefreshInterval = 30000; // 30 seconds

  // Expose enums and labels to template
  TriagePriority = TriagePriority;
  QueueStatus = QueueStatus;
  TRIAGE_PRIORITY_LABELS = TRIAGE_PRIORITY_LABELS;
  QUEUE_STATUS_LABELS = QUEUE_STATUS_LABELS;

  constructor(private waitingQueueService: WaitingQueueService) {}

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
    this.autoRefreshSubscription = interval(this.autoRefreshInterval)
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
}
