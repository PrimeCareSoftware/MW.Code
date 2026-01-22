import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Navbar } from '../../../shared/navbar/navbar';
import { FinancialService } from '../../../services/financial.service';
import { ClinicService } from '../../../services/clinic.service';
import { ProfitabilityAnalysis } from '../../../models/financial.model';
import { Clinic } from '../../../models/clinic.model';

@Component({
  selector: 'app-profitability-analysis',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, Navbar],
  templateUrl: './profitability-analysis.component.html',
  styleUrl: './profitability-analysis.component.scss'
})
export class ProfitabilityAnalysisComponent implements OnInit {
  analysis = signal<ProfitabilityAnalysis | null>(null);
  clinics = signal<Clinic[]>([]);
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');
  
  selectedClinicId = '';
  startDate = '';
  endDate = '';

  constructor(
    private financialService: FinancialService,
    private clinicService: ClinicService
  ) {}

  ngOnInit(): void {
    this.loadClinics();
    this.initializeDates();
  }

  loadClinics(): void {
    this.clinicService.getAllClinics().subscribe({
      next: (clinics) => {
        this.clinics.set(clinics);
        if (clinics.length > 0) {
          this.selectedClinicId = clinics[0].id;
        }
      },
      error: (error) => {
        console.error('Error loading clinics:', error);
        this.errorMessage.set('Erro ao carregar clínicas');
      }
    });
  }

  initializeDates(): void {
    const today = new Date();
    const firstDay = new Date(today.getFullYear(), today.getMonth(), 1);
    const lastDay = new Date(today.getFullYear(), today.getMonth() + 1, 0);
    this.startDate = firstDay.toISOString().split('T')[0];
    this.endDate = lastDay.toISOString().split('T')[0];
  }

  loadAnalysis(): void {
    if (!this.selectedClinicId || !this.startDate || !this.endDate) {
      this.errorMessage.set('Por favor, preencha todos os campos');
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set('');
    
    this.financialService.getProfitabilityAnalysis(this.selectedClinicId, this.startDate, this.endDate).subscribe({
      next: (data) => {
        this.analysis.set(data);
        this.isLoading.set(false);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao carregar análise de rentabilidade');
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

  formatPercent(percent: number): string {
    return `${percent.toFixed(2)}%`;
  }

  formatDate(dateString: string | Date): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('pt-BR');
  }

  exportToPDF(): void {
    // TODO: Implement PDF export
    alert('Exportação para PDF será implementada em breve');
  }

  exportToExcel(): void {
    // TODO: Implement Excel export
    alert('Exportação para Excel será implementada em breve');
  }

  getProfitClass(value: number): string {
    return value > 0 ? 'positive' : value < 0 ? 'negative' : '';
  }
}
