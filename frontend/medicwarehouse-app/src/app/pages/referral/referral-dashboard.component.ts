import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatChipsModule } from '@angular/material/chips';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { Subject, takeUntil } from 'rxjs';
import { ReferralService, ReferralStats, Referral, ReferralProgram, PaymentMethod } from '../../services/referral/referral.service';
import { ReferralInvitationModalComponent } from './referral-invitation-modal.component';
import { Navbar } from '../../shared/navbar/navbar';

@Component({
  selector: 'app-referral-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    Navbar,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    MatChipsModule,
    MatTooltipModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatDialogModule
  ],
  templateUrl: './referral-dashboard.component.html',
  styleUrls: ['./referral-dashboard.component.scss']
})
export class ReferralDashboardComponent implements OnInit, OnDestroy {
  program: ReferralProgram | null = null;
  stats: ReferralStats | null = null;
  referrals: Referral[] = [];
  leaderboard: any[] = [];
  loading = true;
  
  displayedColumns: string[] = ['referredEmail', 'status', 'signedUpAt', 'convertedAt', 'reward'];
  
  private destroy$ = new Subject<void>();

  constructor(
    private referralService: ReferralService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loadData();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadData(): void {
    this.loading = true;

    // Load program details
    this.referralService.getReferralProgram()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (program) => {
          this.program = program;
          this.loading = false;
        },
        error: (error) => {
          console.error('Error loading program:', error);
          this.loading = false;
        }
      });

    // Load stats
    this.referralService.getReferralStats()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (stats) => {
          this.stats = stats;
        },
        error: (error) => {
          console.error('Error loading stats:', error);
        }
      });

    // Load referrals
    this.referralService.getReferrals()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (referrals) => {
          this.referrals = referrals;
        },
        error: (error) => {
          console.error('Error loading referrals:', error);
        }
      });

    // Load leaderboard
    this.referralService.getLeaderboard()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (leaderboard) => {
          this.leaderboard = leaderboard;
        },
        error: (error) => {
          console.error('Error loading leaderboard:', error);
        }
      });
  }

  onCopyLink(): void {
    if (!this.program?.referralLink) return;

    navigator.clipboard.writeText(this.program.referralLink).then(() => {
      this.snackBar.open('Link copiado para a área de transferência!', 'Fechar', {
        duration: 3000
      });
    }).catch(err => {
      console.error('Error copying link:', err);
      this.snackBar.open('Erro ao copiar link', 'Fechar', {
        duration: 3000
      });
    });
  }

  onShareVia(platform: 'whatsapp' | 'email' | 'linkedin' | 'twitter'): void {
    this.referralService.shareVia(platform);
  }

  onInviteFriends(): void {
    const dialogRef = this.dialog.open(ReferralInvitationModalComponent, {
      width: '600px',
      data: {
        referralCode: this.program?.referralCode,
        referralLink: this.program?.referralLink
      }
    });

    dialogRef.afterClosed()
      .pipe(takeUntil(this.destroy$))
      .subscribe(result => {
        if (result) {
          this.snackBar.open('Convites enviados com sucesso!', 'Fechar', {
            duration: 3000
          });
          // Reload referrals
          this.loadData();
        }
      });
  }

  onRequestPayout(): void {
    if (!this.stats || this.stats.pendingRewards < (this.program?.minimumPayoutThreshold || 200)) {
      this.snackBar.open('Você precisa ter pelo menos R$ ' + (this.program?.minimumPayoutThreshold || 200) + ' disponível para solicitar pagamento', 'Fechar', {
        duration: 5000
      });
      return;
    }

    // In a real app, this would open a modal to collect payment details
    this.referralService.requestPayout(this.stats.pendingRewards, PaymentMethod.PIX, 'example@email.com')
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.snackBar.open('Solicitação de pagamento enviada com sucesso!', 'Fechar', {
            duration: 3000
          });
          this.loadData();
        },
        error: (error) => {
          console.error('Error requesting payout:', error);
          this.snackBar.open('Erro ao solicitar pagamento', 'Fechar', {
            duration: 3000
          });
        }
      });
  }

  getStatusLabel(status: string): string {
    const labels: { [key: string]: string } = {
      'PENDING': 'Pendente',
      'SIGNED_UP': 'Cadastrado',
      'CONVERTED': 'Convertido',
      'CANCELLED': 'Cancelado',
      'EXPIRED': 'Expirado'
    };
    return labels[status] || status;
  }

  getStatusClass(status: string): string {
    const classes: { [key: string]: string } = {
      'PENDING': 'status-pending',
      'SIGNED_UP': 'status-signed-up',
      'CONVERTED': 'status-converted',
      'CANCELLED': 'status-cancelled',
      'EXPIRED': 'status-expired'
    };
    return classes[status] || '';
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(value);
  }

  formatDate(date: Date | undefined): string {
    if (!date) return '-';
    return new Date(date).toLocaleDateString('pt-BR');
  }
}
