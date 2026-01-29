import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { ConsentService } from '../../../services/consent.service';
import { DataDeletionService } from '../../../services/data-deletion.service';
import { AuditService } from '../../../services/audit.service';

interface DashboardStats {
  totalConsents: number;
  activeConsents: number;
  revokedConsents: number;
  expiredConsents: number;
  pendingDeletionRequests: number;
  processingDeletionRequests: number;
  completedDeletionRequests: number;
  rejectedDeletionRequests: number;
  todayAuditLogs: number;
  weekAuditLogs: number;
}

interface ConsentByType {
  type: string;
  count: number;
  percentage: number;
}

interface DeletionByType {
  type: string;
  count: number;
}

@Component({
  selector: 'app-lgpd-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './lgpd-dashboard.html',
  styleUrl: './lgpd-dashboard.scss'
})
export class LgpdDashboard implements OnInit {
  stats = signal<DashboardStats>({
    totalConsents: 0,
    activeConsents: 0,
    revokedConsents: 0,
    expiredConsents: 0,
    pendingDeletionRequests: 0,
    processingDeletionRequests: 0,
    completedDeletionRequests: 0,
    rejectedDeletionRequests: 0,
    todayAuditLogs: 0,
    weekAuditLogs: 0
  });

  consentsByType = signal<ConsentByType[]>([]);
  deletionsByType = signal<DeletionByType[]>([]);
  
  loading = signal(false);
  error = signal<string | null>(null);
  lastUpdated = signal<Date>(new Date());

  constructor(
    private consentService: ConsentService,
    private deletionService: DataDeletionService,
    private auditService: AuditService
  ) {}

  ngOnInit(): void {
    this.loadDashboardData();
  }

  loadDashboardData(): void {
    this.loading.set(true);
    this.error.set(null);

    // Load deletion requests (we have this endpoint)
    this.deletionService.getPendingRequests().subscribe({
      next: (requests) => {
        // Calculate stats from deletion requests
        const stats = this.stats();
        stats.pendingDeletionRequests = requests.filter(r => r.status === 'Pending').length;
        stats.processingDeletionRequests = requests.filter(r => r.status === 'Processing').length;
        stats.completedDeletionRequests = requests.filter(r => r.status === 'Completed').length;
        stats.rejectedDeletionRequests = requests.filter(r => r.status === 'Rejected').length;
        this.stats.set({...stats});

        // Calculate deletions by type
        const deletionTypeMap = new Map<string, number>();
        requests.forEach(r => {
          deletionTypeMap.set(r.requestType, (deletionTypeMap.get(r.requestType) || 0) + 1);
        });
        
        const deletionsByType: DeletionByType[] = [];
        deletionTypeMap.forEach((count, type) => {
          deletionsByType.push({ type, count });
        });
        this.deletionsByType.set(deletionsByType);

        this.lastUpdated.set(new Date());
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set('Erro ao carregar dados do dashboard: ' + (err.error?.message || err.message));
        this.loading.set(false);
      }
    });
  }

  refresh(): void {
    this.loadDashboardData();
  }

  getConsentComplianceRate(): number {
    const stats = this.stats();
    if (stats.totalConsents === 0) return 0;
    return Math.round((stats.activeConsents / stats.totalConsents) * 100);
  }

  getDeletionCompletionRate(): number {
    const total = this.stats().pendingDeletionRequests + 
                  this.stats().processingDeletionRequests + 
                  this.stats().completedDeletionRequests + 
                  this.stats().rejectedDeletionRequests;
    if (total === 0) return 0;
    return Math.round((this.stats().completedDeletionRequests / total) * 100);
  }

  formatDate(date: Date): string {
    return date.toLocaleString('pt-BR');
  }

  getConsentTypeColor(index: number): string {
    const colors = ['#4e73df', '#1cc88a', '#36b9cc', '#f6c23e', '#e74a3b', '#858796'];
    return colors[index % colors.length];
  }

  getDeletionTypeLabel(type: string): string {
    switch (type) {
      case 'Complete': return 'Exclusão Completa';
      case 'Anonymization': return 'Anonimização';
      case 'Partial': return 'Exclusão Parcial';
      default: return type;
    }
  }

  getMaxCount(data: any[]): number {
    if (data.length === 0) return 100;
    return Math.max(...data.map(d => d.count || 0));
  }

  getBarWidth(count: number, max: number): string {
    if (max === 0) return '0%';
    return `${(count / max) * 100}%`;
  }
}
