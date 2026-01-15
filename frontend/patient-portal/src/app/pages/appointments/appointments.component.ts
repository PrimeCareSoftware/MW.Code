import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTabsModule } from '@angular/material/tabs';
import { MatChipsModule } from '@angular/material/chips';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDividerModule } from '@angular/material/divider';
import { AppointmentService } from '../../services/appointment.service';
import { NotificationService } from '../../services/notification.service';
import { Appointment } from '../../models/appointment.model';

@Component({
  selector: 'app-appointments',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatTabsModule,
    MatChipsModule,
    MatTooltipModule,
    MatDividerModule
  ],
  templateUrl: './appointments.component.html',
  styleUrls: ['./appointments.component.scss']
})
export class AppointmentsComponent implements OnInit {
  appointments: Appointment[] = [];
  filteredAppointments: Appointment[] = [];
  loading = true;
  loadingError = false;
  selectedTab = 0;

  constructor(
    private appointmentService: AppointmentService,
    private notificationService: NotificationService
  ) {}

  ngOnInit(): void {
    this.loadAppointments();
  }

  loadAppointments(): void {
    this.loading = true;
    this.loadingError = false;
    
    this.appointmentService.getMyAppointments().subscribe({
      next: (appointments) => {
        this.appointments = appointments;
        this.filterAppointments(this.selectedTab);
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading appointments:', error);
        this.loadingError = true;
        this.loading = false;
        this.notificationService.error('Erro ao carregar consultas');
      }
    });
  }

  onTabChange(index: number): void {
    this.selectedTab = index;
    this.filterAppointments(index);
  }

  filterAppointments(tabIndex: number): void {
    const now = new Date();
    
    switch(tabIndex) {
      case 0: // Todas
        this.filteredAppointments = this.appointments;
        break;
      case 1: // Próximas
        this.filteredAppointments = this.appointments.filter(apt => 
          new Date(apt.appointmentDate) >= now && apt.status !== 'Cancelado'
        );
        break;
      case 2: // Passadas
        this.filteredAppointments = this.appointments.filter(apt => 
          new Date(apt.appointmentDate) < now || apt.status === 'Concluído'
        );
        break;
      case 3: // Canceladas
        this.filteredAppointments = this.appointments.filter(apt => 
          apt.status === 'Cancelado'
        );
        break;
    }
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

  getStatusColor(status: string): string {
    const statusMap: { [key: string]: string } = {
      'Agendado': 'primary',
      'Confirmado': 'accent',
      'Concluído': 'success',
      'Cancelado': 'warn'
    };
    return statusMap[status] || 'default';
  }

  retry(): void {
    this.loadAppointments();
  }
}
