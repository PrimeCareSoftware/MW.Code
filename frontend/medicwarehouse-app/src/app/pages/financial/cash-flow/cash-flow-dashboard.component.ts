import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FinancialService } from '../../../services/financial.service';
import { CashFlowSummary } from '../../../models/financial.model';

@Component({
  selector: 'app-cash-flow-dashboard',
  imports: [CommonModule, RouterLink],
  templateUrl: './cash-flow-dashboard.component.html',
  styleUrl: './cash-flow-dashboard.component.scss'
})
export class CashFlowDashboardComponent implements OnInit {
  summary = signal<CashFlowSummary | null>(null);
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');
  startDate = '';
  endDate = '';

  constructor(private financialService: FinancialService) {}

  ngOnInit(): void {
    this.initializeDates();
    this.loadSummary();
  }

  initializeDates(): void {
    const today = new Date();
    const firstDay = new Date(today.getFullYear(), today.getMonth(), 1);
    this.startDate = firstDay.toISOString().split('T')[0];
    this.endDate = today.toISOString().split('T')[0];
  }

  loadSummary(): void {
    this.isLoading.set(true);
    this.financialService.getCashFlowSummary(this.startDate, this.endDate).subscribe({
      next: (data) => {
        this.summary.set(data);
        this.isLoading.set(false);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao carregar resumo de fluxo de caixa');
        this.isLoading.set(false);
        console.error('Error:', error);
      }
    });
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
