import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatChipsModule } from '@angular/material/chips';
import { AuthService } from '../../services/auth.service';
import { AppointmentService } from '../../services/appointment.service';
import { DocumentService } from '../../services/document.service';
import { NotificationService } from '../../services/notification.service';
import { User } from '../../models/auth.model';
import { Appointment } from '../../models/appointment.model';
import { Document } from '../../models/document.model';

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
    MatChipsModule
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

  constructor(
    private authService: AuthService,
    private appointmentService: AppointmentService,
    private documentService: DocumentService,
    private notificationService: NotificationService
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authService.getCurrentUser();
    this.loadDashboardData();
  }

  loadDashboardData(): void {
    this.loading = true;
    this.loadingError = false;
    let completedRequests = 0;
    const totalRequests = 4;

    const checkAllLoaded = () => {
      completedRequests++;
      if (completedRequests === totalRequests) {
        this.loading = false;
      }
    };

    // Load upcoming appointments
    this.appointmentService.getUpcomingAppointments(5).subscribe({
      next: (appointments) => {
        this.upcomingAppointments = appointments;
        checkAllLoaded();
      },
      error: (error) => {
        console.error('Error loading appointments:', error);
        this.loadingError = true;
        checkAllLoaded();
      }
    });

    // Load recent documents
    this.documentService.getRecentDocuments(5).subscribe({
      next: (documents) => {
        this.recentDocuments = documents;
        checkAllLoaded();
      },
      error: (error) => {
        console.error('Error loading documents:', error);
        this.loadingError = true;
        checkAllLoaded();
      }
    });

    // Load counts
    this.appointmentService.getAppointmentsCount().subscribe({
      next: (result) => {
        this.appointmentsCount = result.count;
        checkAllLoaded();
      },
      error: (error) => {
        console.error('Error loading appointments count:', error);
        checkAllLoaded();
      }
    });

    this.documentService.getDocumentsCount().subscribe({
      next: (result) => {
        this.documentsCount = result.count;
        checkAllLoaded();
      },
      error: (error) => {
        console.error('Error loading documents count:', error);
        checkAllLoaded();
      }
    });
  }

  formatDate(date: Date): string {
    return new Date(date).toLocaleDateString('pt-BR', {
      day: '2-digit',
      month: 'short',
      year: 'numeric'
    });
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
