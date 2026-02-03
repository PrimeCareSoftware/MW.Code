import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SalesforceLeadsService } from '../../services/salesforce-leads.service';
import { 
  SalesforceLead, 
  LeadStatistics, 
  LeadStatus 
} from '../../models/salesforce-lead.model';

@Component({
  selector: 'app-salesforce-leads',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './salesforce-leads.html',
  styleUrls: ['./salesforce-leads.scss']
})
export class SalesforceLeadsComponent implements OnInit {
  leads = signal<SalesforceLead[]>([]);
  statistics = signal<LeadStatistics | null>(null);
  loading = signal<boolean>(false);
  error = signal<string | null>(null);
  syncing = signal<boolean>(false);
  testingConnection = signal<boolean>(false);
  connectionStatus = signal<{ connected: boolean; error?: string } | null>(null);
  
  selectedStatus = signal<LeadStatus | 'all'>('all');
  searchTerm = signal<string>('');

  // Expose LeadStatus enum to template
  LeadStatus = LeadStatus;

  constructor(private salesforceService: SalesforceLeadsService) {}

  ngOnInit() {
    this.loadStatistics();
    this.loadLeads();
  }

  loadStatistics() {
    this.salesforceService.getStatistics().subscribe({
      next: (stats) => {
        this.statistics.set(stats);
      },
      error: (err) => {
        console.error('Error loading statistics:', err);
      }
    });
  }

  loadLeads() {
    this.loading.set(true);
    this.error.set(null);

    const status = this.selectedStatus();
    
    const observable = status === 'all' 
      ? this.salesforceService.getUnsyncedLeads()
      : this.salesforceService.getLeadsByStatus(status);

    observable.subscribe({
      next: (leads) => {
        this.leads.set(leads);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set('Erro ao carregar leads: ' + (err.error?.message || err.message));
        this.loading.set(false);
      }
    });
  }

  onStatusChange() {
    this.loadLeads();
  }

  syncAllLeads() {
    if (!confirm('Deseja sincronizar todos os leads não sincronizados com o Salesforce?')) {
      return;
    }

    this.syncing.set(true);
    this.salesforceService.syncAllLeads().subscribe({
      next: (result) => {
        alert(`Sincronização concluída!\n\nTotal: ${result.totalLeads}\nSucesso: ${result.successfulSyncs}\nErros: ${result.failedSyncs}`);
        this.syncing.set(false);
        this.loadLeads();
        this.loadStatistics();
      },
      error: (err) => {
        alert('Erro ao sincronizar leads: ' + (err.error?.message || err.message));
        this.syncing.set(false);
      }
    });
  }

  syncLead(leadId: string) {
    this.salesforceService.syncLead(leadId).subscribe({
      next: (response) => {
        alert(response.message);
        this.loadLeads();
        this.loadStatistics();
      },
      error: (err) => {
        alert('Erro ao sincronizar lead: ' + (err.error?.message || err.message));
      }
    });
  }

  updateLeadStatus(leadId: string, newStatus: LeadStatus) {
    this.salesforceService.updateLeadStatus(leadId, newStatus).subscribe({
      next: (response) => {
        alert(response.message);
        this.loadLeads();
        this.loadStatistics();
      },
      error: (err) => {
        alert('Erro ao atualizar status: ' + (err.error?.message || err.message));
      }
    });
  }

  testConnection() {
    this.testingConnection.set(true);
    this.salesforceService.testConnection().subscribe({
      next: (status) => {
        this.connectionStatus.set(status);
        this.testingConnection.set(false);
      },
      error: (err) => {
        this.connectionStatus.set({ connected: false, error: err.message });
        this.testingConnection.set(false);
      }
    });
  }

  getStatusText(status: LeadStatus): string {
    return this.salesforceService.getStatusText(status);
  }

  getStatusColor(status: LeadStatus): string {
    return this.salesforceService.getStatusColor(status);
  }

  getStepName(step: number): string {
    return this.salesforceService.getStepName(step);
  }

  filteredLeads(): SalesforceLead[] {
    const leads = this.leads();
    const search = this.searchTerm().toLowerCase();
    
    if (!search) {
      return leads;
    }

    return leads.filter(lead => 
      lead.companyName?.toLowerCase().includes(search) ||
      lead.contactName?.toLowerCase().includes(search) ||
      lead.email?.toLowerCase().includes(search) ||
      lead.phone?.includes(search)
    );
  }

  formatDate(date: Date | undefined): string {
    if (!date) return '-';
    return new Date(date).toLocaleDateString('pt-BR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  getPercentage(value: number, total: number): number {
    return total > 0 ? Math.round((value / total) * 100) : 0;
  }
}
