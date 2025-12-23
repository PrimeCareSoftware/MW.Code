import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Navbar } from '../../shared/navbar/navbar';
import { Loading } from '../../shared/loading/loading';
import { Auth } from '../../services/auth';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { format } from 'date-fns';

interface DashboardStats {
  totalPatients: number;
  todayAppointments: number;
  completedAppointments: number;
  waitingQueue: number;
  patientsGrowth: number;
  todaySchedule: AppointmentSummary[];
}

interface AppointmentSummary {
  id: string;
  time: string;
  patientName: string;
  type: string;
  status: string;
}

@Component({
  selector: 'app-dashboard',
  imports: [CommonModule, RouterLink, Navbar, Loading],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss'
})
export class Dashboard implements OnInit {
  loading = true;
  stats: DashboardStats = {
    totalPatients: 0,
    todayAppointments: 0,
    completedAppointments: 0,
    waitingQueue: 0,
    patientsGrowth: 0,
    todaySchedule: []
  };

  constructor(
    public authService: Auth,
    private http: HttpClient
  ) {}

  ngOnInit() {
    this.loadDashboardStats();
  }

  async loadDashboardStats() {
    try {
      const clinicId = this.authService.getClinicId();
      if (!clinicId) {
        console.warn('No clinic ID found');
        this.loading = false;
        return;
      }

      const today = format(new Date(), 'yyyy-MM-dd');
      
      // Load patients count
      const patients = await this.http.get<any[]>(`${environment.apiUrl}/patients?clinicId=${clinicId}`).toPromise();
      this.stats.totalPatients = patients?.length || 0;

      // Load today's appointments
      const appointments = await this.http.get<any[]>(
        `${environment.apiUrl}/appointments?clinicId=${clinicId}&date=${today}`
      ).toPromise();
      
      this.stats.todayAppointments = appointments?.length || 0;
      this.stats.completedAppointments = appointments?.filter(a => a.status === 'Completed').length || 0;
      
      // Map appointments to schedule
      this.stats.todaySchedule = (appointments || []).slice(0, 5).map(apt => ({
        id: apt.id,
        time: format(new Date(apt.scheduledDate), 'HH:mm'),
        patientName: apt.patient?.name || 'Unknown',
        type: this.translateAppointmentType(apt.type),
        status: apt.status
      }));

      // Load waiting queue (mock for now since we don't have an endpoint)
      this.stats.waitingQueue = 2;
      this.stats.patientsGrowth = 12;

    } catch (error) {
      console.error('Error loading dashboard stats:', error);
    } finally {
      this.loading = false;
    }
  }

  translateAppointmentType(type: string): string {
    const translations: { [key: string]: string } = {
      'Regular': 'Consulta Regular',
      'FirstVisit': 'Primeira Consulta',
      'Return': 'Retorno',
      'Emergency': 'Emergência'
    };
    return translations[type] || type;
  }

  getStatusClass(status: string): string {
    const statusClasses: { [key: string]: string } = {
      'Scheduled': 'pending',
      'InProgress': 'in-progress',
      'Completed': 'completed',
      'Cancelled': 'cancelled',
      'NoShow': 'cancelled'
    };
    return statusClasses[status] || 'pending';
  }

  getStatusLabel(status: string): string {
    const statusLabels: { [key: string]: string } = {
      'Scheduled': 'Agendado',
      'InProgress': 'Em andamento',
      'Completed': 'Concluída',
      'Cancelled': 'Cancelada',
      'NoShow': 'Não compareceu'
    };
    return statusLabels[status] || status;
  }
}
