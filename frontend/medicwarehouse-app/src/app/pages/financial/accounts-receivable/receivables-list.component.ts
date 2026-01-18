import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { Navbar } from '../../../shared/navbar/navbar';
import { FinancialService } from '../../../services/financial.service';
import { AccountsReceivable, ReceivableStatus } from '../../../models/financial.model';

@Component({
  selector: 'app-receivables-list',
  imports: [CommonModule, RouterLink, Navbar],
  templateUrl: './receivables-list.component.html',
  styleUrl: './receivables-list.component.scss'
})
export class ReceivablesListComponent implements OnInit {
  receivables = signal<AccountsReceivable[]>([]);
  filteredReceivables = signal<AccountsReceivable[]>([]);
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');
  searchTerm = signal<string>('');
  statusFilter = signal<string>('all');
  totalOutstanding = signal<number>(0);
  totalOverdue = signal<number>(0);

  constructor(
    private financialService: FinancialService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadReceivables();
    this.loadSummary();
  }

  loadReceivables(): void {
    this.isLoading.set(true);
    this.financialService.getAllReceivables().subscribe({
      next: (data) => {
        this.receivables.set(data);
        this.applyFilters();
        this.isLoading.set(false);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao carregar contas a receber');
        this.isLoading.set(false);
        console.error('Error loading receivables:', error);
      }
    });
  }

  loadSummary(): void {
    this.financialService.getTotalOutstandingReceivables().subscribe({
      next: (total) => this.totalOutstanding.set(total),
      error: (error) => console.error('Error loading total outstanding:', error)
    });

    this.financialService.getTotalOverdueReceivables().subscribe({
      next: (total) => this.totalOverdue.set(total),
      error: (error) => console.error('Error loading total overdue:', error)
    });
  }

  applyFilters(): void {
    let filtered = this.receivables();
    
    if (this.searchTerm()) {
      const term = this.searchTerm().toLowerCase();
      filtered = filtered.filter(r => 
        r.documentNumber.toLowerCase().includes(term) ||
        r.description?.toLowerCase().includes(term)
      );
    }

    if (this.statusFilter() !== 'all') {
      filtered = filtered.filter(r => r.status === this.statusFilter());
    }

    this.filteredReceivables.set(filtered);
  }

  onSearchChange(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.searchTerm.set(value);
    this.applyFilters();
  }

  onStatusFilterChange(status: string): void {
    this.statusFilter.set(status);
    this.applyFilters();
  }

  deleteReceivable(id: string): void {
    if (confirm('Tem certeza que deseja excluir esta conta a receber?')) {
      this.financialService.deleteReceivable(id).subscribe({
        next: () => {
          this.loadReceivables();
          this.loadSummary();
        },
        error: (error) => {
          this.errorMessage.set('Erro ao excluir conta a receber');
          console.error('Error deleting receivable:', error);
        }
      });
    }
  }

  cancelReceivable(id: string): void {
    const reason = prompt('Informe o motivo do cancelamento:');
    if (reason) {
      this.financialService.cancelReceivable(id, reason).subscribe({
        next: () => {
          this.loadReceivables();
          this.loadSummary();
        },
        error: (error) => {
          this.errorMessage.set('Erro ao cancelar conta a receber');
          console.error('Error cancelling receivable:', error);
        }
      });
    }
  }

  getStatusBadgeClass(status: string): string {
    const statusMap: { [key: string]: string } = {
      'Pending': 'badge-warning',
      'PartiallyPaid': 'badge-info',
      'Paid': 'badge-success',
      'Overdue': 'badge-error',
      'Cancelled': 'badge-error',
      'InNegotiation': 'badge-info'
    };
    return statusMap[status] || 'badge-default';
  }

  formatStatus(status: string): string {
    const statusMap: { [key: string]: string } = {
      'Pending': 'Pendente',
      'PartiallyPaid': 'Parcialmente Pago',
      'Paid': 'Pago',
      'Overdue': 'Vencido',
      'Cancelled': 'Cancelado',
      'InNegotiation': 'Em Negociação'
    };
    return statusMap[status] || status;
  }

  formatType(type: string): string {
    const typeMap: { [key: string]: string } = {
      'Consultation': 'Consulta',
      'Procedure': 'Procedimento',
      'Exam': 'Exame',
      'HealthInsurance': 'Convênio',
      'Other': 'Outro'
    };
    return typeMap[type] || type;
  }

  formatCurrency(amount: number): string {
    return new Intl.NumberFormat('pt-BR', { 
      style: 'currency', 
      currency: 'BRL' 
    }).format(amount);
  }

  formatDate(dateString: string | Date): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('pt-BR');
  }
}
