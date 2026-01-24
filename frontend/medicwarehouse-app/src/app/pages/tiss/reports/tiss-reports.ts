import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Navbar } from '../../../shared/navbar/navbar';
import { TissAnalyticsService } from '../../../services/tiss-analytics.service';
import { HealthInsuranceOperatorService } from '../../../services/health-insurance-operator.service';
import { Auth } from '../../../services/auth';
import {
  GlosasByOperator,
  ProcedureGlosas,
  ApprovalTime,
  AuthorizationRate,
  HealthInsuranceOperator
} from '../../../models/tiss.model';

type ReportType = 'billing' | 'glosas' | 'denials' | 'approvalTime' | 'procedures';

interface ReportData {
  billing?: GlosasByOperator[];
  glosas?: GlosasByOperator[];
  denials?: AuthorizationRate[];
  approvalTime?: ApprovalTime[];
  procedures?: ProcedureGlosas[];
}

@Component({
  selector: 'app-tiss-reports',
  imports: [CommonModule, FormsModule, Navbar],
  templateUrl: './tiss-reports.html',
  styleUrl: './tiss-reports.scss'
})
export class TissReports implements OnInit {
  // Data signals
  operators = signal<HealthInsuranceOperator[]>([]);
  reportData = signal<ReportData>({});
  
  // UI state signals
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');
  
  // Filter signals
  clinicId = signal<string>('');
  startDate = signal<string>('');
  endDate = signal<string>('');
  selectedOperatorId = signal<string>('all');
  selectedReportType = signal<ReportType>('billing');
  
  // Computed values
  hasReportData = computed(() => {
    const data = this.reportData();
    const reportType = this.selectedReportType();
    return data[reportType] && Array.isArray(data[reportType]) && data[reportType]!.length > 0;
  });

  filteredReportData = computed(() => {
    const data = this.reportData();
    const reportType = this.selectedReportType();
    const operatorId = this.selectedOperatorId();
    
    if (!data[reportType] || operatorId === 'all') {
      return data[reportType] || [];
    }
    
    // Filter by operator for billing and glosas reports
    if (reportType === 'billing' || reportType === 'glosas') {
      return (data[reportType] as GlosasByOperator[]).filter(
        item => item.operatorId === operatorId
      );
    }
    
    // Filter by operator for denials report
    if (reportType === 'denials') {
      return (data[reportType] as AuthorizationRate[]).filter(
        item => item.operatorId === operatorId
      );
    }
    
    // Filter by operator for approval time report
    if (reportType === 'approvalTime') {
      return (data[reportType] as ApprovalTime[]).filter(
        item => item.operatorId === operatorId
      );
    }
    
    return data[reportType] || [];
  });

  // Type-specific computed properties for template use
  billingData = computed(() => {
    const data = this.filteredReportData();
    return this.selectedReportType() === 'billing' ? data as GlosasByOperator[] : [];
  });

  glosasData = computed(() => {
    const data = this.filteredReportData();
    return this.selectedReportType() === 'glosas' ? data as GlosasByOperator[] : [];
  });

  denialsData = computed(() => {
    const data = this.filteredReportData();
    return this.selectedReportType() === 'denials' ? data as AuthorizationRate[] : [];
  });

  approvalTimeData = computed(() => {
    const data = this.filteredReportData();
    return this.selectedReportType() === 'approvalTime' ? data as ApprovalTime[] : [];
  });

  proceduresData = computed(() => {
    const data = this.filteredReportData();
    return this.selectedReportType() === 'procedures' ? data as ProcedureGlosas[] : [];
  });

  reportTypes = [
    { value: 'billing', label: 'Faturamento por Operadora' },
    { value: 'glosas', label: 'Glosas Detalhadas' },
    { value: 'denials', label: 'Autorizações Negadas' },
    { value: 'approvalTime', label: 'Tempo de Aprovação' },
    { value: 'procedures', label: 'Procedimentos Mais Utilizados' }
  ];

  constructor(
    private analyticsService: TissAnalyticsService,
    private operatorService: HealthInsuranceOperatorService,
    private authService: Auth
  ) {
    // Set default date range (last 30 days)
    const endDate = new Date();
    const startDate = new Date();
    startDate.setDate(startDate.getDate() - 30);
    
    this.endDate.set(this.formatDate(endDate));
    this.startDate.set(this.formatDate(startDate));
  }

  ngOnInit(): void {
    this.loadClinicId();
    this.loadOperators();
  }

  private loadClinicId(): void {
    const currentUser = this.authService.currentUser();
    if (currentUser?.clinicId) {
      this.clinicId.set(currentUser.clinicId);
    } else {
      this.errorMessage.set('Clínica não identificada. Por favor, faça login novamente.');
    }
  }

  private loadOperators(): void {
    this.operatorService.getAll().subscribe({
      next: (data) => {
        this.operators.set(data.filter(op => op.isActive));
      },
      error: (error) => {
        console.error('Error loading operators:', error);
        this.errorMessage.set('Erro ao carregar operadoras');
      }
    });
  }

