import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Navbar } from '../../shared/navbar/navbar';
import { Loading } from '../../shared/loading/loading';
import { Auth } from '../../services/auth';
import { NotificationService } from '../../services/notification.service';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { format, startOfMonth, endOfMonth, subMonths } from 'date-fns';
import { MOCK_DASHBOARD_QUEUE_DATA } from '../../mocks/dashboard.mock';
import { AnalyticsBIService } from '../../services/analytics-bi.service';

interface DashboardStats {
  totalPatients: number;
  todayAppointments: number;
  completedAppointments: number;
  waitingQueue: number;
  patientsGrowth: number;
  todaySchedule: AppointmentSummary[];
  // Financial metrics
  monthlyRevenue: number;
  revenueReceived: number;
  revenuePending: number;
  revenueOverdue: number;
  // Clinical metrics
  noShowRate: number;
  newPatients: number;
  returningPatients: number;
  weeklyRecordsCreated: number;
  // Alerts
  pendingPaymentsCount: number;
  overduePaymentsCount: number;
}

interface AppointmentSummary {
  id: string;
  time: string;
  patientName: string;
  type: string;
  status: string;
}

interface AlertItem {
  id: string;
  type: 'warning' | 'error' | 'info';
  title: string;
  message: string;
  link?: string;
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
    todaySchedule: [],
    monthlyRevenue: 0,
    revenueReceived: 0,
    revenuePending: 0,
    revenueOverdue: 0,
    noShowRate: 0,
    newPatients: 0,
    returningPatients: 0,
    weeklyRecordsCreated: 0,
    pendingPaymentsCount: 0,
    overduePaymentsCount: 0
  };
  
  alerts: AlertItem[] = [];

  constructor(
    public authService: Auth,
    private http: HttpClient,
    private notificationService: NotificationService,
    private analyticsBIService: AnalyticsBIService
  ) {}

  ngOnInit() {
    this.loadDashboardStats();
    this.loadFinancialData();
    this.loadClinicalMetrics();
    // Load notifications for authenticated users
    this.notificationService.loadNotifications();
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
      const { firstValueFrom } = await import('rxjs');
      
      // Load patients count
      const patientsResponse = await firstValueFrom(this.http.get<any[]>(`${environment.apiUrl}/patients?clinicId=${clinicId}`));
      const patients = Array.isArray(patientsResponse) ? patientsResponse : [];
      this.stats.totalPatients = patients.length;

      // Load today's appointments
      const appointmentsResponse = await firstValueFrom(this.http.get<any[]>(
        `${environment.apiUrl}/appointments?clinicId=${clinicId}&date=${today}`
      ));
      
      // Ensure appointments is always an array
      const appointments = Array.isArray(appointmentsResponse) ? appointmentsResponse : [];
      
      this.stats.todayAppointments = appointments.length;
      this.stats.completedAppointments = appointments.filter(a => a.status === 'Completed').length;
      
      // Map appointments to schedule
      this.stats.todaySchedule = appointments.slice(0, 5).map(apt => ({
        id: apt.id,
        time: format(new Date(apt.scheduledDate), 'HH:mm'),
        patientName: apt.patient?.name || 'Unknown',
        type: this.translateAppointmentType(apt.type),
        status: apt.status
      }));

      // Load waiting queue and patients growth (mock data only if enabled)
      if (environment.useMockData) {
        this.stats.waitingQueue = MOCK_DASHBOARD_QUEUE_DATA.waitingQueue;
        this.stats.patientsGrowth = MOCK_DASHBOARD_QUEUE_DATA.patientsGrowth;
      } else {
        // TODO: Load from actual API when backend endpoints are ready
        this.stats.waitingQueue = 0;
        this.stats.patientsGrowth = 0;
      }

    } catch (error) {
      console.error('Error loading dashboard stats:', error);
    } finally {
      this.loading = false;
    }
  }

  async loadFinancialData() {
    try {
      const startDate = format(startOfMonth(new Date()), 'yyyy-MM-dd');
      const endDate = format(endOfMonth(new Date()), 'yyyy-MM-dd');
      const { firstValueFrom } = await import('rxjs');
      
      const financialData = await firstValueFrom(
        this.analyticsBIService.getDashboardFinanceiro(startDate, endDate)
      );
      
      this.stats.monthlyRevenue = financialData.receitaTotal || 0;
      this.stats.revenueReceived = financialData.receitaRecebida || 0;
      this.stats.revenuePending = financialData.receitaPendente || 0;
      this.stats.revenueOverdue = financialData.receitaAtrasada || 0;
      
      // Count payments for alerts
      this.stats.pendingPaymentsCount = this.stats.revenuePending > 0 ? 1 : 0;
      this.stats.overduePaymentsCount = this.stats.revenueOverdue > 0 ? 1 : 0;
      
      // Create alerts based on financial data
      if (this.stats.revenueOverdue > 0) {
        this.alerts.push({
          id: 'overdue-payments',
          type: 'error',
          title: 'Pagamentos Atrasados',
          message: `R$ ${this.stats.revenueOverdue.toFixed(2)} em pagamentos vencidos`,
          link: '/financial/receivables'
        });
      }
      
      if (this.stats.revenuePending > 0) {
        this.alerts.push({
          id: 'pending-payments',
          type: 'warning',
          title: 'Pagamentos Pendentes',
          message: `R$ ${this.stats.revenuePending.toFixed(2)} aguardando recebimento`,
          link: '/financial/receivables'
        });
      }
      
    } catch (error) {
      console.error('Error loading financial data:', error);
      // Silently fail - financial data is optional
    }
  }

  async loadClinicalMetrics() {
    try {
      const startDate = format(startOfMonth(new Date()), 'yyyy-MM-dd');
      const endDate = format(endOfMonth(new Date()), 'yyyy-MM-dd');
      const { firstValueFrom } = await import('rxjs');
      
      const clinicalData = await firstValueFrom(
        this.analyticsBIService.getDashboardClinico(startDate, endDate)
      );
      
      this.stats.noShowRate = clinicalData.taxaNoShow || 0;
      this.stats.newPatients = clinicalData.pacientesNovos || 0;
      this.stats.returningPatients = clinicalData.pacientesRetorno || 0;
      
      // Add alert for high no-show rate
      if (this.stats.noShowRate > 15) {
        this.alerts.push({
          id: 'high-noshow',
          type: 'warning',
          title: 'Taxa de Falta Elevada',
          message: `${this.stats.noShowRate.toFixed(1)}% dos pacientes não compareceram`,
          link: '/appointments'
        });
      }
      
    } catch (error) {
      console.error('Error loading clinical metrics:', error);
      // Silently fail - clinical metrics are optional
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

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(value);
  }

  getAlertIcon(type: string): string {
    const icons: { [key: string]: string } = {
      'error': 'M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z',
      'warning': 'M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z',
      'info': 'M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z'
    };
    return icons[type] || icons['info'];
  }
}
