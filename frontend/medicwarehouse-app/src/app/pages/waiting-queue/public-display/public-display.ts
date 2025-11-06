import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { interval, Subscription } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { WaitingQueueService } from '../../../services/waiting-queue.service';
import { PublicQueueDisplay } from '../../../models/waiting-queue.model';

@Component({
  selector: 'app-public-display',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './public-display.html',
  styleUrls: ['./public-display.scss']
})
export class PublicDisplayComponent implements OnInit, OnDestroy {
  queue: PublicQueueDisplay[] = [];
  loading = false;
  error: string | null = null;
  clinicId: string = '';
  tenantId: string = '';
  
  autoRefreshSubscription?: Subscription;
  autoRefreshInterval = 10000; // 10 seconds for public display
  currentTime: Date = new Date();
  timeSubscription?: Subscription;

  constructor(
    private waitingQueueService: WaitingQueueService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    // Get clinicId and tenantId from route params
    this.route.params.subscribe(params => {
      this.clinicId = params['clinicId'];
      this.tenantId = params['tenantId'];
      
      if (this.clinicId && this.tenantId) {
        this.loadPublicQueue();
        this.startAutoRefresh();
        this.startClock();
      }
    });
  }

  ngOnDestroy(): void {
    this.stopAutoRefresh();
    this.stopClock();
  }

  loadPublicQueue(): void {
    this.loading = true;
    this.error = null;

    this.waitingQueueService.getPublicQueueDisplay(this.clinicId, this.tenantId).subscribe({
      next: (queue) => {
        this.queue = queue;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Não foi possível carregar a fila';
        console.error(err);
        this.loading = false;
      }
    });
  }

  startAutoRefresh(): void {
    this.autoRefreshSubscription = interval(this.autoRefreshInterval)
      .pipe(
        switchMap(() => this.waitingQueueService.getPublicQueueDisplay(this.clinicId, this.tenantId))
      )
      .subscribe({
        next: (queue) => {
          this.queue = queue;
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

  startClock(): void {
    this.timeSubscription = interval(1000).subscribe(() => {
      this.currentTime = new Date();
    });
  }

  stopClock(): void {
    if (this.timeSubscription) {
      this.timeSubscription.unsubscribe();
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

  getStatusIcon(status: string): string {
    switch (status.toLowerCase()) {
      case 'waiting':
        return '⏳';
      case 'called':
        return '📢';
      default:
        return '👤';
    }
  }
}
