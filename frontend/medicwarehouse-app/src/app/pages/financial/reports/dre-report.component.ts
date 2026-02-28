import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FinancialService } from '../../../services/financial.service';
import { ClinicAdminService } from '../../../services/clinic-admin.service';
import { DREReport } from '../../../models/financial.model';
import { MyClinicDto } from '../../../models/clinic-admin.model';

@Component({
  selector: 'app-dre-report',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './dre-report.component.html',
  styleUrl: './dre-report.component.scss'
})
export class DREReportComponent implements OnInit {
  report = signal<DREReport | null>(null);
  clinics = signal<MyClinicDto[]>([]);
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');
  
  selectedClinicId = '';
  startDate = '';
  endDate = '';

  constructor(
    private financialService: FinancialService,
    private clinicService: ClinicAdminService
  ) {}

  ngOnInit(): void {
    this.loadClinics();
    this.initializeDates();
  }

  loadClinics(): void {
    this.clinicService.getMyClinics().subscribe({
      next: (clinics: MyClinicDto[]) => {
        this.clinics.set(clinics);
        if (clinics.length > 0) {
          this.selectedClinicId = clinics[0].clinicId;
        }
      },
      error: (error: any) => {
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

  loadReport(): void {
    if (!this.selectedClinicId || !this.startDate || !this.endDate) {
      this.errorMessage.set('Por favor, preencha todos os campos');
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set('');
    
    this.financialService.getDREReport(this.selectedClinicId, this.startDate, this.endDate).subscribe({
      next: (data) => {
        this.report.set(data);
        this.isLoading.set(false);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao carregar relatório DRE');
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
