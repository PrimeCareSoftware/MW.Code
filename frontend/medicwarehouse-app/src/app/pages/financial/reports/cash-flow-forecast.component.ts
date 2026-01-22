import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Navbar } from '../../../shared/navbar/navbar';
import { FinancialService } from '../../../services/financial.service';
import { ClinicService } from '../../../services/clinic.service';
import { CashFlowForecast } from '../../../models/financial.model';
import { Clinic } from '../../../models/clinic.model';

@Component({
  selector: 'app-cash-flow-forecast',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, Navbar],
  templateUrl: './cash-flow-forecast.component.html',
  styleUrl: './cash-flow-forecast.component.scss'
})
export class CashFlowForecastComponent implements OnInit {
  forecast = signal<CashFlowForecast | null>(null);
  clinics = signal<Clinic[]>([]);
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');
  
  selectedClinicId = '';
  months = 3;

  constructor(
    private financialService: FinancialService,
    private clinicService: ClinicService
  ) {}

  ngOnInit(): void {
    this.loadClinics();
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

  loadForecast(): void {
    if (!this.selectedClinicId) {
      this.errorMessage.set('Por favor, selecione uma clínica');
      return;
    }

    if (this.months < 1 || this.months > 12) {
      this.errorMessage.set('O número de meses deve estar entre 1 e 12');
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set('');
    
    this.financialService.getCashFlowForecast(this.selectedClinicId, this.months).subscribe({
      next: (data) => {
        this.forecast.set(data);
        this.isLoading.set(false);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao carregar previsão de fluxo de caixa');
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

  getMonthName(month: number): string {
    const months = [
      'Janeiro', 'Fevereiro', 'Março', 'Abril', 'Maio', 'Junho',
      'Julho', 'Agosto', 'Setembro', 'Outubro', 'Novembro', 'Dezembro'
    ];
    return months[month - 1];
  }

  exportToPDF(): void {
    // TODO: Implement PDF export
    alert('Exportação para PDF será implementada em breve');
  }

  exportToExcel(): void {
    // TODO: Implement Excel export
    alert('Exportação para Excel será implementada em breve');
  }
}
