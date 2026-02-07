import { Component, OnInit, DestroyRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatChipsModule } from '@angular/material/chips';
import { forkJoin } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { AuthService } from '../../services/auth.service';
import { AppointmentService } from '../../services/appointment.service';
import { DocumentService } from '../../services/document.service';
import { NotificationService } from '../../services/notification.service';
import { User } from '../../models/auth.model';
import { Appointment } from '../../models/appointment.model';
import { Document } from '../../models/document.model';
import { ThemeToggleComponent } from '../../shared/theme-toggle/theme-toggle.component';
import { SkeletonLoaderComponent } from '../../shared/components/skeleton-loader/skeleton-loader.component';
import { AnimatedCounterComponent } from '../../shared/components/animated-counter/animated-counter.component';
import { AnimatedCardComponent } from '../../shared/components/animated-card/animated-card.component';
import { FabButtonComponent } from '../../shared/components/fab-button/fab-button.component';
import { LocaleService } from '../../shared/services/locale.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
    MatChipsModule,
    ThemeToggleComponent,
    SkeletonLoaderComponent,
    AnimatedCounterComponent,
    AnimatedCardComponent,
    FabButtonComponent
  ],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  currentUser: User | null = null;
  upcomingAppointments: Appointment[] = [];
  recentDocuments: Document[] = [];
  loading = true;
  appointmentsCount = 0;
  documentsCount = 0;
  loadingError = false;

  private destroyRef = inject(DestroyRef);

  constructor(
    private authService: AuthService,
    private appointmentService: AppointmentService,
    private documentService: DocumentService,
    private notificationService: NotificationService,
    private localeService: LocaleService
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authService.getCurrentUser();
    this.loadDashboardData();
  }

  loadDashboardData(): void {
    this.loading = true;
    this.loadingError = false;

    // Use forkJoin for parallel requests
    const appointments$ = this.appointmentService.getUpcomingAppointments(5);
    const documents$ = this.documentService.getRecentDocuments(5);
    const appointmentsCount$ = this.appointmentService.getAppointmentsCount();
    const documentsCount$ = this.documentService.getDocumentsCount();

    // Combine all requests with automatic unsubscribe
    forkJoin({
      appointments: appointments$,
      documents: documents$,
      appointmentsCount: appointmentsCount$,
      documentsCount: documentsCount$
    })
    .pipe(takeUntilDestroyed(this.destroyRef))
    .subscribe({
      next: (results) => {
        this.upcomingAppointments = results.appointments;
        this.recentDocuments = results.documents;
        this.appointmentsCount = results.appointmentsCount.count;
        this.documentsCount = results.documentsCount.count;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading dashboard data:', error);
        this.loadingError = true;
        this.loading = false;
      }
    });
  }

  formatDate(date: Date): string {
    return this.localeService.formatDate(date);
  }

  formatTime(time: string): string {
    return time.substring(0, 5);
  }

  logout(): void {
    this.authService.logout().subscribe({
      next: () => {
        this.notificationService.success('Logout realizado com sucesso!');
      },
      error: () => {
        this.notificationService.error('Erro ao fazer logout');
      }
    });
  }

  retry(): void {
    this.loadDashboardData();
  }
}
