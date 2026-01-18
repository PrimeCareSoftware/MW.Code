import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Navbar } from '../../../shared/navbar/navbar';
import { FinancialService } from '../../../services/financial.service';
import { CashFlowEntry } from '../../../models/financial.model';

@Component({
  selector: 'app-cash-flow-list',
  imports: [CommonModule, RouterLink, Navbar],
  templateUrl: './cash-flow-list.component.html',
  styleUrl: './cash-flow-list.component.scss'
})
export class CashFlowListComponent implements OnInit {
  entries = signal<CashFlowEntry[]>([]);
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');

  constructor(private financialService: FinancialService) {}

  ngOnInit(): void {
    this.loadEntries();
  }

  loadEntries(): void {
    this.isLoading.set(true);
    this.financialService.getAllCashFlowEntries().subscribe({
      next: (data) => {
        this.entries.set(data);
        this.isLoading.set(false);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao carregar lançamentos');
        this.isLoading.set(false);
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
