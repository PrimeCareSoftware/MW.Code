import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Subject, takeUntil } from 'rxjs';
import { ReferralService, ReferralStats, ReferralProgram } from '../../services/referral/referral.service';

@Component({
  selector: 'app-referral-stats-widget',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatProgressBarModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './referral-stats-widget.component.html',
  styleUrls: ['./referral-stats-widget.component.scss']
})
export class ReferralStatsWidgetComponent implements OnInit, OnDestroy {
  stats: ReferralStats | null = null;
  program: ReferralProgram | null = null;
  loading = true;
  private destroy$ = new Subject<void>();

  constructor(private referralService: ReferralService) {}

  ngOnInit(): void {
    this.loadData();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadData(): void {
    this.referralService.getReferralStats()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (stats) => {
          this.stats = stats;
          this.loading = false;
        },
        error: (error) => {
          console.error('Error loading stats:', error);
          this.loading = false;
        }
      });

    this.referralService.getReferralProgram()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (program) => {
          this.program = program;
        },
        error: (error) => {
          console.error('Error loading program:', error);
        }
      });
  }

  getProgressPercentage(): number {
    if (!this.stats || !this.program) return 0;
    return Math.min((this.stats.pendingRewards / this.program.minimumPayoutThreshold) * 100, 100);
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(value);
  }
}
