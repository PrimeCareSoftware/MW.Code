import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { FinancialService } from '../../../services/financial.service';
import { AccountsPayable, PayableStatus } from '../../../models/financial.model';

@Component({
  selector: 'app-payables-list',
  imports: [CommonModule, RouterLink],
  templateUrl: './payables-list.component.html',
  styleUrl: './payables-list.component.scss'
})
export class PayablesListComponent implements OnInit {
  payables = signal<AccountsPayable[]>([]);
  filteredPayables = signal<AccountsPayable[]>([]);
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
    this.loadPayables();
    this.loadSummary();
  }

  loadPayables(): void {
    this.isLoading.set(true);
    this.financialService.getAllPayables().subscribe({
      next: (data) => {
        this.payables.set(data);
        this.applyFilters();
        this.isLoading.set(false);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao carregar contas a pagar');
        this.isLoading.set(false);
        console.error('Error loading payables:', error);
      }
    });
  }

  loadSummary(): void {
    this.financialService.getTotalOutstandingPayables().subscribe({
      next: (total) => this.totalOutstanding.set(total),
      error: (error) => console.error('Error loading total outstanding:', error)
    });

    this.financialService.getTotalOverduePayables().subscribe({
      next: (total) => this.totalOverdue.set(total),
      error: (error) => console.error('Error loading total overdue:', error)
    });
  }

  applyFilters(): void {
    let filtered = this.payables();
    
    if (this.searchTerm()) {
      const term = this.searchTerm().toLowerCase();
      filtered = filtered.filter(p => 
        p.documentNumber?.toLowerCase().includes(term) ||
        p.description?.toLowerCase().includes(term) ||
        p.supplier?.name?.toLowerCase().includes(term)
      );
    }

    if (this.statusFilter() !== 'all') {
      filtered = filtered.filter(p => p.status === this.statusFilter());
    }

    this.filteredPayables.set(filtered);
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

  deletePayable(id: string): void {
    if (confirm('Tem certeza que deseja excluir esta conta a pagar?')) {
      this.financialService.deletePayable(id).subscribe({
        next: () => {
          this.loadPayables();
          this.loadSummary();
        },
        error: (error) => {
          this.errorMessage.set('Erro ao excluir conta a pagar');
          console.error('Error deleting payable:', error);
        }
      });
    }
  }

  cancelPayable(id: string): void {
    const reason = prompt('Informe o motivo do cancelamento:');
    if (reason) {
      this.financialService.cancelPayable(id, reason).subscribe({
        next: () => {
          this.loadPayables();
          this.loadSummary();
        },
        error: (error) => {
          this.errorMessage.set('Erro ao cancelar conta a pagar');
          console.error('Error cancelling payable:', error);
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
      'Cancelled': 'badge-error'
    };
    return statusMap[status] || 'badge-default';
  }

  formatStatus(status: string): string {
    const statusMap: { [key: string]: string } = {
      'Pending': 'Pendente',
      'PartiallyPaid': 'Parcialmente Pago',
      'Paid': 'Pago',
      'Overdue': 'Vencido',
      'Cancelled': 'Cancelado'
    };
    return statusMap[status] || status;
  }

  formatCategory(category: string): string {
    const categoryMap: { [key: string]: string } = {
      'Rent': 'Aluguel',
      'Salaries': 'Salários',
      'Supplies': 'Suprimentos',
      'Equipment': 'Equipamentos',
      'Maintenance': 'Manutenção',
      'Utilities': 'Utilidades',
      'Marketing': 'Marketing',
      'Insurance': 'Seguros',
      'Taxes': 'Impostos',
      'ProfessionalServices': 'Serviços Profissionais',
      'Laboratory': 'Laboratório',
      'Pharmacy': 'Farmácia',
      'Other': 'Outro'
    };
    return categoryMap[category] || category;
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
