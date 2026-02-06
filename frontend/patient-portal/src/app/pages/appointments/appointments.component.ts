import { Component, OnInit, DestroyRef, inject } from '@angular/core';
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
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { AppointmentService } from '../../services/appointment.service';
import { NotificationService } from '../../services/notification.service';
import { Appointment } from '../../models/appointment.model';
import { CancelDialogComponent } from './cancel-dialog/cancel-dialog.component';
import { RescheduleDialogComponent } from './reschedule-dialog/reschedule-dialog.component';

enum TabFilter {
  All = 0,
  Upcoming = 1,
  Past = 2,
  Cancelled = 3
}

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
    MatDividerModule,
    MatDialogModule
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

  private destroyRef = inject(DestroyRef);

  constructor(
    private appointmentService: AppointmentService,
    private notificationService: NotificationService,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.loadAppointments();
  }

  loadAppointments(): void {
    this.loading = true;
    this.loadingError = false;
    
    this.appointmentService.getMyAppointments()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
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
      case TabFilter.All:
        this.filteredAppointments = this.appointments;
        break;
      case TabFilter.Upcoming:
        this.filteredAppointments = this.appointments.filter(apt => 
          new Date(apt.appointmentDate) >= now && apt.status !== 'Cancelado'
        );
        break;
      case TabFilter.Past:
        this.filteredAppointments = this.appointments.filter(apt => 
          new Date(apt.appointmentDate) < now || apt.status === 'Concluído'
        );
        break;
      case TabFilter.Cancelled:
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

  confirmAppointment(appointment: Appointment): void {
    if (confirm('Deseja confirmar esta consulta?')) {
      this.appointmentService.confirmAppointment(appointment.id).subscribe({
        next: () => {
          this.notificationService.success('Consulta confirmada com sucesso!');
          this.loadAppointments();
        },
        error: (error) => {
          console.error('Error confirming appointment:', error);
          this.notificationService.error('Erro ao confirmar consulta');
        }
      });
    }
  }

  cancelAppointment(appointment: Appointment): void {
    const dialogRef = this.dialog.open(CancelDialogComponent, {
      width: 'min(500px, 95vw)',
      maxWidth: '95vw',
      data: { appointment }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result && result.confirmed) {
        this.appointmentService.cancelAppointment(appointment.id, { reason: result.reason }).subscribe({
          next: () => {
            this.notificationService.success('Consulta cancelada com sucesso!');
            this.loadAppointments();
          },
          error: (error) => {
            console.error('Error cancelling appointment:', error);
            this.notificationService.error('Erro ao cancelar consulta');
          }
        });
      }
    });
  }

  rescheduleAppointment(appointment: Appointment): void {
    const dialogRef = this.dialog.open(RescheduleDialogComponent, {
      width: 'min(600px, 95vw)',
      maxWidth: '95vw',
      data: { appointment }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result && result.confirmed) {
        this.appointmentService.rescheduleAppointment(appointment.id, {
          newDate: result.newDate,
          newTime: result.newTime,
          reason: result.reason
        }).subscribe({
          next: () => {
            this.notificationService.success('Consulta reagendada com sucesso!');
            this.loadAppointments();
          },
          error: (error) => {
            console.error('Error rescheduling appointment:', error);
            this.notificationService.error('Erro ao reagendar consulta');
          }
        });
      }
    });
  }

  canConfirm(appointment: Appointment): boolean {
    return appointment.status === 'Scheduled' || appointment.status === 'Agendado';
  }
}
