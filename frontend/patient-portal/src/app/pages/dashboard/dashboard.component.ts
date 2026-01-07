import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '../../services/auth.service';
import { AppointmentService } from '../../services/appointment.service';
import { DocumentService } from '../../services/document.service';
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
    MatProgressSpinnerModule
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

  constructor(
    private authService: AuthService,
    private appointmentService: AppointmentService,
    private documentService: DocumentService
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authService.getCurrentUser();
    this.loadDashboardData();
  }

  loadDashboardData(): void {
    this.loading = true;

    // Load upcoming appointments
    this.appointmentService.getUpcomingAppointments(5).subscribe({
      next: (appointments) => {
        this.upcomingAppointments = appointments;
      },
      error: (error) => {
        console.error('Error loading appointments:', error);
      }
    });

    // Load recent documents
    this.documentService.getRecentDocuments(5).subscribe({
      next: (documents) => {
        this.recentDocuments = documents;
      },
      error: (error) => {
        console.error('Error loading documents:', error);
      }
    });

    // Load counts
    this.appointmentService.getAppointmentsCount().subscribe({
      next: (result) => {
        this.appointmentsCount = result.count;
      }
    });

    this.documentService.getDocumentsCount().subscribe({
      next: (result) => {
        this.documentsCount = result.count;
        this.loading = false;
      }
    });
  }

  formatDate(date: Date): string {
    return new Date(date).toLocaleDateString('pt-BR');
  }

  formatTime(time: string): string {
    return time.substring(0, 5);
  }

  logout(): void {
    this.authService.logout().subscribe();
  }
}
