import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FinancialService } from '../../../services/financial.service';
import { FinancialClosure } from '../../../models/financial.model';

@Component({
  selector: 'app-closures-list',
  imports: [CommonModule, RouterLink],
  templateUrl: './closures-list.component.html',
  styleUrl: './closures-list.component.scss'
})
export class ClosuresListComponent implements OnInit {
  closures = signal<FinancialClosure[]>([]);
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');

  constructor(private financialService: FinancialService) {}

  ngOnInit(): void {
    this.loadClosures();
  }

  loadClosures(): void {
    this.isLoading.set(true);
    this.financialService.getAllClosures().subscribe({
      next: (data) => {
        this.closures.set(data);
        this.isLoading.set(false);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao carregar fechamentos');
        this.isLoading.set(false);
      }
    });
  }

  deleteClosure(id: string): void {
    if (confirm('Tem certeza que deseja excluir este fechamento?')) {
      this.financialService.deleteClosure(id).subscribe({
        next: () => this.loadClosures(),
        error: (error) => this.errorMessage.set('Erro ao excluir fechamento')
      });
    }
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

  getStatusBadgeClass(status: string): string {
    const map: { [key: string]: string } = {
      'Open': 'badge-info',
      'PendingPayment': 'badge-warning',
      'PartiallyPaid': 'badge-warning',
      'Closed': 'badge-success',
      'Cancelled': 'badge-error'
    };
    return map[status] || 'badge-default';
  }

  formatStatus(status: string): string {
    const map: { [key: string]: string } = {
      'Open': 'Aberto',
      'PendingPayment': 'Pend. Pagamento',
      'PartiallyPaid': 'Parc. Pago',
      'Closed': 'Fechado',
      'Cancelled': 'Cancelado'
    };
    return map[status] || status;
  }
}