  generateReport(): void {
    if (!this.clinicId() || !this.startDate() || !this.endDate()) {
      this.errorMessage.set('Por favor, preencha todos os filtros obrigatórios.');
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set('');
    
    const reportType = this.selectedReportType();
    
    switch (reportType) {
      case 'billing':
        this.loadBillingReport();
        break;
      case 'glosas':
        this.loadGlosasReport();
        break;
      case 'denials':
        this.loadDenialsReport();
        break;
      case 'approvalTime':
        this.loadApprovalTimeReport();
        break;
      case 'procedures':
        this.loadProceduresReport();
        break;
    }
  }

  private loadBillingReport(): void {
    this.analyticsService.getGlosasByOperator(
      this.clinicId(),
      this.startDate(),
      this.endDate()
    ).subscribe({
      next: (data) => {
        this.reportData.update(current => ({ ...current, billing: data }));
        this.isLoading.set(false);
      },
      error: (error) => {
        console.error('Error loading billing report:', error);
        this.errorMessage.set('Erro ao carregar relatório de faturamento');
        this.isLoading.set(false);
      }
    });
  }

  private loadGlosasReport(): void {
    this.analyticsService.getGlosasByOperator(
      this.clinicId(),
      this.startDate(),
      this.endDate()
    ).subscribe({
      next: (data) => {
        this.reportData.update(current => ({ ...current, glosas: data }));
        this.isLoading.set(false);
      },
      error: (error) => {
        console.error('Error loading glosas report:', error);
        this.errorMessage.set('Erro ao carregar relatório de glosas');
        this.isLoading.set(false);
      }
    });
  }

  private loadDenialsReport(): void {
    this.analyticsService.getAuthorizationRate(
      this.clinicId(),
      this.startDate(),
      this.endDate()
    ).subscribe({
      next: (data) => {
        this.reportData.update(current => ({ ...current, denials: data }));
        this.isLoading.set(false);
      },
      error: (error) => {
        console.error('Error loading denials report:', error);
        this.errorMessage.set('Erro ao carregar relatório de autorizações negadas');
        this.isLoading.set(false);
      }
    });
  }

  private loadApprovalTimeReport(): void {
    this.analyticsService.getApprovalTime(
      this.clinicId(),
      this.startDate(),
      this.endDate()
    ).subscribe({
      next: (data) => {
        this.reportData.update(current => ({ ...current, approvalTime: data }));
        this.isLoading.set(false);
      },
      error: (error) => {
        console.error('Error loading approval time report:', error);
        this.errorMessage.set('Erro ao carregar relatório de tempo de aprovação');
        this.isLoading.set(false);
      }
    });
  }

  private loadProceduresReport(): void {
    this.analyticsService.getProcedureGlosas(
      this.clinicId(),
      this.startDate(),
      this.endDate()
    ).subscribe({
      next: (data) => {
        this.reportData.update(current => ({ ...current, procedures: data }));
        this.isLoading.set(false);
      },
      error: (error) => {
        console.error('Error loading procedures report:', error);
        this.errorMessage.set('Erro ao carregar relatório de procedimentos');
        this.isLoading.set(false);
      }
    });
  }

  onReportTypeChange(): void {
    // Clear previous report data when changing report type
    this.reportData.set({});
  }

  exportToPDF(): void {
    // TODO: Implement PDF export using jsPDF library
    // Example implementation:
    // import jsPDF from 'jspdf';
    // import 'jspdf-autotable';
    // 
    // const doc = new jsPDF();
    // doc.text('TISS Report', 14, 15);
    // doc.autoTable({
    //   head: [this.getTableHeaders()],
    //   body: this.getTableData(),
    // });
    // doc.save(`tiss-report-${this.selectedReportType()}-${Date.now()}.pdf`);
    
    alert('Funcionalidade de exportação em PDF será implementada usando a biblioteca jsPDF.');
  }

  exportToExcel(): void {
    // TODO: Implement Excel export using ExcelJS library
    // Example implementation:
    // import * as ExcelJS from 'exceljs';
    // 
    // const workbook = new ExcelJS.Workbook();
    // const worksheet = workbook.addWorksheet('Report');
    // worksheet.columns = this.getExcelColumns();
    // worksheet.addRows(this.getTableData());
    // 
    // workbook.xlsx.writeBuffer().then((buffer) => {
    //   const blob = new Blob([buffer], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
    //   const url = window.URL.createObjectURL(blob);
    //   const anchor = document.createElement('a');
    //   anchor.href = url;
    //   anchor.download = `tiss-report-${this.selectedReportType()}-${Date.now()}.xlsx`;
    //   anchor.click();
    //   window.URL.revokeObjectURL(url);
    // });
    
    alert('Funcionalidade de exportação em Excel será implementada usando a biblioteca ExcelJS.');
  }

  // Helper methods for totals calculation
  getTotalBilled(data: GlosasByOperator[]): number {
    return data.reduce((sum, item) => sum + item.totalBilled, 0);
  }

  getTotalApproved(data: GlosasByOperator[]): number {
    return data.reduce((sum, item) => sum + (item.totalBilled - item.totalGlosed), 0);
  }

  getTotalGlosed(data: GlosasByOperator[]): number {
    return data.reduce((sum, item) => sum + item.totalGlosed, 0);
  }

  getTotalGuides(data: GlosasByOperator[]): number {
    return data.reduce((sum, item) => sum + item.totalGuides, 0);
  }

  getSelectedOperatorName(): string {
    const operator = this.operators().find(o => o.id === this.selectedOperatorId());
    return operator?.tradeName || '';
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(value);
  }

  formatPercentage(value: number): string {
    return `${value.toFixed(2)}%`;
  }

  formatNumber(value: number): string {
    return new Intl.NumberFormat('pt-BR').format(value);
  }

  formatDays(value: number): string {
    return `${value.toFixed(1)} dias`;
  }

  private formatDate(date: Date): string {
    return date.toISOString().split('T')[0];
  }

  getReportTitle(): string {
    const report = this.reportTypes.find(r => r.value === this.selectedReportType());
    return report?.label || 'Relatório TISS';
  }
}
